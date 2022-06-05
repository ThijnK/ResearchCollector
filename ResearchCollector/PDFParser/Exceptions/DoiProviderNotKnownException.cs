using System;

namespace ResearchCollector.PDFParser.Exceptions
{
    [Serializable]
    class DoiProviderNotKnownExpection : Exception
    {
        public DoiProviderNotKnownExpection() { }

        public DoiProviderNotKnownExpection(string message)
        : base(message) { }

        public DoiProviderNotKnownExpection(string message, Exception inner)
            : base(message, inner) { }
    }
}
