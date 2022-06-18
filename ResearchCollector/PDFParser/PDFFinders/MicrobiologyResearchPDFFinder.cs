using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchCollector.PDFParser.PDFFinders
{
    class MicrobiologyResearchPDFFinder : PDFFinder
    {
        public override string FindPDF(string link)
        {
            //HtmlNode.ElementsFlags.Remove("form");
            var htmlweb = new HtmlWeb();
            htmlweb.PreRequest += request =>
            {
                request.CookieContainer = new System.Net.CookieContainer();
                return true;
            };
            var htmlDoc = htmlweb.Load(link);

            

            HtmlNode node =  htmlDoc.GetElementbyId("bellowheadercontainer").ChildNodes[7].ChildNodes[7].ChildNodes[1].ChildNodes[3].ChildNodes[3].ChildNodes[3].ChildNodes[0].ChildNodes[1].ChildNodes[1];

            string pdflink = "https://www.microbiologyresearch.org" + node.Attributes[1].Value;


            /*var doc = new HtmlDocument();
            HtmlWeb();
            doc.LoadHtml(link);
            //HtmlNode startNode = doc.GetElementbyId("pb-page-content");

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a[contains(@title, \"PDF\")]"))
            {
                string value = node.InnerText;
                // etc...
            }*/
            return pdflink;
        }
    }
}
