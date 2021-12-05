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
            settings.CheckCharacters = false;
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.ValidationType = ValidationType.DTD;

            XmlReader reader = XmlReader.Create(path, settings);
            reader.MoveToContent(); // Moves to the <dblp> node
            reader.Read(); // Read one line to get into the children of the <dblp> node
            
            // For every <article> node that can be found
            while (reader.ReadToNextSibling("article"))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(reader.ReadOuterXml());

                Article article = ParseArticle(xml);
                if (article != null)
                    InsertArticle(article);
            }
        }

        private Article ParseArticle(XmlDocument xml)
        {
            // Check that the date is newer than lastUpdated date
            if (xml.Attributes.Count > 1)
                currentDate = DateTime.Parse(xml.Attributes[1].InnerText);

            // If this date has already been added to the database, don't add it again
            if (currentDate == null || currentDate < lastUpdated)
                return null;

            // Get the title of the article and return null if it's invalid
            string title = "";
            XmlNodeList t = xml.GetElementsByTagName("title");
            if (t.Count > 0)
            {
                title = t[0].InnerText;
                if (title == "" || title == "(was never published)" || title == "(error)")
                    return null;
            }
            else return null;

            // Get the authors of the article
            List<Person> authors = new List<Person>();
            foreach (XmlNode node in xml.GetElementsByTagName("author"))
                authors.Add(new Person(node.InnerText, ""));

            // Get the doi link of the article
            string link = "";
            t = xml.GetElementsByTagName("ee");
            if (t.Count > 0)
                link = t[0].InnerText;

            return new Article(title, authors.ToArray(), link);
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
