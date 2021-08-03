using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

namespace MunchenClient.Lua.Analyzer
{
    internal struct FunctionAnalyzeReport
    {
        internal bool found;
        internal int skipAhead;
    }

    internal class LuaAnalyzerFunction
    {
        private const string functionPrefix = "function";

        internal static FunctionAnalyzeReport CheckForFunctions(LuaScript script, int index)
        {
            FunctionAnalyzeReport report = new FunctionAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            //Make sure we got enough space for a potential function and won't hit the end of the script
            if ((script.scriptCode.Length - index) < functionPrefix.Length)
            {
                return report;
            }

            //Look ahead from the start position and see if we can find the 'function' keyword
            int functionIndex = script.scriptCode.IndexOf(functionPrefix, index, functionPrefix.Length);

            if (functionIndex == -1)
            {
                return report;
            }

            //Check if our function has an end
            int functionEndIndex = script.scriptCode.IndexOf(')', functionIndex);

            if (functionEndIndex == -1)
            {
                return report;
            }

            functionEndIndex++;

            //Make sure our function is valid by checking for brackets
            int bracketIndexStart = script.scriptCode.IndexOf('{', functionEndIndex);

            if (bracketIndexStart == -1)
            {
                return report;
            }

            int bracketIndexEnd = LuaAnalyzer.FindCodeSectionEnd(script.scriptCode, bracketIndexStart);

            if (bracketIndexEnd == -1)
            {
                return report;
            }

            //Finish off by adding the function to the collection
            string functionCode = script.scriptCode.Substring(bracketIndexStart + 1, bracketIndexEnd - bracketIndexStart - 2).Trim();

            if (bracketIndexEnd == -1 || bracketIndexStart > bracketIndexEnd)
            {
                return report;
            }

            //Make sure this function is actually valid by checking if everything actually belongs to it
            if (IsFunctionValid(script.scriptCode, functionEndIndex, bracketIndexStart, bracketIndexEnd) == false)
            {
                return report;
            }

            string functionName = script.scriptCode.Substring(functionIndex, functionEndIndex - functionIndex - 2);

            //Make sure we ain't finding duplicated functions
            if (script.scriptFunctions.ContainsKey(functionName) == true)
            {
                return report;
            }

            script.scriptFunctions.Add(functionName, new LuaFunction
            {
                functionParent = script,
                functionName = functionName,
                functionCode = functionCode
            });

            report.found = true;
            report.skipAhead = bracketIndexEnd - index;

            return report;
        }

        internal static bool IsFunctionValid(string script, int functionEndIndex, int bracketIndexStart, int bracketIndexEnd)
        {
            if ((functionEndIndex + 8) > script.Length)
            {
                return true;
            }

            int nextFunctionIndex = script.IndexOf("function", functionEndIndex);

            if (nextFunctionIndex == -1)
            {
                return true;
            }

            if (bracketIndexStart > nextFunctionIndex || bracketIndexEnd > nextFunctionIndex)
            {
                return false;
            }

            return true;
        }
    }
}
