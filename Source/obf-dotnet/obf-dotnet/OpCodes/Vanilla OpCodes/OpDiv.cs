using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpDiv : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpDiv) && (!(Instruction.IsConstantB)) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Stack[Instruction[OP_B]] / Stack[Instruction[OP_C]];";
    };

    public class OpDivB : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpDiv) && (Instruction.IsConstantB) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Constants[Instruction[OP_B]] / Stack[Instruction[OP_C]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RB;
        }
    };

    public class OpDivC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpDiv) && (!(Instruction.IsConstantB)) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Stack[Instruction[OP_B]] / Constants[Instruction[OP_C]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RC;
        }
    };

    public class OpDivBC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpDiv) && (Instruction.IsConstantB) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Constants[Instruction[OP_B]] / Constants[Instruction[OP_C]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RB | InstructionConstantType.RC;
        }
    };
};