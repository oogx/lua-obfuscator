using Obfuscator.Bytecode.IR;
using Obfuscator.Extensions;
using Obfuscator.Obfuscation.OpCodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Obfuscation.Security
{
    public class RegisterMutation
    {
        private Random Random = new Random();

        private Chunk Chunk;
        private List<Instruction> Instructions;
        private List<VOpCode> Virtuals;
        private ObfuscationContext ObfuscationContext;

        private int Start;
        private int End;

        public RegisterMutation(ObfuscationContext ObfuscationContext, Chunk Chunk, List<Instruction> Instructions, List<VOpCode> Virtuals, int Start, int End)
        {
            this.ObfuscationContext = ObfuscationContext; this.Chunk = Chunk; this.Instructions = Instructions; this.Virtuals = Virtuals; this.Start = Start; this.End = End;
        }

        public void DoInstructions()
        {
            List<BasicBlock> BasicBlocks = new BasicBlock().GenerateBasicBlocksFromInstructions(Chunk, Instructions);

            int Enum = Random.Next(0, 255);

            int A = Random.Next(0, 255);
            int B = Random.Next(0, 255);
            int C = Random.Next(0, 255);

            long nA = ObfuscationContext.NumberEquations.Keys.ToList().Random();
            long nB = ObfuscationContext.NumberEquations.Keys.ToList().Random();
            long nC = ObfuscationContext.NumberEquations.Keys.ToList().Random();

            int iA = Random.Next(0, 2);
            int iB = Random.Next(0, 2);
            int iC = Random.Next(0, 2);

            OpCustom Virtual = new OpCustom();
            Virtuals.Add(Virtual);

            foreach (BasicBlock Block in BasicBlocks)
            {
                List<Instruction> InstructionList = Block.Instructions.ToList();

                foreach (Instruction Instruction in InstructionList)
                {
                    bool References = false;
                    foreach (dynamic Reference in Instruction.References) { if (Reference != null) { References = true; }; };
                    if (References) { continue; };

                    if (Instruction.IsConstantA) { continue; };
                    if (Instruction.IsConstantB) { continue; };
                    if (Instruction.IsConstantC) { continue; };

                    if (Instruction.IgnoreInstruction) { continue; };

                    if (Instruction == Block.Instructions.First()) { continue; };

                    if (!object.ReferenceEquals(Instruction.RegisterHandler, null)) { continue; };

                    if (Random.Next(1, 5) != 0) { continue; };

                    Instruction.IgnoreInstruction = true;

                    Instruction rInstruction = new Instruction(Chunk, OpCode.Custom);
                    rInstruction.IgnoreInstruction = true;
                    rInstruction.CustomInstructionData.OpCode = Virtual;
                    rInstruction.References[1] = Instruction;
                    rInstruction.RequiresCustomData = true;
                    rInstruction.InstructionType = InstructionType.ABx;

                    Block.Instructions.Insert(1, rInstruction);

                    rInstruction.RegisterHandler = delegate (Instruction Self)
                    {
                        rInstruction.B = Chunk.InstructionMap[Instruction];

                        ObfuscationContext.InstructionMapping.TryGetValue(OpCode.None, out VOpCode NoOpCode);

                        rInstruction.RequiresCustomData = true;

                        rInstruction.CustomData = new List<int>
                        {
                            Instruction.CustomInstructionData.OpCode.VIndex ^ Enum,
                            (iA == 0) ? Instruction.A ^ A : (int)ObfuscationContext.NumberEquations[nA].ComputeExpression(Instruction.A),
                            (iB == 0) ? Instruction.B ^ B : (int)ObfuscationContext.NumberEquations[nB].ComputeExpression(Instruction.B),
                            (iC == 0) ? Instruction.C ^ C : (int)ObfuscationContext.NumberEquations[nC].ComputeExpression(Instruction.C)
                        };

                        Virtual.Obfuscated = $@"

						local oInstruction = Instructions[Instruction[OP_B]];
						local D = Instruction[OP_D];

						{String.Join("\n", (new List<string> {
                            $"oInstruction[OP_ENUM] = BitXOR(D[{Utility.Utility.IntegerToString(1, 2)}], {Utility.Utility.IntegerToString(Enum, 2)});",
                            $"oInstruction[OP_A] = {((iA == 0) ? $"BitXOR(D[{Utility.Utility.IntegerToString(2, 2)}], {Utility.Utility.IntegerToString(A, 2)});" : $"CalculateVM({nA}, D[{Utility.Utility.IntegerToString(2, 2)}])")}",
                            $"oInstruction[OP_B] = {((iB == 0) ? $"BitXOR(D[{Utility.Utility.IntegerToString(3, 2)}], {Utility.Utility.IntegerToString(B, 2)});" : $"CalculateVM({nB}, D[{Utility.Utility.IntegerToString(3, 2)}])")}",
                            $"oInstruction[OP_C] = {((iC == 0) ? $"BitXOR(D[{Utility.Utility.IntegerToString(4, 2)}], {Utility.Utility.IntegerToString(C, 2)});" : $"CalculateVM({nC}, D[{Utility.Utility.IntegerToString(4, 2)}])")}"
                        }).Shuffle())}

						Instruction[OP_ENUM] = ({Utility.Utility.IntegerToString(NoOpCode.VIndex)});

						";

                        rInstruction.InstructionType = InstructionType.ABx;

                        return (rInstruction);
                    };

                    Instruction.RegisterHandler = delegate (Instruction _)
                    {
                        Instruction.WrittenVIndex = 0;

                        Instruction.A = 0;
                        Instruction.B = 0;
                        Instruction.C = 0;

                        return (Instruction);
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

            (new IMutateCFlow(ObfuscationContext, Chunk, IList, Virtuals, Start, End)).DoInstructions();
        }
    };
};