using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        PDFInfoFinder pdfFixer = new PDFInfoFinder();

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

            //Data data = new Data();
            //Importer.Publication pub = new Importer.Publication("id", "test123", 2000, "doi", new string[] { "topic1", "topic2" });
            //data.publications.Add(pub.id, pub);
            //API api = new API(data);
            //HashSet<Importer.Publication> res = api.Search<Importer.Publication>("publications", SearchType.Loose, ("title", "test123"));

        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                worker.Run(bgWorker);
            }
            catch (Exception ex)
            {
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
                    Log("Data collected. Run again to parse data of another file, or go the the API tab to query the data");
                }
            }
        }

        private void LogCheckBox_Changed(object sender, EventArgs e)
        {
            if (worker != null)
                worker.logActions = (sender as CheckBox).Checked;
        }

        #region Filter UI methods
        private void FilterInputLocation_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                filterInputPath = dialog.FileName;
                inputLocationFilter.Text = filterInputPath;
            }
        }

        private void FilterOutputLocation_Click(object sender, EventArgs e)
        {
            AskForFilterOutputLocation();
        }

        private bool AskForFilterOutputLocation()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a folder to write the output to.";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                filterOutputPath = dialog.SelectedPath;
                outputLocationFilter.Text = filterOutputPath;
                return true;
            }

            return false;
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
                if (!AskForFilterOutputLocation())
                    return;

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

            currentLogBox = logBoxFilter;
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
            if (string.IsNullOrEmpty(importerInputPath) || !File.Exists(importerInputPath))
            {
                Error("Selected input file does not exist");
                return;
            }
            try
            {
                worker = new Importer.Importer(context, importerInputPath);
            }
            catch
            {
                Error("Input file does not exist");
                return;
            }

            currentLogBox = logBoxImporter;
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
        #endregion

        private void ToggleRunButtons()
        {
            runBtnFilter.Enabled = !runBtnFilter.Enabled;
            runBtnImporter.Enabled = !runBtnImporter.Enabled;
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
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void DownLoad_Articles_Click(object sender, EventArgs e)
        {
            foreach (Article article in data.articles.Values)
            {
                try
                {
                    if (!string.IsNullOrEmpty(article.pdfLink))
                        pdfFixer.FindInfo(article.pdfLink, article.id, false);
                    else
                        pdfFixer.FindInfo(article.doi, article.id, true);
                }
                catch (Exception exp) { }
            }
            foreach (Inproceedings inpr in data.inproceedings.Values)
            {
                try
                {
                    if (!string.IsNullOrEmpty(inpr.pdfLink))
                        pdfFixer.FindInfo(inpr.pdfLink, inpr.id, false);
                    else
                        pdfFixer.FindInfo(inpr.doi, inpr.id, true);
                }
                catch (Exception exp) { }
            }
        }
    }
}
