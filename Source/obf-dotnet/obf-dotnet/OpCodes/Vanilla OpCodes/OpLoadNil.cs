using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpLoadNil : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpLoadNil);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "for Index = Instruction[OP_A], Instruction[OP_B] do Stack[Index] = (nil); end;";
    };
};