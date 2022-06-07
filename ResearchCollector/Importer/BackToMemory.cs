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
            JsonMemArticle[] jarticles;
            JsonMemJournal[] jjournals;
            JsonMemInproceedings[] jinproceedings;
            JsonMemProceedings[] jproceedings;
            JsonMemAuthor[] jauthors;
            JsonMemOrganization[] jorganizations;
            JsonMemPerson[] jpersons;

            for (int i = 0; i < 7; i++)
            {
                string current = sr.ReadLine();
                string start = current.Split('"')[1];
                current = sr.ReadLine();
                sb = new StringBuilder(current);
                if(i < 6)
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
