using Obfuscator.Bytecode.IR;
using Obfuscator.Obfuscation.OpCodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Obfuscation.Security
{
    public class TestRemove
    {
        private Random Random = new Random();

        private Chunk Chunk;
        private List<Instruction> Instructions;
        private List<VOpCode> Virtuals;
        private ObfuscationContext ObfuscationContext;

        private int Start;
        private int End;

        public TestRemove(ObfuscationContext ObfuscationContext, Chunk Chunk, List<Instruction> Instructions, List<VOpCode> Virtuals, int Start, int End)
        {
            this.ObfuscationContext = ObfuscationContext; this.Chunk = Chunk; this.Instructions = Instructions; this.Virtuals = Virtuals; this.Start = Start; this.End = End;
        }

        public void DoInstructions()
        {
            List<BasicBlock> BasicBlocks = new BasicBlock().GenerateBasicBlocksFromInstructions(Chunk, Instructions);

            Constant True = Utility.Utility.GetOrAddConstant(Chunk, ConstantType.Boolean, true, out int TrueIndex);
            Constant False = Utility.Utility.GetOrAddConstant(Chunk, ConstantType.Boolean, false, out int FalseIndex);

            foreach (BasicBlock Block in BasicBlocks)
            {
                int InstructionPoint = 0;

                while (InstructionPoint < Block.Instructions.Count)
                {
                    Instruction Instruction = Block.Instructions[InstructionPoint];

                    if (Instruction.OpCode == OpCode.OpTest)
                    {
                        if (Instruction.C == 1)
                        {
                            Instruction L = Instruction.References[2];
                            Instruction R = Instruction.JumpTo;

                            L.BackReferences.Remove(Instruction);
                            R.BackReferences.Remove(Instruction);

                            Instruction.OpCode = OpCode.OpNot;
                            Instruction.B = Instruction.A;
                            Instruction.A = Chunk.StackSize + 1;
                            Instruction.C = 0;
                            Instruction.IsJump = false;
                            Instruction.JumpTo = null;

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 1, new Instruction(Chunk, OpCode.OpNot) { A = Chunk.StackSize + 1, B = Chunk.StackSize + 1 });
                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 2, new Instruction(Chunk, OpCode.OpNewTable) { A = Chunk.StackSize + 2 });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 3, new Instruction(Chunk, OpCode.OpLoadJump, L) { A = Chunk.StackSize + 3 });
                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 4, new Instruction(Chunk, OpCode.OpLoadJump, R) { A = Chunk.StackSize + 4 });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 5, new Instruction(Chunk, OpCode.OpSetTable, True) { A = Chunk.StackSize + 2, B = TrueIndex, C = Chunk.StackSize + 3, IsConstantB = true });
                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 6, new Instruction(Chunk, OpCode.OpSetTable, False) { A = Chunk.StackSize + 2, B = FalseIndex, C = Chunk.StackSize + 4, IsConstantB = true });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 7, new Instruction(Chunk, OpCode.OpGetTable) { A = Chunk.StackSize + 1, B = Chunk.StackSize + 2, C = Chunk.StackSize + 1 });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 8, new Instruction(Chunk, OpCode.OpDynamicJump) { A = Chunk.StackSize + 1 });

                            InstructionPoint += 8;
                        }
                        else
                        {
                            Instruction L = Instruction.References[2];
                            Instruction R = Instruction.JumpTo;

                            L.BackReferences.Remove(Instruction);
                            R.BackReferences.Remove(Instruction);

                            Instruction.OpCode = OpCode.OpNot;
                            Instruction.B = Instruction.A;
                            Instruction.A = Chunk.StackSize + 1;
                            Instruction.C = 0;
                            Instruction.IsJump = false;
                            Instruction.JumpTo = null;

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 1, new Instruction(Chunk, OpCode.OpNot) { A = Chunk.StackSize + 1, B = Chunk.StackSize + 1 });
                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 2, new Instruction(Chunk, OpCode.OpNewTable) { A = Chunk.StackSize + 2 });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 3, new Instruction(Chunk, OpCode.OpLoadJump, L) { A = Chunk.StackSize + 3 });
                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 4, new Instruction(Chunk, OpCode.OpLoadJump, R) { A = Chunk.StackSize + 4 });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 5, new Instruction(Chunk, OpCode.OpSetTable, False) { A = Chunk.StackSize + 2, B = FalseIndex, C = Chunk.StackSize + 3, IsConstantB = true });
                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 6, new Instruction(Chunk, OpCode.OpSetTable, True) { A = Chunk.StackSize + 2, B = TrueIndex, C = Chunk.StackSize + 4, IsConstantB = true });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 7, new Instruction(Chunk, OpCode.OpGetTable) { A = Chunk.StackSize + 1, B = Chunk.StackSize + 2, C = Chunk.StackSize + 1 });

                            Block.Instructions.Insert(Block.Instructions.IndexOf(Instruction) + 8, new Instruction(Chunk, OpCode.OpDynamicJump) { A = Chunk.StackSize + 1 });

                            InstructionPoint += 8;
                        };
                    };

                    InstructionPoint++;
                };
            };

            List<Instruction> Before = Chunk.Instructions.Take(Start).ToList();
            List<Instruction> After = Chunk.Instructions.Skip(End).ToList();

            Chunk.Instructions.Clear();

            Chunk.Instructions.AddRange(Before);

            foreach (BasicBlock Block in BasicBlocks)
            {
                foreach (Instruction Instruction in Block.Instructions)
                {
                    Chunk.Instructions.Add(Instruction);
                };
            };

            Chunk.Instructions.AddRange(After);

            Chunk.UpdateMappings();
        }
    };
};