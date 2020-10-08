using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CultureCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"CurrentUICulture: {System.Threading.Thread.CurrentThread.CurrentUICulture.Name}");
            Console.WriteLine($"CurrentCulture: {System.Threading.Thread.CurrentThread.CurrentCulture.Name}");
            Console.Write("Hit any key...");
            Console.ReadKey();
        }
    }
}
