using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpReturn : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpReturn) && (Instruction.B > 3));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; do return UnPack(Stack, A, A + Instruction[OP_B]) end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B -= 2;
        }
    };

    public class OpReturnB2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpReturn) && (Instruction.B == 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"do return (Stack[Instruction[OP_A]]); end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0;
        }
    };

    public class OpReturnB3 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpReturn) && (Instruction.B == 3));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; do return (Stack[A]), (Stack[A + 1]); end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0;
        }
    };

    public class OpReturnB0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpReturn) && (Instruction.B == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; do return UnPack(Stack, A, Top); end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0;
        }
    };

    public class OpReturnB1 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpReturn) && (Instruction.B == 1));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "do return; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0;
        }
    };
};