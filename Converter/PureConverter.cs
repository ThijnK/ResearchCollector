using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Converter
{
    class PureConverter : Converter
    {
        /* 
         * Input files for this converter are to be harvested from the PURE api using the following python script:
         * https://github.com/zievathustra/uu-rdms-harvest/tree/master/harvest-pure
         * Please note that changes to the naming system of the above script may result in this program breaking..
         */

        private int currentFile;
        private int fileCount;

        public PureConverter(SynchronizationContext context) : base(context)
        {
            fileCount = 1;
            // Increment the progress based on the portion of files that have been processed so far
            progressIncrement = 1.0 / (double)fileCount * 100;
        }

        public override string ToString()
        {
            return "pure";
        }

        public override bool CheckFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                if (sr.ReadLine().StartsWith("<result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"https://research-portal.uu.nl"))
                    return true;
            }

            return false;
        }

        public override void ParseData(string inputPath)
        {
            // Setup settings for the XmlReader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.Schema;
            settings.XmlResolver = new XmlUrlResolver();

            // Get the prepend before the number of the files to be processed
            string directory = Path.GetDirectoryName(inputPath);
            string filePrepend = Path.GetFileName(inputPath).Substring(0, 26);

            // Go through each of the files making up the data set
            for (currentFile = 0; currentFile <= fileCount; currentFile += 200)
            {
                string nr = currentFile.ToString("000000");
                string currentPath = Path.Combine(directory, $"{filePrepend}{nr}.xml");
                    
                ParseXml(currentPath, settings, "researchOutput");
                UpdateProgress();
            }
        }

        public override bool ParsePublicationXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
