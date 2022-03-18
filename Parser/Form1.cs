using System;
using System.Threading.Tasks;
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
        }

        private void InputPanel_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputPath = dialog.FileName;
                inputLabel.Text = inputPath;
            }
        }

        private void OutputPanel_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                outputPath = dialog.SelectedPath;
                outputLabel.Text = outputPath;
            }
        }

        private void ParseBtn_Click(object sender, EventArgs e)
        {
            if (inputPath == "" || inputPath == null)
            {
                Error("No input file selected");
                return;
            }
            else if (outputPath == "" || outputPath == null)
            {
                Error("No output folder selected");
                return;
            }
            else if (typeComboBox.SelectedIndex == -1)
            {
                Error("No date set type selected");
                return;
            }

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

            // Check if input file is the expected data set
            if (!parser.CheckFile(inputPath))
            {
                Error("Selected input file is not valid");
                return;
            }

            try
            {
                bool success = parser.ParseFile(inputPath, outputPath);
                if (success)
                    Log("Parsing finished!");
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void ItemParsed(object sender, ItemParsedEventArgs e)
        {
            Log($"Parsing item: {e.title}");
        }

        // Log a msg to the log
        private void Log(string msg)
        {
            logBox.AppendText($"{msg}{Environment.NewLine}");
        }

        // Display an error in a message box
        private void Error(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
