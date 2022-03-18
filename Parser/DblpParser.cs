using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class DblpParser : Parser
    {
        public override bool CheckFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                sr.ReadLine();
                if (sr.ReadLine() == "<dblp>")
                    return true;
            }

            return false;
        }

        public override bool ParseFile(string inputPath, string outputPath)
        {
            // Needs DTD file...


            throw new NotImplementedException();
        }
    }
}
