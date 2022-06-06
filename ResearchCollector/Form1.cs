using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ResearchCollector.Filter;
using ResearchCollector.Importer;
using ResearchCollector.PDFParser;

namespace ResearchCollector
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bgWorker;
        private bool workerInterrupted;
        private TextBox currentLogBox;
        private Label currentProgressLabel;
        private ProgressBar currentProgressBar;

        // Custom worker class for filter/importer
        private Worker worker;

        // Filter variables
        private string filterInputPath;
        private string filterOutputPath;

        // Importer variables
        private string importerInputPath;
        private Data data;

        // API db stats panel labels
        Label articleCount, inproceedingCount, authorCount, journalCount, proceedingCount, organizationCount;

        PDFInfoFinder pdfFixer = new PDFInfoFinder();

        HashSet<Article> articles;
        HashSet<Inproceedings> inproceedings;
        HashSet<Journal> journals;
        HashSet<Proceedings> proceedings;
        HashSet<Author> authors;
        HashSet<Person> persons;
        HashSet<Organization> organizations;

        /// <summary>
        /// Context used to access UI thread from BackgroundWorker
        /// </summary>
        private readonly System.Threading.SynchronizationContext context;

        public Form1()
        {
            InitializeComponent();
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.ProgressChanged += WorkerProgress;
            bgWorker.DoWork += DoWork;
            bgWorker.RunWorkerCompleted += WorkerCompleted;

            this.context = WindowsFormsSynchronizationContext.Current;

            SetupDbStatistics();
            data = new Data();

            // Use presets if provided
            if (File.Exists("../../config.txt"))
            {
                using (StreamReader sr = new StreamReader("../../config.txt"))
                {
                    filterInputPath = sr.ReadLine();
                    filterOutputPath = sr.ReadLine();
                    inputLocationFilter.Text = filterInputPath;
                    outputLocationFilter.Text = filterOutputPath;
                    outputLabel.Visible = true;
                    outputLocationFilter.Visible = true;
                }
            }
        }

        #region BackgroundWorker event handlers
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                worker.Run(bgWorker);
            }
            catch (Exception ex)
            {
                // Report error to UI thread 
                bgWorker.ReportProgress(progressBarFilter.Value, ex.Message);
                workerInterrupted = true;
            }
        }

        private void WorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                if (!string.IsNullOrEmpty(e.UserState.ToString()))
                    Error(e.UserState.ToString());
            currentProgressLabel.Text = $"{e.ProgressPercentage}%";
            currentProgressBar.Value = e.ProgressPercentage;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ToggleRunButtons();
            if (e.Cancelled || e.Error != null || workerInterrupted)
            {
                Log("Parsing interrupted");
                currentProgressLabel.Text = "";
                currentProgressBar.Value = 0;
                workerInterrupted = false;
            }
            else
            {
                Log("Parsing finished!");
                currentProgressLabel.Text = "100%";
                currentProgressBar.Value = 100;
                if (currentProgressBar.Equals(progressBarFilter)) // Filter finished
                {
                    string outputPath = Path.Combine(filterOutputPath, $"{worker}.json");
                    importerInputPath = outputPath;
                    inputLocationImporter.Text = outputPath;
                    Log($"Output saved to {outputPath}");
                }
                else // Importer finished
                {
                    // Import data to this form for use by the API
                    data = (worker as Importer.Importer).data;
                    Log($"{data.pubCount} publications parsed. The collected data kan be queried using the API (see API tab ↑)");
                    UpdateDbStatistics();
                }
            }
        }
        #endregion

        #region General UI helper methods
        private void LogCheckBox_Changed(object sender, EventArgs e)
        {
            if (worker != null)
                worker.logActions = (sender as CheckBox).Checked;
        }
        
        private string GetFileLocation()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return "";
        }

        private string GetFolderLocation(out bool success)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a folder to write the output to.";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                success = true;
                return dialog.SelectedPath;
            }

            success = false;
            return "";
        }

        private void ToggleRunButtons()
        {
            runBtnFilter.Enabled = !runBtnFilter.Enabled;
            runBtnImporter.Enabled = !runBtnImporter.Enabled;
        }

        // Called when user switches to a new tab
        private void TabSwitched(object sender, EventArgs e)
        {
            switch ((sender as TabPage).Text)
            {
                case "Filter":
                    currentLogBox = logBoxFilter;
                    break;
                case "Importer":
                    currentLogBox = logBoxImporter;
                    break;
                case "API":
                    currentLogBox = logBoxApi;
                    break;
            }
        }

        /// <summary>
        /// Log a msg to the UI
        /// </summary>
        /// <param name="msg">Message to log</param>
        private void Log(string msg)
        {
            currentLogBox.AppendText($"[{DateTime.Now.ToString("H:mm:ss")}] {msg}{Environment.NewLine}");
        }

        /// <summary>
        /// Display an error in a message box
        /// </summary>
        /// <param name="msg">Message to display</param>
        private void Error(string msg)
        {
            Log($"Encountered error: '{msg}'");
            //MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region Filter UI methods
        private void FilterInputLocation_Click(object sender, EventArgs e)
        {
            filterInputPath = GetFileLocation();
            inputLocationFilter.Text = filterInputPath;
        }

        private void FilterOutputLocation_Click(object sender, EventArgs e)
        {
            filterOutputPath = GetFolderLocation(out _);
            outputLocationFilter.Text = filterOutputPath;
        }

        private void FilterComboBox_IndexChanged(object sender, EventArgs e)
        {
            if (typeComboBoxFilter.SelectedIndex == 0)
            {
                inputLabel.Visible = true;
                dtdLabel.Text = "* DTD file is assumed to be located in the same directory as the input file, as well as to have the same name as the input file.";
                dtdLabel.Visible = true;
                inputLocationFilter.Visible = true;
            }
            else if (typeComboBoxFilter.SelectedIndex == 2)
            {
                inputLabel.Visible = true;
                dtdLabel.Text = "* Select the first of the downloaded files. The download date of all files (in the name) is assumed to be the same as this first one.";
                dtdLabel.Visible = true;
                inputLocationFilter.Visible = true;
            }
            else
            {
                inputLabel.Visible = false;
                dtdLabel.Visible = false;
                inputLocationFilter.Visible = false;
            }
        }

        private void FilterRunBtn_Click(object sender, EventArgs e)
        {
            currentLogBox = logBoxFilter;
            if (string.IsNullOrEmpty(filterInputPath) && typeComboBoxFilter.SelectedIndex != 1)
            {
                Error("No input file selected");
                return;
            }
            else if (typeComboBoxFilter.SelectedIndex == -1)
            {
                Error("No date set type selected");
                return;
            }
            else if (!File.Exists(filterInputPath))
            {
                Error("Input file does not exist");
                return;
            }

            // Ask for output location if not provided in config file
            if (string.IsNullOrEmpty(filterOutputPath))
            {
                filterOutputPath = GetFolderLocation(out bool success);
                if (!success)
                    return;
                outputLocationFilter.Text = filterOutputPath;
            }
            RunFilter();
        }

        /// <summary>
        /// Runs the converter on the currently selected data set
        /// </summary>
        private void RunFilter()
        {
            switch (typeComboBoxFilter.SelectedIndex)
            {
                case 0:
                    worker = new DblpFilter(context, filterInputPath, filterOutputPath);
                    break;
                case 1:
                    worker = new PubMedFilter(context, filterInputPath, filterOutputPath);
                    break;
                case 2:
                    worker = new PureFilter(context, filterInputPath, filterOutputPath);
                    break;
                default:
                    worker = new DblpFilter(context, filterInputPath, filterOutputPath);
                    break;
            }

            // Check if input file is the expected data set
            Filter.Filter filter = worker as Filter.Filter;
            if (typeComboBoxFilter.SelectedIndex != 1 && !filter.CheckFile(filterInputPath))
            {
                Error("Selected input file is not valid");
                return;
            }

            currentProgressLabel = progressLabelFilter;
            currentProgressBar = progressBarFilter;
            filter.logActions = logCheckBoxFilter.Checked;
            filter.ActionCompleted += (object sender, ActionCompletedEventArgs ace) => { Log(ace.description); };
            ToggleRunButtons();
            progressBarFilter.Value = 0;
            progressLabelFilter.Text = "0%";
            Log($"Parsing {typeComboBoxFilter.SelectedItem} data set...");
            bgWorker.RunWorkerAsync();
        }
        #endregion

        #region Importer UI methods
        private void ImporterRunBtn_Click(object sender, EventArgs e)
        {
            currentLogBox = logBoxImporter;
            // Check if input path is valid and ask for input if not
            if (string.IsNullOrEmpty(importerInputPath))
            {
                importerInputPath = GetFileLocation();
                if (importerInputPath == "")
                    return;
                inputLocationImporter.Text = importerInputPath;
            }
            
            // Check that the input file exists
            if (!File.Exists(importerInputPath))
            {
                Error("Selected input file does not exist");
                return;
            }

            worker = new Importer.Importer(context, importerInputPath, data);

            currentProgressLabel = progressLabelImporter;
            currentProgressBar = progressBarImporter;
            worker.logActions = logCheckBoxImporter.Checked;
            worker.ActionCompleted += (object s, ActionCompletedEventArgs ace) => { Log(ace.description); };
            ToggleRunButtons();
            progressBarImporter.Value = 0;
            progressLabelImporter.Text = "0%";
            Log($"Parsing native JSON file...");
            bgWorker.RunWorkerAsync();
        }

        private void ImporterInputLocation_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                importerInputPath = dialog.FileName;
                inputLocationImporter.Text = importerInputPath;
            }
        }

        private void DownLoad_Articles_Click(object sender, EventArgs e)
        {
            if (data.pubCount == 0)
            {
                Error("Database does not contain any publications");
                return;
            }
            string outputPath = GetFolderLocation(out bool success);
            if (!success)
            {
                Error("No output path selected");
                return;
            }

            Log("Downloading pdf's for articles in database");
            foreach (Article article in data.articles.Values)
            {
                try
                {
                    if (!string.IsNullOrEmpty(article.pdfLink))
                        pdfFixer.FindInfo(article.pdfLink, article.id, false, outputPath);
                    else
                        pdfFixer.FindInfo(article.doi, article.id, true, outputPath);
                }
                catch (Exception ex) 
                {
                    Error(ex.Message);
                }
            }
            foreach (Inproceedings inpr in data.inproceedings.Values)
            {
                try
                {
                    if (!string.IsNullOrEmpty(inpr.pdfLink))
                        pdfFixer.FindInfo(inpr.pdfLink, inpr.id, false, outputPath);
                    else
                        pdfFixer.FindInfo(inpr.doi, inpr.id, true, outputPath);
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }
            }
            Log("Finished downloading pdf's");
            Log($"Pdf's can be found in the following directory: {outputPath}");
        }

        private void Export_Json_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("pizza.json"))
                {
                    sw.Write(data.ToJson());
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                return;
            }
        }

        private void Query_Json_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder("\'");

                switch (comboBoxApi.SelectedIndex)
                {
                    case 0:                  
                        HashSet<Author> encounteredAuthorsA = new HashSet<Author>();
                        HashSet<Journal> encounteredJournals = data.ArticlesToJson(sb, articles, encounteredAuthorsA);

                        sb.Append(",");

                        (HashSet<Organization> organizations, HashSet<Person> persons) comboA = data.AuthorsToJson(sb, encounteredAuthorsA);

                        sb.Append(",");

                        data.OrganizationsToJson(sb, comboA.organizations);

                        sb.Append(",");

                        data.PersonsToJson(sb, comboA.persons);

                        break;
                    case 1:
                        HashSet<Author> encounteredAuthorsI = new HashSet<Author>();
                        HashSet<Proceedings> encounteredproceedings = data.InproceedingsToJson(sb, inproceedings, encounteredAuthorsI);

                        sb.Append(",");

                        data.ProceedingsToJson(sb, encounteredproceedings);

                        sb.Append(",");

                        (HashSet<Organization> organizations, HashSet<Person> persons) comboI = data.AuthorsToJson(sb, encounteredAuthorsI);

                        sb.Append(",");

                        data.OrganizationsToJson(sb, comboI.organizations);

                        sb.Append(",");

                        data.PersonsToJson(sb, comboI.persons);

                        break;
                    case 2:
                        (HashSet<Organization> organizations, HashSet<Person> persons) comboAu = data.AuthorsToJson(sb, authors);

                        sb.Append(",");

                        data.OrganizationsToJson(sb, comboAu.organizations);

                        sb.Append(",");

                        data.PersonsToJson(sb, comboAu.persons);

                        break;
                    case 3:
                        data.PersonsToJson(sb, persons);
                        break;
                    case 4:
                        data.JournalsToJson(sb, journals);
                        break;
                    case 5:
                        data.ProceedingsToJson(sb, proceedings);
                        break;
                    case 6:
                        data.OrganizationsToJson(sb, organizations);
                        break;
                }
                sb.Append("\'");

                using (StreamWriter sw = new StreamWriter("pizza.json"))
                {
                    sw.Write(sb);
                }

            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }
        #endregion

        #region API UI methods
        private void UpdateDbStatistics()
        {
            articleCount.Text = data.articles.Count.ToString();
            inproceedingCount.Text = data.inproceedings.Count.ToString();
            authorCount.Text = data.authors.Count.ToString();
            journalCount.Text = data.journals.Count.ToString();
            proceedingCount.Text = data.proceedings.Count.ToString();
            organizationCount.Text = data.organizations.Count.ToString();
        }

        private void SetupDbStatistics()
        {
            // Add labels to db stats panel for API
            Label lbl = new Label();
            lbl.Text = "Nr of articles:";
            dbStatsPanel.Controls.Add(lbl, 0, 0);
            articleCount = new Label();
            articleCount.Text = "0";
            //articleCount.Width = 10;
            dbStatsPanel.Controls.Add(articleCount, 1, 0);
            lbl = new Label();
            lbl.Text = "Nr of inproceedings:";
            lbl.AutoSize = true;
            dbStatsPanel.Controls.Add(lbl, 0, 1);
            inproceedingCount = new Label();
            inproceedingCount.Text = "0";
            //inproceedingCount.Width = 10;
            dbStatsPanel.Controls.Add(inproceedingCount, 1, 1);
            lbl = new Label();
            lbl.Text = "Nr of authors:";
            dbStatsPanel.Controls.Add(lbl, 0, 2);
            authorCount = new Label();
            authorCount.Text = "0";
            //authorCount.Width = 10;
            dbStatsPanel.Controls.Add(authorCount, 1, 2);
            lbl = new Label();
            lbl.Text = "Nr of journals:";
            dbStatsPanel.Controls.Add(lbl, 0, 3);
            journalCount = new Label();
            journalCount.Text = "0";
            //journalCount.Width = 10;
            dbStatsPanel.Controls.Add(journalCount, 1, 3);
            lbl = new Label();
            lbl.Text = "Nr of proceedings:";
            dbStatsPanel.Controls.Add(lbl, 0, 4);
            proceedingCount = new Label();
            proceedingCount.Text = "0";
            //proceedingCount.Width = 10;
            dbStatsPanel.Controls.Add(proceedingCount, 1, 4);
            lbl = new Label();
            lbl.Text = "Nr of organizations:";
            dbStatsPanel.Controls.Add(lbl, 0, 5);
            organizationCount = new Label();
            organizationCount.Text = "0";
            //organizationCount.Width = 10;
            dbStatsPanel.Controls.Add(organizationCount, 1, 5);
        }


        private void ApiRunBtn_Click(object sender, EventArgs e)
        {
            currentLogBox = logBoxApi;
            if (apiQuery.Text == "Type your query here..." || string.IsNullOrEmpty(apiQuery.Text))
            {
                Error("No input query given");
                return;
            }
            if (comboBoxApi.SelectedIndex == -1)
            {
                Error("The type of items to search for was not specified");
                return;
            }
            if (comboBoxSearch.SelectedIndex == -1)
            {
                Error("The search method to use was not specified");
                return;
            }

            (string, string)[] args = ParseQuery(out bool success);
            if (!success)
                return;

            Log("Querying the database...");
            API api = new API(data);
            SearchType st;
            if (comboBoxSearch.SelectedIndex == 0)
                st = SearchType.Exact;
            else
                st = SearchType.Loose;

            try
            {
                switch (comboBoxApi.SelectedIndex)
                {
                    case 0:
                        HandleQueryResult(articles = api.Search<Article>(SearchDomain.Articles, st, args));
                        break;
                    case 1:
                        HandleQueryResult(inproceedings = api.Search<Inproceedings>(SearchDomain.Inproceedings, st, args));
                        break;
                    case 2:
                        HandleQueryResult(authors = api.Search<Author>(SearchDomain.Authors, st, args));
                        break;
                    case 3:
                        HandleQueryResult(persons = api.Search<Person>(SearchDomain.Persons, st, args));
                        break;
                    case 4:
                        HandleQueryResult(journals = api.Search<Journal>(SearchDomain.Journals, st, args));
                        break;
                    case 5:
                        HandleQueryResult(proceedings = api.Search<Proceedings>(SearchDomain.Proceedings, st, args));
                        break;
                    case 6:
                        HandleQueryResult(organizations = api.Search<Organization>(SearchDomain.Organizations, st, args));
                        break;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void HandleQueryResult<T>(HashSet<T> results)
        {
            // Do something with results..

            Log($"{results.Count} items found");
        }

        /// <summary>
        /// Parses a query for the api into the format needed by the Search method
        /// </summary>
        /// <returns>A boolean indicating whether the query is valid or not</returns>
        private (string,string)[] ParseQuery(out bool success)
        {
            string[] ps = apiQuery.Text.Split('\u002C');
            if (ps.Length < 1)
            {
                success = false;
                return new (string, string)[0];
            }

            List<(string, string)> predicates = new List<(string, string)>();
            foreach (string p in ps)
            {
                string[] parts = p.Split('=');
                if (parts.Length < 2)
                    continue;

                string attr = parts[0].Trim(' ');
                string val = parts[1].Trim(' ');
                
                if (API.possibleArgs.Contains(attr))
                    predicates.Add((attr, val));
            }

            if (predicates.Count == 0)
            {
                Error("The query did not contain any valid arguments");
                success = false;
                return new (string, string)[0];
            }

            success = true;
            return predicates.ToArray();
        }
        #endregion
    }
}
