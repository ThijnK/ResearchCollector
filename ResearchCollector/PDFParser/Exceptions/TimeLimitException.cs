using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchCollector.PDFParser.Exceptions
{
    [Serializable]
    class TimeLimitException : Exception
    {
        public TimeLimitException() { }

        public TimeLimitException(string message)
        : base(message) { }

        public TimeLimitException(string message, Exception inner)
            : base(message, inner) { }
    }
}
