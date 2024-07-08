using Obfuscator.Bytecode.IR;
using System.Collections.Generic;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpSuperOperator : VOpCode
    {
        public List<Instruction> Instructions = new List<Instruction>();
        public List<VOpCode> Virtuals = new List<VOpCode>();

        public bool IsWritten = false;

        public override bool IsInstruction(Instruction Instruction) => (false);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext)
        {
            string Obfuscated = "";

            int InstructionPoint = 0;

            foreach (Instruction Instruction in Instructions)
            {
                var VirtualOpcode = Virtuals[InstructionPoint];

                Instruction.UpdateRegisters();

                if (!Instruction.CustomInstructionData.Mutated)
                    VirtualOpcode?.Mutate(Instruction);

                Instruction.CustomInstructionData.Mutated = true;

                Obfuscated += VirtualOpcode.GetObfuscated(ObfuscationContext) + " Instruction = Instruction[OP_E]; ";

                Instruction.WrittenVIndex = 0;

                InstructionPoint++;
            };

            return (Obfuscated + " Instruction = Instruction[OP_E]; ");
        }
    };
}