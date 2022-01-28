// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var pathVar = Environment.GetEnvironmentVariable("Path");

if (pathVar == null)
{
    Environment.Exit(1);
}

var paths = pathVar.Split(";");

var goodPaths = new List<string>();

Console.WriteLine("Bad paths:");

foreach (var path in paths)
{
    if (Directory.Exists(path))
    {
        goodPaths.Add(path);
    }
    else
    {
        Console.WriteLine($"bad: {path}");
    }
}

Console.WriteLine();
Console.WriteLine($"Path={string.Join(";", goodPaths)}");
