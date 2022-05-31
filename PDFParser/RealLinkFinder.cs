using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using PDFParser.Exceptions;

namespace PDFParser
{
    class RealLinkFinder
    {
        string doi;
        public RealLinkFinder(string doi)
        {
            this.doi = doi;
        }

        public string GetActualLink()
        {
            try
            {

                // Link.springer seems to be infinitely redirecting to the same web page??
                // Possibly use the Springer API instead of trying to get the pdf from web page

                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(doi);
                req.Method = "GET";
                req.AllowAutoRedirect = true;

                // Get response web page
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                return resp.ResponseUri.AbsoluteUri;
            }
            catch (Exception ex) { throw new RedirectingException(doi, ex); }
        }
    }
}
