using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text;
using System.Text.Json;

namespace ResearchCollector.Importer
{
    #region Data types for storage in memory
    class Data
    {
        /// <summary>
        /// Number of publications contained in this object
        /// </summary>
        public int pubCount;

        // Articles and Inproceedings are indexed by their custom id created by us
        public Dictionary<string, Article> articles;
        public Dictionary<string, Inproceedings> inproceedings;

        public Dictionary<string, Author> authors;
        public Dictionary<string, Person> persons;
        public Dictionary<string, Journal> journals;
        public Dictionary<string, Proceedings> proceedings;
        public Dictionary<string, Organization> organizations;

        public Data()
        {
            articles = new Dictionary<string, Article>();
            inproceedings = new Dictionary<string, Inproceedings>();
            authors = new Dictionary<string, Author>();
            persons = new Dictionary<string, Person>();
            journals = new Dictionary<string, Journal>();
            proceedings = new Dictionary<string, Proceedings>();
            organizations = new Dictionary<string, Organization>();
        }

        void LengthCheck(StringBuilder sb, int length)
        {
            if (length > 0)
                sb.Length--;
        }

        public StringBuilder ToJson()
        {
            StringBuilder sb = new StringBuilder("\'\"articles\":[");

            HashSet<Author> encounteredAuthors = new HashSet<Author>();
            HashSet<Journal> encounteredJournals = new HashSet<Journal>();
            foreach (Article article in this.articles.Values)
            {
                article.ToJson(sb, encounteredJournals, encounteredAuthors);
                sb.Append(",");
            }
            LengthCheck(sb, this.articles.Values.Count);

            sb.Append("],\"journals\":[");
            foreach (Journal journal in encounteredJournals)
            {
                journal.ToJson(sb);
                sb.Append(",");
            }
            LengthCheck(sb, encounteredJournals.Count);

            sb.Append("],\"inproceedings\":[");
            HashSet<Proceedings> encounteredproceedings = new HashSet<Proceedings>();
            foreach (Inproceedings inproceedings in this.inproceedings.Values)
            {
                inproceedings.ToJson(sb, encounteredproceedings, encounteredAuthors);
                sb.Append(",");
            }
            LengthCheck(sb, this.inproceedings.Values.Count);

            sb.Append("],\"proceedings\":[");
            foreach (Proceedings proceedings in encounteredproceedings)
            {
                proceedings.ToJson(sb);
                sb.Append(",");
            }
            LengthCheck(sb, encounteredproceedings.Count);

            sb.Append("],\"authors\":[");
            HashSet<Organization> organizations = new HashSet<Organization>();
            HashSet<Person> persons = new HashSet<Person>();
            foreach (Author author in encounteredAuthors)
            {
                author.ToJson(sb, organizations, persons);
                sb.Append(",");
            }
            LengthCheck(sb, encounteredAuthors.Count);

            sb.Append("],\"organizations\":[");
            foreach (Organization orginazation in organizations)
            {
                orginazation.ToJson(sb);
                sb.Append(",");
            }
            LengthCheck(sb, organizations.Count);

            sb.Append("],\"persons\":[");
            foreach (Person person in persons)
            {
                person.ToJson(sb);
                sb.Append(",");
            }
            LengthCheck(sb, persons.Count);

            sb.Append("]\'");

            return sb;
        }
    }

    class Publication
    {
        public string id;
        public Dictionary<string, string> externalIds;
        public string title;
        public string abstr; //act (abstract is a reserved keyword)
        public int year;
        public string doi;
        public string pdfLink;
        public string[] topics;

        public List<Author> authors;

        public Publication(string id, string title, string abstr, int year, string doi, string pdfLink, string[] topics)
        {
            authors = new List<Author>();
            externalIds = new Dictionary<string, string>();
            this.id = id;
            this.title = title;
            this.abstr = abstr;
            this.year = year;
            this.doi = doi;
            this.pdfLink = pdfLink;
            this.topics = topics;
        }

        /// <summary>
        /// Add the given Author to this publication.
        /// Also adds this Publiction to the Author.
        /// </summary>
        public void AddAuthor(Author author)
        {
            authors.Add(author);
            author.publications.Add(this);
        }
    }

    class Article : Publication
    {
        public Journal partOf;

        public Article(Journal journal, string id, string title, string abstr, int year, string doi, string pdfLink, string[] topics) : base(id, title, abstr, year, doi, pdfLink, topics)
        {
            this.partOf = journal;
        }

        public void ToJson(StringBuilder sb, HashSet<Journal> encounteredJournals, HashSet<Author> encounteredAuthors)
        {
            if (!encounteredJournals.Contains(this.partOf))
                encounteredJournals.Add(this.partOf);

            string[] authors = new string[this.authors.Count];
            for (int i = 0; i < this.authors.Count; i++)
            {
                Author author = this.authors[i];
                if (!encounteredAuthors.Contains(author))
                    encounteredAuthors.Add(author);
                authors[i] = author.name;
            }

            JsonMemArticle jarticle = new JsonMemArticle { abstr = this.abstr, doi = this.doi, authorKeys = authors, id = this.id, journalKey = this.partOf.title, pdfLink = this.pdfLink, title = this.title, topics = this.topics, year = this.year };
            string current = JsonSerializer.Serialize<JsonMemArticle>(jarticle);
            sb.Append(current);
        }
    }

