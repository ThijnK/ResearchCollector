using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
    // Just a setup for now, may be subject to change
    class Data
    {
        public Dictionary<string, Publication> publications;
        public Dictionary<string, Author> authors;
        public Dictionary<string, Person> people;
        public Dictionary<string, Journal> journals;
        public Dictionary<string, Proceedings> proceedings;
        public Dictionary<string, Organization> organizations;

        public Data()
        {
            publications = new Dictionary<string, Publication>();
            authors = new Dictionary<string, Author>();
            people = new Dictionary<string, Person>();
            journals = new Dictionary<string, Journal>();
            proceedings = new Dictionary<string, Proceedings>();
            organizations = new Dictionary<string, Organization>();
        }
    }

    class Publication
    {
        // May have to contain the text extracted from pdf

        public string id;
        public string title;
        public int year;
        public string doi;

        public List<Author> authors;

        public Publication(string id, string title, int year, string doi)
        {
            authors = new List<Author>();
            this.id = id;
            this.title = title;
            this.year = year;
            this.doi = doi;
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

        public Article(Journal journal, string id, string title, int year, string doi) : base(id, title, year, doi)
        {
            this.journal = journal;
        }
    }

    class Inproceedings : Publication
    {
        public Proceedings proceedings;

        public Inproceedings(Proceedings proceedings, string id, string title, int year, string doi) : base(id, title, year, doi)
        {
            this.proceedings = proceedings;
        }
    }

    class Author
    {
        public Person person;
        public Organization organization;
        public string email;
        public string name;
        public List<Publication> publications;

        // This will ensure that the list cannot be altered with Add or Remove and such outside of this class
        //public IReadOnlyCollection<Publication> Publications => publications.AsReadOnly();

        public Author(Person person, Organization organization, string email, string name)
        {
            publications = new List<Publication>();
            this.person = person;
            this.organization = organization;
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
        string name;

        public Organization(string name)
        {
            this.name = name;
        }
    }
    #endregion
}
