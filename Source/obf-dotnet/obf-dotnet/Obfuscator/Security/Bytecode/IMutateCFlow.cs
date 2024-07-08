using Obfuscator.Bytecode.IR;
using Obfuscator.Obfuscation.OpCodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Obfuscation.Security
{
    public class IMutateCFlow
    {
        private Random Random = new Random();

        private Chunk Chunk;
        private List<Instruction> Instructions;
        private List<VOpCode> Virtuals;
        private ObfuscationContext ObfuscationContext;

        private int Start;
        private int End;

        public IMutateCFlow(ObfuscationContext ObfuscationContext, Chunk Chunk, List<Instruction> Instructions, List<VOpCode> Virtuals, int Start, int End)
        {
            this.ObfuscationContext = ObfuscationContext; this.Chunk = Chunk; this.Instructions = Instructions; this.Virtuals = Virtuals; this.Start = Start; this.End = End;
        }

        public void DoInstructions()
        {
            OpCustom SetVMKey = new OpCustom();
            SetVMKey.Obfuscated = $"VMKey = Instruction[OP_A];";
            Virtuals.Add(SetVMKey);

            OpCustom Virtual = new OpCustom();
            Virtuals.Add(Virtual);

            List<BasicBlock> BasicBlocks = new BasicBlock().GenerateBasicBlocksFromInstructions(Chunk, Instructions);

            List<BasicBlock> ProcessedBlocks = new List<BasicBlock>();

            foreach (BasicBlock Block in BasicBlocks)
            {
                if (Block.References.Count > 0)
                {
                    bool Continue = true;

                    foreach (BasicBlock Reference in Block.References)
                        if (Reference.BackReferences.Count != 1)
                            Continue = false;

                    if (!Continue)
                        continue;

                    int Key = Random.Next(0, 256);

                    Block.Instructions.Insert(Block.Instructions.Count - 1, new Instruction(Chunk, OpCode.Custom) { A = Key, CustomInstructionData = new CustomInstructionData { OpCode = SetVMKey } });

                    foreach (BasicBlock Reference in Block.References)
                    {
                        List<Instruction> InstructionList = Reference.Instructions.ToList();

                        foreach (Instruction Instruction in InstructionList)
                        {
                            bool References = false;
                            foreach (dynamic iReference in Instruction.References) { if (iReference != null) { References = true; }; };
                            if (References) { continue; };

                            if (Instruction.IsConstantA) { continue; };
                            if (Instruction.IsConstantB) { continue; };
                            if (Instruction.IsConstantC) { continue; };

                            if (Instruction.IgnoreInstruction) { continue; };

                            if (Instruction == Reference.Instructions.First()) { continue; };

                            if (!object.ReferenceEquals(Instruction.RegisterHandler, null)) { continue; };

                            Instruction.IgnoreInstruction = true;

                            Instruction rInstruction = new Instruction(Chunk, OpCode.Custom);
                            rInstruction.IgnoreInstruction = true;
                            rInstruction.CustomInstructionData.OpCode = Virtual;
                            rInstruction.References[1] = Instruction;
                            rInstruction.InstructionType = InstructionType.ABx;

                            Reference.Instructions.Insert(1, rInstruction);

                            rInstruction.RegisterHandler = delegate (Instruction Self)
                            {
                                rInstruction.B = Chunk.InstructionMap[Instruction];

                                ObfuscationContext.InstructionMapping.TryGetValue(OpCode.None, out VOpCode NoOpCode);

                                Virtual.Obfuscated = $"local oInstruction = Instructions[Instruction[OP_B]]; oInstruction[OP_ENUM] = BitXOR(oInstruction[OP_ENUM], VMKey); Instruction[OP_ENUM] = ({Utility.Utility.IntegerToString(NoOpCode.VIndex)});";

                                rInstruction.InstructionType = InstructionType.ABx;

                                return (rInstruction);
                            };

                            Instruction.RegisterHandler = delegate (Instruction _)
                            {
                                Instruction.WrittenVIndex = Instruction.CustomInstructionData.OpCode.VIndex ^ Key;

                                return (Instruction);
                            };
                        };
                    };
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

            End = Chunk.InstructionMap[BasicBlocks.Last().Instructions.Last()];
            List<Instruction> IList = Chunk.Instructions.Skip(Start).Take(End - Start).ToList();
        }
    };
};