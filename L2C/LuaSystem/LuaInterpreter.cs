using System.Collections.Generic;
using System.Collections;
using System;

namespace MunchenClient.Lua
{
    internal enum ComparatorType : short
    {
        ComparatorType_Unknown = 0,
        ComparatorType_EqualTo = 1,
        ComparatorType_NotEqualTo = 2,
        ComparatorType_LessThan = 3,
        ComparatorType_MoreThan = 4,
        ComparatorType_MoreOrEqualThan = 5,
        ComparatorType_LessOrEqualThan = 6,
    }

    internal struct Comparator
    {
        internal int comparatorIndex;
        internal ComparatorType comparatorType;
    }

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

            Comparator comparator = FindComparator(statement);

            if(comparator.comparatorType == ComparatorType.ComparatorType_Unknown)
            {
                Console.WriteLine("No comparator found at index: " + index);

                return false;
            }

            Console.WriteLine("Comparator found at index: " + comparator.comparatorIndex);

            string comparatorArgumentFirst = statement.Substring(0, comparator.comparatorIndex).Trim();
            string comparatorArgumentSecond = statement.Substring(statement.IndexOf(" ", comparator.comparatorIndex)).Trim();

            Console.WriteLine("First Argument: " + comparatorArgumentFirst);
            Console.WriteLine("Second Argument: " + comparatorArgumentSecond);

            if(ExecuteComparatorCode(comparator.comparatorType, comparatorArgumentFirst, comparatorArgumentSecond) == true)
            {
                
            }
            else
            {
                int elseStatementIndex = script.IndexOf("else", comparator.comparatorIndex);

                if (elseStatementIndex != -1)
                {
                    Console.WriteLine("Found 'Else' statement");
                }
                else
                {
                    Console.WriteLine("No 'Else' statement found");
                }
            }

            return true;
        }

        private static bool ExecuteComparatorCode(ComparatorType comparator, string argumentFirst, string argumentSecond)
        {
            switch(comparator)
            {
                case ComparatorType.ComparatorType_EqualTo:
                {
                    return argumentFirst == argumentSecond;
                }

                case ComparatorType.ComparatorType_NotEqualTo:
                {
                    return argumentFirst != argumentSecond;
                }

                case ComparatorType.ComparatorType_LessThan:
                {
                    if(float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted < argumentSecondConverted;
                }

                case ComparatorType.ComparatorType_MoreThan:
                {
                    if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted > argumentSecondConverted;
                }

                case ComparatorType.ComparatorType_MoreOrEqualThan:
                {
                    if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted >= argumentSecondConverted;
                }

                case ComparatorType.ComparatorType_LessOrEqualThan:
                {
                    if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted <= argumentSecondConverted;
                }

                default:
                {
                    return false;
                }
            }
        }

        private static Comparator FindComparator(string statement)
        {
            //Comparator Check for "=="
            int comparatorIndex = statement.IndexOf("==");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_EqualTo
                };
            }

            //Comparator Check for "!="
            comparatorIndex = statement.IndexOf("!=");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_NotEqualTo
                };
            }

            //Comparator Check for "<"
            comparatorIndex = statement.IndexOf("<");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_LessThan
                };
            }

            //Comparator Check for ">"
            comparatorIndex = statement.IndexOf(">");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_MoreThan
                };
            }

            //Comparator Check for "<="
            comparatorIndex = statement.IndexOf("<=");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_LessOrEqualThan
                };
            }

            //Comparator Check for ">="
            comparatorIndex = statement.IndexOf(">=");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_MoreOrEqualThan
                };
            }

            //Last Resort
            return new Comparator
            {
                comparatorIndex = -1,
                comparatorType = ComparatorType.ComparatorType_Unknown
            }; ;
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
