using System.Collections.Generic;
using System.Collections;
using System;

namespace MunchenClient.Lua
{
    internal enum ComparatorType : short
    {
        ComparatorType_Unknown = 0,
        ComparatorType_EqualTo = 1,
        ComparatorType_NotEqualTo = 2,
        ComparatorType_LessThan = 3,
        ComparatorType_MoreThan = 4,
        ComparatorType_MoreOrEqualThan = 5,
        ComparatorType_LessOrEqualThan = 6,
    }

    internal struct Comparator
    {
        internal int comparatorIndex;
        internal ComparatorType comparatorType;
    }

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
            //TODO: execute instructions in order

            for(int i = 0; i < function.functionExecutionList.Count; i++)
            {
                function.functionExecutionList[i].ExecuteInstruction();
            }

            return true;
        }
    }
}
