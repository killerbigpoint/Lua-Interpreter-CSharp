using System.Collections.Generic;

namespace MunchenClient.Lua
{
    internal class LuaScript
    {
        internal string scriptName;
        internal int scriptVersion;
        internal bool scriptAutoload;
        internal string scriptCode;

        internal readonly Dictionary<string, object> scriptGlobalVariables = new Dictionary<string, object>();
        internal readonly Dictionary<string, LuaFunction> scriptFunctions = new Dictionary<string, LuaFunction>();
    }
}
