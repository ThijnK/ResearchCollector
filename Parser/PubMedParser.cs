using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class PubMedParser : Parser
    {
        public override string GetTypeName()
        {
            return "pubmed";
        }

        public override bool CheckFile(string path)
        {
            throw new NotImplementedException();
        }

        public override bool ParseFile(string inputPath)
        {
            throw new NotImplementedException();
        }
    }
}
