using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpJump : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpJump);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "InstructionPoint = Instruction[OP_B];";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = Instruction.Chunk.InstructionMap[Instruction.References[0]];
        }
    };
};