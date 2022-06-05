using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchCollector.Importer
{
    public enum SearchType { Exact, Loose }

    internal class API
    {
        Data data;

        public API(Data data)
        {
            this.data = data;
        }

        public HashSet<T> Search<T>(string searchDomain, SearchType searchType, params (string key, string query)[] arguments)
        {
            bool howToSearch = searchType == SearchType.Exact; //mogelijk beter om searchtype een klasse te maken met een ingebouwde bool -> bool -> bool functie en start bool waarde
            switch (searchDomain)
            {
                case "articles":
                    Func<Article, (string, string), bool> satisfiesArticle = (article, arg) =>
                    {
                        //perhaps also abstract???
                        switch (arg.Item1)
                        {
                            case "title":
                                return article.title == arg.Item2;
                            case "journal":
                                return article.partOf.title == arg.Item2;
                            case "id":
                                return article.id == arg.Item2;
                            case "externals":
                                string[] realIds = article.externalIds.Values.ToArray<string>();
                                return CollectionQuery(realIds, arg.Item2.Split('|'), howToSearch, howToSearch);
                            case "year":
                                return article.year.ToString() == arg.Item2;
                            case "doi":
                                return article.doi == arg.Item2;
                            case "authors":
                                string[] realNames = article.authors.ConvertAll<string>(a => { return a.name; }).ToArray();
                                return CollectionQuery(realNames, arg.Item2.Split('|'), howToSearch, howToSearch);
                            case "topics":
                                return CollectionQuery(article.topics, arg.Item2.Split('|'), howToSearch, howToSearch);
                            default:
                                throw new ArgumentException($"{searchDomain} does not have {arg.Item1} as key");
                        }
                    };
                    return FindItems<T>(data.articles as Dictionary<string, T>, searchType, arguments, satisfiesArticle as Func<T, (string, string), bool>);
                case "inproceedings":
                    Func<Inproceedings, (string, string), bool> satisfiesInproceedings = (inpr, arg) =>
                    {
                        //perhaps also abstract???
                        switch (arg.Item1)
                        { 
                            case "title":
                                return inpr.title == arg.Item2;
                            case "proceedings":
                                return inpr.partOf.title == arg.Item2;
                            case "id":
                                return inpr.id == arg.Item2;
                            case "externals":
                                string[] realIds = inpr.externalIds.Values.ToArray<string>();
                                return CollectionQuery(realIds, arg.Item2.Split('|'), howToSearch, howToSearch);
                            case "year":
                                return inpr.year.ToString() == arg.Item2;
                            case "doi":
                                return inpr.doi == arg.Item2;
                            case "authors":
                                string[] realNames = inpr.authors.ConvertAll<string>(a => { return a.name; }).ToArray();
                                return CollectionQuery(realNames, arg.Item2.Split('|'), howToSearch, howToSearch);
                            case "topics":
                                return CollectionQuery(inpr.topics, arg.Item2.Split('|'), howToSearch, howToSearch);
                            default:
                                throw new ArgumentException($"{searchDomain} does not have {arg.Item1} as key");
                        }
                    };
                    return FindItems<T>(data.inproceedings as Dictionary<string, T>, searchType, arguments, satisfiesInproceedings as Func<T, (string, string), bool>);
                case "authors":
                    Func<Author, (string, string), bool> satisfiesAuthor = (author, arg) =>
                    {
                        //toch raar om te zoeken naar author.person?
                        switch (arg.Item1)
                        {
                            case "affiliation":
                                return author.affiliatedTo.name == arg.Item2;
                            case "email":
                                return author.email == arg.Item2;
                            case "name":
                                return author.name == arg.Item2;
                            case "publications":
                                string[] realTitles = author.publications.ConvertAll<string>(p => { return p.title; }).ToArray();
                                return CollectionQuery(realTitles, arg.Item2.Split('|'), howToSearch, howToSearch);
                            default:
                                throw new ArgumentException($"{searchDomain} does not have {arg.Item1} as key");
                        }
                    };
                    return FindItems<T>(data.authors as Dictionary<string, T>, searchType, arguments, satisfiesAuthor as Func<T, (string, string), bool>);
                case "people":
                    Func<Person, (string, string), bool> satisfiesPerson = (person, arg) =>
                    {
                        switch (arg.Item1)
                        {
                            case "name":
                                return person.name == arg.Item2;
                            case "orcid":
                                return person.orcid == arg.Item2;
                            case "authored":
                                string[] realNames = person.authored.ConvertAll<string>(a => { return a.name; }).ToArray();
                                return CollectionQuery(realNames, arg.Item2.Split('|'), howToSearch, howToSearch);
                            default:
                                throw new ArgumentException($"{searchDomain} does not have {arg.Item1} as key");
                        }
                    };
                    return FindItems<T>(data.persons as Dictionary<string, T>, searchType, arguments, satisfiesPerson as Func<T, (string, string), bool>);
                case "journals":
                    Func<Journal, (string, string), bool> satisfiesJournal = (journal, arg) =>
                    {
                        switch (arg.Item1)
                        {                           
                            case "title":
                                return journal.title == arg.Item2;
                            case "issue":
                                return journal.issue == arg.Item2;
                            case "series":
                                return journal.series == arg.Item2;
                            case "volume":
                                return journal.volume == arg.Item2;
                            default:
                                throw new ArgumentException($"{searchDomain} does not have {arg.Item1} as key");
                        }
                    };
                    return FindItems<T>(data.journals as Dictionary<string, T>, searchType, arguments, satisfiesJournal as Func<T, (string, string), bool>);
                case "proceedings":
                    Func<Proceedings, (string, string), bool> satisfiesProceedings = (proc, arg) =>
                    {
                        switch (arg.Item1)
                        { 
                            case "title":
                                return proc.title == arg.Item2;
                            default:
                                throw new ArgumentException($"{searchDomain} does not have {arg.Item1} as key");
                        }
                    };
                    return FindItems<T>(data.proceedings as Dictionary<string, T>, searchType, arguments, satisfiesProceedings as Func<T, (string, string), bool>);
                case "organizations":
                    Func<Organization, (string, string), bool> satisfiesOrganization = (org, arg) =>
                    {
                        switch (arg.Item1)
                        {
                            case "name":
                                return org.name == arg.Item2;
                            case "location":
                                return org.locatedAt.ToString() == arg.Item2;
                            default:
                                throw new ArgumentException($"{searchDomain} does not have {arg.Item1} as key");
                        }
                    };
                    return FindItems<T>(data.organizations as Dictionary<string, T>, searchType, arguments, satisfiesOrganization as Func<T, (string, string), bool>);
                default:
                    throw new ArgumentException($"Searchdomain {searchDomain} does not exist");
            }
        }       
        
        bool CollectionQuery(string[] realCollection, string[] queryCollection, bool howToCompare, bool start)
        {          
            foreach (string real in realCollection)
            {
                bool thisOne = false;
                foreach (string query in queryCollection)
                    if (real == query)
                        thisOne = true;
                start = howToCompare ? start && thisOne : start || thisOne;
            }
            return start;
        }

        private HashSet<T> FindItems<T>(Dictionary<string, T> toSearch, SearchType searchType, (string, string)[] args, Func<T,(string,string),bool> Satisfies)
        {
            HashSet<T> result = new HashSet<T>();
            foreach (T item in toSearch.Values)
            {
                // Whether or not this item satisfies the arguments, given the searchType
                // i.e. it does not need to satisfy ALL if searchType is loose
                bool satisfied = true;

                for (int i = 0; i < args.Length; i++)
                {
                    // Whether or not this items satisfies CURRENT argument
                    bool satisfiesArg = Satisfies(item, args[i]);

                    // Loose search
                    if (searchType == SearchType.Loose)
                    {
                        // If this item satisfies this argument, add to result and move on to next item
                        if (satisfiesArg)
                            break;
                        // If we get to the last arg and the item has not satisfied any of the args, do not add to result
                        else if (i == args.Length - 1)
                            satisfied = false;
                    }
                    // Exact search: ...
                    else if (searchType == SearchType.Exact)
                    {
                        satisfied &= satisfiesArg;
                        // If it does not satisfy current argument, immediately move on to next item
                        if (!satisfiesArg)
                            break;
                    }
                }

                if (satisfied)
                    result.Add(item);
            }

            return result;
        }

        
    }
}
