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
    abstract class Filter : Worker
    {
        /// <summary>
        /// Path of the file to read the input from
        /// </summary>
        protected string inputPath;
        /// <summary>
        /// Path of the file to write the output to
        /// </summary>
        protected string outputPath;
        /// <summary>
        /// Whether or not the publications array in the output JSON was started
        /// </summary>
        private bool arrayStarted;
        /// <summary>
        /// Stores the info of the publication currently being parsed.
        /// Same object is reused for every publication.
        /// </summary>
        protected JsonPublication item;
        protected JsonVolume proceedings;
        protected JsonJournal journal;

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

        public Filter(SynchronizationContext context, string inputPath, string outputPath) : base(context) 
        {
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }

        /// <summary>
        /// Convert the given input file to the JSON format <see cref="JsonPublication"/>
        /// </summary>
        /// <param name="worker">Background worker that this method is being run on</param>
        public override void Run(BackgroundWorker worker)
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
            item = new JsonPublication();
            //item.externalIds = new JsonExternalId[1] {new JsonExternalId("", "")};
            item.externalId = "";
            item.origin = "";
            item.type = "";
            item.title = "";
            item.year = -1;
            item.pages = "";
            proceedings = new JsonVolume("");
            journal = new JsonJournal("", "", "", "");
            item.partof = proceedings;
            item.doi = "";
            item.pdfLink = "";
            item.has = new JsonAuthor[0];
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
        /// Moves the reader to the start element that comes right after the end element of the current depth
        /// </summary>
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
}
