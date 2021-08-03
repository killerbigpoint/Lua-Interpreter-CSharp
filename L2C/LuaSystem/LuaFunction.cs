using MunchenClient.Lua.Instructions;
using System.Collections.Generic;

namespace MunchenClient.Lua
{
    internal class LuaFunction : LuaCodeExecutor
    {
        internal string functionName;
        internal string functionCode;

        internal readonly Dictionary<string, object> functionTempVariables = new Dictionary<string, object>();
        internal readonly List<LuaInstruction> functionExecutionList = new List<LuaInstruction>();

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
