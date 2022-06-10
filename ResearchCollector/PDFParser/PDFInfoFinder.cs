using System;
using ResearchCollector.PDFParser.PDFFinders;
using ResearchCollector.PDFParser.Exceptions;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Threading;
using ResearchCollector.Importer;

namespace ResearchCollector.PDFParser
{
    /// <summary>
    /// Tries to find and donwload the PDF and convert it to a text file
    /// </summary>
    class PDFInfoFinder : Worker
    {
        Data data;
        string savePath;

        public PDFInfoFinder(SynchronizationContext context, Data data, string savePath) : base(context)
        {
            this.savePath = savePath;
            this.data = data;
            progressIncrement = 1.0 / (double)data.pubCount * 100;
        }

        /// <summary>
        /// If the DOI is given, it starts with trying to find a link to the PDF
        /// With the link to the PDF, either as input or via finding it, it downloads it and converts it into text. It then delets the PDF file again. It saves the txt file with the given ID
        /// </summary>
        /// <param name="link">either the doi link or the direct PDF link</param>
        /// <param name="id">the ID of the article</param>
        /// <param name="doiOrDirect">wether the link is a doi or a direct link</param>
        public void FindInfo(string link, string id, bool doiOrDirect)
        {
            string pdflink;
            if (doiOrDirect) //if the link is a doi link
            {
                //if the link does not start with doi.org (PubMed links do not), add it to the start
                if (!link.StartsWith("https://doi.org"))
                    link = "https://doi.org/" + link;

                //If possible, get the real link to the document by redicrecting
                string realLink = (new RealLinkFinder(link)).GetActualLink();

                //If possible, get the link to the PDF
                PDFFinder finder = (new PDFFinderFactory(realLink)).correctPDFFinder();
                pdflink = finder.FindPDF(realLink);
            }
            else
            {
                pdflink = link;
            }

            DownloadPDF(pdflink, id, savePath);

            string textOfPDF = GetTextFromPDF(id);

            //now that the pdf is not needed anymore, delete it
            File.Delete(Path.Combine(savePath, $"{id}.pdf"));

            //write the text from the PDF to a txt file
            File.WriteAllText(Path.Combine(savePath, $"{id}.txt"), textOfPDF);
        }

        public override void Run(BackgroundWorker worker)
        {
            this.worker = worker;
            foreach (Article article in data.articles.Values)
                HandlePublication(article);
            foreach (Inproceedings inpr in data.inproceedings.Values)
                HandlePublication(inpr);
        }

        /// <summary>
        /// Finds and downloads pdf for one publication
        /// </summary>
        private void HandlePublication(Publication publication)
        {
            try
            {
                if (!string.IsNullOrEmpty(publication.pdfLink))
                    FindInfo(publication.pdfLink, publication.id, false);
                else
                    FindInfo(publication.doi, publication.id, true);
                UpdateProgress();
            }
            catch (Exception ex)
            {
                // Send error message to the UI
                worker.ReportProgress(prevProgress, ex.Message);
            }
        }

        /// <summary>
        /// Downloads the PDF file via the link
        /// </summary>
        /// <param name="pdflink">the link to the PDF</param>
        /// <param name="id">The ID from the article</param>
        void DownloadPDF(string pdflink, string id, string savePath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(pdflink /*"https://dspace.library.uu.nl/bitstream/handle/1874/22547/simons-creating%2520strategic%2520value.pdf?sequence=2"*/, Path.Combine(savePath, $"{id}.pdf"));
            }
        }

        /// <summary>
        /// Get the text corresponding with the PDF file
        /// </summary>
        /// <param name="id">The ID from the article</param>
        string GetTextFromPDF(string id)
        {
            IFilterTextReader.FilterReader fileReader = new IFilterTextReader.FilterReader(Path.Combine(savePath, $"{id}.pdf"));
            string textOfPDF = fileReader.ReadToEnd();
            fileReader.Close();
            return textOfPDF;
        }
    }
}
