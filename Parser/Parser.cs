using System;

namespace Parser
{
    abstract class Parser
    {
        // Checks if given file corresponds to the correct type of data set
        public abstract bool CheckFile(string path);
        // Parses a file and writes the result to the given output location
        public abstract bool ParseFile(string inputPath, string outputPath);
        public event EventHandler<ItemParsedEventArgs> ItemParsed;
    }

    public class ItemParsedEventArgs : EventArgs
    {
        public string title;

        public ItemParsedEventArgs(string title)
        {
            this.title = title;
        }
    }
}
