using Obfuscator.Bytecode.IR;
using Obfuscator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Obfuscator.Bytecode
{
    public class Deserializer
    {
        private MemoryStream MemoryStream;

        private bool Endian;
        private byte SizeNumber;
        private byte SizeSizeT;

        private bool ExpectingSetListData;

        private const bool BYTECODE_OPTIMIZATIONS = false;

        private static Encoding LuaEncoding = Encoding.GetEncoding(28591);

        public static Dictionary<OpCode, InstructionType> InstructionMappings = new Dictionary<OpCode, InstructionType>()
        {
            { OpCode.OpMove       , InstructionType.ABC  },
            { OpCode.OpLoadK      , InstructionType.ABx  },
            { OpCode.OpLoadBool   , InstructionType.ABC  },
            { OpCode.OpLoadNil    , InstructionType.ABC  },
            { OpCode.OpGetUpValue , InstructionType.ABC  },
            { OpCode.OpGetGlobal  , InstructionType.ABx  },
            { OpCode.OpGetTable   , InstructionType.ABC  },
            { OpCode.OpSetGlobal  , InstructionType.ABx  },
            { OpCode.OpSetUpValue , InstructionType.ABC  },
            { OpCode.OpSetTable   , InstructionType.ABC  },
            { OpCode.OpNewTable   , InstructionType.ABC  },
            { OpCode.OpSelf       , InstructionType.ABC  },
            { OpCode.OpAdd        , InstructionType.ABC  },
            { OpCode.OpSub        , InstructionType.ABC  },
            { OpCode.OpMul        , InstructionType.ABC  },
            { OpCode.OpDiv        , InstructionType.ABC  },
            { OpCode.OpMod        , InstructionType.ABC  },
            { OpCode.OpPow        , InstructionType.ABC  },
            { OpCode.OpUnm        , InstructionType.ABC  },
            { OpCode.OpNot        , InstructionType.ABC  },
            { OpCode.OpLen        , InstructionType.ABC  },
            { OpCode.OpConcat     , InstructionType.ABC  },
            { OpCode.OpJump        , InstructionType.AsBx },
            { OpCode.OpEq         , InstructionType.ABC  },
            { OpCode.OpLt         , InstructionType.ABC  },
            { OpCode.OpLe         , InstructionType.ABC  },
            { OpCode.OpTest       , InstructionType.ABC  },
            { OpCode.OpTestSet    , InstructionType.ABC  },
            { OpCode.OpCall       , InstructionType.ABC  },
            { OpCode.OpTailCall   , InstructionType.ABC  },
            { OpCode.OpReturn     , InstructionType.ABC  },
            { OpCode.OpForLoop    , InstructionType.AsBx },
            { OpCode.OpForPrep    , InstructionType.AsBx },
            { OpCode.OpTForLoop   , InstructionType.ABC  },
            { OpCode.OpSetList    , InstructionType.ABC  },
            { OpCode.OpClose      , InstructionType.ABC  },
            { OpCode.OpClosure    , InstructionType.ABx  },
            { OpCode.OpVarArg     , InstructionType.ABC  }
        };

        public Deserializer(byte[] Input) => MemoryStream = new MemoryStream(Input);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Read(int Size, bool FactorEndianness = true) { byte[] Bytes = new byte[Size]; MemoryStream.Read(Bytes, 0, Size); if (FactorEndianness && (Endian == BitConverter.IsLittleEndian)) { Bytes = Bytes.Reverse().ToArray(); } return (Bytes); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadSizeT() => SizeSizeT == 4 ? ReadInt32() : ReadInt64();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64() => BitConverter.ToInt64(Read(8), 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32(bool FactorEndianness = true) => BitConverter.ToInt32(Read(4, FactorEndianness), 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() => Read(1)[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString() { int Count = (int)ReadSizeT(); if (Count == 0) return (""); byte[] Value = Read(Count, false); return LuaEncoding.GetString(Value, 0, Count - 1); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble() => BitConverter.ToDouble(Read(SizeNumber), 0);

        public Instruction DecodeInstruction(Chunk Chunk)
        {
            int Code = ReadInt32();

            Instruction Instruction = new Instruction(Chunk, (OpCode)(Code & 0x3F));
            Instruction.Data = Code;

            if (this.ExpectingSetListData) { this.ExpectingSetListData = (false); Instruction.InstructionType = InstructionType.Data; return (Instruction); };

            Instruction.A = (Code >> 6) & 0xFF;

            switch (Instruction.InstructionType)
            {
                case (InstructionType.ABC): { Instruction.B = (Code >> 6 + 8 + 9) & 0x1FF; Instruction.C = (Code >> 6 + 8) & 0x1FF; break; }
                case (InstructionType.ABx): { Instruction.B = (Code >> 6 + 8) & 0x3FFFF; Instruction.C = (0); break; };
                case (InstructionType.AsBx): { Instruction.B = ((Code >> 6 + 8) & 0x3FFFF) - 131071; Instruction.C = (0); break; };
            };

            if ((Instruction.OpCode == OpCode.OpSetList) && (Instruction.C == 0)) { this.ExpectingSetListData = (true); };

            return (Instruction);
        }

        public void DecodeInstructions(Chunk Chunk)
        {
            List<Instruction> Instructions = new List<Instruction>();
            Dictionary<Instruction, int> InstructionMap = new Dictionary<Instruction, int>();

            int InstructionCount = ReadInt32();

            for (int I = 0; I < InstructionCount; I++)
            {
                Instruction Instruction = DecodeInstruction(Chunk);
                Instructions.Add(Instruction);
                InstructionMap.Add(Instruction, I);
            };

            Chunk.Instructions = Instructions;
            Chunk.InstructionMap = InstructionMap;
        }

        public Constant DecodeConstant()
        {
            Constant Constant = new Constant();
            byte Type = ReadByte();

            switch (Type)
            {
                case (0): { Constant.Type = ConstantType.Nil; Constant.Data = (null); break; };
                case (1): { Constant.Type = ConstantType.Boolean; Constant.Data = (ReadByte() != 0); break; };
                case (3): { Constant.Type = ConstantType.Number; Constant.Data = ReadDouble(); break; };
                case (4): { Constant.Type = ConstantType.String; Constant.Data = ReadString(); break; };
            };

            return (Constant);
        }

        public void DecodeConstants(Chunk Chunk)
        {
            List<Constant> Constants = new List<Constant>();
            Dictionary<Constant, int> ConstantMap = new Dictionary<Constant, int>();

            int ConstantCount = ReadInt32();

            for (int I = 0; I < ConstantCount; I++)
            {
                Constant Constant = DecodeConstant();
                Constants.Add(Constant);
                ConstantMap.Add(Constant, I);
            };

            Chunk.Constants = Constants;
            Chunk.ConstantMap = ConstantMap;
        }

        public Chunk DecodeChunk()
        {
            Chunk Chunk = new Chunk { Name = ReadString(), Line = ReadInt32(), LastLine = ReadInt32(), UpValueCount = ReadByte(), ParameterCount = ReadByte(), VarArgFlag = ReadByte(), StackSize = ReadByte(), UpValues = new List<string>() };

            DecodeInstructions(Chunk);
            DecodeConstants(Chunk);
            DecodeChunks(Chunk);

            int Count = ReadInt32(); for (int I = 0; I < Count; I++) { Chunk.Instructions[I].Line = ReadInt32(); };
            Count = ReadInt32(); for (int I = 0; I < Count; I++) { ReadString(); ReadInt32(); ReadInt32(); };
            Count = ReadInt32(); for (int I = 0; I < Count; I++) { Chunk.UpValues.Add(ReadString()); };

            foreach (Instruction Instruction in Chunk.Instructions)
                Instruction.SetupReferences();

            return (Chunk);
        }

        public void DecodeChunks(Chunk Chunk)
        {
            List<Chunk> Chunks = new List<Chunk>();
            Dictionary<Chunk, int> ChunkMap = new Dictionary<Chunk, int>();

            int ChunkCount = ReadInt32();

            for (int I = 0; I < ChunkCount; I++) { Chunk SubChunk = DecodeChunk(); Chunks.Add(SubChunk); ChunkMap.Add(SubChunk, I); };

            Chunk.Chunks = Chunks;
            Chunk.ChunkMap = ChunkMap;
        }

        public Chunk DecodeFile()
        {
            int Header = ReadInt32();

            if (Header != 0x1B4C7561 && Header != 0x61754C1B) { throw new Exception("Obfuscation Error: Invalid LuaC File."); };
            if (ReadByte() != 0x51) { throw new Exception("Obfuscation Error: Only Lua 5.1 is Supported."); };

            ReadByte(); Endian = ReadByte() == 0;

            ReadByte(); SizeSizeT = ReadByte();

            ReadByte(); SizeNumber = ReadByte();

            ReadByte();

            Chunk HeadChunk = DecodeChunk();

            // Removes Unneccessary JMPs After EQ/LT/LE/TEST/TESTSET/TFORLOOP:
            void RemoveJumps(Chunk Chunk)
            {
                int InstructionPoint = 0;
                while (InstructionPoint < Chunk.Instructions.Count)
                {
                    Instruction Instruction = Chunk.Instructions[InstructionPoint];

                    switch (Instruction.OpCode)
                    {
                        case (OpCode.OpEq):
                        case (OpCode.OpLt):
                        case (OpCode.OpLe):
                        case (OpCode.OpTest):
                        case (OpCode.OpTestSet):
                        case (OpCode.OpTForLoop):
                            {
                                Utility.Utility.VoidInstruction(Chunk.Instructions[InstructionPoint + 1]);
                                Chunk.Instructions.RemoveAt(InstructionPoint + 1);

                                break;
                            };
                    };

                    InstructionPoint++;
                };

                foreach (Chunk SubChunk in Chunk.Chunks)
                    RemoveJumps(SubChunk);

                Chunk.UpdateMappings();
            }; RemoveJumps(HeadChunk);

            // Changes how the 'Closure' Instruction Works:
            void EditClosure(Chunk Chunk)
            {
                int InstructionPoint = 0;
                while (InstructionPoint < Chunk.Instructions.Count)
                {
                    Instruction Instruction = Chunk.Instructions[InstructionPoint];

                    if ((Instruction.OpCode == OpCode.OpClosure) && (Instruction.References[0].UpValueCount > 0))
                    {
                        Instruction.CustomData = new List<int[]>();
                        for (int I = 1; I <= Instruction.References[0].UpValueCount; I++)
                        {
                            Instruction UpValue = Chunk.Instructions[InstructionPoint + I];
                            Instruction.CustomData.Add(new int[] { UpValue.OpCode == OpCode.OpMove ? 0 : 1, UpValue.B });
                        };

                        for (int I = 1; I <= Instruction.References[0].UpValueCount; I++)
                            Chunk.Instructions.RemoveAt(InstructionPoint + 1);
                    };

                    InstructionPoint++;
                };

                foreach (Chunk SubChunk in Chunk.Chunks)
                    EditClosure(SubChunk);

                Chunk.UpdateMappings();
            }; EditClosure(HeadChunk);

            // Update & Shuffle Chunk:
            void UpdateChunk(Chunk Chunk)
            {
                foreach (Chunk SubChunk in Chunk.Chunks) { UpdateChunk(SubChunk); };

                Chunk.Constants.Shuffle();

                Chunk.UpdateMappings();

                foreach (Instruction Instruction in Chunk.Instructions)
                    Instruction.UpdateRegisters();
            }; UpdateChunk(HeadChunk);

            // Repair JMPs for EQ/LT/LE/TEST/TESTSET/TFORLOOP:
            void FixJumps(Chunk Chunk)
            {
                int InstructionPoint = 0;
                while (InstructionPoint < Chunk.Instructions.Count)
                {
                    Instruction Instruction = Chunk.Instructions[InstructionPoint];

                    switch (Instruction.OpCode)
                    {
                        case (OpCode.OpEq):
                        case (OpCode.OpLt):
                        case (OpCode.OpLe):
                        case (OpCode.OpTest):
                        case (OpCode.OpTestSet):
                        case (OpCode.OpTForLoop):
                        case (OpCode.OpForLoop):
                        case (OpCode.OpForPrep):
                            {
                                Instruction.IsJump = true;
                                Instruction.JumpTo = Chunk.Instructions[InstructionPoint + 1];
                                Instruction.JumpTo.BackReferences.Add(Instruction);

                                break;
                            };
                        case OpCode.OpLoadBool when (Instruction.C == 1):
                            {
                                Instruction.IsJump = true;
                                Instruction.JumpTo = Chunk.Instructions[InstructionPoint + 2];
                                Instruction.JumpTo.BackReferences.Add(Instruction);

                                break;
                            };
                    };

                    InstructionPoint++;
                };

                foreach (Chunk SubChunk in Chunk.Chunks)
                    FixJumps(SubChunk);

                Chunk.UpdateMappings();
            }; FixJumps(HeadChunk);

            UpdateChunk(HeadChunk);

            return (HeadChunk);
        }
    };
};