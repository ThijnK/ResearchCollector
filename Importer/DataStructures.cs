using System.Collections.Generic;
using System.Device.Location;

namespace Importer
{
    #region Data types used for JSON deserialization
    struct JsonPublication
    {
        public string id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string partof { get; set; }
        public string doi { get; set; }
        public JsonPerson[] authors { get; set; }

        public JsonPublication(string id, string type, string title, int year, string partof, string doi, JsonPerson[] authors)
        {
            this.id = id;
            this.type = type;
            this.title = title;
            this.year = year;
            this.doi = doi;
            this.partof = partof;
            this.authors = authors;
        }
    }

    struct JsonPerson
    {
        public string name { get; set; }
        public string orcid { get; set; }

        public JsonPerson(string name, string orcid)
        {
            this.name = name;
            this.orcid = orcid;
        }
    }
    #endregion

    #region Data types for storage in memory
    class Data
    {
        public Dictionary<string, Publication> publications;
        public Dictionary<string, Author> authors;
        public Dictionary<string, Person> people;
        public Dictionary<string, PublicationVolume> volumes;
        public Dictionary<string, Organization> organizations;

        public Data()
        {
            publications = new Dictionary<string, Publication>();
            authors = new Dictionary<string, Author>();
            people = new Dictionary<string, Person>();
            volumes = new Dictionary<string, PublicationVolume>();
            organizations = new Dictionary<string, Organization>();
        }
    }

    class Publication
    {
        public string id;
        public string title;
        public int year;
        public string doi;
        public string[] topics;

        public List<Author> authors;

        public Publication(string id, string title, int year, string doi, string[] topics)
        {
            authors = new List<Author>();
            this.id = id;
            this.title = title;
            this.year = year;
            this.doi = doi;
            this.topics = topics;
        }

        public void AddAuthor(Author author)
        {
            authors.Add(author);
            author.publications.Add(this);
        }
    }

    class Article : Publication
    {
        public Journal journal;

        public Article(Journal journal, string id, string title, int year, string doi, string[] topics) : base(id, title, year, doi, topics)
        {
            this.journal = journal;
        }
    }

    class Inproceedings : Publication
    {
        public Proceedings proceedings;

        public Inproceedings(Proceedings proceedings, string id, string title, int year, string doi, string[] topics) : base(id, title, year, doi, topics)
        {
            this.proceedings = proceedings;
        }
    }

    class Author
    {
        public Person person;
        public Organization affiliation;
        public string email;
        public string name;
        public List<Publication> publications;

        // This will ensure that the list cannot be altered with Add or Remove and such outside of this class
        //public IReadOnlyCollection<Publication> Publications => publications.AsReadOnly();

        public Author(Person person, Organization affiliation, string email, string name)
        {
            publications = new List<Publication>();
            this.person = person;
            this.affiliation = affiliation;
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

        public Person(string orcid, string name)
        {
            this.orcid = orcid;
            this.name = name;
            authored = new List<Author>();
        }
    }

    class PublicationVolume
    {
        string title;

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
        GeoCoordinate location;
        string name;

        public Organization(string name)
        {
            this.name = name;
            // Retrieve location from wikipedia (or somewhere else)
            this.location = new GeoCoordinate(-90.0, -180.0);
        }
    }
    #endregion
}
