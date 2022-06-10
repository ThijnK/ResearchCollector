using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ResearchCollector.PDFParser.Exceptions;

namespace ResearchCollector.PDFParser
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

        /// <summary>
        /// redirect until the real link is found (goes wrong often)
        /// </summary>
        /// <returns></returns>
        public string GetActualLink()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(doi);
            req.Method = "GET";
            req.CookieContainer = new CookieContainer(); //some websites only work when cookies are allowed
            req.AllowAutoRedirect = true;
            //Possibly problems can be fixed by tweaking a lot with these settings

            HttpWebResponse resp = null;
              
            bool linkFound = ExecuteWithTimeLimit(TimeSpan.FromSeconds(1), () => { resp = (HttpWebResponse)req.GetResponse(); });
            if(linkFound)
                return resp.ResponseUri.AbsoluteUri;
            throw new TimeLimitException("The real link was not found in the time limit");
        }

        bool ExecuteWithTimeLimit(TimeSpan timeSpan, Action codeBlock)
        {
            try
            {
                Task task = Task.Factory.StartNew(() => codeBlock());
                task.Wait(timeSpan);
                return task.IsCompleted;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerExceptions[0];
            }
        }
    }
}
