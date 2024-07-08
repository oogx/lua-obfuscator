using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class NoOpCode : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.None);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "";
    };
};