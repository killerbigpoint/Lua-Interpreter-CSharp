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

        internal static int FindCodeSectionEnd(string script, int startIndex)
        {
            int functionCurrentIndex = startIndex;
            int bracketDifference = 1;

            while (bracketDifference >= 1)
            {
                if (script[functionCurrentIndex] == '{' && functionCurrentIndex != startIndex)
                {
                    bracketDifference++;
                }
                else if (script[functionCurrentIndex] == '}')
                {
                    bracketDifference--;
                }

                functionCurrentIndex++;

                if (functionCurrentIndex > (script.Length - 1))
                {
                    return -1;
                }
            }

            return functionCurrentIndex - 1;
        }
    }
}
