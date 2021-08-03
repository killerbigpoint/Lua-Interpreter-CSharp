﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua
{
    internal class LuaCodeExecutor
    {
        internal LuaCodeExecutor executionParent;

        private readonly Dictionary<string, object> executionVariables = new Dictionary<string, object>();
        internal readonly Dictionary<string, LuaFunction> executionFunctions = new Dictionary<string, LuaFunction>();

        internal void InsertVariable(string variableName, object variableValue)
        {
            if (executionVariables.ContainsKey(variableName) == true)
            {
                return;
            }

            executionVariables.Add(variableName, variableValue);
        }

        internal object GetVariable(string variableName)
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