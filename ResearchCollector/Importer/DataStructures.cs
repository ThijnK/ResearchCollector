using System.Collections.Generic;
using System.Device.Location;

namespace ResearchCollector.Importer
{
    #region Data types for storage in memory
    class Data
    {
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
    }

    class Inproceedings : Publication
    {
        public Proceedings partOf;

        public Inproceedings(Proceedings proceedings, string id, string title, string abstr, int year, string doi, string pdfLink, string[] topics) : base(id, title, abstr, year, doi, pdfLink, topics)
        {
            this.partOf = proceedings;
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
    }

    class Proceedings : PublicationVolume
    {
        public Proceedings(string title) : base(title)
        {

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
    }
    #endregion
}
