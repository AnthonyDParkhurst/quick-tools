using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<int> Go()
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
                    done = await CopyFile(file, Path.Combine(_destPath, fileName));

                    if (!done)
                    {
                        sleeptime += 10;
                        sleeptime = Math.Min(sleeptime, 60);
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

            return 0;
        }

        private static async Task<bool> CopyFile(string file, string destFile)
        {
            try
            {
                Console.WriteLine();
                Log($"Copying {Path.GetFileName(file)}");

                using var outstream = File.OpenWrite(destFile);

                var position = 0L;

                while (true)
                {
                    using (var instream = File.OpenRead(file))
                    {
                        Log("File opened...");

                        var fileSize = instream.Length;

                        Log($"Input file size: {ShowBytes(fileSize)}");

                        if (position != 0)
                        {
                            Log($"Seeking to position: {ShowBytes(position)}");
                            instream.Position = position;
                        }

                        var bufsize = 1024 * 64;

                        var buf = new byte[bufsize];

                        var bytes = 1;

                        try
                        {
                            var downloadStopwatch = new Stopwatch();
                            var saveStopWatch = new Stopwatch();
                            var transferred = 0L;
                            var startTransfer = DateTime.Now;

                            while (bytes > 0)
                            {
                                downloadStopwatch.Start();
                                bytes = await instream.ReadAsync(buf, 0, bufsize);
                                downloadStopwatch.Stop();

                                if (bytes > 0)
                                {
                                    var elapsed = DateTime.Now - startTransfer;
                                    transferred += bytes;

                                    saveStopWatch.Start();
                                    await outstream.WriteAsync(buf, 0, bytes);
                                    saveStopWatch.Stop();

                                    position += bytes;

                                    Console.Write($"\r({Time()}) Copied: {ShowBytes(position)}  [Rate: {ShowBytes(transferred / elapsed.TotalSeconds)}/s]     "); // can't use Log() here...
                                };
                            }

                            Console.WriteLine();
                            Console.WriteLine($"Total download time: {downloadStopwatch.Elapsed}; total write time: {saveStopWatch.Elapsed}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine();
                            Console.WriteLine();
                            Log(e.Message);
                            Console.WriteLine();
                            Console.WriteLine("Retry in 30 seconds...");
                            Thread.Sleep(TimeSpan.FromSeconds(30));
                            continue;
                        }

                        return true;
                    }
                }
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

            while (size >= 1000.0)
            {
                size /= 1000.0;
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

            while (!string.IsNullOrEmpty(directoryName))
            {
                if (Path.GetFileName(directoryName).Any(char.IsDigit))
                {
                    break;
                }

                directoryName = Path.GetDirectoryName(directoryName);
            }

            while (!string.IsNullOrEmpty(directoryName))
            {
                if (files.All(f => f.StartsWith(directoryName)))
                    return directoryName;

                directoryName = Path.GetDirectoryName(directoryName);
            }

            directoryName = Path.GetDirectoryName(files[0]);

            while (!string.IsNullOrEmpty(directoryName))
            {
                if (files.All(f => f.StartsWith(directoryName)))
                    return directoryName;

                directoryName = Path.GetDirectoryName(directoryName);
            }

            return @"_MISC_FILES_";
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
