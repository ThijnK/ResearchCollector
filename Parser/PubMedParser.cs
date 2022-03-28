using System;
using System.IO;

namespace Parser
{
    class PubMedParser : Parser
    {
        public override string ToString()
        {
            return "pubmed";
        }

        public override bool CheckFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                if (sr.ReadLine().StartsWith("<!DOCTYPE PubmedArticleSet"))
                    return true;
            }

            return false;
        }

        public override bool ParseFile(string inputPath)
        {
            // Ignore inputPath, as we get necessary files from the web
            throw new NotImplementedException();


        }
    }
}
