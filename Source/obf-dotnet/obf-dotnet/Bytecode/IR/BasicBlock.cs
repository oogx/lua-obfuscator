using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Bytecode.IR
{
    public class BasicBlock
    {
        public List<Instruction> Instructions = new List<Instruction>();
        public List<BasicBlock> References = new List<BasicBlock>();
        public List<BasicBlock> BackReferences = new List<BasicBlock>();

        public List<BasicBlock> GenerateBasicBlocks(Chunk Chunk)
        {
            Random Random = new Random();

            List<BasicBlock> BasicBlocks = new List<BasicBlock>();
            int InstructionPoint = 0;

            BasicBlock BasicBlock = null;

            Dictionary<int, BasicBlock> BlockMap = new Dictionary<int, BasicBlock>();

            while (InstructionPoint < Chunk.Instructions.Count)
            {
                Instruction Instruction = Chunk.Instructions[InstructionPoint];

                if (Instruction.BackReferences.Count > 0)
                    BasicBlock = null;

                if (BasicBlock == null)
                {
                    BasicBlock = new BasicBlock();
                    BasicBlocks.Add(BasicBlock);
                };

                BasicBlock.Instructions.Add(Instruction);
                BlockMap[InstructionPoint] = BasicBlock;

                switch (Instruction.OpCode)
                {
                    case (OpCode.OpEq):
                    case (OpCode.OpLt):
                    case (OpCode.OpLe):
                    case (OpCode.OpTest):
                    case (OpCode.OpTestSet):
                    case (OpCode.OpJump):
                    case (OpCode.OpForLoop):
                    case (OpCode.OpForPrep):
                    case (OpCode.OpTForLoop):
                    case (OpCode.OpReturn):
                        {
                            BasicBlock = null;

                            break;
                        };
                };

                if (Instruction.IsJump)
                    BasicBlock = null;

                InstructionPoint++;
            };

            BasicBlocks.First().BackReferences.Add(new BasicBlock());

            foreach (BasicBlock Block in BasicBlocks)
            {
                if (Block.Instructions.Count == 0) { continue; }; // ???

                Instruction Instruction = Block.Instructions.Last();

                switch (Instruction.OpCode)
                {
                    case (OpCode.OpForPrep):
                    case (OpCode.OpForLoop):
                        {
                            Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction] + 1]);
                            Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction.References[0]]]);

                            break;
                        };

                    case (OpCode.OpJump): { Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction.References[0]]]); break; };

                    case (OpCode.OpEq):
                    case (OpCode.OpLt):
                    case (OpCode.OpLe):
                    case (OpCode.OpTest):
                    case (OpCode.OpTestSet):
                    case (OpCode.OpTForLoop):
                        {
                            Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction] + 1]);
                            Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction.References[2]]]);

                            break;
                        };

                    case (OpCode.OpReturn): { break; };

                    default: { Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction] + 1]); break; };
                };

                foreach (BasicBlock Reference in Block.References)
                    Reference.BackReferences.Add(Block);
            };

            return (BasicBlocks);
        }

        public List<BasicBlock> GenerateBasicBlocksFromInstructions(Chunk Chunk, List<Instruction> Instructions)
        {
            Random Random = new Random();

            List<BasicBlock> BasicBlocks = new List<BasicBlock>();
            int InstructionPoint = 0;

            BasicBlock BasicBlock = null;

            Dictionary<int, BasicBlock> BlockMap = new Dictionary<int, BasicBlock>();

            while (InstructionPoint < Instructions.Count)
            {
                Instruction Instruction = Instructions[InstructionPoint];

                if (Instruction.BackReferences.Count > 0)
                    BasicBlock = null;

                if (BasicBlock == null)
                {
                    BasicBlock = new BasicBlock();
                    BasicBlocks.Add(BasicBlock);
                };

                BasicBlock.Instructions.Add(Instruction);
                BlockMap[InstructionPoint] = BasicBlock;

                switch (Instruction.OpCode)
                {
                    case (OpCode.OpEq):
                    case (OpCode.OpLt):
                    case (OpCode.OpLe):
                    case (OpCode.OpTest):
                    case (OpCode.OpTestSet):
                    case (OpCode.OpJump):
                    case (OpCode.OpForLoop):
                    case (OpCode.OpForPrep):
                    case (OpCode.OpTForLoop):
                    case (OpCode.OpReturn):
                        {
                            BasicBlock = null;

                            break;
                        }
                };

                if (Instruction.IsJump)
                    BasicBlock = null;

                InstructionPoint++;
            };

            foreach (BasicBlock Block in BasicBlocks)
            {
                if (Block.Instructions.Count == 0) { continue; }; // ???

                Instruction Instruction = Block.Instructions.Last();

                switch (Instruction.OpCode)
                {
                    case (OpCode.OpForPrep):
                    case (OpCode.OpForLoop):
                        {
                            if (BlockMap.ContainsKey(Chunk.InstructionMap[Instruction] + 1))
                                Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction] + 1]);

                            if (BlockMap.ContainsKey(Chunk.InstructionMap[Instruction.References[0]]))
                                Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction.References[0]]]);

                            break;
                        };

                    case (OpCode.OpJump):
                        {
                            if (BlockMap.ContainsKey(Chunk.InstructionMap[Instruction.References[0]]))
                                Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction.References[0]]]);

                            break;
                        };

                    case (OpCode.OpEq):
                    case (OpCode.OpLt):
                    case (OpCode.OpLe):
                    case (OpCode.OpTest):
                    case (OpCode.OpTestSet):
                    case (OpCode.OpTForLoop):
                        {
                            if (BlockMap.ContainsKey(Chunk.InstructionMap[Instruction] + 1))
                                Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction] + 1]);

                            if (BlockMap.ContainsKey(Chunk.InstructionMap[Instruction.References[2]]))
                                Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction.References[2]]]);

                            break;
                        };

                    case (OpCode.OpReturn): { break; };

                    default:
                        {
                            if (BlockMap.ContainsKey(Chunk.InstructionMap[Instruction] + 1))
                                Block.References.Add(BlockMap[Chunk.InstructionMap[Instruction] + 1]);

                            break;
                        };
                };

                foreach (BasicBlock Reference in Block.References)
                    Reference.BackReferences.Add(Block);
            };

            return (BasicBlocks);
        }
    };
};