using System;
using System.Xml;

namespace ResearchDashboard
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path to data set:");

            // Get input from user
            string path = Console.ReadLine();
            while (!File.Exists(path))
            {
                Console.WriteLine("File not found, please try again:");
                path = Console.ReadLine();
            }

            Console.WriteLine("Enter username for database:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password for database:");
            string password = Console.ReadLine();

            // Argument of ctor should be the date that the database was last updated, possibly to be stored in a file somewhere
            DblpUpdater dblp = new DblpUpdater(DateTime.MinValue, username, password);
            dblp.ParseXML(path);
            
            Console.ReadLine();

            //BibliograhpyAPI api = new DblpAPI();
            //// As an example, this will fetch the XML of a specific author
            //api.FetchXML("https://dblp.org/pid/65/9612.xml");
        }
    }
}