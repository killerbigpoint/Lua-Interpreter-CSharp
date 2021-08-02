namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionInternal : LuaInstruction
    {
        internal override void ExecuteInstruction()
        {
            LuaWrapper.CallInternalFunction(instructionName, instructionParameters);
        }
    }
}
