using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpVarArg : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpVarArg) && (Instruction.B != 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "local A = Instruction[OP_A]; local B = Instruction[OP_B]; for Index = 1, B, 1 do Stack[A + Index - 1] = VarArg[Index - 1]; end;";
    };

    public class OpVarArgB0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpVarArg) && (Instruction.B == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Top = A + VarArgs - 1; for Index = 0, VarArgs do Stack[A + Index] = VarArg[Index]; end; for I = Top + 1, StackSize do Stack[I] = nil; end;";
    };
};