using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;

namespace Parser
{
    class DblpParser : Parser
    {
        public DblpParser(SynchronizationContext context) : base(context) 
        {
            progressIncrement = 1.0 / 53831.75;
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
            item.type = "article";
            ParseXml(inputPath, settings, item.type);
            item.type = "inproceedings";
            ParseXml(inputPath, settings, item.type);
        }

        public override bool ParsePublicationXml(XmlReader reader)
        {
            // Authors
            reader.Read(); reader.Read();
            List<Person> authors = new List<Person>();
            while (reader.Name == "author")
                ParseAuthor(authors, reader);
            item.authors = authors.ToArray();

            // Title
            item.title = reader.ReadInnerXml();
            if (!IsValidTitle(item.title))
            {
                while (reader.Name != item.type)
                    reader.Read();
                return false;
            }
            reader.Read();

            // Publish year
            while (reader.Name != "year" && reader.Name != item.type)
                reader.Read();
            // Couldn't find publish year
            if (reader.Name == item.type)
                return false;
            item.year = reader.ReadElementContentAsInt();

            // Journal/conference
            while (reader.Name != "journal" && reader.Name != "booktitle" && reader.Name != item.type)
                reader.Read();
            if (reader.Name == item.type)
                return false;
            item.partof = reader.ReadElementContentAsString();

            // Doi
            while (reader.Name != "ee" && reader.Name != item.type)
                reader.Read();
            if (reader.Name == item.type)
                return false;
            item.doi = reader.ReadElementContentAsString();

            // Move to end of article/inproceedings node
            while (reader.Name != item.type)
                reader.Read();

            UpdateProgress();
            ReportAction($"Item parsed: '{item.title}'");

            return true;
        }

        private void ParseAuthor(List<Person> authors, XmlReader reader)
        {
            string orcid = reader.GetAttribute("orcid");
            if (orcid == null)
                orcid = "";
            authors.Add(new Person(reader.ReadElementContentAsString(), orcid));
            reader.Read();
        }

        private bool IsValidTitle(string title)
        {
            return title != "" && title != "(was never published)" && title != "(error)";
        }
    }
}
