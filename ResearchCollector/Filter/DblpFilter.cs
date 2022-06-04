using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;

namespace ResearchCollector.Filter
{
    class DblpFilter : Filter
    {
        public DblpFilter(SynchronizationContext context) : base(context) 
        {
            // Since there are roughly 4,836,150 publications that we extract from the dblp data set
            progressIncrement = 1.0 / 48361.50;
        }

        public override string ToString()
        {
            return "dblp";
        }

        public override bool CheckFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                if (sr.ReadLine().StartsWith("<!DOCTYPE dblp"))
                    return true;
            }

            return false;
        }

        public override void ParseData(string inputPath)
        {
            // Create copy of DTD file in working directory
            // DTD file is assumed to be located in the same directory and have the same name as XML file
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string dtdPath = Path.Combine(Path.GetDirectoryName(inputPath), fileName + ".dtd");
            if (!File.Exists(dtdPath))
                throw new Exception("Unable to find .dtd file");
            File.Copy(dtdPath, $"./{fileName}.dtd", true);

            // Setup settings for the XmlReader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.XmlResolver = new XmlUrlResolver();

            progress = 0;
            item.origin = "dblp";
            ParseXml(inputPath, settings, "article", "inproceedings");
            File.Delete($"./{fileName}.dtd");
        }

        public override bool ParsePublicationXml(XmlReader reader)
        {
            int depth = reader.Depth;

            // DBLP's unique id
            item.externalId = reader.GetAttribute("key");

            // Publication type
            item.type = reader.Name;

            // Authors
            reader.Read(); reader.Read();
            List<JsonAuthor> authors = new List<JsonAuthor>();
            while (reader.Name == "author")
                ParseAuthor(authors, reader);
            item.has = authors.ToArray();

            // Title
            item.title = reader.ReadInnerXml();
            if (!IsValidTitle(item.title))
                return false;
            reader.Read();

            proceedings.Reset();
            journal.Reset();
            item.year = -1;
            item.doi = "";
            while (reader.Depth > depth)
            {
                ParseAttribute(reader);
                reader.Read();
            }

            // Don't add if it does not contain a doi
            if (item.doi == "")
                return false;

            // Add the right reference to journal/proceedings to current item
            if (item.type == "article")
                item.partof = journal;
            else
                item.partof = proceedings;

            UpdateProgress();
            ReportAction($"{item.type} parsed: '{item.title}'");
            return true;
        }

        /// <summary>
        /// Checks if current element represents data that we want to extract, and if so, extracts it to the current item.
        /// This is necessary because these elements can occur in different orders within a publication in the DBLP data set.
        /// </summary>
        private void ParseAttribute(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "ee":
                    string ee = reader.ReadElementContentAsString();
                    if (ee.StartsWith("https://doi.org/"))
                        item.doi = ee;
                    break;
                case "year":
                    item.year = reader.ReadElementContentAsInt();
                    break;
                case "booktitle":
                    proceedings.title = reader.ReadElementContentAsString();
                    break;
                case "volume":
                    journal.volume = reader.ReadElementContentAsString();
                    break;
                case "number":
                    journal.issue = reader.ReadElementContentAsString();
                    break;
                case "journal":
                    journal.title = reader.ReadElementContentAsString();
                    break;
                // If the element is nothing of interest, skip to its end tag
                default:
                    reader.Read();
                    reader.Read();
                    reader.Read();
                    break;
            }
        }

        private void ParseAuthor(List<JsonAuthor> authors, XmlReader reader)
        {
            string orcid = reader.GetAttribute("orcid");
            if (orcid == null)
                orcid = "";
            authors.Add(new JsonAuthor("", "", reader.ReadElementContentAsString(), "", orcid, ""));
            reader.Read();
        }

        private bool IsValidTitle(string title)
        {
            return title != "" && title != "(was never published)" && title != "(error)";
        }
    }
}
