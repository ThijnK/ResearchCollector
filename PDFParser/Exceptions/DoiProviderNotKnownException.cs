using System;
using System.Collections.Generic;
using System.Text;

namespace PDFParser.Exceptions
{
    [Serializable]
    class DOiProviderNotKnownExpection : Exception
    {
        public DOiProviderNotKnownExpection() { }

        public DOiProviderNotKnownExpection(string message)
        : base(message) { }

        public DOiProviderNotKnownExpection(string message, Exception inner)
            : base(message, inner) { }
    }
}
