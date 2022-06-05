using ResearchCollector.PDFParser.PDFFinders;
using ResearchCollector.PDFParser.Exceptions;

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
                    return new IEEEPDFFinder(); //heeft account nodig
                case "link.springer.com":
                    return new SpringerPDFFinder(); //blijft redirecten
                case "www.sciencedirect.com":
                    return null; //nog doen! Moet op uni netwerk
                case "dl.acm.org":
                    return new ACMPDFFinder(); //nog doen! probleem met die ddos
                default:
                    throw new DoiProviderNotKnownExpection(identifier);
            }
        }


    }
}
