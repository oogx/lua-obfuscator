using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpSelf : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpSelf) && (!(Instruction.IsConstantC)));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "local A = Instruction[OP_A]; local B = Stack[Instruction[OP_B]]; Stack[A + 1] = B; Stack[A] = B[Stack[Instruction[OP_C]]];";
    };

    public class OpSelfC : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpSelf) && (Instruction.IsConstantC));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "local A = Instruction[OP_A]; local B = Stack[Instruction[OP_B]]; Stack[A + 1] = B; Stack[A] = B[Constants[Instruction[OP_C]]];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.ConstantType |= InstructionConstantType.RC;
        }
    };
};