using MunchenClient.Lua.Instructions;
using System.Collections.Generic;
using System;

namespace MunchenClient.Lua.Analyzer
{
    internal class LuaAnalyzerInstruction
    {
        internal static bool CheckForInstructions(LuaFunction function)
        {
            Console.WriteLine("Analyze Script: " + function.functionCode);

            for (int i = 0; i < function.functionCode.Length; i++)
            {
                if (CheckForInternalInstruction(function, i) == true)
                {
                    continue;
                }
                else if (CheckForIfStatement(function, i) == true)
                {
                    continue;
                }
            }

            return true;
        }

        private static bool CheckForInternalInstruction(LuaFunction function, int index)
        {
            //Make sure we got enough space for a potential function and won't hit the end of the script
            if ((function.functionCode.Length - index) < 8)
            {
                return false;
            }

            int instructionEndIndex = function.functionCode.IndexOf(";", index);

            if (instructionEndIndex == -1)
            {
                return false;
            }

            int instructionParameterStart = function.functionCode.IndexOf("(", index);
            int instructionParameterEnd = function.functionCode.IndexOf(")", index);

            if (instructionParameterStart == -1 || instructionParameterEnd == -1 || instructionParameterStart > instructionParameterEnd)
            {
                return false;
            }

            string instructionName = function.functionCode.Substring(index, instructionParameterStart - index);

            if (LuaWrapper.InternalFunctionExists(instructionName) == false)
            {
                return false;
            }

            if (index > 1)
            {
                int commentedOutIndex = function.functionCode.IndexOf("//", index - 2, 2);

                if (commentedOutIndex != -1)
                {
                    return false;
                }
            }

            List<object> parameters = new List<object>();

            foreach (string parameter in function.functionCode.Substring(instructionParameterStart + 1, instructionParameterEnd - instructionParameterStart - 1).Split(','))
            {
                parameters.Add(DetermineParamterType(parameter.Trim()));
            }

            function.functionExecutionList.Add(new LuaInstructionInternal
            {
                instructionName = instructionName,
                instructionCode = function.functionCode.Substring(index, instructionEndIndex - index),
                instructionParameters = parameters.ToArray()
            });

            return true;
        }

        private static object DetermineParamterType(string parameter)
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
            else if (float.TryParse(nonStringParameter, out float floatParameter) == true)
            {
                return floatParameter;
            }

            //TODO: Potentially add class types from the client here

            return parameter;
        }

        private static bool CheckForIfStatement(LuaFunction function, int index)
        {
            if (function.functionCode.Substring(index).StartsWith("if") == false)
            {
                return false;
            }

            int instructionParameterStart = function.functionCode.IndexOf("(", index);
            int instructionParameterEnd = function.functionCode.IndexOf(")", index);

            if (instructionParameterStart == -1 || instructionParameterEnd == -1)
            {
                Console.WriteLine("Invalid statement found at index: " + index);

                return false;
            }

            string statement = function.functionCode.Substring(instructionParameterStart + 1, instructionParameterEnd - instructionParameterStart - 1);

            Console.WriteLine("Statement: " + statement);

            Comparator comparator = FindComparator(statement);

            if (comparator.comparatorType == ComparatorType.ComparatorType_Unknown)
            {
                Console.WriteLine("No comparator found at index: " + index);

                return false;
            }

            Console.WriteLine("Comparator found at index: " + comparator.comparatorIndex);

            string comparatorArgumentFirst = statement.Substring(0, comparator.comparatorIndex).Trim();
            string comparatorArgumentSecond = statement.Substring(statement.IndexOf(" ", comparator.comparatorIndex)).Trim();

            Console.WriteLine("First Argument: " + comparatorArgumentFirst);
            Console.WriteLine("Second Argument: " + comparatorArgumentSecond);

            if (ExecuteComparatorCode(comparator.comparatorType, comparatorArgumentFirst, comparatorArgumentSecond) == true)
            {

            }
            else
            {
                int elseStatementIndex = function.functionCode.IndexOf("else", comparator.comparatorIndex);

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

        private static bool ExecuteComparatorCode(ComparatorType comparator, string argumentFirst, string argumentSecond)
        {
            switch (comparator)
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
                        if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
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
    }
}
