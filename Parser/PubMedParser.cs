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
        private int currentFile;
        private int fileCount;
        private int progress;

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

        public override void ParseData(string inputPath)
        {
            tempPath = $"{Path.GetDirectoryName(path)}\\temp.xml";

            // Setup settings for the XmlReader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.XmlResolver = new XmlUrlResolver();

            // Go through each of the files making up the data set
            fileCount = 1114;
            for (currentFile = 1; currentFile <= fileCount; currentFile++)
            {
                string nr = currentFile.ToString("0000");
                string fileName = $"pubmed22n{nr}";
                Uri url = new Uri($"https://ftp.ncbi.nlm.nih.gov/pubmed/baseline/{fileName}.xml.gz");

                using (WebClient client = new WebClient())
                {
                    string compressedPath = $"{tempPath}.gz";
                    client.DownloadFile(url, compressedPath);
                    worker.ReportProgress(progress, $"File downloaded: '{fileName}'");
                    DecompressFile(compressedPath);
                    ParseXml(tempPath, settings, "PubmedArticle");
                }
                progress = (int)((double)currentFile / (double)fileCount * 100);
            }
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
            reader.ReadToDescendant("PubDate");
            int year = 0;
            reader.Read(); reader.Read();
            if (reader.Name == "Year")
                year = reader.ReadElementContentAsInt();
            else
            {
                string content = reader.ReadElementContentAsString();
                // Check if we can parse the year from this date
                if (!int.TryParse(content.Split(' ')[0], out year))
                    return null;
            }

            // Title of journal and article
            reader.ReadToFollowing("Title");
            string journal = reader.ReadElementContentAsString();
            reader.ReadToFollowing("ArticleTitle");
            string title = reader.ReadElementContentAsString();
            while (reader.Name != "Journal")
                reader.Read();

            // Authors
            List<Person> authors = new List<Person>();
            if (reader.ReadToNextSibling("AuthorList"))
            {
                if (reader.ReadToDescendant("Author"))
                {
                    ParseAuthor(authors, reader);
                    while (reader.ReadToNextSibling("Author"))
                        ParseAuthor(authors, reader);
                }
            }
            while (reader.Name != "MedlineCitation")
                reader.Read();

            // Doi
            if (!reader.ReadToNextSibling("PubmedData"))
            {
                MoveToNextPublication(reader);
                return null;
            }

            string doi = "";
            if (reader.ReadToDescendant("ArticleIdList"))
            {
                reader.ReadToDescendant("ArticleId");
                while (reader.GetAttribute("IdType") != "doi" && reader.Name != "ArticleIdList")
                    for (int i = 0; i < 4; i++)
                        reader.Read();

                // Check if no doi link was given, if so, ignore this article
                if (reader.Name == "ArticleIdList")
                {
                    MoveToNextPublication(reader);
                    return null;
                }
                doi = reader.ReadElementContentAsString();
            }
            else
            {
                MoveToNextPublication(reader);
                return null;
            }

            MoveToNextPublication(reader);
            worker.ReportProgress(progress, $"Article parsed: '{title}'");
            return new Article(title, year, doi, authors.ToArray(), journal);
        }

        // Move to end tag of the current publication XML node
        private void MoveToNextPublication(XmlReader reader)
        {
            while (reader.Name != "PubmedArticle")
                reader.Read();
        }

        private void ParseAuthor(List<Person> authors, XmlReader reader)
        {
            reader.Read(); reader.Read();
            string name = reader.ReadElementContentAsString();
            reader.Read();
            if (reader.Name == "ForeName")
                name = $"{reader.ReadElementContentAsString()} {name}";
            authors.Add(new Person(name, ""));

            while (reader.Name != "Author")
                reader.Read();
        }
    }
}
