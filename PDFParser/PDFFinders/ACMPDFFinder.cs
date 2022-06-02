using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PDFParser.PDFFinders
{
    /// <summary>
    /// If possible, finds the PDF on an ACM website
    /// Has free articles when on the university network
    /// Has one problem: It uses some type of DDOS protection called cloudflare, but instead from redirecting to the ACM page, it stays there
    /// </summary>
    class ACMPDFFinder : PDFFinder
    {
        public override string FindPDF(string link)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(link);
            req.Method = "GET";
            req.CookieContainer = new CookieContainer();
            req.AllowAutoRedirect = true;


            // Get response web page
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e) {
                if (e.Message.Contains("302"))
                    return "";
                   //response = e.Result;
            }

                return ""; //resp.ResponseUri.AbsoluteUri;

            /*var doc = new HtmlDocument();
            doc.LoadHtml(link);
            //HtmlNode startNode = doc.GetElementbyId("pb-page-content");

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a[contains(@title, \"PDF\")]"))
            {
                string value = node.InnerText;
                // etc...
            }

            //startNode.ChildNodes[0];
            //System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
            //nav.*/
        }
    }
}
