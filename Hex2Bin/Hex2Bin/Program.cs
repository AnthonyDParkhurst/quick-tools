using System;
using System.IO;
using System.Linq;

namespace Hex2Bin
{
    class Program
    {
        static void Main(string[] args)
        {
            var hex = File.ReadAllText(args[0]);

            var bytes = Enumerable.Range(2, hex.Length - 2)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

            File.WriteAllBytes(args[1], bytes);
        }
    }
}
