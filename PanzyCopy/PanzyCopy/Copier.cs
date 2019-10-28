using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace PanzyCopy
{
    public class Copier
    {
        private readonly List<string> _specificallyDesired = new List<string>();

        private readonly List<string> _desiredFileRegexList = new List<string>();

        private readonly List<string> _undesiredFileRegexList = new List<string>();

        private string _destPath = @"D:\Cached";

        private readonly List<string> _allFiles = new List<string>();

        public void Initialize(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("--"))
                {
                    switch (arg)
                    {
                        case "--dest":

                            i++;

                            var dest = i < args.Length ? args[i] : null;

                            _destPath = dest ?? throw new Exception("Error: --dest option missing parameter!");

                            continue;

                        case "--get":

                            i++;

                            var get = i < args.Length ? args[i] : null;

                            if (get == null)
                            {
                                throw new Exception("Error: --dest option missing parameter!");
                            }

                            _desiredFileRegexList.Add(WildcardToRegex(get));

                            continue;

                        case "--skip":

                            i++;

                            var skip = i < args.Length ? args[i] : null;

                            if (skip == null)
                            {
                                throw new Exception("Error: --dest option missing parameter!");
                            }

                            _undesiredFileRegexList.Add(WildcardToRegex(skip));

                            continue;

                        default:
                            throw new Exception($"Error: Unknown option {arg}");
                    }
                }

                Log($"Checking if {arg} is a directory or a file...");

                if (Directory.Exists(arg))
                {
                    Log($"Fetching a recursive directory listing of {arg}");
                    var files = Directory.GetFiles(arg, "*.*", SearchOption.AllDirectories);
                    _allFiles.AddRange(files);
                }
                else if (File.Exists(arg))
                {
                    _allFiles.Add(arg);
                    _specificallyDesired.Add(arg);
                }
                else
                {
                    throw new Exception($"Error: Cannot find directory or file called {arg}");
                }
            }

            if (!_allFiles.Any())
            {
                throw new Exception("Error: No files found to copy.");
            }
        }

        public void Go()
        {
            Console.WriteLine($"Destination folder: {_destPath}");

            var startTime = DateTime.Now;

            var commonPath = GetCommonPath(_allFiles);

            Console.WriteLine($"Common directory of input files: {commonPath}");

            var directoryName = Path.GetFileName(commonPath);

            _destPath = Path.Combine(_destPath, directoryName);

            Directory.CreateDirectory(_destPath);

            foreach (var file in _allFiles)
            {
                var fileName = Path.GetFileName(file);

                if (!Desired(file, fileName))
                {
                    Log($"Skipping {fileName}");
                    continue;
                }

                var done = false;

                var sleeptime = 0;

                while (!done)
                {
                    done = CopyFile(file, Path.Combine(_destPath, fileName));

                    if (!done)
                    {
                        sleeptime += 60;
                        sleeptime = Math.Min(sleeptime, 600);
                        Log($"Sleeping {sleeptime} seconds...");
                        Thread.Sleep(TimeSpan.FromSeconds(sleeptime));
                    }
                    else
                    {
                        Console.WriteLine();
                        Log($"Done copying {fileName}\n");
                    }
                }
            }

            var endTime = DateTime.Now;

            Console.WriteLine();
            Log($"Success (Duration: {endTime - startTime})");

        }

        private static bool CopyFile(string file, string destFile)
        {
            try
            {
                Console.WriteLine();
                Log($"Copying {Path.GetFileName(file)}");

                using (var instream = File.OpenRead(file))
                using (var outstream = File.OpenWrite(destFile))
                {
                    Log("File opened, trying to read...");

                    var fileSize = instream.Length;

                    Log($"Input file size: {ShowBytes(fileSize)}");

                    var bufsize = 1024 * 64;

                    var buf = new byte[bufsize];

                    var bytes = 1;

                    var total = 0L;

                    while (bytes > 0)
                    {
                        bytes = instream.Read(buf, 0, bufsize);

                        if (bytes > 0)
                        {
                            outstream.Write(buf, 0, bytes);

                            total += bytes;

                            Console.Write($"\r({Time()}) Copied: {ShowBytes(total)}     "); // can't use Log() here...
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Log(e.Message);
                Console.WriteLine();
                return false;
            }
        }

        private static readonly List<string> UnitStrings = new List<string>
        {
            "bytes",
            "kb",
            "MB",
            "GB",
            "TB",
        };

        private static string ShowBytes(double size)
        {
            var units = 0;

            while (size >= 1000)
            {
                size /= 1000;
                units += 1;
            }

            return size.ToString("F") + " " + UnitStrings[units];
        }

        private bool Desired(string filePath, string fileName)
        {
            if (_specificallyDesired.Contains(filePath, StringComparer.OrdinalIgnoreCase))
                return true;

            if (_desiredFileRegexList.Any(d => Regex.IsMatch(fileName, d, RegexOptions.IgnoreCase)))
            {
                if (!_undesiredFileRegexList.Any(d => Regex.IsMatch(fileName, d, RegexOptions.IgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetCommonPath(IReadOnlyList<string> files)
        {
            var directoryName = Path.GetDirectoryName(files[0]);

            while (!String.IsNullOrEmpty(directoryName))
            {
                if (Path.GetFileName(directoryName).Any(char.IsDigit))
                {
                    break;
                }

                directoryName = Path.GetDirectoryName(directoryName);
            }

            while (!String.IsNullOrEmpty(directoryName))
            {
                if (files.All(f => f.StartsWith(directoryName)))
                    return directoryName;

                directoryName = Path.GetDirectoryName(directoryName);
            }

            throw new Exception("Can't find common folder from list of files...");
        }

        private static string Time()
        {
            return DateTime.Now.ToString("h:mm:ss.fff tt");
        }

        private static void Log(string message)
        {
            Console.WriteLine($"({Time()}) {message}");
        }

        private static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                           .Replace(@"\*", ".*")
                           .Replace(@"\?", ".")
                       + "$";
        }
    }
}
