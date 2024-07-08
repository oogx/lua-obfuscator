using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpGe : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpLe) && (Instruction.A != 0) && (!(Instruction.IsConstantB)) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "if (Stack[Instruction[OP_A]] <= Stack[Instruction[OP_C]]) then InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.IsConstantA = false; Instruction.IsConstantB = false; Instruction.IsConstantC = false; Instruction.A = Instruction.B; Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC;
        }
    };

    public class OpGeB : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpLe) && (Instruction.A != 0) && (Instruction.IsConstantB) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "if (Constants[Instruction[OP_A]] <= Stack[Instruction[OP_C]]) then InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.IsConstantA = true; Instruction.IsConstantB = false; Instruction.IsConstantC = false; Instruction.A = Instruction.B; Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC; Instruction.ConstantType |= InstructionConstantType.RA;
        }
    };

    public class OpGeC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpLe) && (Instruction.A != 0) && (!(Instruction.IsConstantB)) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "if (Stack[Instruction[OP_A]] <= Constants[Instruction[OP_C]]) then InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.IsConstantA = false; Instruction.IsConstantB = false; Instruction.IsConstantC = true; Instruction.A = Instruction.B; Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC; Instruction.ConstantType |= InstructionConstantType.RC;
        }
    };

    public class OpGeBC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpLe) && (Instruction.A != 0) && (Instruction.IsConstantB) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "if (Constants[Instruction[OP_A]] <= Constants[Instruction[OP_C]]) then InstructionPoint = Instruction[OP_B]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.IsConstantA = true; Instruction.IsConstantB = false; Instruction.IsConstantC = true; Instruction.A = Instruction.B; Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[2]]; Instruction.InstructionType = InstructionType.AsBxC; Instruction.ConstantType |= InstructionConstantType.RA | InstructionConstantType.RC;
        }
    };
};