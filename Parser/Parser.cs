using System;
using System.IO;
using System.Text.Json;

namespace Parser
{
    abstract class Parser
    {
        private string path;
        private bool arrayStarted;

        // Get the type of the parser (i.e. DBLP, PubMed etc.)
        public override abstract string ToString();
        // Checks if given file corresponds to the correct type of data set
        public abstract bool CheckFile(string path);
        // Parses a file and writes the result to the given output location
        public abstract bool ParseFile(string inputPath);
        public event EventHandler<ItemParsedEventArgs> ItemParsed;

        public bool Run(string inputPath, string outputPath)
        {
            // Set up JSON output file
            string name = ToString();
            path = Path.Combine(outputPath, name + ".json");
            File.WriteAllText(path, $"{{\n\t\"{ToString()}\": [");

            // Parse input and write results to output file
            bool success = ParseFile(inputPath);

            // Close off JSON output file
            File.AppendAllText(path, "\n\t]\n}");
            
            return success;
        }

        public void WriteToOutput(Publication pub)
        {
            ItemParsed(this, new ItemParsedEventArgs(pub.title));

            string prepend = ",";
            if (!arrayStarted)
            {
                prepend = "";
                arrayStarted = true;
            }
            File.AppendAllText(path, $"{prepend}\n\t\t{JsonSerializer.Serialize(pub)}");
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
        public string type { get; set; }
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
            this.type = "article";
        }
    }

    class Inproceedings : Publication
    {
        public string conf { get; set; }

        public Inproceedings(string title, int year, string link, Person[] authors, string conf) : base(title, year, link, authors)
        {
            this.conf = conf;
            this.type = "inproceedings";
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
