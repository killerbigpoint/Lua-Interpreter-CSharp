using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2C
{
    internal class Debug
    {
        public static void OutputToConsole(string text)
        {
            Console.WriteLine($"OutputToConsole: {text}");
        }

        public static void OutputToConsole(string text, string text2)
        {
            Console.WriteLine($"OutputToConsole 2: {text} + {text2}");
        }

        public static void OutputBoolean(bool boolean)
        {
            Console.WriteLine($"OutputBoolean: {boolean}");
        }
    }
}
