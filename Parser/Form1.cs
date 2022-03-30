using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Parser
{
    public partial class Form1 : Form
    {
        private string inputPath;
        private string outputPath;
        private BackgroundWorker worker;
        private Parser parser;

        public Form1()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

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
                    outputPanel.Visible = true;
                }
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runBtn.Enabled = true;
            if (e.Cancelled || e.Error != null)
            {
                Log("Parsing interrupted");
                progressLabel.Text = "";
                progressBar.Value = 0;
            }
            else
            {
                Log("Parsing finished!");
                progressLabel.Text = "100%";
                progressBar.Value = 100;
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            parser.Run(inputPath, outputPath, worker);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressLabel.Text = $"{e.ProgressPercentage}%";
            progressBar.Value = e.ProgressPercentage;
            Log((string)e.UserState);
        }

        private void InputPanel_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputPath = dialog.FileName;
                inputLocation.Text = inputPath;
            }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex == 0)
            {
                inputLabel.Visible = true;
                dtdLabel.Visible = true;
                inputPanel.Visible = true;
            }
            else
            {
                inputLabel.Visible = false;
                dtdLabel.Visible = false;
                inputPanel.Visible = false;
            }
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

            if (string.IsNullOrEmpty(outputPath))
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    outputPath = dialog.SelectedPath;
                    RunParser();
                }
            }
            else
                RunParser();
        }

        private void RunParser()
        {
            switch (typeComboBox.SelectedIndex)
            {
                case 0:
                    parser = new DblpParser();
                    break;
                case 1:
                    parser = new PubMedParser();
                    break;
                default:
                    parser = new DblpParser();
                    break;
            }

            // Check if input file is the expected data set
            if (typeComboBox.SelectedIndex != 1 && !parser.CheckFile(inputPath))
            {
                Error("Selected input file is not valid");
                return;
            }

            parser.reportProgress = logCheckBox.Checked;
            runBtn.Enabled = false;
            try
            {
                Log($"Parsing {typeComboBox.SelectedItem} data set...");
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        // Log a msg to the log
        private void Log(string msg)
        {
            logBox.AppendText($"[{DateTime.Now.ToString("H:mm:ss")}] {msg}{Environment.NewLine}");
        }

        // Display an error in a message box
        private void Error(string msg)
        {
            Log($"Encountered error: '{msg}'");
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void logCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (parser != null)
                parser.reportProgress = logCheckBox.Checked;
        }
    }
}
