using Neo4j.Driver;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;

namespace Importer
{
    class Importer
    {
        private BackgroundWorker worker;

        private Publication pub;
        private int pubCount;

        public Importer()
        {

        }

        // Insert content from given JSON file into database
        public void Run(string path, BackgroundWorker worker)
        {
            this.worker = worker;
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                sr.ReadLine();
                string line = "";
                string json = "";
                while ((line = sr.ReadLine()) != null)
                {
                    json = line.Remove(line.Length - 1, 1);
                    pub = JsonSerializer.Deserialize<Publication>(json);
                    HandlePublication();
                }
            }
        }

        private void HandlePublication()
        {
            pubCount++;
            //worker.ReportProgress(pubCount++ / ..);
            
            // Get text from pdf
            string text = GetText();
        }

        private string GetText()
        {
            // Doi can be accessed as pub.doi

            // (1) Get pdf by searching Google
            // (2) Extract text from that

            return "";
        }

        
        // Old code that may or may not be of use:

        /*
        List<string> links = new List<string>();
        private async Task<List<string>> GetKeyWordsFromLink(string link)
        {
            links.Add(link);
            HttpClient client = new HttpClient();
            using (var result = await client.GetAsync(link))
            {
                string content = await result.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument(content);
                doc.LoadHtml(content);
                var articleAbstract = doc.GetElementById("Abs1-content");
                string theAbstract;
                if (articleAbstract != null)
                    theAbstract = articleAbstract.ChildNodes[0].ChildNodes[0].InnerText;
                //sommige(/misschien erg veel) die hier komen zijn niet van het type die extractbaar zijn met de methode hierboven
                //omdat het andere type links zijn, zoals gelijk al de pdf.
                //dus hier moet nog flink wat check gebeuren op wat voor content het is
            }

            return null;
        }
        */
    }
}
