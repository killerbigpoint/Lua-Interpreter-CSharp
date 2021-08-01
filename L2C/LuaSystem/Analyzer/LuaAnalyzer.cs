using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Analyzer
{
    internal class LuaAnalyzer
    {
        internal static void AnalyzeScript(LuaScript script)
        {
            //Part 1 - Analyze Functions
            for (int i = 0; i < script.scriptCode.Length; i++)
            {
                LuaAnalyzerFunction.CheckForFunctions(script, i);
            }

            //Part 2 - Analyze Instructions
            for (int i = 0; i < script.scriptFunctions.Count(); i++)
            {
                LuaAnalyzerInstruction.CheckForInstructions(script.scriptFunctions.ElementAt(i).Value);
            }
        }
    }
}
