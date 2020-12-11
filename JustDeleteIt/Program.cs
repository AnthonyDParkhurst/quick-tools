using System;
using System.IO;

namespace JustDeleteIt
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg))
                    {
                        Console.WriteLine($"Deleting file: {Path.GetFileName(arg)}");
                        File.Delete(arg);
                        continue;
                    }

                    Console.WriteLine($"Deleting directory tree: {Path.GetFileName(arg)}");
                    Directory.Delete(arg, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nException: {ex.Message}");
            }

            Console.Write("\rEnter key to quit...");
            Console.ReadKey();
        }
    }
}
