using System;
using System.Threading;

namespace PickColor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            while (true)
            {
                var color = Win32.GetCurrentColor();

                Console.WriteLine($"#{color.R:x2}{color.G:x2}{color.B:x2}");

                Thread.Sleep(1000);
            }
        }
    }
}
