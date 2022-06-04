using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Xml;

namespace ResearchCollector.Filter
{
    class PubMedFilter : Filter
    {
        private string tempPath;
        private int currentFile;
        private int fileCount;

        public PubMedFilter(SynchronizationContext context, string inputPath, string outputPath) : base(context, inputPath, outputPath)
        {
            fileCount = 1114;
            progressIncrement = 1.0 / (double)fileCount * 100.0;
        }

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
            tempPath = $"{Path.GetDirectoryName(outputPath)}\\temp.xml";
            string compressedPath = $"{tempPath}.gz";

            // Setup settings for the XmlReader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.XmlResolver = new XmlUrlResolver();
            item.type = "article"; // Only has articles afaik
            item.origin = "pubmed";

            // Go through each of the files making up the data set
            for (currentFile = 1; currentFile <= fileCount; currentFile++)
            {
                string nr = currentFile.ToString("0000");
                string fileName = $"pubmed22n{nr}";
                Uri url = new Uri($"https://ftp.ncbi.nlm.nih.gov/pubmed/baseline/{fileName}.xml.gz");

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(url, compressedPath);
                    }
                    catch (WebException ex)
                    {
                        // If request gets timed out, try again after 5s
                        Thread.Sleep(5000);
                        if (ex.Message == "The operation has timed out")
                            currentFile--;
                    }
                    ReportAction($"File downloaded: '{fileName}'");
                    DecompressFile(compressedPath);
                    ParseXml(tempPath, settings, "PubmedArticle");
                }
                UpdateProgress();
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

        public override bool ParsePublicationXml(XmlReader reader)
        {
            // PubMed's unique id
            reader.ReadToDescendant("PMID");
            item.externalId = reader.ReadElementContentAsString();

            // Journal volume/issue
            journal.Reset();
            reader.ReadToFollowing("JournalIssue");
            int depth = reader.Depth;
            reader.Read();reader.Read();
            if (reader.Name == "Volume")
            {
                journal.volume = reader.ReadElementContentAsString();
                reader.Read();
            }
            if (reader.Name == "Issue")
            {
                journal.issue = reader.ReadElementContentAsString();
                reader.Read();
            }
            if (reader.Name != "PubDate")
                reader.ReadToFollowing("PubDate");
            reader.Read(); reader.Read();
            // Parse the date of publication
            ParseDate(reader);
            // Move to end of the JournalIssue node
            MoveToNextNode(reader, depth);
            reader.Read();

            // Journal title
            if (reader.Name == "Title" || reader.ReadToNextSibling("Title"))
                journal.title = reader.ReadElementContentAsString();
            item.partof = journal;

            // Article title
            reader.ReadToFollowing("ArticleTitle");
            item.title = reader.ReadInnerXml();

            // Authors
            List<JsonAuthor> authors = new List<JsonAuthor>();
            if (reader.ReadToNextSibling("AuthorList"))
            {
                if (reader.ReadToDescendant("Author"))
                {
                    ParseAuthor(authors, reader);
                    while (reader.ReadToNextSibling("Author"))
                        ParseAuthor(authors, reader);
                }
            }
            item.has = authors.ToArray();

            while (reader.Name != "MedlineCitation")
                reader.Read();

            // Doi
            if (!reader.ReadToNextSibling("PubmedData"))
            {
                // Cannot find doi, ignore this publication
                MoveToNextPublication(reader);
                return false;
            }

            reader.ReadToDescendant("ArticleIdList");
            reader.ReadToDescendant("ArticleId");
            while (reader.GetAttribute("IdType") != "doi" && reader.Name != "ArticleIdList")
                for (int i = 0; i < 4; i++)
                    reader.Read();

            // Check if no doi link was given, if so, ignore this article
            if (reader.Name == "ArticleIdList")
            {
                MoveToNextPublication(reader);
                return false;
            }
            item.doi = reader.ReadElementContentAsString();

            MoveToNextPublication(reader);
            item.pdfLink = "";
            ReportAction($"Item parsed: '{item.title}'");
            return true;
        }

        // Move to end tag of the current publication XML node
        private void MoveToNextPublication(XmlReader reader)
        {
            while (reader.Name != "PubmedArticle")
                reader.Read();
        }

        private void ParseDate(XmlReader reader)
        {
            if (reader.Name == "Year")
                item.year = reader.ReadElementContentAsInt();
            // Account for MedlineDate instead of separate elements for Year, Month, Day
            else if (reader.Name == "MedlineDate")
            {
                string[] date = reader.ReadElementContentAsString().Split(' ');
                // If the first string in the above array has 4 chars (digits), it's the year
                if (date.Length > 0 && date[0].Length == 4)
                    item.year = int.Parse(date[0]);
            }
        }

        private void ParseAuthor(List<JsonAuthor> authors, XmlReader reader)
        {
            int depth = reader.Depth;
            reader.Read(); reader.Read();
            string lname = reader.ReadElementContentAsString();
            reader.Read();
            string fname = "";
            if (reader.Name == "ForeName")
                fname = reader.ReadElementContentAsString();

            // Move to AffilitionInfo node if present
            string affiliation = "";
            string orcid = "";
            while (reader.Depth > depth)
            {
                if (reader.IsStartElement() && reader.Name == "AffiliationInfo")
                {
                    reader.Read(); reader.Read();
                    affiliation = reader.ReadElementContentAsString();
                }
                else if (reader.IsStartElement() && reader.Name == "Identifier")
                {
                    if (reader.GetAttribute(0) == "ORCID")
                        orcid = ParseOrcid(reader);
                }
                reader.Read();
            }

            authors.Add(new JsonAuthor(fname, lname, $"{fname} {lname}", "", orcid, affiliation));
        }

        // Parse Orcid from Identifier element
        private string ParseOrcid(XmlReader reader)
        {
            string content = reader.ReadElementContentAsString();
            if (content.StartsWith("http://orcid.org/"))
                return content.Split('/')[1];

            return content;
        }
    }
}
