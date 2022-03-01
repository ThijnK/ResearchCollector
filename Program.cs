using System;
using System.IO;
using System.Xml;

namespace ResearchDashboard
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path to data set:");

            // Get input from user
            string? path;
            while ((path = Console.ReadLine()) == null || !File.Exists(path))
                Console.WriteLine("File not found, please try again:");

            Console.WriteLine("Enter username for database:");
            string? username;
            while ((username = Console.ReadLine()) == null)
                Console.WriteLine("Invalid username, please try again:");
            Console.WriteLine("Enter password for database:");
            string? password;
            while ((password = Console.ReadLine()) == null)
                Console.WriteLine("Invalid password, please try again:");

            DblpUpdater dblp = new DblpUpdater(GetMostRecent(), username, password);
            DateTime newDate = dblp.ParseXML(path);
            UpdateMostRecent(newDate);

            Console.ReadLine();
        }

        // Get the date of the most recent article added to the database from stored file
        static DateTime GetMostRecent()
        {
            try
            {
                StreamReader sr = new StreamReader("../../../date.txt");
                string dateString = sr.ReadToEnd();
                sr.Close();
                return DateTime.Parse(dateString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return DateTime.MinValue;
            }
        }

        // Update the date of most recent article added to database
        static void UpdateMostRecent(DateTime newDate)
        {
            try
            {
                FileStream fs = File.Create("../../../date.txt");
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(newDate.ToString());
                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}