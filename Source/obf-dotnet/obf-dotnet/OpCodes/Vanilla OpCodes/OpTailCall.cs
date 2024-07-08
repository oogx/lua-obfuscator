using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpTailCall : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpTailCall) && (Instruction.B > 1));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; do return Stack[A](UnPack(Stack, A + 1, Instruction[OP_B])) end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B += Instruction.A - 1;
        }
    };

    public class OpTailCallB0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpTailCall) && (Instruction.B == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; do return Stack[A](UnPack(Stack, A + 1, Top)) end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0;
        }
    };

    public class OpTailCallB1 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpTailCall) && (Instruction.B == 1));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "do return Stack[Instruction[OP_A]](); end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0;
        }
    };
};