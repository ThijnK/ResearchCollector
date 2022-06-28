using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private CustomProgressBar currentProgressBar;

        // Custom worker class for filter/importer
        private Worker worker;

        // Filter variables
        private string filterInputPath;
        private string filterOutputPath;

        // Importer variables
        private string importerInputPath;
        private Data data;

        // Outputpath for pdf finder;
        private string pdfFinderOutputPath;

        // API db stats panel labels (some for API tab, some for Importer tab
        Label articleCount, inproceedingCount, authorCount, journalCount, proceedingCount, organizationCount;
        Label articleCountI, inproceedingCountI, authorCountI, journalCountI, proceedingCountI, organizationCountI;
        private string apiOutputPath;

        // Progress bars
        CustomProgressBar pbImporter, pbFilter, pbPdf, pbPubmed;

        /// <summary>
        /// Context used to access UI thread from BackgroundWorker
        /// </summary>
        private readonly System.Threading.SynchronizationContext context;

        public Form1()
        {
            InitializeComponent();

            // Add custom progress bars
            pbImporter = new CustomProgressBar();
            pbImporter.Size = new Size(224, 23);
            pbImporter.Location = new Point(8, 81);
            importerTab.Controls.Add(pbImporter);
            pbFilter = new CustomProgressBar();
            pbFilter.Size = new Size(224, 23);
            pbFilter.Location = new Point(8, 110);
            filterTab.Controls.Add(pbFilter);
            currentProgressBar = pbFilter;
            pbPubmed = new CustomProgressBar();
            pbPubmed.Size = new Size(224, 23);
            pbPubmed.Location = new Point(8, 412);
            filterTab.Controls.Add(pbPubmed);
            pbPdf = new CustomProgressBar();
            pbPdf.Size = new Size(224, 23);
            pbPdf.Location = new Point(8, 175);
            importerTab.Controls.Add(pbPdf);

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
                bgWorker.ReportProgress(currentProgressBar.Value, ex.Message);
                workerInterrupted = true;
            }
        }

        private void WorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                if (!string.IsNullOrEmpty(e.UserState.ToString()))
                    Error(e.UserState.ToString());
            currentProgressBar.Value = e.ProgressPercentage;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ToggleRunButtons();
            if (e.Cancelled || e.Error != null || workerInterrupted)
            {
                Log("Parsing interrupted");
                currentProgressBar.Value = 0;
                workerInterrupted = false;
            }
            else
            {
                Log("Parsing finished!");
                currentProgressBar.Value = 100;
                if (currentProgressBar.Equals(pbFilter)) // Filter finished
                {
                    string outputPath = Path.Combine(filterOutputPath, $"{worker}.json");
                    Log($"Output saved to {outputPath}");
                }
                else if (currentProgressBar.Equals(pbImporter)) // Importer finished
                {
                    // Import data to this form for use by the API
                    data = (worker as Importer.Importer).data;
                    Log($"{data.pubCount} publications parsed. The collected data can be exported on the left or queried using the API");
                    UpdateDbStatistics();
                }
                else // Pdf finder finished
                {
                    Log("Finished downloading pdf's and extracting text");
                    Log($"Text files can be found in the following directory: {pdfFinderOutputPath}");
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
            ApiRunBtn.Enabled = !ApiRunBtn.Enabled;
            downloadPdfBtn.Enabled = !downloadPdfBtn.Enabled;
            Export_Json.Enabled = !Export_Json.Enabled;
            MemoryJson_Memory.Enabled = !MemoryJson_Memory.Enabled;
            pubmedDownloadBtn.Enabled = !pubmedDownloadBtn.Enabled;
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
            // Below can be uncommented to also show a popup, but can hinder usage of UI
            //MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region Filter UI methods
        private void FilterInputLocation_Click(object sender, EventArgs e)
        {
            string path = GetFileLocation();
            if (path != "")
            {
                filterInputPath = path;
                inputLocationFilter.Text = filterInputPath;
            }
        }

        private void FilterOutputLocation_Click(object sender, EventArgs e)
        {
            string path = GetFolderLocation(out bool success);
            if (success)
            {
                filterOutputPath = path;
                outputLocationFilter.Text = filterOutputPath;
            }
        }

        private void FilterComboBox_IndexChanged(object sender, EventArgs e)
        {
            if (typeComboBoxFilter.SelectedIndex == 0)
                dtdLabel.Text = "* DTD file is assumed to be located in the same directory as the input file, as well as to have the same name as the input file.";
            else if (typeComboBoxFilter.SelectedIndex == 2)
                dtdLabel.Text = "* Select the first of the downloaded files. The download date of all files (in the name) is assumed to be the same as this first one.";
            else
                dtdLabel.Text = "* Select any of the downloaded PubMed files. All necessary files are assumed to be located in the same folder.";
        }

        private void pubmedDownloadBtn_Click(object sender, EventArgs e)
        {
            currentLogBox = logBoxFilter;
            string path = GetFolderLocation(out bool success);
            if (!success)
            {
                Error("No output location provided");
                return;
            }

            PubMedFilter filter = new PubMedFilter(context, filterInputPath, filterOutputPath);
            worker = filter;
            ToggleRunButtons();
            pbPubmed.Value = 0;

            Log("Downloading PubMed files...");
            filter.DownloadFiles(path, (int p) => { pbPubmed.Value = p; });
            Log($"Downloading finished, files saved to {path}");
        }

        private void FilterRunBtn_Click(object sender, EventArgs e)
        {
            currentLogBox = logBoxFilter;
            if (string.IsNullOrEmpty(filterInputPath))
            {
                Error("No input file selected");
                return;
            }
            else if (typeComboBoxFilter.SelectedIndex == -1)
            {
                Error("No data set type selected");
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
            if (!filter.CheckFile(filterInputPath))
            {
                Error("Selected input file is not valid");
                return;
            }

            currentProgressBar = pbFilter;
            filter.logActions = logCheckBoxFilter.Checked;
            filter.ActionCompleted += (object sender, ActionCompletedEventArgs ace) => { Log(ace.description); };
            ToggleRunButtons();
            pbFilter.Value = 0;
            Log($"Parsing {typeComboBoxFilter.SelectedItem} data set...");
            bgWorker.RunWorkerAsync();
        }
        #endregion

        #region Importer UI methods
        private void ImporterRunBtn_Click(object sender, EventArgs e)
        {
            // Ask for input location
            importerInputPath = GetFileLocation();
            if (string.IsNullOrEmpty(importerInputPath) || !File.Exists(importerInputPath))
            {
                Error("Selected input file does not exist");
                return;
            }

            worker = new Importer.Importer(context, importerInputPath, data);

            currentProgressBar = pbImporter;
            worker.logActions = logCheckBoxImporter.Checked;
            worker.ActionCompleted += (object s, ActionCompletedEventArgs ace) => { Log(ace.description); };
            ToggleRunButtons();
            pbImporter.Value = 0;
            Log($"Parsing native JSON file...");
            bgWorker.RunWorkerAsync();
        }

        private void MemoryJson_Memory_Click(object sender, EventArgs e)
        {
            string filepath = GetFileLocation();

            Log("Importing database from JSON...");
            BackToMemory backToMemory = new BackToMemory();
            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    backToMemory.JsonToMemory(sr, data);
                }
            }
            catch (Exception exp) { }
            Log("Importing finished!");

            UpdateDbStatistics();
        }

        private void DownloadPdf_Click(object sender, EventArgs e)
        {
            if (data.pubCount == 0)
            {
                Error("Database does not contain any publications");
                return;
            }
            pdfFinderOutputPath = GetFolderLocation(out bool success);
            if (!success)
            {
                Error("No output path selected");
                return;
            }

            worker = new PDFInfoFinder(context, data, pdfFinderOutputPath);

            currentProgressBar = pbPdf;
            worker.logActions = logCheckBoxImporter.Checked;
            worker.ActionCompleted += (object s, ActionCompletedEventArgs ace) => { Log(ace.description); };
            ToggleRunButtons();
            pbPdf.Value = 0;
            Log("Downloading pdf's for articles in database...");
            bgWorker.RunWorkerAsync();
        }

        private void Export_Json_Click(object sender, EventArgs e)
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

            Log("Exporting database to JSON...");
            string path = Path.Combine(outputPath, $"export_{DateTimeOffset.Now.ToUnixTimeSeconds()}.json");
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(data.ToJson().ToString());
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            Log($"Exporting finished! Output saved to {path}");
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
            articleCountI.Text = data.articles.Count.ToString();
            inproceedingCountI.Text = data.inproceedings.Count.ToString();
            authorCountI.Text = data.authors.Count.ToString();
            journalCountI.Text = data.journals.Count.ToString();
            proceedingCountI.Text = data.proceedings.Count.ToString();
            organizationCountI.Text = data.organizations.Count.ToString();
        }

        private void SetupDbStatistics()
        {
            // Add labels to db stats panel for API
            Label lbl = new Label(); lbl.Text = "Nr of articles:";
            dbStatsPanel.Controls.Add(lbl, 0, 0);
            lbl = new Label(); lbl.Text = "Nr of articles:";
            dbStatsPanelImporter.Controls.Add(lbl, 0, 0);
            articleCount = new Label(); articleCount.Text = "0";
            dbStatsPanel.Controls.Add(articleCount, 1, 0);
            articleCountI = new Label(); articleCountI.Text = "0";
            dbStatsPanelImporter.Controls.Add(articleCountI, 1, 0);

            lbl = new Label(); lbl.Text = "Nr of inproceedings:"; lbl.Width = 150;
            dbStatsPanel.Controls.Add(lbl, 0, 1);
            lbl = new Label(); lbl.Text = "Nr of inproceedings:"; lbl.Width = 150;
            dbStatsPanelImporter.Controls.Add(lbl, 0, 1);
            inproceedingCount = new Label(); inproceedingCount.Text = "0";
            dbStatsPanel.Controls.Add(inproceedingCount, 1, 1);
            inproceedingCountI = new Label(); inproceedingCountI.Text = "0";
            dbStatsPanelImporter.Controls.Add(inproceedingCountI, 1, 1);

            lbl = new Label(); lbl.Text = "Nr of authors:";
            dbStatsPanel.Controls.Add(lbl, 0, 2);
            lbl = new Label(); lbl.Text = "Nr of authors:";
            dbStatsPanelImporter.Controls.Add(lbl, 0, 2);
            authorCount = new Label(); authorCount.Text = "0";
            dbStatsPanel.Controls.Add(authorCount, 1, 2);
            authorCountI = new Label(); authorCountI.Text = "0";
            dbStatsPanelImporter.Controls.Add(authorCountI, 1, 2);

            lbl = new Label(); lbl.Text = "Nr of journals:";
            dbStatsPanel.Controls.Add(lbl, 0, 3);
            lbl = new Label(); lbl.Text = "Nr of journals:";
            dbStatsPanelImporter.Controls.Add(lbl, 0, 3);
            journalCount = new Label(); journalCount.Text = "0";
            dbStatsPanel.Controls.Add(journalCount, 1, 3);
            journalCountI = new Label(); journalCountI.Text = "0";
            dbStatsPanelImporter.Controls.Add(journalCountI, 1, 3);

            lbl = new Label(); lbl.Text = "Nr of proceedings:";
            dbStatsPanel.Controls.Add(lbl, 0, 4);
            lbl = new Label(); lbl.Text = "Nr of proceedings:";
            dbStatsPanelImporter.Controls.Add(lbl, 0, 4);
            proceedingCount = new Label(); proceedingCount.Text = "0";
            dbStatsPanel.Controls.Add(proceedingCount, 1, 4);
            proceedingCountI = new Label(); proceedingCountI.Text = "0";
            dbStatsPanelImporter.Controls.Add(proceedingCountI, 1, 4);

            lbl = new Label(); lbl.Text = "Nr of organizations:";
            dbStatsPanel.Controls.Add(lbl, 0, 5);
            lbl = new Label(); lbl.Text = "Nr of organizations:";
            dbStatsPanelImporter.Controls.Add(lbl, 0, 5);
            organizationCount = new Label(); organizationCount.Text = "0";
            dbStatsPanel.Controls.Add(organizationCount, 1, 5);
            organizationCountI = new Label(); organizationCountI.Text = "0";
            dbStatsPanelImporter.Controls.Add(organizationCountI, 1, 5);
        }

        private void OutputLocationApi_Click(object sender, EventArgs e)
        {
            string path = GetFolderLocation(out bool success);
            if (success)
            {
                apiOutputPath = path;
                outputLocationApi.Text = apiOutputPath;
            }
        }

        private void ApiRunBtn_Click(object sender, EventArgs e)
        {
            currentLogBox = logBoxApi;
            if (data.pubCount == 0)
            {
                Error("Database does not contain any publications");
                return;
            }
            if (string.IsNullOrEmpty(apiOutputPath) || !Directory.Exists(apiOutputPath))
                OutputLocationApi_Click(this, new EventArgs());
            if (string.IsNullOrEmpty(apiOutputPath) || !Directory.Exists(apiOutputPath))
            {
                Error("No output path selected");
                return;
            }

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
                        HandleQueryResult(api.Search<Article>(SearchDomain.Articles, st, args), apiOutputPath);
                        break;
                    case 1:
                        HandleQueryResult(api.Search<Inproceedings>(SearchDomain.Inproceedings, st, args), apiOutputPath);
                        break;
                    case 2:
                        HandleQueryResult(api.Search<Author>(SearchDomain.Authors, st, args), apiOutputPath);
                        break;
                    case 3:
                        HandleQueryResult(api.Search<Person>(SearchDomain.Persons, st, args), apiOutputPath);
                        break;
                    case 4:
                        HandleQueryResult(api.Search<Journal>(SearchDomain.Journals, st, args), apiOutputPath);
                        break;
                    case 5:
                        HandleQueryResult(api.Search<Proceedings>(SearchDomain.Proceedings, st, args), apiOutputPath);
                        break;
                    case 6:
                        HandleQueryResult(api.Search<Organization>(SearchDomain.Organizations, st, args), apiOutputPath);
                        break;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void HandleQueryResult<T>(HashSet<T> results, string outputPath)
        {
            Log($"{results.Count} items found");
            Log($"Exporting results to JSON...");
            try
            {
                StringBuilder sb = new StringBuilder($"{{\n\t\"query\": \"{apiQuery.Text}\",\n\t");

                switch (comboBoxApi.SelectedIndex)
                {
                    case 0:
                        HashSet<Author> encounteredAuthorsA = new HashSet<Author>();
                        data.ArticlesToJson(sb, results as HashSet<Article>, encounteredAuthorsA);

                        sb.Append(",\n\t");

                        data.AuthorsToJson(sb, encounteredAuthorsA);
                        break;
                    case 1:
                        HashSet<Author> encounteredAuthorsI = new HashSet<Author>();
                        data.InproceedingsToJson(sb, results as HashSet<Inproceedings>, encounteredAuthorsI);

                        sb.Append(",\n\t");

                        data.AuthorsToJson(sb, encounteredAuthorsI);
                        break;
                    case 2:
                        data.AuthorsToJson(sb, results as HashSet<Author>);
                        break;
                    case 3:
                        data.PersonsToJson(sb, results as HashSet<Person>);
                        break;
                    case 4:
                        data.JournalsToJson(sb, results as HashSet<Journal>);
                        break;
                    case 5:
                        data.ProceedingsToJson(sb, results as HashSet<Proceedings>);
                        break;
                    case 6:
                        data.OrganizationsToJson(sb, results as HashSet<Organization>);
                        break;
                }
                sb.Append("\n}");

                string path = Path.Combine(outputPath, $"query_{DateTimeOffset.Now.ToUnixTimeSeconds()}.json");
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(sb);
                }
                Log($"Query results successfully exported to file {path}");
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        /// <summary>
        /// Parses a query for the api into the format needed by the Search method
        /// </summary>
        /// <returns>A boolean indicating whether the query is valid or not</returns>
        private (string, string)[] ParseQuery(out bool success)
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

    /// <summary>
    /// Custom class for the progress bars.
    /// Source: https://stackoverflow.com/questions/3529928/how-do-i-put-text-on-progressbar.
    /// </summary>
    class CustomProgressBar : ProgressBar
    {
        public CustomProgressBar()
        {
            // Modify the ControlStyles flags
            //http://msdn.microsoft.com/en-us/library/system.windows.forms.controlstyles.aspx
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;

            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            rect.Inflate(-1, -1);
            if (Value > 0)
            {
                // As we doing this ourselves we need to draw the chunks on the progress bar
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);
                ProgressBarRenderer.DrawHorizontalChunks(g, clip);
            }

            // Set the Display text (Either a % amount or our custom text
            int percent = (int)(((double)this.Value / (double)this.Maximum) * 100);
            string text = percent.ToString() + '%';

            using (Font f = new Font(FontFamily.GenericSansSerif, 10))
            {
                SizeF len = g.MeasureString(text, f);
                // Calculate the location of the text (the middle of progress bar)
                // Point location = new Point(Convert.ToInt32((rect.Width / 2) - (len.Width / 2)), Convert.ToInt32((rect.Height / 2) - (len.Height / 2)));
                Point location = new Point(Convert.ToInt32((Width / 2) - len.Width / 2), Convert.ToInt32((Height / 2) - len.Height / 2));
                // The commented-out code will centre the text into the highlighted area only. This will centre the text regardless of the highlighted area.
                // Draw the custom text
                g.DrawString(text, f, Brushes.Black, location);
            }
        }
    }
}
