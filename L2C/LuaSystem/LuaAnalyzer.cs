using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua
{
    internal class LuaAnalyzer
    {
        private const string functionPrefix = "function";

        internal static Dictionary<string, LuaFunction> AnalyzeScript(string script)
        {
            Dictionary<string, LuaFunction> functions = new Dictionary<string, LuaFunction>();

            for (int i = 0; i < script.Length; i++)
            {
                CheckForFunction(script, i, ref functions);
            }

            return functions;
        }

        internal static bool CheckForFunction(string script, int index, ref Dictionary<string, LuaFunction> list)
        {
            //Make sure we got enough space for a potential function and won't hit the end of the script
            if ((script.Length - index) < functionPrefix.Length)
            {
                return false;
            }

            //Look ahead from the start position and see if we can find the 'function' keyword
            int functionIndex = script.IndexOf(functionPrefix, index, functionPrefix.Length);

            if (functionIndex == -1)
            {
                return false;
            }

            //Check if our function has an end
            int functionEndIndex = script.IndexOf(')', functionIndex);

            if (functionEndIndex == -1)
            {
                return false;
            }

            functionEndIndex++;

            //Make sure our function is valid by checking for brackets
            int bracketIndexStart = script.IndexOf('{', functionEndIndex);

            if(bracketIndexStart == -1)
            {
                return false;
            }

            int functionCurrentIndex = bracketIndexStart;
            int bracketDifference = 1;

            while (bracketDifference >= 1)
            {
                if(script[functionCurrentIndex] == '{' && functionCurrentIndex != bracketIndexStart)
                {
                    bracketDifference++;
                }
                else if(script[functionCurrentIndex] == '}')
                {
                    bracketDifference--;
                }

                functionCurrentIndex++;

                if (functionCurrentIndex > (script.Length - 1))
                {
                    return false;
                }
            }

            int bracketIndexEnd = functionCurrentIndex;

            //Finish off by adding the function to the collection
            string functionCode = script.Substring(bracketIndexStart + 1, bracketIndexEnd - bracketIndexStart - 2).Trim();

            Console.WriteLine($"Done function analyzing: {functionCode}");

            if (bracketIndexEnd == -1 || bracketIndexStart > bracketIndexEnd)
            {
                return false;
            }

            //Make sure this function is actually valid by checking if everything actually belongs to it
            if (IsFunctionValid(script, functionEndIndex, bracketIndexStart, bracketIndexEnd) == false)
            {
                return false;
            }

            string functionName = script.Substring(functionIndex, functionEndIndex - functionIndex - 2);

            //Make sure we ain't finding duplicated functions
            if (list.ContainsKey(functionName) == true)
            {
                Console.WriteLine($"Function: {functionName} duplicate entry found");

                return false;
            }

            Console.WriteLine($"Function: {functionName} Code: {functionCode}");

            list.Add(functionName, new LuaFunction
            {
                functionName = functionName,
                functionCode = functionCode
            });

            Console.WriteLine($"Function: {functionName} found");

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
