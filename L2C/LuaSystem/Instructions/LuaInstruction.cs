﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal interface ILuaInstruction
    {
        string InstructionName { get; }
        string InstructionCode { get; }

        void ExecuteInstruction();
    }
}
