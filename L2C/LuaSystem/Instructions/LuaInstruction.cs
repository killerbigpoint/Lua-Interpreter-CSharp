using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal abstract class ILuaInstruction
    {
        internal LuaFunction instructionFunction;
        internal string instructionName;
        internal string instructionCode;
        internal object[] instructionParameters;

        internal abstract void ExecuteInstruction();
    }
}
