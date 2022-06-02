using System;
using System.Collections.Generic;
using System.Text;

namespace PDFParser.PDFFinders
{
    abstract class PDFFinder
    {
        /// <summary>
        /// If Possible, finds the PDF from the website
        /// </summary>
        public abstract string FindPDF(string link);
    }
}
