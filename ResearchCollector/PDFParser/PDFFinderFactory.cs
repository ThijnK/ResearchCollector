using ResearchCollector.PDFParser.PDFFinders;
using System;

namespace ResearchCollector.PDFParser
{
    class PDFFinderFactory
    {
        string identifier;
        public PDFFinderFactory(string link)
        {
            identifier = link.Split('/')[2];
        }
        public PDFFinder correctPDFFinder()
        {
            switch (identifier)
            {
                case "www.computer.org":
                    return new IEEEPDFFinder(); 
                case "link.springer.com":
                    return new SpringerPDFFinder();
                case "www.sciencedirect.com":
                    return null; 
                case "dl.acm.org":
                    return new ACMPDFFinder(); 
                case "www.jci.org":
                    return new JCIPDFFinder();
                case "www.microbiologyresearch.org":
                    return new MicrobiologyResearchPDFFinder();
                default:
                    throw new ArgumentException($"Unable to extract pdf from {identifier}");
            }
        }
    }
}
