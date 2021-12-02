using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Clippy
{
    class Program
    {
        private static readonly string _folder = @"D:\Work\Large Training Sets\Test";

        [STAThread]
        static void Main(string[] args)
        {
            var dataObject = Clipboard.GetDataObject();

            if (dataObject == null)
            {
                Console.WriteLine("Clipboard is empty.");
                Environment.Exit(0);
            }

            var formats = dataObject.GetFormats(false);

            foreach (var format in formats)
            {
                Console.Write($"{format}: ");

                object data;

                try
                {
                    data = dataObject.GetData(format);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    continue;
                }

                if (data == null)
                {
                    Console.WriteLine("<null> returned.");
                    continue;
                }

                Console.WriteLine($"{data.GetType()}");

                ////if (data is MemoryStream ms)
                ////{
                ////    File.WriteAllBytes(Path.Combine(_folder, Guid.NewGuid().ToString("N") + "." + format.Replace(" ", "").Replace("/", "_").Replace("\\", "_")), ms.ToArray());
                ////}

                if (data is System.Windows.Interop.InteropBitmap swib)
                {
                    Console.WriteLine($" - DpiX: {swib.DpiX}");
                    Console.WriteLine($" - DpiY: {swib.DpiY}");

                    Console.WriteLine($" - PixelHeight: {swib.PixelHeight}");
                    Console.WriteLine($" - PixelWidth: {swib.PixelWidth}");
                    Console.WriteLine($" - Height: {swib.Height}");
                    Console.WriteLine($" - Width: {swib.Width}");

                    Console.WriteLine($" - Format: {swib.Format}");

                    Console.WriteLine($" - Number of colors in palette: {swib.Palette?.Colors.Count}");
                }
            }

            Environment.Exit(0);
        }
    }
}
