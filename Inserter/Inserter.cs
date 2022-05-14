using Neo4j.Driver;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;

namespace Inserter
{
    class Inserter
    {
        private BackgroundWorker worker;
        public string username;
        public string password;

        private Publication pub;
        private int pubCount;

        public Inserter(string username, string password)
        {
            this.username = username;
            this.password = password;
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

        private async void InsertPublication()
        {
            // Insert article into database..
            Console.WriteLine("Inserting article:" + pub.title);
            string art = "";
            if (pub.type == "article")
            {
                art = $"(a:Article {{title:'{Validate(pub.title)}', link:'{pub.doi}', journal:'{pub.partof}'}})";
            }
            else
            {
                art = $"(a:Inproceeding {{title:'{Validate(pub.title)}', link:'{pub.doi}', conference:'{pub.partof}'}})";
            }
            
            using (IDriver driver = GraphDatabase.Driver("bolt://localhost", AuthTokens.Basic(username, password)))
            {
                using (IAsyncSession session = driver.AsyncSession())
                {
                    // First, add the article to the database
                    await session.RunAsync($"CREATE {art}");

                    // Add relationship between this article and each of its authors
                    foreach (Person person in pub.authors)
                    {
                        // Then, add a relation from each person to the created article (and create person if not yet in database)
                        string query = $"MATCH {art} MERGE (p:Person {{name:'{Validate(person.name)}',orcid:'{Validate(person.orcid)}'}}) MERGE (a)-[:WRITTEN_BY]->(p) RETURN NULL";
                        await session.RunAsync(query);
                    }
                }
            }
        }

        // Validate a string for use in a Cypher query
        private string Validate(String str)
        {
            return str.Replace("'", "\\'");
        }
    }

    #region Data types used for JSON deserialization
    struct Data
    {
        public Publication[] dblp { get; set; }
    }

    struct Publication
    {
        public string type { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string partof { get; set; }
        public string doi { get; set; }
        public Person[] authors { get; set; }

        public Publication(string type, string title, int year, string partof, string doi, Person[] authors)
        {
            this.type = type;
            this.title = title;
            this.year = year;
            this.doi = doi;
            this.partof = partof;
            this.authors = authors;
        }
    }

    struct Person
    {
        public string name { get; set; }
        public string orcid { get; set; }

        public Person(string name, string orcid)
        {
            this.name = name;
            this.orcid = orcid;
        }
    }
    #endregion
}
