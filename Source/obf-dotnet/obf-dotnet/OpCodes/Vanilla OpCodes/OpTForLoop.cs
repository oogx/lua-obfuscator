using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpTForLoop : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpTForLoop);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local C = Instruction[OP_C]; local D = A + 2; local Result = ({ Stack[A](Stack[A + 1], Stack[D]); }); for Index = 1, C do Stack[D + Index] = Result[Index]; end; local R = Result[1]; if (R) then Stack[D] = R; InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC;
        }
    };
};