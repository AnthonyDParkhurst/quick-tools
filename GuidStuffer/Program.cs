namespace GuidStuffer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);

            var outLines = new List<string>();

            foreach (var line in lines)
            {
                var newLine = line.Replace(@"Guid=""""", $@"Guid=""{Guid.NewGuid()}""");

                outLines.Add(newLine);
            }

            File.WriteAllLines(args[0], outLines);
        }
    }
}