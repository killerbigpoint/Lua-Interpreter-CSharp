using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionIfStatement : ILuaInstruction
    {
        internal List<ILuaInstruction> yes = new List<ILuaInstruction>();
        internal List<ILuaInstruction> no = new List<ILuaInstruction>();

        internal override void ExecuteInstruction()
        {
            
        }
    }
}
