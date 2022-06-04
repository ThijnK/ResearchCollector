using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ResearchCollector.Filter;
using ResearchCollector.Importer;

namespace ResearchCollector
{
    public partial class Form1 : Form
    {
        private string inputPath;
        private string outputPath;
        private BackgroundWorker worker;
        private bool workerInterrupted;
        private ResearchCollector.Filter.Filter filter;

        /// <summary>
        /// Context used to access UI thread from BackgroundWorker
        /// </summary>
        private readonly System.Threading.SynchronizationContext context;

        public Form1()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            this.context = WindowsFormsSynchronizationContext.Current;
            
            // Use presets if provided
            if (File.Exists("../../config.txt"))
            {
                using (StreamReader sr = new StreamReader("../../config.txt"))
                {
                    inputPath = sr.ReadLine();
                    outputPath = sr.ReadLine();
                    inputLocation.Text = inputPath;
                    outputLocation.Text = outputPath;
                    outputLabel.Visible = true;
                    outputLocation.Visible = true;
                }
            }

            //Data data = new Data();
            //Importer.Publication pub = new Importer.Publication("id", "test123", 2000, "doi", new string[] { "topic1", "topic2" });
            //data.publications.Add(pub.id, pub);
            //API api = new API(data);
            //HashSet<Importer.Publication> res = api.Search<Importer.Publication>("publications", SearchType.Loose, ("title", "test123"));

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runBtn.Enabled = true;
            if (e.Cancelled || e.Error != null || workerInterrupted)
            {
                Log("Parsing interrupted");
                progressLabel.Text = "";
                progressBar.Value = 0;
                workerInterrupted = false;
            }
            else
            {
                Log("Parsing finished!");
                progressLabel.Text = "100%";
                progressBar.Value = 100;
                Log($"Output saved to {outputPath}\\{filter.ToString()}.json");
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
                filter.Run(inputPath, outputPath, worker);
            try
            {
            }
            catch (Exception ex)
            {
                worker.ReportProgress(progressBar.Value, ex.Message);
                workerInterrupted = true;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                if (!string.IsNullOrEmpty(e.UserState.ToString()))
                    Error(e.UserState.ToString());
            progressLabel.Text = $"{e.ProgressPercentage}%";
            progressBar.Value = e.ProgressPercentage;
        }

        private void inputLocation_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputPath = dialog.FileName;
                inputLocation.Text = inputPath;
            }
        }

        private void outputLocation_Click(object sender, EventArgs e)
        {
            AskForOutputLocation();
        }

        private bool AskForOutputLocation()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a folder to write the output to.";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                outputPath = dialog.SelectedPath;
                outputLocation.Text = outputPath;
                return true;
            }

            return false;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex == 0)
            {
                inputLabel.Visible = true;
                dtdLabel.Text = "* DTD file is assumed to be located in the same directory as the input file, as well as to have the same name as the input file.";
                dtdLabel.Visible = true;
                inputLocation.Visible = true;
            }
            else if (typeComboBox.SelectedIndex == 2)
            {
                inputLabel.Visible = true;
                dtdLabel.Text = "* Select the first of the downloaded files. The download date of all files (in the name) is assumed to be the same as this first one.";
                dtdLabel.Visible = true;
                inputLocation.Visible = true;
            }
            else
            {
                inputLabel.Visible = false;
                dtdLabel.Visible = false;
                inputLocation.Visible = false;
            }
        }

        private void logCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (filter != null)
                filter.logActions = logCheckBox.Checked;
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputPath) && typeComboBox.SelectedIndex != 1)
            {
                Error("No input file selected");
                return;
            }
            else if (typeComboBox.SelectedIndex == -1)
            {
                Error("No date set type selected");
                return;
            }

            // Ask for output location if not provided in config file
            if (string.IsNullOrEmpty(outputPath))
                if (!AskForOutputLocation())
                    return;

            RunFilter();
        }

        /// <summary>
        /// Runs the converter on the currently selected data set
        /// </summary>
        private void RunFilter()
        {
            switch (typeComboBox.SelectedIndex)
            {
                case 0:
                    filter = new DblpFilter(context);
                    break;
                case 1:
                    filter = new PubMedFilter(context);
                    break;
                case 2:
                    filter = new PureFilter(context);
                    break;
                default:
                    filter = new DblpFilter(context);
                    break;
            }

            // Check if input file is the expected data set
            if (typeComboBox.SelectedIndex != 1 && !filter.CheckFile(inputPath))
            {
                Error("Selected input file is not valid");
                return;
            }

            filter.logActions = logCheckBox.Checked;
            filter.ActionCompleted += (object sender, ActionCompletedEventArgs ace) => { Log(ace.description); };
            runBtn.Enabled = false;
            progressBar.Value = 0;
            progressLabel.Text = "0%";
            Log($"Parsing {typeComboBox.SelectedItem} data set...");
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Log a msg to the UI
        /// </summary>
        /// <param name="msg">Message to log</param>
        private void Log(string msg)
        {
            logBox.AppendText($"[{DateTime.Now.ToString("H:mm:ss")}] {msg}{Environment.NewLine}");
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
    }
}
