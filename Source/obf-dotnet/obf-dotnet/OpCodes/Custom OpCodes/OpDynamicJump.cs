using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpDynamicJump : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpDynamicJump);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "InstructionPoint = Stack[Instruction[OP_A]];";
    };
};