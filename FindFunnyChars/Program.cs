// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var alltext = File.ReadAllText(args[0]);

var index = 0;

foreach (var c in alltext)
{
    if (c > 160)
    {
        Console.WriteLine($"Character at position: {index} is {(int)c}, or '{c}'");
    }

    index++;
}