﻿using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Analyzer
{
    internal struct VariableAnalyzeReport
    {
        internal bool found;
        internal int skipAhead;
    }

    internal class LuaAnalyzerVariable
    {
        internal static VariableAnalyzeReport CheckForVariables(LuaScript script, int index)
        {
            VariableAnalyzeReport report = new VariableAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            //Make sure we got enough space for a potential variable and won't hit the end of the script
            if ((script.scriptCode.Length - index) < 3)
            {
                return report;
            }

            string scriptCodeFixed = script.scriptCode.Substring(index);
            
            if (scriptCodeFixed.StartsWith("var") == true)
            {
                report = DetermineVariable(script, scriptCodeFixed, index);
            }

            return report;
        }

        internal static VariableAnalyzeReport DetermineVariable(LuaScript script, string scriptCode, int scriptIndex)
        {
            VariableAnalyzeReport report = new VariableAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            int variableEnd = scriptCode.IndexOf(';');

            if (variableEnd == -1)
            {
                return report;
            }

            string variableCode = scriptCode.Substring(3, variableEnd - 3);

            int setterIndex = variableCode.IndexOf('=');

            if(setterIndex == -1)
            {
                return report;
            }

            string variableName = variableCode.Substring(0, setterIndex).Trim();
            string variableValue = variableCode.Substring(setterIndex + 1, variableCode.Length - setterIndex - 1).Trim();

            if (script.GetVariable(variableName) != null)
            {
                return report;
            }

            script.InsertVariable(variableName, LuaAnalyzer.DetermineParameterType(script, variableValue), scriptIndex, true);

            report.found = true;
            report.skipAhead = variableEnd;

            return report;
        }

        internal static VariableAnalyzeReport DetermineVariable(LuaFunction function, string scriptCode, int scriptIndex)
        {
            VariableAnalyzeReport report = new VariableAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            int variableEnd = scriptCode.IndexOf(';');

            if (variableEnd == -1)
            {
                return report;
            }

            string variableCode = scriptCode.Substring(3, variableEnd - 3);

            int setterIndex = variableCode.IndexOf('=');

            if (setterIndex == -1 || setterIndex > variableEnd)
            {
                return report;
            }

            string variableName = variableCode.Substring(0, setterIndex).Trim();

            if (function.GetVariable(variableName) != null)
            {
                return report;
            }

            string variableValue = variableCode.Substring(setterIndex + 1, variableCode.Length - setterIndex - 1).Trim();

            function.InsertVariable(variableName, LuaAnalyzer.DetermineParameterType(function, variableValue), scriptIndex, false);

            report.found = true;
            report.skipAhead = variableEnd;

            return report;
        }
    }
}
