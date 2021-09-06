using MunchenClient.Lua.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionVariable : LuaInstruction
    {
        internal Manipulator variableManipulator;
        internal string variableName;
        internal object variableValue;

        internal override void ExecuteInstruction()
        {
            instructionFunction.ManipulateVariable(variableManipulator, variableName, variableValue);
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
