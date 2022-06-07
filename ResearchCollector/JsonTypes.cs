using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchCollector
{
    // Contains classes used for JSON serialization into the native json format

    class JsonPublication
    {
        public JsonExternalId[] externalIds { get; set; }
        public string origin { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string doi { get; set; }
        public string pdfLink { get; set; }
        public string pages { get; set; }
        public object partof { get; set; }
        public JsonAuthor[] has { get; set; }
    }

    struct JsonExternalId
    {
        public string id { get; set; }
        public string origin { get; set; }

        public JsonExternalId(string id, string origin)
        {
            this.id = id;
            this.origin = origin;
        }
    }

    class JsonVolume
    {
        public string title { get; set; }
        public string publisher { get; set; }

        public JsonVolume(string title, string publisher)
        {
            this.title = title;
            this.publisher = publisher;
        }

        /// <summary>
        /// Reset the attributes of this Volume to an empty string, to be filled for the next publication
        /// </summary>
        public virtual void Reset()
        {
            this.title = "";
        }
    }

    class JsonJournal : JsonVolume
    {
        public string issue { get; set; }
        public string volume { get; set; }
        public string series { get; set; }

        public JsonJournal(string title, string publisher, string issue, string volume, string series) : base(title, publisher)
        {
            this.issue = issue;
            this.volume = volume;
            this.series = series;
        }

        public override void Reset()
        {
            base.Reset();
            this.issue = "";
            this.volume = "";
            this.series = "";
        }
    }

    struct JsonAuthor
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string orcid { get; set; }
        public string affiliatedTo { get; set; }

        public JsonAuthor(string fname, string lname, string name, string email, string orcid, string affiliatedTo)
        {
            this.fname = fname;
            this.lname = lname;
            this.name = name;
            this.email = email;
            this.orcid = orcid;
            this.affiliatedTo = affiliatedTo;
        }
    }
}
