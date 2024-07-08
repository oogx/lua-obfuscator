using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpClosure : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpClosure) && (Instruction.References[0].UpValueCount > 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local Function = Functions[Instruction[OP_B]]; local fUpValues = Instruction[OP_D]; local Indexes = {}; local nUpValues = SetMetaTable({}, { __index = function(_, Key) local UpValue = Indexes[Key]; return (UpValue[1][UpValue[2]]); end, __newindex = function(_, Key, Value) local UpValue = Indexes[Key]; UpValue[1][UpValue[2]] = Value; end; }); for Index = 1, Instruction[OP_C], 1 do local UpValue = fUpValues[Index]; if (UpValue[0] == 0) then Indexes[Index - 1] = ({ Stack, UpValue[1] }); else Indexes[Index - 1] = ({ UpValues, UpValue[1] }); end; lUpValues[#lUpValues + 1] = Indexes; end; Stack[Instruction[OP_A]] = Wrap(Function, nUpValues, Environment);";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.InstructionType = InstructionType.Closure; Instruction.C = Instruction.Chunk.Chunks[Instruction.B].UpValueCount;
        }
    };

    public class OpClosureNU : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpClosure) && (Instruction.References[0].UpValueCount == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Wrap(Functions[Instruction[OP_B]], (nil), Environment);";
    };
};