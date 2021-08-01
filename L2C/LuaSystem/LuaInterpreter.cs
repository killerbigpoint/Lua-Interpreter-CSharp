using System.Collections.Generic;
using System.Collections;
using System;

namespace MunchenClient.Lua
{
    internal class LuaInterpreter
    {
        internal static bool ExecuteFunction(LuaScript script, string functionName)
        {
            if(script == null)
            {
                Console.WriteLine("Failed to execute function due to script being null");

                return false;
            }

            string functionNameFixed = $"function {functionName}";

            if (script.scriptFunctions.ContainsKey(functionNameFixed) == false)
            {
                Console.WriteLine("Failed to execute function due to script not containing it");

                return false;
            }

            return ExecuteFunction(script.scriptFunctions[functionNameFixed]);
        }

        internal static bool ExecuteFunction(LuaFunction function)
        {
            function.ExecuteFunction();

            return true;
        }
    }
}
