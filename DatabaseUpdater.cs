using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using HtmlAgilityPack;
using Neo4j.Driver;

namespace ResearchDashboard
{
    abstract class DatabaseUpdater
    {
        // Possible superclass to be used in the 
    }

    class DblpUpdater : DatabaseUpdater
    {
        private DateTime prevMostRecent;
        private DateTime currentMostRecent;
        private string dbUsername;
        private string dbPassword;

        public DblpUpdater(DateTime prevMostRecent, string dbUsername, string dbPassword)
        {
            this.prevMostRecent = prevMostRecent;
            this.dbUsername = dbUsername;
            this.dbPassword = dbPassword;
        }

        // Parse a dblp XML file, returns the most recent date of all added articles
        public DateTime ParseXML(string path)
        {
            Console.WriteLine("Parsing xml data set...");

            // Setup settings for the XmlReader
            XmlReaderSettings settings = new XmlReaderSettings();
            //settings.CheckCharacters = false;
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.XmlResolver = new XmlUrlResolver();
            settings.ValidationEventHandler += new ValidationEventHandler(OnValidationEvent);

            FileStream fs = File.Open(path, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs, settings);
            reader.MoveToContent(); // Moves to the <dblp> node
            reader.Read(); // Read one line to get into the children of the <dblp> node
            
            // For every <article> node that can be found
            while (reader.ReadToNextSibling("article"))
            {
                Article? article = ParseArticle(reader);
                if (article != null)
                    InsertArticle(article);
            }

            return currentMostRecent;
        }

        private void OnValidationEvent(object? sender, ValidationEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
        List<string> links = new List<string>();
        private async Task<List<string>> GetKeyWordsFromLink(string link)
        {
            links.Add(link);
            HttpClient client = new HttpClient();
            using (var result = await client.GetAsync(link))
            {
                string content = await result.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(content);
                var articleAbstract = doc.GetElementbyId("Abs1-content");
                string theAbstract;
                if (articleAbstract != null)
                    theAbstract = articleAbstract.ChildNodes[0].ChildNodes[0].InnerText;
            }
            
            return null;
        }

        private async Task<string> GetRealLink(string link)
        {
            HttpClient client = new HttpClient();
            using (var result = await client.GetAsync(link))
            {
                string content = await result.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(content);
                string realLink = doc.DocumentNode.ChildNodes[0].ChildNodes[2].ChildNodes[0].ChildNodes[0].InnerText;
                return realLink;
            }
            return null;
        }

        // Returns null if the article has an invalid title or link
        private Article? ParseArticle(XmlReader reader)
        {
            // Get the publish date of the article
            string? dateString = reader.GetAttribute("mdate");
            DateTime date = dateString != null ? DateTime.Parse(dateString) : DateTime.MinValue;

            // If no date was found or this date was previously added to the database, don't add it
            if (date <= prevMostRecent)
                return null;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(reader.ReadOuterXml());

            // Get the title
            string title = "";
            XmlNodeList t = xml.GetElementsByTagName("title");
            if (t.Count > 0)
            {
                if (IsValidTitle(t[0].InnerText))
                    title = t[0].InnerText;
                else
                    return null;
            }
            else return null;

            // Get the authors
            List<Person> authors = new List<Person>();
            foreach (XmlNode node in xml.GetElementsByTagName("author"))
                authors.Add(new Person(node.InnerText, ""));

            // Get the doi link
            string doi = "";
            t = xml.GetElementsByTagName("ee");
            if (t.Count > 0)
                doi = t[0].InnerText;
            else
                return null;


            List<string> keywords;
            //SOMETIMES DIRECTLY REAL LINK GIVEN!!!, LIKE WITH ARTICLE Bringing Semantics to Web Services with OWL-S
            string realLink = GetRealLink(doi).Result;
            if(realLink != null)
                keywords = GetKeyWordsFromLink(realLink).Result;

            // If this article is going to be added, update the current latest date if necessary
            if (date > currentMostRecent)
                currentMostRecent = date;

            return new Article(title, authors.ToArray(), doi);
        }

        private bool IsValidTitle(string title)
        {
            return title != "" && title != "(was never published)" && title != "(error)";
        }

        private async void InsertArticle(Article article)
        {
            // Insert article into database..
            Console.WriteLine("Inserting article:" + article.title);
            string art = $"(a:Article {{title:'{Validate(article.title)}', link:'{article.link}'}})";
            
            // ==> Replace the below username/password
            using var driver = GraphDatabase.Driver("bolt://localhost", AuthTokens.Basic(dbUsername, dbPassword));
            using var session = driver.AsyncSession();

            // First, add the article to the database
            await session.RunAsync($"CREATE {art}");

            // Add relationship between this article and each of its authors
            foreach (Person person in article.authors)
            {
                // Then, add a relation from each person to the created article (and create person if not yet in database)
                string query = $"MATCH {art} MERGE (p:Person {{name:'{Validate(person.name)}',orcid:'{Validate(person.orcid)}'}}) MERGE (a)-[:WRITTEN_BY]->(p) RETURN NULL";
                await session.RunAsync(query);
            }
        }

        // Validate a string for use in a Cypher query
        private string Validate(String str)
        {
            return str.Replace("'", "\\'");
        }
    }

    struct Person 
    {
        public string name;
        public string orcid;

        public Person(string name, string orcid)
        {
            this.name = name;
            this.orcid = orcid;
        }
    }

    class Article
    {
        public string title;
        public Person[] authors;
        public string link;

        public Article(string title, Person[] authors, string link)
        {
            this.title = title;
            this.authors = authors;
            this.link = link;
        }
    }
}
