using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionVariable : LuaInstruction
    {
        internal string variableName;
        internal object variableValue;

        internal override void ExecuteInstruction()
        {
            instructionFunction.ManipulateVariable(variableName, variableValue);
        }

        internal object GetUpdatedValue()
        {
            LuaVariable variable = instructionFunction.GetVariable(variableName);

            if(variable == null)
            {
                return variableValue;
            }

            return variable.variableValue;
        }
    }
}
