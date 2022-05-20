using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace Converter
{
    abstract class Converter
    {
        protected string path;
        private bool arrayStarted;
        protected BackgroundWorker worker;
        public bool logActions;
        protected Publication item;

        // Progress variables
        protected double progress;
        protected double progressIncrement;
        protected int prevProgress;

        // Get the type of the parser (i.e. DBLP, PubMed etc.)
        public override abstract string ToString();
        // Checks if given file corresponds to the correct type of data set
        public abstract bool CheckFile(string path);
        // Parses a file and writes the result to the given output location
        public abstract void ParseData(string inputPath);
        public abstract bool ParsePublicationXml(XmlReader reader);

        public event EventHandler<ActionCompletedEventArgs> ActionCompleted;
        private readonly SynchronizationContext context;

        public Converter(SynchronizationContext context)
        {
            this.context = context;
        }

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

        protected void ParseXml(string path, XmlReaderSettings settings, string[] nodeNames)
        {
            FileStream fs = File.OpenRead(path);
            XmlReader reader = XmlReader.Create(fs, settings);
            reader.MoveToContent();

            // Read through entire document, parsing any publication with one of specified nodeNames
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    Array.ForEach<string>(nodeNames, (string nodeName) =>
                    {
                        if (nodeName == reader.Name)
                            if (ParsePublicationXml(reader))
                                WriteToOutput();
                    });
                }
            }

            fs.Close();
        }

        // Write item to output JSON
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

        // Increment progress and update if necessary
        protected void UpdateProgress()
        {
            progress = Math.Min(100.0, progress + progressIncrement);
            if ((int)progress > prevProgress)
            {
                prevProgress = (int)progress;
                worker.ReportProgress(prevProgress);
            }
        }

        // Report item being parsed
        protected void ReportAction(string description)
        {
            if (logActions)
                context.Post(new SendOrPostCallback(RaiseActionEvent), description);
        }

        private void RaiseActionEvent(object state)
        {
            ActionCompleted(this, new ActionCompletedEventArgs((string)state));
        }
    }

    public class ActionCompletedEventArgs : EventArgs
    {
        public string description;

        public ActionCompletedEventArgs(string description)
        {
            this.description = description;
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

    struct Person
    {
        public string name { get; set; }
        public string orcid { get; set; }
        public string affiliation { get; set; }

        public Person(string name, string orcid, string affiliation)
        {
            this.name = name;
            this.orcid = orcid;
            this.affiliation = affiliation;
        }
    }
    #endregion
}
