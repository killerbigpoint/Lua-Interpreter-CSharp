using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Analyzer
{
    internal class LuaAnalyzerFunction
    {
        private const string functionPrefix = "function";

        internal static bool CheckForFunctions(LuaScript script, int index)
        {
            //Make sure we got enough space for a potential function and won't hit the end of the script
            if ((script.scriptCode.Length - index) < functionPrefix.Length)
            {
                return false;
            }

            //Look ahead from the start position and see if we can find the 'function' keyword
            int functionIndex = script.scriptCode.IndexOf(functionPrefix, index, functionPrefix.Length);

            if (functionIndex == -1)
            {
                return false;
            }

            //Check if our function has an end
            int functionEndIndex = script.scriptCode.IndexOf(')', functionIndex);

            if (functionEndIndex == -1)
            {
                return false;
            }

            functionEndIndex++;

            //Make sure our function is valid by checking for brackets
            int bracketIndexStart = script.scriptCode.IndexOf('{', functionEndIndex);

            if (bracketIndexStart == -1)
            {
                return false;
            }

            int functionCurrentIndex = bracketIndexStart;
            int bracketDifference = 1;

            while (bracketDifference >= 1)
            {
                if (script.scriptCode[functionCurrentIndex] == '{' && functionCurrentIndex != bracketIndexStart)
                {
                    bracketDifference++;
                }
                else if (script.scriptCode[functionCurrentIndex] == '}')
                {
                    bracketDifference--;
                }

                functionCurrentIndex++;

                if (functionCurrentIndex > (script.scriptCode.Length - 1))
                {
                    return false;
                }
            }

            int bracketIndexEnd = functionCurrentIndex;

            //Finish off by adding the function to the collection
            string functionCode = script.scriptCode.Substring(bracketIndexStart + 1, bracketIndexEnd - bracketIndexStart - 2).Trim();

            if (bracketIndexEnd == -1 || bracketIndexStart > bracketIndexEnd)
            {
                return false;
            }

            //Make sure this function is actually valid by checking if everything actually belongs to it
            if (IsFunctionValid(script.scriptCode, functionEndIndex, bracketIndexStart, bracketIndexEnd) == false)
            {
                return false;
            }

            string functionName = script.scriptCode.Substring(functionIndex, functionEndIndex - functionIndex - 2);

            //Make sure we ain't finding duplicated functions
            if (script.scriptFunctions.ContainsKey(functionName) == true)
            {
                return false;
            }

            script.scriptFunctions.Add(functionName, new LuaFunction
            {
                functionName = functionName,
                functionCode = functionCode
            });

            return true;
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
