using MunchenClient.Lua.Instructions;
using System.Collections.Generic;

namespace MunchenClient.Lua
{
    internal class LuaFunction
    {
        internal LuaScript functionScript;
        internal string functionName;
        internal string functionCode;

        internal readonly Dictionary<string, object> functionTempVariables = new Dictionary<string, object>();
        internal readonly List<ILuaInstruction> functionExecutionList = new List<ILuaInstruction>();

        internal object GetVariable(string variableName)
        {
            if(functionTempVariables.ContainsKey(variableName) == true)
            {
                return functionTempVariables[variableName];
            }

            if(functionScript.scriptGlobalVariables.ContainsKey(variableName) == true)
            {
                return functionScript.scriptGlobalVariables[variableName];
            }

            return null;
        }

        internal void ExecuteFunction()
        {
            functionTempVariables.Clear();

            for (int i = 0; i < functionExecutionList.Count; i++)
            {
                functionExecutionList[i].ExecuteInstruction();
            }
        }
    }
}
