using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchCollector.Importer
{
    struct OriginIdPair
    {
        public string origin { get; set; }
        public string id { get; set; }
    }
    struct JsonMemArticle
    {
        public OriginIdPair[] externalIds { get; set; }
        public string journalKey { get; set; }
        public string[] topics { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string abstr { get; set; }
        public int year { get; set; }
        public string doi { get; set; }
        public string pdfLink { get; set; }
        public string[] authorKeys { get; set; }
    }

    struct JsonMemJournal
    {
        public string title { get; set; }
        public string issue { get; set; }
        public string volume { get; set; }
        public string series { get; set; }
    }

    struct JsonMemInproceedings
    {
        public OriginIdPair[] externalIds { get; set; }
        public string proceedingsKey { get; set; }
        public string[] topics { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string abstr { get; set; }
        public int year { get; set; }
        public string doi { get; set; }
        public string pdfLink { get; set; }
        public string[] authorKeys { get; set; }
    }

    struct JsonMemProceedings
    {
        public string title { get; set; }
    }

    struct JsonMemAuthor
    {
        public string email { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string name { get; set; }
        public string personKey { get; set; }
        public string affiliatedToKey { get; set; }
    }

    struct JsonMemPerson
    {
        public string orcid { get; set; }
        public string name { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
    }

    struct JsonMemOrganization
    {
        //public GeoCoordinate locatedAt;
        public string name { get; set; }
    }
}
