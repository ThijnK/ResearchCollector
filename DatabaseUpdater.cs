using System;
using System.Xml;
using System.Xml.Schema;

namespace ResearchDashboard
{
    abstract class DatabaseUpdater
    {
        // Possible superclass to be used in the future
    }

    class DblpUpdater : DatabaseUpdater
    {
        private DateTime lastUpdated;
        private DateTime currentDate;

        public DblpUpdater(DateTime lastUpdated)
        {
            this.lastUpdated = lastUpdated;
        }

        // Parse a dblp XML file
        public void ParseXML(string path)
        {
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
        }

        private void OnValidationEvent(object? sender, ValidationEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private Article? ParseArticle(XmlReader reader)
        {
            // Get the publish date of the article
            string? date = reader.GetAttribute("mdate");
            // If no date was found or this date was previously added to the database, don't add it
            if (date == null || (currentDate = DateTime.Parse(date)) < lastUpdated)
                return null;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(reader.ReadOuterXml());

            // Get the title
            string title = "Unknown";
            XmlNodeList t = xml.GetElementsByTagName("title");
            if (t.Count > 0)
            {
                if (IsValidTitle(t[0].InnerText))
                    title = t[0].InnerText;
            }

            // Get the authors
            List<Person> authors = new List<Person>();
            foreach (XmlNode node in xml.GetElementsByTagName("author"))
                authors.Add(new Person(node.InnerText, ""));

            // Get the doi link
            string doi = "-";
            t = xml.GetElementsByTagName("ee");
            if (t.Count > 0)
                doi = t[0].InnerText;

            return new Article(title, authors.ToArray(), doi);
        }

        private bool IsValidTitle(string title)
        {
            return title != "" && title != "(was never published)" && title != "(error)";
        }

        private void InsertArticle(Article article)
        {
            // Insert article into database..
            Console.WriteLine("Inserting article:" + article.title);
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
