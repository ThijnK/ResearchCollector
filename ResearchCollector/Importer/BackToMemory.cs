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
        public void JsonToMemory(StreamReader sr)
        {
            StringBuilder sb;
            sr.ReadLine();
            string current = sr.ReadLine();
            JsonMemArticle[] jarticles;
            if (current.StartsWith("\"articles\""))
            {
                sb = new StringBuilder(sr.ReadLine());
                sb.Length -= 2;
                sb.Append("]");
                jarticles = JsonSerializer.Deserialize<JsonMemArticle[]>(sb.ToString());
                current = sr.ReadLine();
            }

            JsonMemJournal[] jjournals;
            if (current.StartsWith("\"journals\""))
            {
                sb = new StringBuilder(sr.ReadLine());
                sb.Length -= 2;
                sb.Append("]");
                jjournals = JsonSerializer.Deserialize<JsonMemJournal[]>(sb.ToString());
                current = sr.ReadLine();
            }

            JsonMemInproceedings[] jinproceedings;
            if (current.StartsWith("\"inproceedings\""))
            {
                sb = new StringBuilder(sr.ReadLine());
                sb.Length -= 2;
                sb.Append("]");
                jinproceedings = JsonSerializer.Deserialize<JsonMemInproceedings[]>(sb.ToString());
                current = sr.ReadLine();
            }

            JsonMemProceedings[] jproceedings;
            if (current.StartsWith("\"proceedings\""))
            {
                sb = new StringBuilder(sr.ReadLine());
                sb.Length -= 2;
                sb.Append("]");
                jproceedings = JsonSerializer.Deserialize<JsonMemProceedings[]>(sb.ToString());
                current = sr.ReadLine();
            }

            JsonMemAuthor[] jauthors;
            if (current.StartsWith("\"authors\""))
            {
                sb = new StringBuilder(sr.ReadLine());
                sb.Length -= 2;
                sb.Append("]");
                jauthors = JsonSerializer.Deserialize<JsonMemAuthor[]>(sb.ToString());
                current = sr.ReadLine();
            }

            JsonMemOrganization[] jorganizations;
            if (current.StartsWith("\"organizations\""))
            {
                sb = new StringBuilder(sr.ReadLine());
                sb.Length -= 2;
                sb.Append("]");
                jorganizations = JsonSerializer.Deserialize<JsonMemOrganization[]>(sb.ToString());
                current = sr.ReadLine();
            }

            //persons is anders omdat het de laatste is. dus geen comma
            JsonMemPerson[] jpersons;
            if (current.StartsWith("\"persons\""))
            {
                sb = new StringBuilder(sr.ReadLine());
                sb.Length -= 2;
                sb.Append("]");
                jpersons = JsonSerializer.Deserialize<JsonMemPerson[]>(sb.ToString());
                current = sr.ReadLine();
            }
        }
    }
}
