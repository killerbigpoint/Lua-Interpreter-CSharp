using System.Collections.Generic;
using System.Linq;

namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionInternal : LuaInstruction
    {
        internal override void ExecuteInstruction()
        {
            List<object> variabler = new List<object>();

            foreach(var lmao in instructionParameters)
            {
                variabler.Add(lmao.GetUpdatedValue());
            }

            LuaWrapper.CallInternalFunction(instructionClass, instructionName, variabler.ToArray());
        }
    }
}
