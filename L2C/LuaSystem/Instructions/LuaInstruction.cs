using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
