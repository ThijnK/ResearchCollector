using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ResearchCollector.Importer
{
    class BackToMemory
    {
        public void JsonToMemory(StreamReader sr, Data data)
        {           
            JsonMemArticle[] jarticles = null; JsonMemJournal[] jjournals = null; JsonMemInproceedings[] jinproceedings = null; JsonMemProceedings[] jproceedings = null;
            JsonMemAuthor[] jauthors = null; JsonMemOrganization[] jorganizations = null; JsonMemPerson[] jpersons = null;
            ParseJsonContent(sr, ref jarticles, ref jjournals, ref jinproceedings, ref jproceedings, ref jauthors, ref jorganizations, ref jpersons);

            data.Clear();

            if (jjournals != null)
                foreach (JsonMemJournal jjournal in jjournals)
                    if (!data.journals.ContainsKey(jjournal.title))
                        data.journals.Add(jjournal.title, new Journal(jjournal.issue, jjournal.volume, jjournal.series, jjournal.title, jjournal.publisher));
            if (jproceedings != null)
                foreach (JsonMemProceedings jproceeding in jproceedings)
                    if(!data.proceedings.ContainsKey(jproceeding.title))
                        data.proceedings.Add(jproceeding.title, new Proceedings(jproceeding.title, jproceeding.publisher));
            if (jpersons != null)
                foreach (JsonMemPerson jperson in jpersons)
                    if(!data.persons.ContainsKey(jperson.name))
                        data.persons.Add(jperson.name, new Person(jperson.orcid, jperson.name) { fname = jperson.fname, lname = jperson.lname });
            if (jorganizations != null)
                foreach (JsonMemOrganization jorganization in jorganizations)
                    if(!data.organizations.ContainsKey(jorganization.name))
                        data.organizations.Add(jorganization.name, new Organization(jorganization.name) { locatedAt = new System.Device.Location.GeoCoordinate() }); //locatedAdd nog incorporaten. seperated by , dus .Split(',')[0] en [1]
            if (jauthors != null)
                foreach (JsonMemAuthor jauthor in jauthors)
                    if(!data.authors.ContainsKey(jauthor.name))
                        data.authors.Add(jauthor.name, new Author(data.persons[jauthor.personKey], data.organizations[jauthor.affiliatedToKey], jauthor.email, jauthor.name) { fname = jauthor.fname, lname = jauthor.lname });
            if (jarticles != null)
                foreach (JsonMemArticle jarticle in jarticles)
                    if (!data.articles.ContainsKey(jarticle.id))
                    {
                        data.articles.Add(jarticle.id, new Article(data.journals[jarticle.journalKey], jarticle.id, jarticle.title, jarticle.abstr, jarticle.year, jarticle.doi, jarticle.pdfLink, jarticle.topics, jarticle.pages));
                        data.pubCount++;
                    }
            if (jinproceedings != null)
                foreach (JsonMemInproceedings jinproceeding in jinproceedings)
                    if (!data.inproceedings.ContainsKey(jinproceeding.id))
                    {
                        data.inproceedings.Add(jinproceeding.id, new Inproceedings(data.proceedings[jinproceeding.proceedingsKey], jinproceeding.id, jinproceeding.title, jinproceeding.abstr, jinproceeding.year, jinproceeding.doi, jinproceeding.pdfLink, jinproceeding.topics, jinproceeding.pages));
                        data.pubCount++;
                    }
        }

        void ParseJsonContent(StreamReader sr, ref JsonMemArticle[] jarticles, ref JsonMemJournal[] jjournals, ref JsonMemInproceedings[] jinproceedings, ref JsonMemProceedings[] jproceedings, ref JsonMemAuthor[] jauthors, ref JsonMemOrganization[] jorganizations, ref JsonMemPerson[] jpersons)
        {
            StringBuilder sb;
            sr.ReadLine();
            for (int i = 0; i < 7; i++)
            {
                string current = sr.ReadLine();
                string start = current.Split('"')[1];
                current = sr.ReadLine();
                sb = new StringBuilder(current);
                if (i < 6)
                    RemoveComma(sb, current);
                switch (start)
                {
                    case "articles":
                        jarticles = JsonSerializer.Deserialize<JsonMemArticle[]>(sb.ToString());
                        break;
                    case "journals":
                        jjournals = JsonSerializer.Deserialize<JsonMemJournal[]>(sb.ToString());
                        break;
                    case "inproceedings":
                        jinproceedings = JsonSerializer.Deserialize<JsonMemInproceedings[]>(sb.ToString());
                        break;
                    case "proceedings":
                        jproceedings = JsonSerializer.Deserialize<JsonMemProceedings[]>(sb.ToString());
                        break;
                    case "authors":
                        jauthors = JsonSerializer.Deserialize<JsonMemAuthor[]>(sb.ToString());
                        break;
                    case "organizations":
                        jorganizations = JsonSerializer.Deserialize<JsonMemOrganization[]>(sb.ToString());
                        break;
                    case "persons":
                        //persons is anders omdat het de laatste is. dus geen comma
                        jpersons = JsonSerializer.Deserialize<JsonMemPerson[]>(sb.ToString());
                        break;
                    default:
                        break;
                }
            }
        }

        void RemoveComma(StringBuilder sb, string current)
        {            
            sb.Length -= 2;
            sb.Append("]");
        }
    }
}
