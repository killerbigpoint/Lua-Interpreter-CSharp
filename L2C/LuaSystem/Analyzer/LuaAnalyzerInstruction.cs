using MunchenClient.Lua.Instructions;
using MunchenClient.Lua.Utils;
using System.Collections.Generic;
using System.Linq;
using System;

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

                //Check for variable instantiator
                VariableAnalyzeReport report = CheckForVariableInstantiator(function, i);

                if (report.found == true)
                {
                    i += report.skipAhead;
                }

                //Check for variable manipulator
                report = CheckForVariableManipulator(function, i);

                if (report.found == true)
                {
                    i += report.skipAhead;
                }
            }

            return true;
        }

        private static VariableAnalyzeReport CheckForVariableManipulator(LuaFunction function, int index)
        {
            VariableAnalyzeReport report = new VariableAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            string scriptCodeFixed = function.functionCode.Substring(index);

            Dictionary<string, LuaVariable> luaVariables = function.GetAllVariables();

            for (int i = 0; i < luaVariables.Count; i++)
            {
                KeyValuePair<string, LuaVariable> variable = luaVariables.ElementAt(i);

                if (scriptCodeFixed.StartsWith(variable.Key) == true)
                {
                    int variableEnd = scriptCodeFixed.IndexOf(';');

                    if (variableEnd == -1)
                    {
                        return report;
                    }

                    string instructionCode = scriptCodeFixed.Substring(0, variableEnd);

                    Manipulator manipulator = LuaManipulator.FindManipulator(instructionCode);

                    if(manipulator.manipulatorIndex == -1)
                    {
                        return report;
                    }

                    string variableName = instructionCode.Substring(0, manipulator.manipulatorIndex).Trim();
                    string variableValue = instructionCode.Substring(manipulator.manipulatorIndex + manipulator.manipulatorIndexLength, variableEnd - manipulator.manipulatorIndex - manipulator.manipulatorIndexLength).Trim();
                    
                    function.functionExecutionList.Add(new LuaInstructionVariable
                    {
                        instructionFunction = function,
                        instructionName = $"Variable Manipulator: {variableName}",
                        instructionCode = instructionCode,
                        instructionParameters = null,

                        variableManipulator = manipulator,
                        variableName = variableName,
                        variableValue = variableValue
                    });

                    report.found = true;
                    report.skipAhead = variableEnd;
                }
            }

            return report;
        }

        private static VariableAnalyzeReport CheckForVariableInstantiator(LuaFunction function, int index)
        {
            VariableAnalyzeReport report = new VariableAnalyzeReport
            {
                found = false,
                skipAhead = 0
            };

            if ((function.functionCode.Length - index) < 3)
            {
                return report;
            }

            string scriptCodeFixed = function.functionCode.Substring(index);

            if (scriptCodeFixed.StartsWith("var") == false)
            {
                return report;
            }

            return LuaAnalyzerVariable.DetermineVariable(function, scriptCodeFixed, index);
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

            string functionCode = function.functionCode.Substring(index).Trim();
            string functionClass = string.Empty;

            bool foundClass = false;

            foreach (var registeredClass in LuaWrapper.classes)
            {
                if (functionCode.StartsWith(registeredClass.Key) == true)
                {
                    functionClass = registeredClass.Key;
                    foundClass = true;

                    break;
                }
            }

            if(foundClass == false)
            {
                return report;
            }

            int instructionEndIndex = functionCode.IndexOf(";");

            if (instructionEndIndex == -1)
            {
                return report;
            }

            int instructionParameterStart = functionCode.IndexOf("(");
            int instructionParameterEnd = functionCode.IndexOf(")");

            if (instructionParameterStart == -1 || instructionParameterEnd == -1 || instructionParameterStart > instructionParameterEnd)
            {
                return report;
            }

            string instructionNameFull = functionCode.Substring(0, instructionParameterStart).Trim();
            string instructionActual = instructionNameFull.Split('.')[1];

            if (LuaWrapper.InternalFunctionExists(functionClass, instructionActual) == false)
            {
                return report;
            }

            if (index > 1)
            {
                int commentedOutIndex = functionCode.IndexOf("//", index - 2, 2);

                if (commentedOutIndex != -1)
                {
                    return report;
                }
            }

            List<LuaInstructionVariable> parameters = new List<LuaInstructionVariable>();

            foreach (string parameter in functionCode.Substring(instructionParameterStart + 1, instructionParameterEnd - instructionParameterStart - 1).Split(','))
            {
                parameters.Add(new LuaInstructionVariable
                {
                    instructionFunction = function,
                    variableName = parameter,
                    variableValue = LuaAnalyzer.DetermineParameterType(function, parameter.Trim())
                });
            }

            function.functionExecutionList.Add(new LuaInstructionInternal
            {
                instructionFunction = function,
                instructionClass = functionClass,
                instructionName = instructionActual,
                instructionCode = functionCode.Substring(0, instructionEndIndex),
                instructionParameters = parameters
            });

            report.found = true;
            report.skipAhead = instructionEndIndex;

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

            Comparator comparator = LuaComparator.FindComparator(statement);

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
    }
}
