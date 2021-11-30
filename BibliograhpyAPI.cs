using System.Net;
using System.Xml;

namespace ResearchDashboard
{
    abstract class BibliograhpyAPI
    {
        public HttpClient client;

        public virtual void SetClient(string address)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(address);
        }

        // Get the content of the given url relative to baseAdress of the current client
        public async Task<string> FetchURL(string url)
        {
            string result = "";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
                result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public abstract XmlDocument FetchXML(string url);
    }

    class DblpAPI : BibliograhpyAPI
    {
        public override XmlDocument FetchXML(string url)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(url);
            return doc;
        }
    }
}
