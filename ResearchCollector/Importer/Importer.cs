using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace ResearchCollector.Importer
{
    class Importer : Worker
    {
        private JsonPublication pub;
        private int pubCount;

        public Importer(SynchronizationContext context) : base(context) { }

        /// <summary>
        /// Insert content from given JSON file into (in-memory mock) database
        /// </summary>
        /// <param name="path">Path to the JSON file</param>
        /// <param name="worker">BackgroundWorker this will be run on</param>
        public void Run(string path, BackgroundWorker worker)
        {
            this.worker = worker;
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                sr.ReadLine();
                string line = "";
                string json = "";
                while ((line = sr.ReadLine()) != null)
                {
                    json = line.Remove(line.Length - 1, 1);
                    pub = JsonSerializer.Deserialize<JsonPublication>(json);
                    HandlePublication();
                }
            }
        }

        private void HandlePublication()
        {
            pubCount++;
            //worker.ReportProgress(pubCount++ / ..);

            // Get text from pdf
            string text = GetText();
        }

        private string GetText()
        {
            /// Use the field <see cref="pub"/> to access the data of current json publication
            return "";
        }
    }
}
