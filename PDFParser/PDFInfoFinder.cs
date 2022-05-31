using System;
using System.Collections.Generic;
using System.Text;
using PDFParser.PDFFinders;
using PDFParser.Exceptions;

namespace PDFParser
{
    /// <summary>
    /// Find the right pdf and gets all the needed info from it
    /// </summary>
    class PDFInfoFinder
    {

        public void FindInfo(string doi)
        {
            //If possible, get the real link by redicrecting
            try
            {
                string realLink = (new RealLinkFinder(doi)).GetActualLink();

                //If possible, get PDF
                PDFFinder finder = (new PDFFinderFactory(realLink)).correctPDFFinder();
                finder.FindPDF(doi);

                //Parse info from PDF
            }
            catch (RedirectingException) { }
            catch (DOiProviderNotKnownExpection) { }
            catch (Exception) { }
        }

        
    }
}
