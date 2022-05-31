using PDFParser.PDFFinders;
using PDFParser.Exceptions;

namespace PDFParser
{
    class PDFFinderFactory
    {
        string identifier;
        public PDFFinderFactory(string link)
        {
            identifier = link.Split("/")[2];          
        }
        public PDFFinder correctPDFFinder()
        {
            switch (identifier)
            {
                case "www.computer.org":
                    return new IEEEPDFFinder();
                case "link.springer.com":
                    return new SpringerPDFFinder();
                default:
                    throw new DOiProviderNotKnownExpection(identifier);
            }
        }

        
    }

    
}
