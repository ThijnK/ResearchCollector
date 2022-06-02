using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using PDFParser.Exceptions;

namespace PDFParser
{
    /// <summary>
    /// Starts redirecting from a doi link and tries to end up at the real website
    /// </summary>
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
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(doi);
                req.Method = "GET";
                req.CookieContainer = new CookieContainer(); //some websites only work when cookies are allowed
                req.AllowAutoRedirect = true;
                //Possibly problems can be fixed by tweaking a lot with these settings

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                return resp.ResponseUri.AbsoluteUri;
            }
            catch (Exception ex) { throw new RedirectingException(doi, ex); }
        }
    }
}
