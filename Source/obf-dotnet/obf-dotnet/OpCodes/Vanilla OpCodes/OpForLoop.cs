using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpForLoop : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpForLoop);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Step = Stack[A + 2]; local Index = Stack[A] + Step; Stack[A] = Index; if (Step > 0) then if (Index <= Stack[A + 1]) then InstructionPoint = Instruction[OP_B]; Stack[A + 3] = Index; end; elseif (Index >= Stack[A+1]) then InstructionPoint = Instruction[OP_B]; Stack[A + 3] = Index; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[0]];
        }
    };
};