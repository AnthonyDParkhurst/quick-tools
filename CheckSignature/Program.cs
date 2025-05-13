using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CheckSignature
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: CheckSignature <file|directory>");
                return;
            }

            try
            {
                var attr = File.GetAttributes(args[0]);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    ScanDirectory(args[0]);
                    Environment.Exit(0);
                }
                else
                {
                    var result = IsTrusted(args[0]);
                    Environment.Exit(result ? 0 : 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(-1);
            }
        }

        private static void ScanDirectory(string directory)
        {
            foreach (var file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(file).ToUpperInvariant();

                if (ext != ".EXE" && ext != ".DLL" && ext != ".MSI" && ext != ".CAB" && ext != ".MSP")
                {
                    continue;
                }

                var isTrusted = IsTrusted(file);

                if (!isTrusted)
                {
                    Console.WriteLine($"File: {file}");
                }
            }
        }

        private static bool IsTrusted(string file)
        {
            try
            {
                return AuthenticodeTools.IsTrusted(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
                return false;
            }
        }
    }
}
