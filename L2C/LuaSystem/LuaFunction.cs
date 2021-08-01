using MunchenClient.Lua.Instructions;
using System.Collections.Generic;

namespace MunchenClient.Lua
{
    internal class LuaFunction
    {
        internal string functionName;
        internal string functionCode;

        internal readonly List<ILuaInstruction> functionExecutionList = new List<ILuaInstruction>();
    }
}
