using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System;

namespace MunchenClient.Lua.Instructions
{
    internal abstract class LuaInstruction
    {
        internal LuaFunction instructionFunction;
        internal string instructionClass;
        internal string instructionName;
        internal string instructionCode;
        internal List<LuaInstructionVariable> instructionParameters;

        internal abstract void ExecuteInstruction();
    }
}
