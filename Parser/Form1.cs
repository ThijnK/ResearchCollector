using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
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

            byte[] bytes = Encoding.ASCII.GetBytes("<?xml version=\"1.0\" encoding=\"utf - 8\"?>< !DOCTYPE PubmedArticleSet SYSTEM \"http://dtd.nlm.nih.gov/ncbi/pubmed/out/pubmed_190101.dtd\" > ");
            long t = bytes.Length;
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
            downloadPath = $"{Path.GetDirectoryName(inputPath)}\\pubmed.xml.gz";
            FileStream fs;
            if (!File.Exists(downloadPath))
            {
                fs = File.Create(downloadPath);
                fs.Close();
            }

            // Clear the file where we'll write all of our data to
            fs = File.Create(inputPath);
            fs.Close();
            // Path for temporary storage of intermediate results
            string tempPath = $"{Path.GetDirectoryName(inputPath)}\\temp.xml";

            // Go through each of the files making up the data set
            for (int i = 1; i <= 1114; i++) //1114
            {
                string nr = i.ToString("0000");
                string fileName = $"pubmed22n{nr}";
                Uri url = new Uri($"https://ftp.ncbi.nlm.nih.gov/pubmed/baseline/{fileName}.xml.gz");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, downloadPath);
                    AppendXml(i == 1, fileName, tempPath);
                }
            }

            File.Delete(tempPath);
        }

        // Append xml of pubmed file to the combined file
        private void AppendXml(bool first, string fileName, string tempPath)
        {
            
            using (FileStream originalFileStream = File.OpenRead(downloadPath))
            {
                string currentFileName = Path.GetFileName(downloadPath);
                string newFileName = inputPath;

                using (FileStream targetStream = File.OpenWrite(inputPath))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        // Position stream at the end of the file
                        targetStream.Position = targetStream.Length;
                        // Write data to temp file
                        using (FileStream tempStream = File.Create(tempPath))
                        {
                            decompressionStream.CopyTo(tempStream);
                        }
                        // Open temp file and write data except first two lines to target file
                        using (FileStream tempStream = File.OpenRead(tempPath))
                        {
                            if (!first)
                                tempStream.Position = 133;
                            tempStream.CopyTo(targetStream);
                        }
                    }
                }
            }


            Log($"Downloaded file: {fileName}");
        }
    }
}
