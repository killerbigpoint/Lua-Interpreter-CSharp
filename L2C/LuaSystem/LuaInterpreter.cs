using System.Collections.Generic;
using System.Collections;
using System;

namespace MunchenClient.Lua
{
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
            Console.WriteLine("Script: " + function);

            for (int i = 0; i < function.functionCode.Length; i++)
            {
                if(CheckForInternalInstruction(function.functionCode, i) == true)
                {
                    continue;
                }
                else if(CheckForIfStatement(function.functionCode, i) == true)
                {
                    continue;
                }
            }

            return true;
        }

        private static bool CheckForIfStatement(string script, int index)
        {
            if (script.Substring(index).StartsWith("if") == false)
            {
                return false;
            }

            int instructionParameterStart = script.IndexOf("(", index);
            int instructionParameterEnd = script.IndexOf(")", index);

            if(instructionParameterStart == -1 || instructionParameterEnd == -1)
            {
                Console.WriteLine("Invalid statement found at index: " + index);

                return false;
            }

            string statement = script.Substring(instructionParameterStart + 1, instructionParameterEnd - instructionParameterStart - 1);

            Console.WriteLine("Statement: " + statement);

            int comparatorIndex = FindComparator(statement);

            if(comparatorIndex == -1)
            {
                Console.WriteLine("No comparator found at index: " + index);

                return false;
            }

            Console.WriteLine("Comparator found at index: " + comparatorIndex);

            string comparatorArgumentFirst = statement.Substring(0, comparatorIndex).Trim();
            string comparatorArgumentSecond = statement.Substring(statement.IndexOf(" ", comparatorIndex)).Trim();

            Console.WriteLine("First Argument: " + comparatorArgumentFirst);
            Console.WriteLine("Second Argument: " + comparatorArgumentSecond);

            if(comparatorArgumentFirst == comparatorArgumentSecond)
            {
                Console.WriteLine("Arguments were the same");
            }
            else
            {
                int elseStatementIndex = script.IndexOf("else", comparatorIndex);

                if(elseStatementIndex != -1)
                {
                    Console.WriteLine("Else statement found");
                }
            }

            return true;
        }

        private static int FindComparator(string statement)
        {
            //Comparator Check for "=="
            int comparatorIndex = statement.IndexOf("==");

            if (comparatorIndex != -1)
            {
                return comparatorIndex;
            }

            //Comparator Check for "!="
            comparatorIndex = statement.IndexOf("!=");

            if (comparatorIndex != -1)
            {
                return comparatorIndex;
            }

            //Comparator Check for "<"
            comparatorIndex = statement.IndexOf("<");

            if (comparatorIndex != -1)
            {
                return comparatorIndex;
            }

            //Comparator Check for ">"
            return statement.IndexOf("!=");
        }

        private static bool CheckForInternalInstruction(string script, int index)
        {
            //Make sure we got enough space for a potential function and won't hit the end of the script
            if ((script.Length - index) < 8)
            {
                return false;
            }

            int instructionIndex = script.IndexOf(";", index);

            if (instructionIndex == -1)
            {
                return false;
            }

            int instructionParameterStart = script.IndexOf("(", index);
            int instructionParameterEnd = script.IndexOf(")", index);

            if(instructionParameterStart == -1 || instructionParameterEnd == -1 || instructionParameterStart > instructionParameterEnd)
            {
                return false;
            }

            string instructionName = script.Substring(index, instructionParameterStart - index);

            if(LuaWrapper.InternalFunctionExists(instructionName) == true)
            {
                if(index > 1)
                {
                    int commentedOutIndex = script.IndexOf("//", index - 2, 2);

                    if (commentedOutIndex != -1)
                    {
                        return false;
                    }
                }

                string instructionParameters = script.Substring(instructionParameterStart + 1, instructionParameterEnd - instructionParameterStart - 1);
                List<object> parameters = new List<object>();

                foreach (string parameter in instructionParameters.Split(','))
                {
                    parameters.Add(DetermineParamterType(parameter.Trim()));
                }

                LuaWrapper.CallInternalFunction(instructionName, parameters.ToArray());

                return true;
            }

            return false;
        }

        private static object DetermineParamterType(string parameter)
        {
            if(parameter[0] == '"' && parameter[parameter.Length - 1] == '"')
            {
                return parameter;
            }
            
            string nonStringParameter = parameter.Substring(0, parameter.Length - (parameter.EndsWith("f") ? 1 : 0));

            if(int.TryParse(nonStringParameter, out int intParameter) == true)
            {
                return intParameter;
            }
            else if(float.TryParse(nonStringParameter, out float floatParameter) == true)
            {
                return floatParameter;
            }
            
            //TODO: Potentially add class types from the client here

            return parameter;
        }
    }
}
