﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Converter
{
    class PureConverter : Converter
    {
        /* 
         * Input files for this converter are to be harvested from the PURE api using the following python script:
         * https://github.com/zievathustra/uu-rdms-harvest/tree/master/harvest-pure
         * Please note that changes to the naming system of the above script may result in this program breaking..
         */

        private int currentFile;
        private int fileCount;

        public PureConverter(SynchronizationContext context) : base(context)
        {
            fileCount = 80;
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
                List<Person> authors = new List<Person>();
                if (reader.ReadToDescendant("personAssociation"))
                {
                    depth = reader.Depth;
                    ParseAuthor(reader, authors);
                    while (reader.Name == "personAssociation")
                        ParseAuthor(reader, authors);
                    item.authors = authors.ToArray();
                    MoveToNextNode(reader, depth);
                }
            }
            else
                item.authors = new Person[0];

            // Doi (not always present)
            // Move to to electronicVersions if it exists, if not, skip this publication (no doi or file)
            if (!reader.ReadToNextSibling("electronicVersions"))
                return false;
            item.doi = ""; item.file = "";
            if (reader.Name == "electronicVersions")
            {
                depth = reader.Depth;
                // Try to a find the doi and the file url
                if (!MoveToSibling(reader, "doi", "file"))
                    return false;
                if (reader.Name == "file")
                {
                    if (reader.ReadToDescendant("fileURL"))
                        item.file = reader.ReadElementContentAsString();
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
                if (reader.Name != "journalAssociation")
                    reader.ReadToNextSibling("journalAssociation");
                // Couldn't find journal
                if (reader.Name == nodeType)
                    return false;
                reader.ReadToDescendant("title");
                item.partof = reader.ReadElementContentAsString();
            }
            else
            {
                if (reader.Name != "event")
                    reader.ReadToNextSibling("event");
                // Couldn't find journal
                if (reader.Name == nodeType)
                    return false;
                reader.ReadToDescendant("name");
                reader.ReadToDescendant("text");
                item.partof = reader.ReadElementContentAsString();
            }

            ReportAction($"{item.type} parsed: '{item.title}'");
            return true;
        }

        /// <summary>
        /// Moves the reader to the start element that comes right after the end element of the current depth
        /// </summary>
        /// 
        private void MoveToNextNode(XmlReader reader, int targetDepth)
        {
            int depth = reader.Depth;
            while (reader.Depth > targetDepth)
                reader.Read();
            reader.Read();
        }

        /// <summary>
        /// Moves the reader to the first sibling node with one of the given names that can be found.
        /// If no such sibling exists, the reader is positioned at the end of the end element of the current parent element.
        /// </summary>
        /// <returns><c>true</c> if a matching sibling was found, <c>false</c> otherwise.</returns>
        private bool MoveToSibling(XmlReader reader, params string[] nodeNames)
        {
            int depth = reader.Depth;
            while (reader.Depth >= depth && !nodeNames.Contains(reader.Name))
                reader.Read();
            if (reader.Depth < depth)
                return false;
            return true;
        }
        
        private void ParseAuthor(XmlReader reader, List<Person> authors)
        {
            string name = "";
            if (reader.ReadToDescendant("firstName"))
                name = reader.ReadElementContentAsString();
            if (name == "" && !reader.ReadToNextSibling("lastName"))
                return;
            int depth = reader.Depth;
            name += " " + reader.ReadElementContentAsString();
            
            MoveToNextNode(reader, depth - 1);

            // Role of the person (we only include authors)
            if (reader.Name != "personRole" && !reader.ReadToNextSibling("personRole"))
                return;
            reader.ReadToDescendant("text");
            if (reader.ReadElementContentAsString() != "Author")
                return;

            // TODO: check if the affiliation is not the UU, what it actually is
            authors.Add(new Person(name, "", "Utrecht University"));

            // Move to end tag of this person element
            while (reader.Name != "personAssociation")
                reader.Read();
            reader.Read();
        }
    }
}
