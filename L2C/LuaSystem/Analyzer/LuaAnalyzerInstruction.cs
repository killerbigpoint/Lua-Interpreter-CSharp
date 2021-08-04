using MunchenClient.Lua.Instructions;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace MunchenClient.Lua.Analyzer
{
    internal struct InstructionAnalyzeReport
    {
        internal bool found;
        internal int skipAhead;
    }

    internal class LuaAnalyzerInstruction
    {
        internal static bool CheckForInstructions(LuaFunction function)
        {
            if(function == null)
            {
                return false;
            }

            for (int i = 0; i < function.functionCode.Length; i++)
            {
                //Check for 'if' statement
                InstructionAnalyzeReport postAnalyze = CheckForIfStatement(function, i);

                if (postAnalyze.found == true)
                {
                    i += postAnalyze.skipAhead;
                }

                //Check for internal instruction
                postAnalyze = CheckForInternalInstruction(function, i);

                if (postAnalyze.found == true)
                {
                    i += postAnalyze.skipAhead;
                }

                //Make sure we got enough space for a potential variable and won't hit the end of the script
                if ((function.functionCode.Length - i) < 3)
                {
                    return false;
                }

                string scriptCodeFixed = function.functionCode.Substring(i);

                if (scriptCodeFixed.StartsWith("var") == true)
                {
                    LuaAnalyzerVariable.DetermineVariable(function, scriptCodeFixed);
                }
            }

            return true;
        }

        private static InstructionAnalyzeReport CheckForInternalInstruction(LuaFunction function, int index)
        {
            InstructionAnalyzeReport report = new InstructionAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            //Make sure we got enough space for a potential instruction and won't hit the end of the script
            if ((function.functionCode.Length - index) < 8)
            {
                return report;
            }

            int instructionEndIndex = function.functionCode.IndexOf(";", index);

            if (instructionEndIndex == -1)
            {
                return report;
            }

            int instructionParameterStart = function.functionCode.IndexOf("(", index);
            int instructionParameterEnd = function.functionCode.IndexOf(")", index);

            if (instructionParameterStart == -1 || instructionParameterEnd == -1 || instructionParameterStart > instructionParameterEnd)
            {
                return report;
            }

            string instructionName = function.functionCode.Substring(index, instructionParameterStart - index);

            if (LuaWrapper.InternalFunctionExists(instructionName) == false)
            {
                return report;
            }

            if (index > 1)
            {
                int commentedOutIndex = function.functionCode.IndexOf("//", index - 2, 2);

                if (commentedOutIndex != -1)
                {
                    return report;
                }
            }

            List<object> parameters = new List<object>();

            foreach (string parameter in function.functionCode.Substring(instructionParameterStart + 1, instructionParameterEnd - instructionParameterStart - 1).Split(','))
            {
                parameters.Add(LuaAnalyzer.DetermineParameterType(parameter.Trim()));
            }

            function.functionExecutionList.Add(new LuaInstructionInternal
            {
                instructionFunction = function,
                instructionName = instructionName,
                instructionCode = function.functionCode.Substring(index, instructionEndIndex - index),
                instructionParameters = parameters.ToArray()
            });

            report.found = true;
            report.skipAhead = instructionEndIndex - index;

            return report;
        }

        private static InstructionAnalyzeReport CheckForIfStatement(LuaFunction function, int index)
        {
            InstructionAnalyzeReport report = new InstructionAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            if (function.functionCode.Substring(index).StartsWith("if") == false)
            {
                return report;
            }

            int instructionParameterStart = function.functionCode.IndexOf("(", index);
            int instructionParameterEnd = function.functionCode.IndexOf(")", index);

            if (instructionParameterStart == -1 || instructionParameterEnd == -1)
            {
                Console.WriteLine("Invalid statement found at index: " + index);

                return report;
            }

            string statement = function.functionCode.Substring(instructionParameterStart + 1, instructionParameterEnd - instructionParameterStart - 1);

            Comparator comparator = FindComparator(statement);

            if (comparator.comparatorType == ComparatorType.ComparatorType_Unknown)
            {
                Console.WriteLine("No comparator found at index: " + index);

                return report;
            }

            string comparatorArgumentFirst = statement.Substring(0, comparator.comparatorIndex).Trim();
            string comparatorArgumentSecond = statement.Substring(statement.IndexOf(" ", comparator.comparatorIndex)).Trim();

            int firstCodeBlockIndexStart = function.functionCode.IndexOf("{", instructionParameterEnd);
            int firstCodeBlockIndexEnd = LuaAnalyzer.FindCodeSectionEnd(function.functionCode, firstCodeBlockIndexStart);

            if (firstCodeBlockIndexStart == -1 || firstCodeBlockIndexEnd == -1)
            {
                return report;
            }

            report.skipAhead = firstCodeBlockIndexEnd - index;

            string firstCodeBlockCode = function.functionCode.Substring(firstCodeBlockIndexStart + 1, firstCodeBlockIndexEnd - firstCodeBlockIndexStart - 1);
            string secondCodeBlockCode = string.Empty;

            int elseStatementIndex = function.functionCode.IndexOf("else", firstCodeBlockIndexEnd);

            if (elseStatementIndex != -1)
            {
                int secondCodeBlockIndexStart = function.functionCode.IndexOf("{", elseStatementIndex);
                int secondCodeBlockIndexEnd = LuaAnalyzer.FindCodeSectionEnd(function.functionCode, secondCodeBlockIndexStart);

                if (secondCodeBlockIndexStart != -1 && secondCodeBlockIndexEnd != -1)
                {
                    secondCodeBlockCode = function.functionCode.Substring(secondCodeBlockIndexStart + 1, secondCodeBlockIndexEnd - secondCodeBlockIndexStart - 1);

                    report.skipAhead = secondCodeBlockIndexEnd - index;
                }
            }

            LuaInstructionIfStatement instruction = new LuaInstructionIfStatement
            {
                argumentComparator = comparator,
                argumentFirst = comparatorArgumentFirst,
                argumentSecond = comparatorArgumentSecond,

                instructionFunction = function,
                instructionName = "If-Statement",
                instructionCode = function.functionCode.Substring(index, report.skipAhead + 1),
                instructionParameters = null,

                codeSectionFirst = new LuaFunction
                {
                    executionParent = function,
                    functionName = "If-Statement-First",
                    functionCode = firstCodeBlockCode,
                },
                codeSectionSecond = string.IsNullOrEmpty(secondCodeBlockCode) ? null : new LuaFunction
                {
                    executionParent = function,
                    functionName = "If-Statement-Second",
                    functionCode = secondCodeBlockCode,
                }
            };

            function.functionExecutionList.Add(instruction);

            CheckForInstructions(instruction.codeSectionFirst);
            CheckForInstructions(instruction.codeSectionSecond);

            report.found = true;

            return report;
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
            };
        }
    }
}
