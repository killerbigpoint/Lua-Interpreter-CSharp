using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Analyzer
{
    internal class LuaAnalyzerVariable
    {
        private const int longestVariableType = 6;

        private static readonly Dictionary<string, Type> variableTypes = new Dictionary<string, Type>
        {
            { "byte", typeof(byte) },
            { "int", typeof(int) },
            { "float", typeof(float) },
            { "long", typeof(long) },
            { "double", typeof(double) },
            { "bool", typeof(bool) },
            { "string", typeof(string) },
            { "char", typeof(char) }
        };

        internal static bool CheckForVariables(LuaScript script, int index)
        {
            //Make sure we got enough space for a potential variable and won't hit the end of the script
            if ((script.scriptCode.Length - index) < longestVariableType)
            {
                return false;
            }

            string scriptCodeFixed = script.scriptCode.Substring(index);

            for (int i = 0; i < variableTypes.Count; i++)
            {
                if (scriptCodeFixed.StartsWith(variableTypes.ElementAt(i).Key) == true)
                {
                    DetermineVariable(script, scriptCodeFixed, variableTypes.ElementAt(i).Key.Length);

                    break;
                }
            }

            return false;
        }

        private static bool DetermineVariable(LuaScript script, string scriptCode, int typeLength)
        {
            int variableEnd = scriptCode.IndexOf(';');

            if (variableEnd == -1)
            {
                return false;
            }

            string variableCode = scriptCode.Substring(typeLength, variableEnd);

            int setterIndex = variableCode.IndexOf('=');

            if(setterIndex == -1)
            {
                return false;
            }

            string variableName = variableCode.Substring(0, setterIndex).Trim();
            string variableValue = variableCode.Substring(setterIndex + 1, variableEnd - setterIndex - 1).Trim();
            string variableValueFixed = variableValue.Substring(0, variableValue.Length - 1);

            if(script.scriptGlobalVariables.ContainsKey(variableName) == true)
            {
                return false;
            }

            script.scriptGlobalVariables.Add(variableName, LuaAnalyzer.DetermineParameterType(variableValueFixed));

            Console.WriteLine("Registered Global Variable: " + variableName);

            return true;
        }
    }
}
