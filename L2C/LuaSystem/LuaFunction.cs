using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua
{
    internal struct LuaFunction
    {
        internal string functionName;
        internal string functionCode;
        internal List<LuaInstruction> functionInstructions;
    }
}
