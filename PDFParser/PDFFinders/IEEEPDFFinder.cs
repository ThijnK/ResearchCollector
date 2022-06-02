using System;
using System.Collections.Generic;
using System.Text;

namespace PDFParser.PDFFinders
{
    /// <summary>
    /// If possible, finds the PDF on an IEEE website
    /// Has two problems, one: you need an account, two: the pdfs arent free most of the time
    /// </summary>
    class IEEEPDFFinder : PDFFinder
    {
        public override string FindPDF(string link)
        {
            throw new NotImplementedException();
        }
    }
}
