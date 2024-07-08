using Obfuscator.Bytecode.IR;
using Obfuscator.Obfuscation.OpCodes;
using System.Collections.Generic;

namespace Obfuscator.Obfuscation.Generation.Macros
{
    public class LuaMacros
    {
        private static List<string> Macros = new List<string> { "Vestra_GETSTACK" };

        private Chunk HeadChunk;
        private List<VOpCode> Virtuals;

        public LuaMacros(Chunk HeadChunk, List<VOpCode> Virtuals)
        {
            this.HeadChunk = HeadChunk; this.Virtuals = Virtuals;
        }

        private void DoChunk(Chunk Chunk)
        {
            foreach (Chunk SubChunk in Chunk.Chunks) { DoChunk(SubChunk); };

            int InstructionPoint = 0;

            while (InstructionPoint < Chunk.Instructions.Count)
            {
                Instruction Instruction = Chunk.Instructions[InstructionPoint];

                if ((Instruction.OpCode == OpCode.OpGetGlobal) && (Macros.Contains(Instruction.References[0].Data)))
                {
                    int A = Instruction.A;

                    if ((Chunk.Instructions[InstructionPoint + 1].OpCode == OpCode.OpCall) && (Chunk.Instructions[InstructionPoint + 1].A == Instruction.A))
                    {
                        switch (Instruction.References[0].Data)
                        {
                            case ("Vestra_GETSTACK"):
                                {
                                    Utility.Utility.VoidInstruction(Chunk.Instructions[InstructionPoint + 1]);
                                    Chunk.Instructions.RemoveAt(InstructionPoint + 1);

                                    Utility.Utility.VoidInstruction(Instruction);
                                    Instruction.OpCode = OpCode.OpGetStack;
                                    Instruction.A = A;

                                    break;
                                };
                        };
                    };
                };

                InstructionPoint++;
            };

            Chunk.UpdateMappings();
        }

        public void DoChunks()
        {
            DoChunk(HeadChunk);
        }
    };
};