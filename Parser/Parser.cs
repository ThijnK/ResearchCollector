using System;

namespace Parser
{
    abstract class Parser
    {
        // Checks if given file corresponds to the correct type of data set
        public abstract bool CheckFile(string path);
        // Parses a file and writes the result to the given output location
        public abstract bool ParseFile(string inputPath, string outputPath);
        public event EventHandler<ItemParsedEventArgs> ItemParsed;

        public void WriteToFile(Publication pub, string path)
        {
            ItemParsed(this, new ItemParsedEventArgs(pub.title));

            throw new NotImplementedException();
        }
    }

    public class ItemParsedEventArgs : EventArgs
    {
        public string title;

        public ItemParsedEventArgs(string title)
        {
            this.title = title;
        }
    }

    #region Data types for JSON serialization
    abstract class Publication
    {
        public string title { get; set; }
        public int year { get; set; }
        public string link { get; set; }
        public Person[] authors { get; set; }

        public Publication(string title, int year, string link, Person[] authors)
        {
            this.title = title;
            this.year = year;
            this.link = link;
            this.authors = authors;
        }
    }

    class Article : Publication
    {
        public string journal { get; set; }

        public Article(string title, int year, string link, Person[] authors, string journal) : base(title, year, link, authors)
        {
            this.journal = journal;
        }
    }

    class Inproceedings : Publication
    {
        public string conf { get; set; }

        public Inproceedings(string title, int year, string link, Person[] authors, string conf) : base(title, year, link, authors)
        {
            this.conf = conf;
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
