using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpMod : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpMod) && (!(Instruction.IsConstantB)) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Stack[Instruction[OP_B]] % Stack[Instruction[OP_C]];";
    };

    public class OpModB : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpMod) && (Instruction.IsConstantB) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Constants[Instruction[OP_B]] % Stack[Instruction[OP_C]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RB;
        }
    };

    public class OpModC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpMod) && (!(Instruction.IsConstantB)) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Stack[Instruction[OP_B]] % Constants[Instruction[OP_C]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RC;
        }
    };

    public class OpModBC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpMod) && (Instruction.IsConstantB) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Constants[Instruction[OP_B]] % Constants[Instruction[OP_C]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RB | InstructionConstantType.RC;
        }
    };
};