using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchCollector.Importer
{
    enum SearchWay { Exact, Loose }
    internal class API
    {
        public HashSet<Article> SearchArticle(Dictionary<string, Article> searchedItems, SearchWay searchWay, params (string key, string query)[] queryArguments)
        {

            HashSet<Article> result = new HashSet<Article>();         
           
            foreach(Article article in searchedItems.Values)
            {
                bool addIt = false;

                //eerste los want exact
                InitialQuery:
                (string key, string query) = queryArguments[0];
                switch (key)
                {
                    case "title":
                        addIt = CompareFirst(article.title, query, searchWay);
                        break;
                    default: //als de eerste key invalid is telt ie niet mee als eerste
                        goto InitialQuery;
                }

                for (int i = 1; i < queryArguments.Length; i++)
                {
                    (key, query) = queryArguments[i];
                    switch (key)
                    {
                        case "title":
                            addIt = Compare(article.title, query, searchWay, addIt);
                            break;
                        default:
                            continue;
                    }
                }
                if (addIt)
                    result.Add(article);
            }
            return result;
        }

        bool Compare(string real, string query, SearchWay searchWay, bool currentJudgement)
        {
            bool equal = real == query;
            if (searchWay == SearchWay.Exact)
                return currentJudgement && equal;
            if (searchWay == SearchWay.Loose)
                return currentJudgement || equal;
            throw new Exception("Not all search ways implemented");
        }

        bool CompareFirst(string real, string query, SearchWay searchWay)
        {
            bool equal = real == query;
            if (searchWay == SearchWay.Exact)
                return equal;
            if (searchWay == SearchWay.Loose)
                return equal;
            throw new Exception("Not all search ways implemented");
        }

    }
}
