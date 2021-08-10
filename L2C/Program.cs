using MunchenClient.Lua;
using System;

namespace L2C
{
    class Program
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

        public static void Main(string[] args)
        {
            //LuaAPI
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            LuaWrapper.RegisterFunctionCallback("console.log", typeof(Program), nameof(OutputToConsole));
            LuaWrapper.RegisterFunctionCallback("console.boolean", typeof(Program), nameof(OutputBoolean));

            stopwatch.Stop();

            //LuaLoad
            System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
            stopwatch2.Start();

            LuaScript hack = LuaLoader.LoadLua("hack");

            stopwatch2.Stop();

            //LuaExecute
            System.Diagnostics.Stopwatch stopwatch3 = new System.Diagnostics.Stopwatch();
            stopwatch3.Start();

            LuaInterpreter.ExecuteFunction(hack, "OnUpdate");

            stopwatch3.Stop();

            //Output profiler stats
            Console.WriteLine($"API Load Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Lua Load Time: {stopwatch2.ElapsedMilliseconds} ms");
            Console.WriteLine($"Lua Execute Time: {stopwatch3.ElapsedMilliseconds} ms");

            //Wait for input
            Console.ReadLine();
        }
    }
}
