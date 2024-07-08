using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpTestSet : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpTestSet) && (Instruction.C == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "local B = Stack[Instruction[OP_C]]; if (not (B)) then Stack[Instruction[OP_A]] = B; InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.C = Instruction.B; Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC;
        }
    };

    public class OpTestSetC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpTestSet) && (Instruction.C != 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "local B = Stack[Instruction[OP_C]]; if (B) then Stack[Instruction[OP_A]] = B; InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.C = Instruction.B; Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC;
        }
    };
};