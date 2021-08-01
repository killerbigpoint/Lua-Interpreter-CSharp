using System.Collections.Generic;

namespace MunchenClient.Lua
{
    internal class LuaScript : LuaCodeExecutor
    {
        internal string scriptName;
        internal int scriptVersion;
        internal bool scriptAutoload;
        internal string scriptCode;
    }
}
