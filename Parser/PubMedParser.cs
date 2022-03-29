using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;

namespace Parser
{
    class PubMedParser : Parser
    {
        private string tempPath;

        public override string ToString()
        {
            return "pubmed";
        }

        public override bool CheckFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                if (sr.ReadLine().StartsWith("<!DOCTYPE PubmedArticleSet"))
                    return true;
            }

            return false;
        }

        public override bool ParseData(string inputPath)
        {
            tempPath = $"{Path.GetDirectoryName(path)}\\temp.xml";

            // Setup settings for the XmlReader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.XmlResolver = new XmlUrlResolver();

            // Go through each of the files making up the data set
            for (int i = 1; i <= 1114; i++) //1114
            {
                string nr = i.ToString("0000");
                string fileName = $"pubmed22n{nr}";
                Uri url = new Uri($"https://ftp.ncbi.nlm.nih.gov/pubmed/baseline/{fileName}.xml.gz");

                using (WebClient client = new WebClient())
                {
                    string compressedPath = $"{tempPath}.gz";
                    client.DownloadFile(url, compressedPath);
                    FileDownloadNotification(fileName);
                    DecompressFile(compressedPath);
                    ParseXml(tempPath, settings, "PubmedArticle");
                }
            }

            return true;
        }

        // Decompress a file and write the result to tempPath
        private void DecompressFile(string compressedPath)
        {
            using (FileStream compressedStream = File.OpenRead(compressedPath))
            {
                using (FileStream targetStream = File.Create(tempPath))
                {
                    using (GZipStream decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }
            }

            // Clear the compressed file
            File.Create(compressedPath).Close();
        }

        public override Publication ParsePublicationXml(XmlReader reader)
        {
            // Publish year
            reader.ReadToDescendant("DateCompleted");
            reader.ReadToDescendant("Year");
            int year = reader.ReadElementContentAsInt();

            // Title of journal and article
            reader.ReadToFollowing("Journal");
            reader.ReadToDescendant("Title");
            string journal = reader.ReadElementContentAsString();
            reader.ReadToFollowing("ArticleTitle");
            string title = reader.ReadElementContentAsString();

            // Can get abstract if needed
            //reader.ReadToFollowing("AbstractText");
            //string abs = reader.ReadElementContentAsString();

            // Authors
            List<Person> authors = new List<Person>();
            reader.ReadToFollowing("Author");
            ParseAuthor(authors, reader);
            while (reader.ReadToNextSibling("Author"))
                ParseAuthor(authors, reader);

            // Doi
            reader.ReadToFollowing("ArticleIdList");
            reader.ReadToDescendant("ArticleId");
            string t = reader.GetAttribute("IdType");
            while (reader.GetAttribute("IdType") != "doi" && reader.Name != "ArticleIdList")
                for (int i = 0; i < 4; i++)
                    reader.Read();

            // Check if no doi link was given, if so, ignore this article
            if (reader.Name == "ArticleIdList")
                return null;
            string doi = reader.ReadElementContentAsString();

            // Move to end tag of this PubmedArticle node
            while (reader.Name != "PubmedArticle")
                reader.Read();

            return new Article(title, year, doi, authors.ToArray(), journal);
        }

        private void ParseAuthor(List<Person> authors, XmlReader reader)
        {
            reader.ReadToDescendant("LastName");
            string lname = reader.ReadElementContentAsString();
            reader.ReadToNextSibling("ForeName");
            string fname = reader.ReadElementContentAsString();
            authors.Add(new Person($"{fname} {lname}", ""));
            for (int i = 0; i < 5; i++)
                reader.Read();
        }
    }
}
