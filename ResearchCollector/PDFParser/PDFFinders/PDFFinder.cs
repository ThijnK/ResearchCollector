using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchCollector.PDFParser.PDFFinders
{
    abstract class PDFFinder
    {
        /// <summary>
        /// If Possible, finds the PDF from the website
        /// </summary>
        public abstract string FindPDF(string link);
    }
}
