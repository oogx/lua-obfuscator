using Obfuscator.Bytecode.IR;
using Obfuscator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Obfuscation.Security
{
    public class InstructionSwap
    {
        private Random Random = new Random();

        public ObfuscationContext ObfuscationContext;
        public Chunk HeadChunk;

        public InstructionSwap(ObfuscationContext ObfuscationContext, Chunk HeadChunk)
        {
            this.ObfuscationContext = ObfuscationContext; this.HeadChunk = HeadChunk;
        }

        public void DoChunk(Chunk Chunk)
        {
            Chunk.UpdateMappings();

            foreach (Chunk SubChunk in Chunk.Chunks) { DoChunk(SubChunk); }

            List<Instruction> Instructions = Chunk.Instructions.ToList();

            int InstructionPoint = 0;

            List<Instruction> IgnoredInstructions = new List<Instruction>();

            while (InstructionPoint < Instructions.Count)
            {
                Instruction Instruction = Instructions[InstructionPoint];
                InstructionPoint++;

                switch (Instruction.OpCode)
                {
                    case (OpCode.OpEq):
                    case (OpCode.OpLt):
                    case (OpCode.OpLe):
                    case (OpCode.OpTest):
                    case (OpCode.OpTestSet):
                    case (OpCode.OpForLoop):
                    case (OpCode.OpTForLoop):
                    case (OpCode.OpForPrep):
                    case (OpCode.OpJump):
                        {
                            IgnoredInstructions.Add(Instruction);

                            continue;
                        };
                };

                if ((Instruction.IsJump) || (Instruction.BackReferences.Count > 0) || (InstructionPoint == 1) || (InstructionPoint == (Chunk.Instructions.Count)))
                    IgnoredInstructions.Add(Instruction);
            };

            List<Instruction> Swapped = new List<Instruction>();

            foreach (Instruction Instruction in Instructions)
            {
                if (!IgnoredInstructions.Contains(Instruction))
                {
                    Instruction PreviousInstruction = Chunk.Instructions[Chunk.InstructionMap[Instruction] - 1];
                    Instruction NextInstruction = Chunk.Instructions[Chunk.InstructionMap[Instruction] + 1];

                    if (IgnoredInstructions.Contains(PreviousInstruction)) { continue; };
                    if (IgnoredInstructions.Contains(NextInstruction)) { continue; };

                    PreviousInstruction.IsJump = true;
                    PreviousInstruction.JumpTo = Instruction;
                    Instruction.BackReferences.Add(PreviousInstruction);

                    Instruction.IsJump = true;
                    Instruction.JumpTo = NextInstruction;
                    NextInstruction.BackReferences.Add(Instruction);

                    Swapped.Add(Instruction);

                    IgnoredInstructions.Add(Instruction);
                    IgnoredInstructions.Add(PreviousInstruction);
                    IgnoredInstructions.Add(NextInstruction);
                };
            };

            Swapped.Shuffle();

            foreach (Instruction Instruction in Swapped) { Chunk.Instructions.Remove(Instruction); };

            Chunk.Instructions.AddRange(Swapped);

            Chunk.UpdateMappings();
        }

        public void DoChunks()
        {
            DoChunk(HeadChunk);
        }
    };
};