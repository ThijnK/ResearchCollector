using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ResearchCollector.Filter
{
    class PureFilter : Filter
    {
        /* 
         * Input files for this converter are to be harvested from the PURE api using the following python script:
         * https://github.com/zievathustra/uu-rdms-harvest/tree/master/harvest-pure
         * Please note that changes to the naming system of the above script may result in this program breaking..
         */

        private int currentFile;
        private int fileCount;

        public PureFilter(SynchronizationContext context) : base(context)
        {
            fileCount = 117;
            // Increment the progress based on the portion of files that have been processed so far
            progressIncrement = 1.0 / (double)fileCount * 100;
        }

        public override string ToString()
        {
            return "pure";
        }

        public override bool CheckFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                if (sr.ReadLine().StartsWith("<result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"https://research-portal.uu.nl"))
                    return true;
            }

            return false;
        }

        public override void ParseData(string inputPath)
        {
            // Setup settings for the XmlReader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.Schema;
            settings.XmlResolver = new XmlUrlResolver();

            // Get the prepend before the number of the files to be processed
            string directory = Path.GetDirectoryName(inputPath);
            string filePrepend = Path.GetFileName(inputPath).Substring(0, 26);
            item.origin = "pure";

            // Go through each of the files making up the data set
            for (currentFile = 0; currentFile <= fileCount * 2000; currentFile += 2000)
            {
                string nr = currentFile.ToString("000000");
                string currentPath = Path.Combine(directory, $"{filePrepend}{nr}.xml");
                    
                ParseXml(currentPath, settings, "contributionToConference", "contributionToJournal");
                UpdateProgress();
            }
        }

        public override bool ParsePublicationXml(XmlReader reader)
        {
            // Pure's unique id
            item.externalId = reader.GetAttribute("pureId");

            // Type (article/inproceedings)
            string nodeType = reader.Name;
            if (nodeType == "contributionToJournal")
                item.type = "article";
            else
                item.type = "inproceedings";

            // Title
            if (!reader.ReadToDescendant("title"))
                return false;
            item.title = reader.ReadInnerXml();
            if (string.IsNullOrEmpty(item.title))
                return false;

            // Publication date
            // Don't include this item if we cannot find the year of publication
            if (!reader.ReadToNextSibling("publicationStatuses"))
                return false;
            int depth = reader.Depth;
            if (!reader.ReadToDescendant("year"))
                return false;
            item.year = reader.ReadElementContentAsInt();
            MoveToNextNode(reader, depth);

            // Authors
            string partofNode = item.type == "article" ? "journalAssociation" : "event";
            if (!MoveToSibling(reader, "electronicVersions", "personAssociations"))
                return false;
            if (reader.Name == "personAssociations")
            {
                List<Author> authors = new List<Author>();
                if (reader.ReadToDescendant("personAssociation"))
                {
                    depth = reader.Depth;
                    ParseAuthor(reader, authors);
                    while (reader.Name == "personAssociation")
                        ParseAuthor(reader, authors);
                    item.has = authors.ToArray();
                    MoveToNextNode(reader, depth);
                }
            }
            else
                item.has = new Author[0];

            // Doi (not always present)
            // Move to to electronicVersions if it exists, if not, skip this publication (no doi or file)
            if (!reader.ReadToNextSibling("electronicVersions"))
                return false;
            item.doi = ""; item.pdfLink = "";
            if (reader.Name == "electronicVersions")
            {
                depth = reader.Depth;
                // Try to a find the doi and the file url
                if (!MoveToSibling(reader, "doi", "file"))
                    return false;
                if (reader.Name == "file")
                {
                    if (reader.ReadToDescendant("fileURL"))
                        item.pdfLink = reader.ReadElementContentAsString();
                }

                if (reader.Name == "doi")
                    item.doi = reader.ReadElementContentAsString();
                else if (reader.ReadToNextSibling("doi"))
                    item.doi = reader.ReadElementContentAsString();
                
                MoveToNextNode(reader, depth);
            }

            // Part of (journal/proceedings)
            if (item.type == "article")
            {
                if (!ParseJournal(reader))
                    return false;
            }
            else
            {
                proceedings.Reset();
                if (reader.Name != "event")
                    reader.ReadToNextSibling("event");
                // Couldn't find journal
                if (reader.Name == nodeType)
                    return false;
                reader.ReadToDescendant("name");
                reader.ReadToDescendant("text");
                // Reuse existing Volume object to avoid excessive memory usage
                proceedings.title = reader.ReadElementContentAsString();
                item.partof = proceedings;
            }

            ReportAction($"{item.type} parsed: '{item.title}'");
            return true;
        }
        
        private void ParseAuthor(XmlReader reader, List<Author> authors)
        {
            string fname = "", lname = "", name = "";
            if (reader.ReadToDescendant("firstName"))
                fname = reader.ReadElementContentAsString();
            if (fname == "" && !reader.ReadToNextSibling("lastName"))
                return;
            int depth = reader.Depth;
            lname = reader.ReadElementContentAsString();
            if (fname != "" && lname != "")
                name = fname + " " + lname;
            
            MoveToNextNode(reader, depth - 1);

            // Role of the person (we only include authors)
            if (reader.Name != "personRole" && !reader.ReadToNextSibling("personRole"))
                return;
            depth = reader.Depth;
            reader.ReadToDescendant("text");
            if (reader.ReadElementContentAsString() != "Author")
                return;
            MoveToNextNode(reader, depth);

            // Get affiliation
            string affiliation = "";
            if (reader.Name == "organisationalUnits" || reader.ReadToNextSibling("organisationalUnits"))
            {
                reader.ReadToDescendant("name");
                reader.ReadToDescendant("text");
                affiliation = reader.ReadElementContentAsString();
            }

            // TODO: affiliation can be a faculty, without reference to actual organization
            authors.Add(new Author(fname, lname, name, "", "", affiliation));

            // Move to end tag of this person element
            while (reader.Name != "personAssociation")
                reader.Read();
            reader.Read();
        }

        /// <summary>
        /// Parse a journal along with information about it (volume/issue)
        /// </summary>
        /// <returns><c>true</c> if successfully parsed, <c>false</c> if no journal info could be found</returns>
        private bool ParseJournal(XmlReader reader)
        {
            // Reset current journal object
            journal.Reset();

            // Journal volume
            if (reader.Name != "volume")
                reader.ReadToNextSibling("volume");
            journal.volume = reader.ReadElementContentAsString();

            // Title
            if (reader.Name != "journalAssociation")
                reader.ReadToNextSibling("journalAssociation");
            int depth = reader.Depth;
            // Couldn't find journal
            if (reader.Depth < depth)
                return false;
            reader.ReadToDescendant("title");
            journal.title = reader.ReadElementContentAsString();
            MoveToNextNode(reader, depth);

            // Issue (number)
            if (reader.Depth >= depth)
            {
                if (reader.Name == "journalNumber" || reader.ReadToNextSibling("journalNumber"))
                    journal.issue = reader.ReadElementContentAsString();
            }
            item.partof = journal;
            return true;
        }
    }
}
