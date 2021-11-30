using System;

namespace ResearchDashboard
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            BibliograhpyAPI api = new DblpAPI();
            // As an example, this will fetch the XML of a specific author
            api.FetchXML("https://dblp.org/pid/65/9612.xml");
        }
    }
}