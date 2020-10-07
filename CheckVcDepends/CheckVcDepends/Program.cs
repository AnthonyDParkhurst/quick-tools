using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace CheckVcDepends
{
    class Program
    {
        private const string DumpBin = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\VC\Tools\MSVC\14.26.28801\bin\Hostx86\x86\dumpbin.exe";

        private const string IgnoreKey = "-ignore=";
        private const string OutKey = "-out=";
        private const string LogKey = "-log=";

        private static readonly List<string> Ignores = new List<string>();

        private static TextWriter Output = Console.Out;
        private static TextWriter Error = Console.Error;


        public static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (arg.StartsWith(IgnoreKey))
                    {
                        Ignores.Add(arg.Substring(IgnoreKey.Length));
                        continue;
                    }

                    if (arg.StartsWith(OutKey))
                    {
                        Output = new StreamWriter(arg.Substring(OutKey.Length));
                        continue;
                    }

                    if (arg.StartsWith(LogKey))
                    {
                        Error = new StreamWriter(arg.Substring(LogKey.Length));
                        continue;
                    }

                    Error.WriteLine($"Unknown parameter {arg}");
                    continue;
                }

                if (File.Exists(arg))
                {
                    SearchFile(arg);
                }
                else if (Directory.Exists(arg))
                {
                    SearchFolder(arg);
                }
                else
                {
                    Error.WriteLine($"Parameter not file or directory: {arg}");
                }
            }

            Output.Dispose();
            Error.Dispose();

            Environment.Exit(0);
        }

        private static void SearchFolder(string folder)
        {
            foreach (var file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) || file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    SearchFile(file);
                }
            }
        }

        private static void SearchFile(string filePath)
        {
            try
            {
                Error.WriteLine($"Checking {filePath}");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = DumpBin,
                        Arguments = $"/dependents /nologo \"{filePath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    }
                };

                process.Start();

                var outlines = new List<string>();

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();

                    if (line.StartsWith("Dump of"))
                    {
                        continue;
                    }

                    if (line.Contains(".dll"))
                    {
                        outlines.Add(line.Trim());
                    }
                }

                var vcRuntime = outlines.FirstOrDefault(l => l.StartsWith("msvc", true, CultureInfo.InvariantCulture));

                if (vcRuntime != null && !Ignores.Contains(vcRuntime, StringComparer.OrdinalIgnoreCase))
                {
                    Output.WriteLine($"{vcRuntime} used by {filePath}");
                }

                process.WaitForExit();
            }
            catch (Exception e)
            {
                Error.WriteLine($"{filePath}: ");

                Error.WriteLine(e);
                // ignore
            }
        }
    }
}
