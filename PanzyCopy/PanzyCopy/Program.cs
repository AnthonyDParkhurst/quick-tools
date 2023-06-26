using System;

namespace PanzyCopy
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1) Usage();

            try
            {
                var copier = new Copier();

                copier.Initialize(args);

                copier.Go();

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: PanzyCopy [--dest <folder>] [--get <filename>] [--skip <filename>] <folder on Panzura>");
            Environment.Exit(0);
        }
    }
}