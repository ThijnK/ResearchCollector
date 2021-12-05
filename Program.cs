using System;
using System.Xml;

namespace ResearchDashboard
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Argument of ctor should be the date that the database was last updated, possibly to be stored in a file somewhere
            DblpUpdater dblp = new DblpUpdater(DateTime.MinValue);
            dblp.ParseXML(""); // Use local path
            
            Console.ReadLine();

            //BibliograhpyAPI api = new DblpAPI();
            //// As an example, this will fetch the XML of a specific author
            //api.FetchXML("https://dblp.org/pid/65/9612.xml");
        }
    }
}