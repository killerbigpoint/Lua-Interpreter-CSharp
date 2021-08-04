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
        internal static bool CheckForVariables(LuaScript script, int index)
        {
            //Make sure we got enough space for a potential variable and won't hit the end of the script
            if ((script.scriptCode.Length - index) < 3)
            {
                return false;
            }

            string scriptCodeFixed = script.scriptCode.Substring(index);

            if (scriptCodeFixed.StartsWith("var") == true)
            {
                DetermineVariable(script, scriptCodeFixed);
            }

            return false;
        }

        internal static bool DetermineVariable(LuaScript script, string scriptCode)
        {
            int variableEnd = scriptCode.IndexOf(';');

            if (variableEnd == -1)
            {
                return false;
            }

            string variableCode = scriptCode.Substring(3, variableEnd - 3);

            int setterIndex = variableCode.IndexOf('=');

            if(setterIndex == -1)
            {
                return false;
            }

            string variableName = variableCode.Substring(0, setterIndex).Trim();
            string variableValue = variableCode.Substring(setterIndex + 1, variableCode.Length - setterIndex - 1).Trim();

            if (script.GetVariable(variableName) != null)
            {
                return false;
            }

            script.InsertVariable(variableName, LuaAnalyzer.DetermineParameterType(variableValue));

            Console.WriteLine($"Registered Global Variable: {variableName} with value: {variableValue}");

            return true;
        }

        internal static bool DetermineVariable(LuaFunction function, string scriptCode)
        {
            int variableEnd = scriptCode.IndexOf(';');

            if (variableEnd == -1)
            {
                return false;
            }

            string variableCode = scriptCode.Substring(3, variableEnd - 3);

            int setterIndex = variableCode.IndexOf('=');

            if (setterIndex == -1)
            {
                return false;
            }

            string variableName = variableCode.Substring(0, setterIndex).Trim();
            string variableValue = variableCode.Substring(setterIndex, variableCode.Length - setterIndex).Trim();

            if (function.GetVariable(variableName) != null)
            {
                return false;
            }

            function.InsertVariable(variableName, LuaAnalyzer.DetermineParameterType(variableValue));

            Console.WriteLine($"Registered Local Variable: {variableName} with value: {variableValue} under function: {function.functionName}");

            return true;
        }
    }
}
