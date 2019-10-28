using System;
using System.IO;

namespace QuickMd5
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine($"Missing filename?");

                Environment.ExitCode = 1;

                return;
            }

            var sha256 = false;

            foreach (var file in args)
            {
                if (file == "-sha256")
                {
                    sha256 = true;
                    continue;
                }

                if (Path.GetExtension(file).ToUpper() == ".MD5")
                {
                    var lines = File.ReadAllLines(file);

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        var fields = line.Split(' ', 2);

                        if (fields.Length < 2)
                        {
                            Console.Error.WriteLine($"Bad MD5 line: {line}");
                            continue;
                        }

                        Console.Write($"{fields[1]}... ");

                        var actualFile = fields[1].Replace("*", Path.GetDirectoryName(file) + "\\");

                        var newhash = Hash.GetMd5Hash(actualFile);

                        if (fields[0] == newhash)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("*MATCH*");
                            Console.ResetColor();
                            Console.Write(" got");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("*CORRUPT*");
                            Console.ResetColor();
                            Console.Write(" expected");
                        }

                        Console.WriteLine($" {fields[0]}");
                    }
                }
                else if (sha256)
                {
                    var result = Hash.GetSha256Hash(file);

                    Console.WriteLine($"{result} *{Path.GetFileName(file)}");
                }
                else
                {
                    var result = Hash.GetMd5Hash(file);

                    Console.WriteLine($"{result} *{Path.GetFileName(file)}");
                }
            }
        }
    }
}
