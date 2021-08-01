﻿using MunchenClient.Lua;
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

        public static void Main(string[] args)
        {
            LuaWrapper.RegisterFunctionCallback("console.log", typeof(Program), nameof(OutputToConsole));
            LuaWrapper.RegisterFunctionCallback("console.debug", typeof(Program), nameof(OutputToConsole));
            LuaWrapper.RegisterFunctionCallback("console.debug", typeof(Program), nameof(OutputToConsole));

            LuaScript hack = LuaLoader.LoadLua("hack");

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            LuaInterpreter.ExecuteFunction(hack, "OnUpdate");

            stopwatch.Stop();

            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedTicks / 10000} ms");

            Console.ReadLine();
        }
    }
}