    class Inproceedings : Publication
    {
        public Proceedings partOf;

        public Inproceedings(Proceedings proceedings, string id, string title, string abstr, int year, string doi, string pdfLink, string[] topics) : base(id, title, abstr, year, doi, pdfLink, topics)
        {
            this.partOf = proceedings;
        }

        public void ToJson(StringBuilder sb, HashSet<Proceedings> encounteredproceedings, HashSet<Author> encounteredAuthors)
        {
            if (!encounteredproceedings.Contains(this.partOf))
                encounteredproceedings.Add(this.partOf);

            string[] authors = new string[this.authors.Count];
            for (int i = 0; i < this.authors.Count; i++)
            {
                Author author = this.authors[i];
                if (!encounteredAuthors.Contains(author))
                    encounteredAuthors.Add(author);
                authors[i] = author.name;
            }

            JsonMemInproceedings jinproceedings = new JsonMemInproceedings { abstr = this.abstr, doi = this.doi, authorKeys = authors, id = this.id, proceedingsKey = this.partOf.title, pdfLink = this.pdfLink, title = this.title, topics = this.topics, year = this.year };
            string current = JsonSerializer.Serialize<JsonMemInproceedings>(jinproceedings);
            sb.Append(current);
        }
    }

    class Author
    {
        public string email;
        public string fname;
        public string lname;
        public string name;
        public Person person;
        public Organization affiliatedTo;
        public List<Publication> publications;

        // This will ensure that the list cannot be altered with Add or Remove and such outside of this class
        //public IReadOnlyCollection<Publication> Publications => publications.AsReadOnly();

        public Author(Person person, Organization affiliation, string email, string name)
        {
            publications = new List<Publication>();
            this.person = person;
            this.affiliatedTo = affiliation;
            this.email = email;
            this.name = name;
            person.authored.Add(this);
        }

        public void ToJson(StringBuilder sb, HashSet<Organization> organizations, HashSet<Person> persons)
        {
            if (!organizations.Contains(this.affiliatedTo))
                organizations.Add(this.affiliatedTo);
            if (!persons.Contains(this.person))
                persons.Add(this.person);

            JsonMemAuthor jauthor = new JsonMemAuthor { affiliatedToKey = this.affiliatedTo.name, email = this.email, fname = this.fname, lname = this.lname, name = this.name, personKey = this.person.name };
            string current = JsonSerializer.Serialize<JsonMemAuthor>(jauthor);
            sb.Append(current);
        }
    }

    class Person
    {
        public List<Author> authored;
        public string orcid;
        public string name;
        public string fname;
        public string lname;

        public Person(string orcid, string name)
        {
            this.orcid = orcid;
            this.name = name;
            authored = new List<Author>();
        }

        public void ToJson(StringBuilder sb)
        {
            JsonMemPerson jperson = new JsonMemPerson { name = this.name, fname = this.fname, lname = this.lname, orcid = this.orcid };
            string current = JsonSerializer.Serialize<JsonMemPerson>(jperson);
            sb.Append(current);
        }
    }

    class PublicationVolume
    {
        public string title;

        public PublicationVolume(string title)
        {
            this.title = title;
        }
    }

    class Journal : PublicationVolume
    {
        public string issue;
        public string volume;
        public string series;

        public Journal(string issue, string volume, string series, string title) : base(title)
        {
            this.issue = issue;
            this.volume = volume;
            this.series = series;
        }

        public void ToJson(StringBuilder sb)
        {
            JsonMemJournal jjournal = new JsonMemJournal { issue = this.issue, series = this.series, title = this.title, volume = this.volume };
            string current = JsonSerializer.Serialize<JsonMemJournal>(jjournal);
            sb.Append(current);
        }
    }

    class Proceedings : PublicationVolume
    {
        public Proceedings(string title) : base(title)
        {

        }

        public void ToJson(StringBuilder sb)
        {
            JsonMemProceedings jproceedings = new JsonMemProceedings { title = this.title };
            string current = JsonSerializer.Serialize<JsonMemProceedings>(jproceedings);
            sb.Append(current);
        }
    }

    class Organization
    {
        public GeoCoordinate locatedAt;
        public string name;

        public Organization(string name)
        {
            this.name = name;
            // Retrieve location from wikipedia (or somewhere else)
            this.locatedAt = new GeoCoordinate(-90.0, -180.0);
        }

        public void ToJson(StringBuilder sb)
        {
            JsonMemOrganization jorganization = new JsonMemOrganization { name = this.name };
            string current = JsonSerializer.Serialize<JsonMemOrganization>(jorganization);
            sb.Append(current);
        }
    }

    #endregion
}
