using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpSetUpValue : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpSetUpValue);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "UpValues[Instruction[OP_B]] = Stack[Instruction[OP_A]];";
    };
};