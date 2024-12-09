namespace SplitFile
{
    internal class Program
    {
        private const int MaxLines = 500000;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: SplitFile <filename");
                Environment.Exit(0);
            }

            foreach (var filename in args)
            {
                Split(filename);
            }
        }

        private static void Split(string filename)
        {
            var dirname = Path.GetDirectoryName(filename);
            var baseName = Path.GetFileNameWithoutExtension(filename);
            var extension = Path.GetExtension(filename);

            var index = 0;

            using var input = File.OpenText(filename);

            while (input.EndOfStream == false)
            {
                var outputFilename = Path.Combine(dirname, baseName + $"_{index}" + extension);

                Console.WriteLine($"{outputFilename}...");

                index++;

                var lines = new List<string>(MaxLines);

                for (var lineNumber = 0; lineNumber < MaxLines; lineNumber++)
                {
                    if (input.EndOfStream)
                    {
                        break;
                    }

                    lines.Add(input.ReadLine());
                }

                File.WriteAllLines(outputFilename, lines);

                lines.Clear();
            }
        }
    }
}
