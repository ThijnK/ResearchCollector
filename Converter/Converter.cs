using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace Converter
{
    abstract class Converter
    {
        /// <summary>
        /// Path of the file to write the output to
        /// </summary>
        protected string path;
        private bool arrayStarted;
        protected BackgroundWorker worker;
        public bool logActions;
        protected Publication item;

        /// <summary>
        /// Current progress
        /// </summary>
        protected double progress;
        /// <summary>
        /// How much the progress is incremented with each publication that is parsed
        /// </summary>
        protected double progressIncrement;
        /// <summary>
        /// Previous progress 
        /// </summary>
        protected int prevProgress;

        /// <returns>The type of the data set being converted, e.g. "dblp" or "pubmed"</returns>
        public override abstract string ToString();
        /// <returns>True if the file at the provided path can be parsed, false otherwise</returns>
        public abstract bool CheckFile(string path);
        /// <summary>
        /// Parses the data found in the file at the given path and writes results to a json file
        /// </summary>
        public abstract void ParseData(string inputPath);
        /// <summary>
        /// Parse an xml node representing an article
        /// </summary>
        /// <param name="reader">XmlReader to parse the publication from, positioned at the start of the node to parse</param>
        /// <returns>True if the publication was parsed successfully, false otherwise</returns>
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

        protected void ParseXml(string path, XmlReaderSettings settings, params string[] nodeNames)
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
                        {
                            if (ParsePublicationXml(reader))
                            {
                                ComputeHash();
                                WriteToOutput();
                            }
                        }
                    });
                }
            }

            fs.Close();
        }

        // Write item to output JSON
        public void WriteToOutput()
        {
            StringBuilder sb = new StringBuilder();
            if (!arrayStarted)
                arrayStarted = true;
            else
                sb.Append(",");
            sb.Append("\n\t\t");
            sb.Append(JsonSerializer.Serialize(item));
            File.AppendAllText(path, sb.ToString());
        }

        // Compute SHA256 hash of current item to use as its id
        private string ComputeHash()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(item.title));

                // Convert byte array to a string   
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    sb.Append(bytes[i].ToString("x2"));
                item.id = sb.ToString();
                return item.id;
            }
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
        public string id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string partof { get; set; }
        public string doi { get; set; }
        public Person[] authors { get; set; }

        public Publication(string id, string type, string title, int year, string partof, string doi, Person[] authors)
        {
            this.id = id;
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
