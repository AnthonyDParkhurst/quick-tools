using System;
using System.Threading.Tasks;

namespace PanzyCopy
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length < 1) Usage();

            try
            {
                var copier = new Copier();

                copier.Initialize(args);

                await copier.Go();

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            return 0;
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: PanzyCopy [--dest <folder>] [--get <filename>] [--skip <filename>] <folder on Panzura>");
            Environment.Exit(0);
        }
    }
}