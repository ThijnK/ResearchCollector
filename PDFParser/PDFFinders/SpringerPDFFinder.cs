﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFParser.PDFFinders
{
    /// <summary>
    /// If possible, finds the PDF on an Springer website
    /// The articles are often free
    /// It seems to redirect you an infinite number of times
    /// </summary>
    class SpringerPDFFinder : PDFFinder
    {
        public override string FindPDF(string link)
        {
            throw new NotImplementedException();
        }
    }
}
