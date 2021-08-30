using MunchenClient.Lua;
using System;

namespace L2C
{
    class Program
    {
        public static void Main(string[] args)
        {
            //LuaAPI
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            LuaClassWrapper classWrapper = LuaWrapper.RegisterClassCallback(typeof(Debug));
            LuaWrapper.RegisterFunctionCallback(classWrapper, nameof(Debug.OutputToConsole), "Log");
            LuaWrapper.RegisterFunctionCallback(classWrapper, nameof(Debug.OutputBoolean), "Boolean");

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
