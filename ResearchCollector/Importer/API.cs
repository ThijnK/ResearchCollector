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
        #region Version Thinus
        Data data;

        public API(Data data)
        {
            this.data = data;
        }

        public HashSet<T> Search<T>(string searchDomain, SearchType searchType, params (string key, string query)[] arguments)
        {
            switch (searchDomain)
            {
                case "publications":
                    Func<Publication, (string, string), bool> satisfies = (pub, arg) =>
                    {
                        switch (arg.Item1)
                        {
                            case "title":
                                return pub.title == arg.Item2;
                            default:
                                return false;
                        }
                    };
                    return FindItems<T>(data.publications as Dictionary<string,T>, searchType, arguments, satisfies as Func<T,(string,string),bool>);
                default:
                    throw new ArgumentException("Specified search domain does not exist");
            }
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
        #endregion

        #region Version Doormat
        public HashSet<Article> SearchArticle(Dictionary<string, Article> searchedItems, SearchType searchType, params (string key, string query)[] queryArguments)
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
                        addIt = CompareFirst(article.title, query, searchType);
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
                            addIt = Compare(article.title, query, searchType, addIt);
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

        bool Compare(string real, string query, SearchType searchWay, bool currentJudgement)
        {
            bool equal = real == query;
            if (searchWay == SearchType.Exact)
                return currentJudgement && equal;
            if (searchWay == SearchType.Loose)
                return currentJudgement || equal;
            throw new Exception("Not all search ways implemented");
        }

        bool CompareFirst(string real, string query, SearchType searchWay)
        {
            bool equal = real == query;
            if (searchWay == SearchType.Exact)
                return equal;
            if (searchWay == SearchType.Loose)
                return equal;
            throw new Exception("Not all search ways implemented");
        }
        #endregion
    }
}
