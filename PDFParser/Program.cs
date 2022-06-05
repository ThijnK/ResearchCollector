using System;

namespace PDFParser
{
    class Program
    {
        static void Main(string[] args)
        {
            PDFInfoFinder infoFinder = new PDFInfoFinder();
            //infoFinder.FindInfo(Console.ReadLine(), int.Parse(Console.ReadLine()), false);
            infoFinder.FindInfo(args[0], int.Parse(args[1]), bool.Parse(args[2]));
        }
    }
}
