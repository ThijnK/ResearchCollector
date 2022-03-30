using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Xml;

namespace Parser
{
    abstract class Parser
    {
        protected string path;
        private bool arrayStarted;
        protected BackgroundWorker worker;
        public bool reportProgress;
        protected Publication item;

        // Get the type of the parser (i.e. DBLP, PubMed etc.)
        public override abstract string ToString();
        // Checks if given file corresponds to the correct type of data set
        public abstract bool CheckFile(string path);
        // Parses a file and writes the result to the given output location
        public abstract void ParseData(string inputPath);
        public abstract bool ParsePublicationXml(XmlReader reader);

        public void Run(string inputPath, string outputPath, BackgroundWorker worker)
        {
            this.worker = worker;

            // Set up JSON output file
            string name = ToString();
            path = Path.Combine(outputPath, name + ".json");
            File.WriteAllText(path, $"{{\n\t\"{ToString()}\": [");

            // Parse input and write results to output file
            ParseData(inputPath);

            // Close off JSON output file
            File.AppendAllText(path, "\n\t]\n}");
        }

        protected void ParseXml(string path, XmlReaderSettings settings, string nodeName)
        {
            FileStream fs = File.OpenRead(path);
            XmlReader reader = XmlReader.Create(fs, settings);
            reader.MoveToContent();
            reader.ReadToDescendant(nodeName);

            if (ParsePublicationXml(reader))
                WriteToOutput();
            // Go through every node with the given name that can be found
            while (reader.ReadToNextSibling(nodeName))
            {
                if (ParsePublicationXml(reader))
                    WriteToOutput();
            }
            fs.Close();
        }

        public void WriteToOutput()
        {
            string prepend = ",";
            if (!arrayStarted)
            {
                prepend = "";
                arrayStarted = true;
            }
            File.AppendAllText(path, $"{prepend}\n\t\t{JsonSerializer.Serialize(item)}");
        }
    }

    #region Data types for JSON serialization
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

    //class Article : Publication
    //{
    //    public string journal { get; set; }

    //    public Article(string title, int year, string doi, Person[] authors, string journal) : base(title, year, doi, authors)
    //    {
    //        this.partof = journal;
    //        this.type = "article";
    //    }
    //}

    //class Inproceedings : Publication
    //{
    //    public string conf { get; set; }

    //    public Inproceedings(string title, int year, string doi, Person[] authors, string conf) : base(title, year, doi, authors)
    //    {
    //        this.partof = conf;
    //        this.type = "inproceedings";
    //    }
    //}

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
