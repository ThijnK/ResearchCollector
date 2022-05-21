using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer
{
    class API
    {
        private Data data;

        // Have to make sure returned data cannot be changed 

        public Author GetAuthorByName(string name)
        {
            return data.authors[name];
        }

        public Author GetAuthorById(string id)
        {
            throw new NotImplementedException();
        }

        public List<Publication> GetPublicationsByAuthor(Author author)
        {
            throw new NotImplementedException();
        }

        public List<Publication> GetPublicationsByAuthor(string name)
        {
            Author author = GetAuthorByName(name);
            return GetPublicationsByAuthor(author);
        }
    }
}
