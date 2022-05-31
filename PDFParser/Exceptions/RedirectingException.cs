using System;
using System.Collections.Generic;
using System.Text;

namespace PDFParser.Exceptions
{
    [Serializable]
    class RedirectingException : Exception
    {
        public RedirectingException() { }

        public RedirectingException(string message)
        : base(message) { }

        public RedirectingException(string message, Exception inner)
            : base(message, inner) { }
    }
}
