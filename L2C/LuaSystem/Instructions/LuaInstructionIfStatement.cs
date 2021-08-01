using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionIfStatement : ILuaInstruction
    {
        public string InstructionName => string.Empty;
        public string InstructionCode => string.Empty;

        public List<ILuaInstruction> yes = new List<ILuaInstruction>();
        public List<ILuaInstruction> no = new List<ILuaInstruction>();

        public void ExecuteInstruction()
        {
            
        }
    }
}
