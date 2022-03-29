using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Parser
{
    public partial class Form1 : Form
    {
        private string inputPath;
        private string outputPath;

        public Form1()
        {
            InitializeComponent();

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

        private void InputPanel_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("ehlo");
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
            Parser parser;
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
            parser.ItemParsed += (object s, CustomEventArgs ce) => { Log($"Item parsed: '{ce.msg}'"); };
            parser.FileDownloaded += (object s, CustomEventArgs ce) => { Log($"File downloaded: '{ce.msg}'"); };
            parser.ProgressChanged += (object s, ProgressEventArgs pe) => { progressBar.Value = pe.progress; };

            // Check if input file is the expected data set
            if (typeComboBox.SelectedIndex != 1 && !parser.CheckFile(inputPath))
            {
                Error("Selected input file is not valid");
                return;
            }

            try
            {
                Log($"Parsing {typeComboBox.SelectedItem} data set...");
                bool success = parser.Run(inputPath, outputPath);
                if (success)
                    Log("Parsing finished!");
                else
                    Log("Parsing interrupted");
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
    }
}
