using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua
{
    internal class LuaCodeExecutor
    {
        internal readonly Dictionary<string, object> scriptGlobalVariables = new Dictionary<string, object>();
        internal readonly Dictionary<string, LuaFunction> scriptFunctions = new Dictionary<string, LuaFunction>();
    }
}
