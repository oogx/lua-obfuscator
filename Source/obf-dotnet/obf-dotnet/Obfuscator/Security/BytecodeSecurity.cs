using Obfuscator.Bytecode.IR;
using Obfuscator.Obfuscation.OpCodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Obfuscation.Security
{
    public class BytecodeSecurity
    {
        private static List<string> Macros = new List<string> { "PSU_MAX_SECURITY_START", "PSU_MAX_SECURITY_END" };

        private Random Random = new Random();

        private Chunk HeadChunk;
        private ObfuscationSettings ObfuscationSettings;
        private ObfuscationContext ObfuscationContext;
        private List<VOpCode> Virtuals;

        public BytecodeSecurity(Chunk HeadChunk, ObfuscationSettings ObfuscationSettings, ObfuscationContext ObfuscationContext, List<VOpCode> Virtuals)
        {
            this.Virtuals = Virtuals; this.HeadChunk = HeadChunk; this.ObfuscationSettings = ObfuscationSettings; this.ObfuscationContext = ObfuscationContext;
        }

        public void DoChunk(Chunk Chunk)
        {
            foreach (Chunk SubChunk in Chunk.Chunks) { DoChunk(SubChunk); };

            bool ByteCodeSecurity = (false);

            Instruction Begin = (null);

            int InstructionPoint = 0;

            while (InstructionPoint < Chunk.Instructions.Count)
            {
                Instruction Instruction = Chunk.Instructions[InstructionPoint];
                if ((Instruction.OpCode == OpCode.OpGetGlobal) && (Macros.Contains(Instruction.References[0].Data)))
                {
                    int A = Instruction.A;

                    if ((Chunk.Instructions[InstructionPoint + 1].OpCode == OpCode.OpCall) && (Chunk.Instructions[InstructionPoint + 1].A == Instruction.A))
                    {
                        string Global = (string)Instruction.References[0].Data;

                        Utility.Utility.VoidInstruction(Chunk.Instructions[InstructionPoint + 1]);
                        Utility.Utility.VoidInstruction(Instruction);

                        switch (Global)
                        {
                            case "PSU_MAX_SECURITY_START" when (!ByteCodeSecurity):
                                {
                                    Begin = Chunk.Instructions[InstructionPoint + 2];
                                    ByteCodeSecurity = (true);

                                    break;
                                };

                            case "PSU_MAX_SECURITY_END" when (ByteCodeSecurity):
                                {
                                    Chunk.UpdateMappings();

                                    if (Begin == Instruction) { Begin = null; ByteCodeSecurity = false; break; }

                                    Instruction = Chunk.Instructions[InstructionPoint];

                                    int Start = Chunk.InstructionMap[Begin];
                                    int End = Chunk.InstructionMap[Instruction] + 1;
                                    List<Instruction> InstructionList = Chunk.Instructions.Skip(Start).Take(End - Start).ToList();

                                    (new RegisterMutation(ObfuscationContext, Chunk, InstructionList, Virtuals, Start, End)).DoInstructions();

                                    Start = Chunk.InstructionMap[Begin];
                                    End = Chunk.InstructionMap[Instruction] + 1;
                                    InstructionList = Chunk.Instructions.Skip(Start).Take(End - Start).ToList();

                                    (new TestSpam(ObfuscationContext, Chunk, InstructionList, Virtuals, Start, End)).DoInstructions();

                                    Start = Chunk.InstructionMap[Begin];
                                    End = Chunk.InstructionMap[Instruction] + 1;
                                    InstructionList = Chunk.Instructions.Skip(Start).Take(End - Start).ToList();
                                    break;


                                };
                        };
                    };
                };
                InstructionPoint++;
            };

            // Chunk.Instructions.Insert(0, new Instruction(Chunk, OpCode.OpNewStack)); -- will error 
            Chunk.Instructions.Insert(0, new Instruction(Chunk, OpCode.None));

            Chunk.UpdateMappings();
        }

        public void DoChunks()
        {
            DoChunk(HeadChunk);
        }
    };
};