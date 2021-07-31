using System.Collections.Generic;

namespace MunchenClient.Lua
{
    internal class LuaScript
    {
        internal string scriptName;
        internal int scriptVersion;
        internal bool scriptAutoload;
        internal string scriptCode;
        internal Dictionary<string, LuaFunction> scriptFunctions;
        internal Dictionary<string, object> scriptTempVariables;
    }
}
