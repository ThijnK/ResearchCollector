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
        public string type { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string partof { get; set; }
        public string doi { get; set; }
        public JsonPerson[] authors { get; set; }

        public JsonPublication(string type, string title, int year, string partof, string doi, JsonPerson[] authors)
        {
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
        public Dictionary<string, Inproceedings> inproceedings;
    }

    class Publication
    {

    }

    class Author
    {

    }

    class Person
    {

    }

    class Journal
    {

    }

    class Inproceedings
    {

    }
    #endregion
}
