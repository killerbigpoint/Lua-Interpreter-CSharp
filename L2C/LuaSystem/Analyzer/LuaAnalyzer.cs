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
                FunctionAnalyzeReport report = LuaAnalyzerFunction.CheckForFunctions(script, i);

                if(report.found == true)
                {
                    i += report.skipAhead;
                }
                
                LuaAnalyzerVariable.CheckForVariables(script, i);
            }

            //Part 2 - Analyze Instructions
            for (int i = 0; i < script.executionFunctions.Count(); i++)
            {
                LuaAnalyzerInstruction.CheckForInstructions(script.executionFunctions.ElementAt(i).Value);
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

                if (functionCurrentIndex > script.Length)
                {
                    return -1;
                }
            }

            return functionCurrentIndex - 1;
        }

        internal static object DetermineParameterType(string parameter)
        {
            if (parameter[0] == '"' && parameter[parameter.Length - 1] == '"')
            {
                return parameter;
            }

            string nonStringParameter = parameter.Substring(0, parameter.Length - (parameter.EndsWith("f") ? 1 : 0));

            if (int.TryParse(nonStringParameter, out int intParameter) == true)
            {
                return intParameter;
            }

            if (float.TryParse(nonStringParameter, out float floatParameter) == true)
            {
                return floatParameter;
            }

            if (bool.TryParse(nonStringParameter, out bool boolParameter) == true)
            {
                return boolParameter;
            }

            //TODO: Potentially add class types from the client here

            return parameter;
        }
    }
}
