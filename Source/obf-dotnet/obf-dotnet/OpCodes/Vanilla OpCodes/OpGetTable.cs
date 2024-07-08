using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpGetTable : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpGetTable) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Stack[Instruction[OP_B]][Stack[Instruction[OP_C]]];";
    };

    public class OpGetTableC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpGetTable) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = Stack[Instruction[OP_B]][Constants[Instruction[OP_C]]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RC;
        }
    };
};