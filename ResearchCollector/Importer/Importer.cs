using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace ResearchCollector.Importer
{
    class Importer : Worker
    {
        /// <summary>
        /// Data object containing all of the parsed data
        /// </summary>
        public Data data;
        private JsonPublication pub;
        private int currentPubCount;
        private int totalPubCount;
        private string path;

        public Importer(SynchronizationContext context, string path) : base(context)
        {
            this.path = path;
            if (!File.Exists(path))
                throw new System.Exception("Input file does not exist");
            // Get the lineCount, and thus the nr of publications, in the input file
            int lineCount = File.ReadLines(path).Count();
            totalPubCount = lineCount - 4;
            progressIncrement = 1.0 / (double)totalPubCount * 100.0;
        }

        /// <summary>
        /// Insert content from given JSON file into (in-memory mock) database
        /// </summary>
        /// <param name="path">Path to the JSON file</param>
        /// <param name="worker">BackgroundWorker this will be run on</param>
        public override void Run(BackgroundWorker worker)
        {
            data = new Data();
            this.worker = worker;

            using (StreamReader sr = new StreamReader(path))
            {
                string t = sr.ReadLine();
                string t2 = sr.ReadLine();
                string line = "";
                while (currentPubCount < totalPubCount)
                {
                    line = sr.ReadLine();
                    // For all lines that are not the last line, remove the comma from the end
                    if (currentPubCount < totalPubCount - 1)
                        line = line.Remove(line.Length - 1, 1);
                    pub = JsonSerializer.Deserialize<JsonPublication>(line);
                    ParsePublication();
                }
            }
        }

        private void ParsePublication()
        {
            currentPubCount++;

            // Our custom id for publication
            string customId = CreateId();

            //hier article / dat andere ding aanmaken

            foreach (JsonAuthor author in pub.has)
            {
                if (!data.organizations.TryGetValue(author.affiliatedTo, out Organization currentAffiliation))
                {
                    currentAffiliation = new Organization(author.affiliatedTo);
                    data.organizations.Add(author.affiliatedTo, currentAffiliation);
                }

                if (data.persons.TryGetValue(author.orcid, out Person currentPerson)
                    && !data.authors.TryGetValue(author.email, out Author currentAuthor)) //if the person already exists but the author doesn't (if person exists, sets it, and same for author
                {
                    currentAuthor = new Author(currentPerson, currentAffiliation, author.email, author.name);
                    data.authors.Add(author.email, currentAuthor);
                }
                //als de person niet bestaat bestaat de author (denk ik) sws niet
                else
                {
                    currentPerson = new Person(author.orcid, author.name);
                    data.persons.Add(author.orcid, currentPerson);
                    currentAuthor = new Author(currentPerson, currentAffiliation, author.email, author.name);
                    data.authors.Add(author.email, currentAuthor); //email als key voor de author!!!!!
                }

                //hier author toevoegen aan de publication zn authors
            }

            // Get text from pdf
            string text = GetText();

            // Report action and progress to UI
            ReportAction($"Item parsed: '{pub.title}'");
            UpdateProgress();
        }

        private string GetText()
        {
            /// Use the field <see cref="pub"/> to access the data of current json publication
            return "";
        }

        /// <summary>
        /// Create an id for the current publication based on its title, year of pulication and number of authors.
        /// The following formula is used: <code>hash(title) ++ nrOfAuthors ++ yearOfPublication</code>
        /// </summary>
        private string CreateId()
        {
            string hash = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pub.title));

                // Convert byte array to a string   
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    sb.Append(bytes[i].ToString("x2"));
                hash = sb.ToString();
            }

            return hash + pub.has.Length + pub.year;
        }
    }
}
