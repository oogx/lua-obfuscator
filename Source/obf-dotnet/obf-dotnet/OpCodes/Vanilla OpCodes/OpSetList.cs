using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpSetList : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpSetList) && (Instruction.B != 0) && (Instruction.C != 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local B = Instruction[OP_B]; local Offset = 50 * (Instruction[OP_C] - 1); local T = Stack[A]; local Count = 0; for Index = A + 1, B do T[Offset + Count + 1] = Stack[A + (Index - A)]; Count = Count + 1; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B += Instruction.A;
        }
    };

    public class OpSetListB0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpSetList) && (Instruction.B == 0) && (Instruction.C != 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Table = Stack[A]; local Count, Offset = 0, 50 * (Instruction[OP_C] - 1); for Index = A + 1, Top, 1 do Table[Offset + Count + 1] = Stack[Index]; Count = Count + 1; end;";
    };

    public class OpSetListC0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpSetList) && (Instruction.C == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"";

        public override void Mutate(Instruction Instruction)
        {
        }
    };
};