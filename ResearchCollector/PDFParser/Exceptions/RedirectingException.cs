using System;

namespace ResearchCollector.PDFParser.Exceptions
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
