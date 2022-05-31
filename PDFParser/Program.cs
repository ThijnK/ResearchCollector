using System;

namespace PDFParser
{
    class Program
    {
        static void Main(string[] args)
        {
            PDFInfoFinder infoFinder = new PDFInfoFinder();
            infoFinder.FindInfo(Console.ReadLine());
        }
    }
}
