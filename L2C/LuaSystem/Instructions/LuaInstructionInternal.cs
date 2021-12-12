using System.Collections.Generic;
using System.Linq;

namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionInternal : LuaInstruction
    {
        internal override void ExecuteInstruction()
        {
            object[] variables = new object[instructionParameters.Count];

            for(int i = 0; i < instructionParameters.Count; i++)
            {
                variables[i] = instructionParameters[i].GetUpdatedValue();
            }

            LuaWrapper.CallInternalFunction(instructionClass, instructionName, variables);
        }
    }
}
