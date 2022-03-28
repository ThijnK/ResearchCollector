using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Parser
{
    public partial class Form1 : Form
    {
        private string inputPath;
        private string outputPath;
        private string downloadPath;

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
                    inputLabel.Text = inputPath;
                    outputLabel.Text = outputPath;
                }
            }
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
            if (string.IsNullOrEmpty(inputPath))
            {
                Error("No input file selected");
                return;
            }
            else if (string.IsNullOrEmpty(outputPath))
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
            parser.ItemParsed += ItemParsed;

            // Check if input file is the expected data set
            if (!parser.CheckFile(inputPath))
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
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void ItemParsed(object sender, ItemParsedEventArgs e)
        {
            Log($"Item parsed: '{e.title}'");
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

        private void pubmedDownloadBtn_Click(object sender, EventArgs e)
        {
            downloadPath = @"C:\Users\Thijn Kroon\Downloads\pubmed.xml.gz";
            if (!File.Exists(downloadPath))
            {
                FileStream fs = File.Create(downloadPath);
                fs.Close();

            }

            // Go through each of the files making up the data set
            for (int i = 1; i <= 2; i++) //1114
            {
                string nr = i.ToString("0000");
                Uri url = new Uri($"https://ftp.ncbi.nlm.nih.gov/pubmed/baseline/pubmed22n{nr}.xml.gz");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, downloadPath);
                    AppendXml();
                    //XmlDocument xml = new XmlDocument();
                    //xml.LoadXml(data);
                    //XmlNodeList t = xml.GetElementsByTagName("PubmedArticleSet");
                    //if (t.Count > 0)
                    //{
                    //    GZipStream gs = new GZipStream(data, CompressionMode.Decompress);
                    //}
                }
            }

        }

        private void AppendXml()
        {
            // Append to file
            Log("File downloaded!");
            

            using (FileStream originalFileStream = File.OpenRead(downloadPath))
            {
                string currentFileName = Path.GetFileName(downloadPath);
                string newFileName = currentFileName.Remove(currentFileName.Length - Path.GetExtension(downloadPath).Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(File.OpenWrite(inputPath));
                    }
                }
            }
        }
    }
}
