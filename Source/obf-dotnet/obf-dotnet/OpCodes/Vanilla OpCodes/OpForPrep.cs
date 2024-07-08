using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpForPrep : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpForPrep);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A] = 0 + (Stack[A]); Stack[A + 1] = 0 + (Stack[A + 1]); Stack[A + 2] = 0 + (Stack[A + 2]); local Index = Stack[A]; local Step = Stack[A + 2]; if (Step > 0) then if (Index > Stack[A + 1]) then InstructionPoint = Instruction[OP_B]; else Stack[A + 3] = Index; end; elseif (Index < Stack[A + 1]) then InstructionPoint = Instruction[OP_B]; else Stack[A + 3] = Index; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[0]];
        }
    };
};