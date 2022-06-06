using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private int totalPubCount;
        private string path;

        public Importer(SynchronizationContext context, string path, Data data) : base(context)
        {
            this.path = path;
            this.data = data;
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
            this.worker = worker;

            using (StreamReader sr = new StreamReader(path))
            {
                string t = sr.ReadLine();
                string t2 = sr.ReadLine();
                string line = "";
                while (data.pubCount < totalPubCount)
                {
                    line = sr.ReadLine();
                    // For all lines that are not the last line, remove the comma from the end
                    if (data.pubCount < totalPubCount - 1)
                        line = line.Remove(line.Length - 1, 1);
                    pub = JsonSerializer.Deserialize<JsonPublication>(line);
                    ParsePublication();
                }

                StringBuilder sbTotal = new StringBuilder("\'\"articles\":[");

                HashSet<Author> encounteredAuthors = new HashSet<Author>();
                HashSet<Journal> encounteredJournals = new HashSet<Journal>();
                foreach (Article article in data.articles.Values)
                {
                    if (!encounteredJournals.Contains(article.partOf))
                        encounteredJournals.Add(article.partOf);

                    string[] authors = new string[article.authors.Count];
                    for( int i = 0; i < article.authors.Count; i++)
                    {
                        Author author = article.authors[i];
                        if (!encounteredAuthors.Contains(author))
                            encounteredAuthors.Add(author);
                        authors[i] = author.name;
                    }


                    JsonMemArticle jarticle = new JsonMemArticle { abstr = article.abstr, doi = article.doi, authorKeys = authors, id = article.id, journalKey = article.partOf.title, pdfLink = article.pdfLink, title = article.title, topics = article.topics, year = article.year };
                    string current = JsonSerializer.Serialize<JsonMemArticle>(jarticle);
                    sbTotal.Append(current);
                    sbTotal.Append(",");
                }
                if(data.articles.Values.Count > 0)
                    sbTotal.Length--;
                sbTotal.Append("],\"journals\":[");

                foreach (Journal journal in encounteredJournals)
                {
                    JsonMemJournal jjournal = new JsonMemJournal { issue = journal.issue, series = journal.series, title = journal.title, volume = journal.volume };
                    string current = JsonSerializer.Serialize<JsonMemJournal>(jjournal);
                    sbTotal.Append(current);
                    sbTotal.Append(",");
                }
                if(encounteredJournals.Count > 0)
                    sbTotal.Length--;
                sbTotal.Append("],\"inproceedings\":[");

                HashSet<Proceedings> encounteredproceedings = new HashSet<Proceedings>();
                foreach (Inproceedings inproceedings in data.inproceedings.Values)
                {
                    if (!encounteredproceedings.Contains(inproceedings.partOf))
                        encounteredproceedings.Add(inproceedings.partOf);

                    string[] authors = new string[inproceedings.authors.Count];
                    for (int i = 0; i < inproceedings.authors.Count; i++)
                    {
                        Author author = inproceedings.authors[i];
                        if (!encounteredAuthors.Contains(author))
                            encounteredAuthors.Add(author);
                        authors[i] = author.name;
                    }

                    JsonMemInproceedings jinproceedings = new JsonMemInproceedings { abstr = inproceedings.abstr, doi = inproceedings.doi, authorKeys = authors, id = inproceedings.id, proceedingsKey = inproceedings.partOf.title, pdfLink = inproceedings.pdfLink, title = inproceedings.title, topics = inproceedings.topics, year = inproceedings.year };
                    string current = JsonSerializer.Serialize<JsonMemInproceedings>(jinproceedings);
                    sbTotal.Append(current);
                    sbTotal.Append(",");
                }
                if (data.inproceedings.Values.Count > 0)
                    sbTotal.Length--;
                sbTotal.Append("],\"proceedings\":[");

                foreach (Proceedings proceedings in encounteredproceedings)
                {
                    JsonMemProceedings jproceedings = new JsonMemProceedings { title = proceedings.title };
                    string current = JsonSerializer.Serialize<JsonMemProceedings>(jproceedings);
                    sbTotal.Append(current);
                    sbTotal.Append(",");
                }
                if (encounteredproceedings.Count > 0)
                    sbTotal.Length--;
                sbTotal.Append("],\"authors\":[");

                HashSet<Organization> organizations = new HashSet<Organization>();
                HashSet<Person> persons = new HashSet<Person>();
                foreach (Author author in encounteredAuthors)
                {
                    if (!organizations.Contains(author.affiliatedTo))
                        organizations.Add(author.affiliatedTo);
                    if (!persons.Contains(author.person))
                        persons.Add(author.person);

                    JsonMemAuthor jauthor = new JsonMemAuthor { affiliatedToKey = author.affiliatedTo.name, email = author.email, fname = author.fname, lname = author.lname, name = author.name, personKey = author.person.name };
                    string current = JsonSerializer.Serialize<JsonMemAuthor>(jauthor);
                    sbTotal.Append(current);
                    sbTotal.Append(",");
                }
                if (encounteredAuthors.Count > 0)
                    sbTotal.Length--;
                sbTotal.Append("],\"organizations\":[");

                foreach (Organization orginazation in organizations)
                {
                    JsonMemOrganization jorganization = new JsonMemOrganization { name = orginazation.name };
                    string current = JsonSerializer.Serialize<JsonMemOrganization>(jorganization);
                    sbTotal.Append(current);
                    sbTotal.Append(",");
                }
                if (organizations.Count > 0)
                    sbTotal.Length--;
                sbTotal.Append("],\"persons\":[");

                foreach (Person person in persons)
                {
                    JsonMemPerson jperson = new JsonMemPerson { name = person.name, fname = person.fname, lname = person.lname, orcid = person.orcid };
                    string current = JsonSerializer.Serialize<JsonMemPerson>(jperson);
                    sbTotal.Append(current);
                    sbTotal.Append(",");
                }
                if (persons.Count > 0)
                    sbTotal.Length--;
                sbTotal.Append("]\'");

                using (StreamWriter sw = new StreamWriter("pizza.json"))
                {
                    sw.Write(sbTotal);
                }
            }
        }

        private void ParsePublication()
        {
            data.pubCount++;

            // Our custom id for publication
            string customId = CreateId();

            Publication currentPublication = GoThroughPublication(customId);

            GoThroughAuthors(currentPublication);

            // Report action and progress to UI          
            ReportAction($"Item parsed: '{pub.title}'");
            UpdateProgress();
        }

        Publication GoThroughPublication(string customId)
        {
            Publication currentPublication;           

            //if the publication is an article
            if (pub.type == "article")
            {
                //if it is an article, it is part of a journal
                JsonJournal jsonJournal = JsonSerializer.Deserialize<JsonJournal>(pub.partof.ToString());

                if (!data.journals.TryGetValue(jsonJournal.title, out Journal currentJournal))
                {
                    currentJournal = new Journal(jsonJournal.issue, jsonJournal.volume, jsonJournal.series, jsonJournal.title);
                    //try
                    //{
                        data.journals.Add(currentJournal.title, currentJournal);
                    //}
                    //catch(Exception e) { }
                }

                if (!data.articles.TryGetValue(customId, out Article currentArticle))
                {
                    //abstract and topics are left empty because those do not get retrieved currently
                    currentArticle = new Article(currentJournal, customId, pub.title, null, pub.year, pub.doi, pub.pdfLink, null);
                    //try
                    //{
                        data.articles.Add(customId, currentArticle);
                    //}
                    //catch(Exception e) { }
                }

                currentPublication = currentArticle;
            }
            //if the publication is an inproceedings
            else
            {
                //if it is an inproceedings, it is part of a proceedings
                JsonVolume jsonVolume = JsonSerializer.Deserialize<JsonVolume>(pub.partof.ToString());
                if (!data.proceedings.TryGetValue(jsonVolume.title, out Proceedings currentProceedings))
                {
                    currentProceedings = new Proceedings(jsonVolume.title);
                    //try
                    //{
                        data.proceedings.Add(currentProceedings.title, currentProceedings);
                    //}
                    //catch(Exception e) { }
                }

                if (!data.inproceedings.TryGetValue(customId, out Inproceedings currentInproceedings))
                {
                    //abstract and topics are left empty because those do not get retrieved currently
                    currentInproceedings = new Inproceedings(currentProceedings, customId, pub.title, null, pub.year, pub.doi, pub.pdfLink, null);
                    //try
                    //{
                        data.inproceedings.Add(customId, currentInproceedings);
                    //}
                    //catch (Exception e) { }
                }

                currentPublication = currentInproceedings;
            }
            //add the external id from the source to the id's of the publication
            foreach (JsonExternalId exId in pub.externalIds)
            {
                if (!string.IsNullOrEmpty(exId.id) && !currentPublication.externalIds.ContainsKey(exId.origin))
                    currentPublication.externalIds.Add(exId.origin, exId.id);
            }

            return currentPublication;
        }

        void GoThroughAuthors(Publication currentPublication)
        {
            foreach (JsonAuthor author in pub.has)
            {
                if (!data.organizations.TryGetValue(author.affiliatedTo, out Organization currentAffiliation))
                {
                    //if affilition == "", skip it possibly TODO
                    currentAffiliation = new Organization(author.affiliatedTo);
                    //try
                    //{
                        data.organizations.Add(author.affiliatedTo, currentAffiliation);
                    //}
                    //catch(Exception e) { }

                    //still needs geolocalization
                }

                if (!data.persons.TryGetValue(author.name, out Person currentPerson))
                {
                    currentPerson = new Person(author.orcid, author.name);
                    currentPerson.fname = author.fname; currentPerson.lname = author.lname;
                    //try
                    //{
                        data.persons.Add(author.name, currentPerson);
                    //}
                    //catch(Exception e) { }
                }

                if (!data.authors.TryGetValue(author.name, out Author currentAuthor))
                {
                    currentAuthor = new Author(currentPerson, currentAffiliation, author.email, author.name);
                    currentAuthor.fname = author.fname; currentAuthor.lname = author.lname;
                    //possibly first check if it was already added TODO
                    currentAuthor.publications.Add(currentPublication);

                    //try
                    //{
                        data.authors.Add(author.name, currentAuthor);
                    //}
                    //catch(Exception e) { }
                }

                currentPublication.authors.Add(currentAuthor);
            }
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
