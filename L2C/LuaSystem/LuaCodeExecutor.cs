using MunchenClient.Lua.Utils;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MunchenClient.Lua.Analyzer;

namespace MunchenClient.Lua
{
    internal class LuaCodeExecutor
    {
        internal LuaCodeExecutor executionParent;

        private readonly Dictionary<string, LuaVariable> executionVariables = new Dictionary<string, LuaVariable>();
        internal readonly Dictionary<string, LuaFunction> executionFunctions = new Dictionary<string, LuaFunction>();

        internal Dictionary<string, LuaVariable> GetAllVariables()
        {
            Dictionary<string, LuaVariable> variables = executionParent != null ? executionParent.GetAllVariables() : new Dictionary<string, LuaVariable>();

            foreach(KeyValuePair<string, LuaVariable> variable in executionVariables)
            {
                variables.Add(variable.Key, variable.Value);
            }

            return variables;
        }

        internal void InsertVariable(string variableName, object variableValue, int variableIndex, bool variableGlobal)
        {
            if (executionVariables.ContainsKey(variableName) == true)
            {
                return;
            }

            executionVariables.Add(variableName, new LuaVariable
            {
                variableValue = variableValue,
                variableIndex = variableIndex,
                variableGlobal = variableGlobal
            });

            Console.WriteLine($"Inserted variable: {variableName} at index {variableIndex} (Global: {variableGlobal})");
        }

        internal void ManipulateVariable(Manipulator manipulator, string variableName, object variableValue)
        {
            if (executionVariables.ContainsKey(variableName) == true)
            {
                ManipulateVariableWithComparator(manipulator, ref executionVariables[variableName].variableValue, variableValue);

                return;
            }

            if (executionParent != null)
            {
                executionParent.ManipulateVariable(manipulator, variableName, variableValue);

                return;
            }

            Console.WriteLine($"No variable found with name: {variableName}");
        }

        internal void ManipulateVariableWithComparator(Manipulator manipulator, ref object variableFirstValue, object variableSecondValue)
        {
            Type firstValueType = variableFirstValue.GetType();
            Type secondValueType = variableSecondValue.GetType();

            Console.WriteLine("Type: " + firstValueType.Name);

            if(firstValueType != secondValueType)
            {
                Console.WriteLine("Types are not the same smh");

                return;
            }

            switch(manipulator.manipulatorType)
            {
                case ManipulatorType.ManipulatorType_Assign:
                {
                    variableFirstValue = variableSecondValue;

                    break;
                }

                case ManipulatorType.ManipulatorType_Addition:
                {
                    switch(firstValueType.Name)
                        {
                            case "String":
                            {
                                string firstValue = (string)variableFirstValue;
                                string secondValue = (string)variableSecondValue;

                                string variableValue = (string)LuaAnalyzer.DetermineParameterType(null, firstValue) + (string)LuaAnalyzer.DetermineParameterType(null, secondValue);
                                variableFirstValue = variableValue;

                                Console.WriteLine("Manipulated with a string");

                                break;
                            }

                            case "Double":
                            {
                                break;
                            }
                        }

                    break;
                }
            }
        }

        internal LuaVariable GetVariable(string variableName)
        {
            if (executionVariables.ContainsKey(variableName) == true)
            {
                return executionVariables[variableName];
            }

            if (executionParent != null)
            {
                return executionParent.GetVariable(variableName);
            }

            return null;
        }
    }
}
