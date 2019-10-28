using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CopyToWhiteSourceVm
{
    static class Program
    {
        private static string DESTPATH = @"\\US01Palscan2\CleanRoom\TotalAgility-7.7.0\ADR\source";
        private static string SRCPATH = @"D:\Dev\TD_Main\Products\";
        private static int SRCPATHLEN = SRCPATH.Length;

        static void Main(string[] args)
        {
            var folders = new List<string>
            {
                "Cascade", "Cascade.Extensions", "Configuration", "FoX", "Graphics", "Integration", "Interops", "Kofax.Cascade", "Locator", "Memphis"
            };


            Console.WriteLine("Hello World!");

            var includes = new List<string>();
            var excludes = new List<string>();

            if (File.Exists("includes.txt"))
            {
                var inc = File.ReadAllText("includes.txt");
                includes = inc.DeserializeObject<List<string>>();
            }

            if (File.Exists("excludes.txt"))
            {
                var excl = File.ReadAllText("excludes.txt");
                excludes = excl.DeserializeObject<List<string>>();
            }

            foreach (var folder in folders)
            {
                var folderPath = Path.Combine(SRCPATH, folder);

                foreach (var file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))
                {
                    Console.Write($"{file}");

                    var extension = Path.GetExtension(file).ToLowerInvariant();

                    var destFile = Path.Combine(DESTPATH, file.Substring(SRCPATHLEN));

                    var destFolder = Path.GetDirectoryName(destFile);

                    while (true)
                    {
                        if (includes.Contains(extension))
                        {
                            try
                            {
                                Directory.CreateDirectory(destFolder);

                                try
                                {
                                    File.Delete(destFile);
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }

                                File.Copy(file, destFile);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(" copied");
                                Console.ResetColor();
                            }
                            catch (Exception e)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(" Error!");
                                Console.ResetColor();

                                Console.WriteLine(e.Message);
                                Console.Write("Hit return to continue...");
                                Console.ReadLine();
                            }

                            break;
                        }

                        if (excludes.Contains(extension))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine(" skipped");
                            Console.ResetColor();

                            break;
                        }

                        //  Need to add extension here....
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("\nCopy files of this type? y or n  ");
                        Console.ResetColor();

                        var response = Console.ReadLine();

                        if (response.StartsWith('y'))
                        {
                            includes.Add(extension);
                        }
                        else if (response.StartsWith('n'))
                        {
                            excludes.Add(extension);
                        }
                    }
                }
            }

            File.WriteAllText("includes.txt", includes.SerializeObject());
            File.WriteAllText("excludes.txt", excludes.SerializeObject());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Done");
            Console.ResetColor();
            Console.Write(" - Hit return to continue...");
            Console.ReadLine();
        }

        public static string SerializeObject<T>(this T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T DeserializeObject<T>(this string fromSerialize)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(fromSerialize))
            {
                return (T) xmlSerializer.Deserialize(textReader);
            }
        }
    }
}
