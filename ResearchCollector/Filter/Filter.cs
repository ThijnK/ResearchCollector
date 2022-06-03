using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace ResearchCollector.Filter
{
    abstract class Filter
    {
        /// <summary>
        /// Path of the file to write the output to
        /// </summary>
        protected string outputPath;
        /// <summary>
        /// Whether or not the publications array in the output JSON was started
        /// </summary>
        private bool arrayStarted;
        /// <summary>
        /// Reference to the background worker this is being run on to report progress
        /// </summary>
        protected BackgroundWorker worker;
        public bool logActions;
        /// <summary>
        /// Stores the info of the publication currently being parsed.
        /// Same object is reused for every publication.
        /// </summary>
        protected Publication item;
        protected Volume proceedings;
        protected Journal journal;

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

        /// <summary>
        /// Event that is raised when an action was completed (like a publication being parsed and added to the output)
        /// </summary>
        public event EventHandler<ActionCompletedEventArgs> ActionCompleted;
        /// <summary>
        /// Context used for posting messages to the UI (since <see cref="Run(string, string, BackgroundWorker)"/> will be run an a separate background thread)
        /// </summary>
        private readonly SynchronizationContext context;

        public Filter(SynchronizationContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Convert the given input file to the JSON format <see cref="Publication"/>
        /// </summary>
        /// <param name="worker">Background worker that this method is being run on</param>
        public void Run(string inputPath, string outputPath, BackgroundWorker worker)
        {
            this.worker = worker;

            // Set up JSON output file
            string name = ToString();
            this.outputPath = Path.Combine(outputPath, name + ".json");
            File.WriteAllText(this.outputPath, $"{{\n\t\"publications\": [");

            // Parse input and write results to output file
            InitializeReusables();
            ParseData(inputPath);

            // Close off JSON output file
            File.AppendAllText(this.outputPath, "\n\t]\n}");
        }

        /// <summary>
        /// Initialize values for the reusable objects of Publication/Journal/Volume so they can be reused for each parsed item
        /// </summary>
        private void InitializeReusables()
        {
            item = new Publication();
            item.externalId = "";
            item.origin = "";
            item.type = "";
            item.title = "";
            item.year = -1;
            proceedings = new Volume("");
            journal = new Journal("", "", "", "");
            item.partof = proceedings;
            item.doi = "";
            item.pdfLink = "";
            item.has = new Author[0];
        }

        /// <summary>
        /// Go through an XML file and call the <see cref="ParsePublicationXml(XmlReader)"/> method for each node that has one of the specified node names
        /// </summary>
        /// <param name="path">Path to the XML file to parse</param>
        /// <param name="settings">Settings for the <see cref="XmlReader"/></param>
        /// <param name="nodeNames">Names of the XML nodes to include in the output</param>
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
                    foreach (string nodeName in nodeNames)
                    {
                        if (nodeName == reader.Name)
                        {
                            if (ParsePublicationXml(reader))
                                WriteToOutput();
                            break;
                        }
                    }
                }
            }
            
            fs.Close();
        }

        /// <summary>
        /// Write current publication to the output JSON file
        /// </summary>
        public void WriteToOutput()
        {
            StringBuilder sb = new StringBuilder();
            if (!arrayStarted)
                arrayStarted = true;
            else
                sb.Append(",");
            sb.Append("\n\t\t");
            sb.Append(JsonSerializer.Serialize(item));
            File.AppendAllText(outputPath, sb.ToString());
        }

        /// <summary>
        /// Increment progress and inform the UI if progress has been made beyond a few decimals
        /// </summary>
        protected void UpdateProgress()
        {
            progress = Math.Min(100.0, progress + progressIncrement);
            if ((int)progress > prevProgress)
            {
                prevProgress = (int)progress;
                worker.ReportProgress(prevProgress);
            }
        }

        /// <summary>
        /// Report to the UI a description of what is being done right now
        /// </summary>
        protected void ReportAction(string description)
        {
            if (logActions)
                context.Post(new SendOrPostCallback(RaiseActionEvent), description);
        }

        /// <summary>
        /// Event to be raised from within the context of the UI
        /// </summary>
        /// <param name="state"></param>
        private void RaiseActionEvent(object state)
        {
            ActionCompleted(this, new ActionCompletedEventArgs((string)state));
        }

        /// <summary>
        /// Moves the reader to the start element that comes right after the end element of the current depth
        /// </summary>
        /// 
        protected void MoveToNextNode(XmlReader reader, int targetDepth)
        {
            while (reader.Depth > targetDepth)
                reader.Read();
            reader.Read();
        }

        /// <summary>
        /// Moves the reader to the first sibling node with one of the given names that can be found.
        /// If no such sibling exists, the reader is positioned at the end of the end element of the current parent element.
        /// </summary>
        /// <returns><c>true</c> if a matching sibling was found, <c>false</c> otherwise.</returns>
        protected bool MoveToSibling(XmlReader reader, params string[] nodeNames)
        {
            int depth = reader.Depth;
            while (reader.Depth >= depth && !nodeNames.Contains(reader.Name))
                reader.Read();
            if (reader.Depth < depth)
                return false;
            return true;
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
    class Publication
    {
        public string externalId { get; set; }
        public string origin { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string doi { get; set; }
        public string pdfLink { get; set; }
        public object partof { get; set; }
        public Author[] has { get; set; }
    }

    class Volume
    {
        public string title { get; set; }

        public Volume(string title)
        {
            this.title = title;
        }

        /// <summary>
        /// Reset the attributes of this Volume to an empty string, to be filled for the next publication
        /// </summary>
        public virtual void Reset()
        {
            this.title = "";
        }
    }

    class Journal : Volume
    {
        public string issue { get; set; }
        public string volume { get; set; }
        public string series { get; set; }

        public Journal(string title, string issue, string volume, string series) : base(title)
        {
            this.issue = issue;
            this.volume = volume;
            this.series = series;
        }

        public override void Reset()
        {
            base.Reset();
            this.issue = "";
            this.volume = "";
            this.series = "";
        }
    }

    struct Author
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string orcid { get; set; }
        public string affiliatedTo { get; set; }

        public Author(string fname, string lname, string name, string email, string orcid, string affiliatedTo)
        {
            this.fname = fname;
            this.lname = lname;
            this.name = name;
            this.email = email;
            this.orcid = orcid;
            this.affiliatedTo = affiliatedTo;
        }
    }
    #endregion
}
