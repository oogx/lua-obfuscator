using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpLen : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpLen);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]] = #Stack[Instruction[OP_B]];";
    };
};