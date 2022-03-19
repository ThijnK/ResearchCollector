using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Parser
{
    class DblpParser : Parser
    {
        public override string GetTypeName()
        {
            return "dblp";
        }

        public override bool CheckFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                sr.ReadLine();
                if (sr.ReadLine() == "<dblp>")
                    return true;
            }

            return false;
        }

        public override bool ParseFile(string inputPath)
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

            FileStream fs = File.OpenRead(inputPath);
            FindNodes("article", XmlReader.Create(fs, settings));
            fs.Close(); fs = File.OpenRead(inputPath);
            FindNodes("inproceedings", XmlReader.Create(fs, settings));
            fs.Close();

            return true;
        }

        // TO DO: make this asynchronous? ↓
        private void FindNodes(string nodeName, XmlReader reader)
        {
            reader.MoveToContent(); // Moves to the <dblp> node
            reader.Read(); // Read one line to get into the children of the <dblp> node

            // Go through every node with the given name that can be found
            while (reader.ReadToNextSibling(nodeName))
            {
                Publication pub = ParsePublication(reader);
                if (pub != null)
                    WriteToOutput(pub);
            }
        }

        private Publication ParsePublication(XmlReader reader)
        {
            // Publish year
            int year = 0;
            string date = reader.GetAttribute("mdate");
            if (date != null)
                year = DateTime.Parse(date).Year;
            else
                return null;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(reader.ReadOuterXml());

            // Title
            string title = "";
            XmlNodeList t = xml.GetElementsByTagName("title");
            if (t.Count > 0)
            {
                if (IsValidTitle(t[0].InnerText))
                    title = t[0].InnerText;
                else
                    return null;
            }
            else return null;

            // Authors
            List<Person> authors = new List<Person>();
            foreach (XmlNode node in xml.GetElementsByTagName("author"))
            {
                // Try to get the orcid from the attributes if it exists
                string orcid = "";
                if (node.Attributes != null)
                    if (node.Attributes.Count > 0)
                        if (node.Attributes[0].Name == "orcid")
                            orcid = node.Attributes[0].Value;
                authors.Add(new Person(node.InnerText, orcid));
            }

            // Doi link
            string doi = "";
            t = xml.GetElementsByTagName("ee");
            if (t.Count > 0)
                doi = t[0].InnerText;
            else
                return null;

            // Journal/Conference
            string partof = "";
            if (xml.FirstChild.Name == "article")
            {
                t = xml.GetElementsByTagName("journal");
                if (t.Count > 0)
                    partof = t[0].InnerText;

                return new Article(title, year, doi, authors.ToArray(), partof);
            }
            else
            {
                t = xml.GetElementsByTagName("booktitle");
                if (t.Count > 0)
                    partof = t[0].InnerText;

                return new Inproceedings(title, year, doi, authors.ToArray(), partof);
            }
        }

        private bool IsValidTitle(string title)
        {
            return title != "" && title != "(was never published)" && title != "(error)";
        }
    }
}
