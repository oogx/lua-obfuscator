using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpTest : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpTest) && (Instruction.C == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "if (not (Stack[Instruction[OP_A]])) then InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC;
        }
    };

    public class OpTestC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpTest) && (Instruction.C != 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "if (Stack[Instruction[OP_A]]) then InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC;
        }
    };
};