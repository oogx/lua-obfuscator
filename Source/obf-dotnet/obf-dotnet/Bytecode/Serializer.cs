using Obfuscator.Bytecode.IR;
using Obfuscator.Extensions;
using Obfuscator.Obfuscation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Bytecode
{
    public class Serializer
    {
        private ObfuscationContext ObfuscationContext;
        private ObfuscationSettings ObfuscationSettings;

        private Random Random = new Random();
        private Encoding LuaEncoding = Encoding.GetEncoding(28591);

        public Serializer(ObfuscationContext ObfuscationContext, ObfuscationSettings ObfuscationSettings)
        {
            this.ObfuscationContext = ObfuscationContext; this.ObfuscationSettings = ObfuscationSettings;
        }

        public List<string> Types = new List<string>();

        public void SerializeLChunk(Chunk Chunk, List<byte> Bytes)
        {
            void WriteByte(byte Byte) { Bytes.Add((byte)(Byte ^ ObfuscationContext.PrimaryXORKey)); ObfuscationContext.PrimaryXORKey = Byte % 256; }
            void Write(byte[] ToWrite, bool CheckEndian = true) { if (!BitConverter.IsLittleEndian && CheckEndian) { ToWrite = ToWrite.Reverse().ToArray(); }; Bytes.AddRange(ToWrite.Select(Byte => { byte XOR = (byte)(Byte ^ ObfuscationContext.PrimaryXORKey); ObfuscationContext.PrimaryXORKey = Byte % 256; return (XOR); })); }
            void WriteInt32(int Int32) => Write(BitConverter.GetBytes(Int32));
            void WriteInt16(short Int16) => Write(BitConverter.GetBytes(Int16));
            void WriteNumber(double Number) => Write(BitConverter.GetBytes(Number));
            void WriteBool(bool Bool) => Write(BitConverter.GetBytes(Bool));
            void WriteString(string String)
            {
                byte[] sBytes = LuaEncoding.GetBytes(String);
                WriteInt32(sBytes.Length);
                Bytes.AddRange(sBytes.Select(Byte =>
                {
                    byte XOR = (byte)(Byte ^ ObfuscationContext.PrimaryXORKey);
                    ObfuscationContext.PrimaryXORKey = Byte % 256;
                    return (XOR);
                }));
            }

            void WriteStringRaw(string String)
            {
                byte[] sBytes = LuaEncoding.GetBytes(String);
                WriteInt32(sBytes.Length);
                Bytes.AddRange(sBytes);
            }

            void SerializeInstruction(Instruction Instruction)
            {
                if ((!Instruction.CustomInstructionData.Serialize) || (Instruction.InstructionType == InstructionType.Data)) { WriteByte(0); return; };

                var CustomInstructionData = Instruction.CustomInstructionData;
                int OpCode = (int)Instruction.OpCode;

                var VirtualOpcode = CustomInstructionData.OpCode;

                if (!CustomInstructionData.Mutated) { VirtualOpcode?.Mutate(Instruction); };
                CustomInstructionData.Mutated = true;

                if (!object.ReferenceEquals(Instruction.RegisterHandler, null))
                    Instruction = Instruction.RegisterHandler(Instruction);

                int A = Instruction.A;
                int B = Instruction.B;
                int C = Instruction.C;

                bool KA = Instruction.IsConstantA;
                bool KB = Instruction.IsConstantB;
                bool KC = Instruction.IsConstantC;

                int InstructionData = (int)Instruction.InstructionType + 1;

                if (!ObfuscationSettings.ConstantEncryption)
                {
                    InstructionData |= Instruction.IsConstantA ? (int)InstructionConstantType.RA : (int)InstructionConstantType.NK;
                    InstructionData |= Instruction.IsConstantB ? (int)InstructionConstantType.RB : (int)InstructionConstantType.NK;
                    InstructionData |= Instruction.IsConstantC ? (int)InstructionConstantType.RC : (int)InstructionConstantType.NK;
                };

                if (Instruction.RequiresCustomData) { InstructionData |= 0b01000000; };

                if (Instruction.IsJump) { InstructionData |= 0b10000000; };

                OpCode = Instruction.WrittenVIndex ?? VirtualOpcode.VIndex;

                Instruction.A = Instruction.WrittenA ?? Instruction.A;
                Instruction.B = Instruction.WrittenB ?? Instruction.B;
                Instruction.C = Instruction.WrittenC ?? Instruction.C;

                A = Instruction.A;
                B = Instruction.B;
                C = Instruction.C;

                WriteByte((byte)InstructionData);

                foreach (InstructionStep InstructionStep in ObfuscationContext.InstructionSteps)
                {
                    switch (InstructionStep)
                    {
                        case (InstructionStep.Enum): { WriteByte((byte)(OpCode)); break; };

                        case (InstructionStep.A):
                            {
                                switch (Instruction.InstructionType)
                                {
                                    case (InstructionType.ABC): { WriteInt16((short)Instruction.A); break; };
                                    case (InstructionType.ABx): { WriteInt16((short)Instruction.A); break; };
                                    case (InstructionType.AsBx): { WriteInt16((short)Instruction.A); break; };
                                    case (InstructionType.AsBxC): { WriteInt16((short)Instruction.A); break; };
                                    case (InstructionType.Closure): { WriteInt16((short)Instruction.A); break; };
                                };

                                break;
                            };

                        case (InstructionStep.B):
                            {
                                switch (Instruction.InstructionType)
                                {
                                    case (InstructionType.ABC): { WriteInt16((short)Instruction.B); break; };
                                    case (InstructionType.ABx): { WriteInt32((int)Instruction.B); break; };
                                    case (InstructionType.AsBx): { WriteInt32((int)Instruction.B); break; };
                                    case (InstructionType.AsBxC): { WriteInt32((int)Instruction.B); break; };
                                    case (InstructionType.Closure): { WriteInt32((int)Instruction.B); break; };
                                };

                                break;
                            };

                        case (InstructionStep.C):
                            {
                                switch (Instruction.InstructionType)
                                {
                                    case (InstructionType.ABC): { WriteInt16((short)Instruction.C); break; };
                                    case (InstructionType.AsBxC): { WriteInt16((short)Instruction.C); break; };
                                    case (InstructionType.Closure): { WriteInt16((short)Instruction.C); break; };
                                };

                                break;
                            };
                    };
                };

                if (Instruction.InstructionType == InstructionType.Closure)
                {
                    foreach (int[] UpValue in Instruction.CustomData)
                    {
                        WriteByte((byte)UpValue[0]);
                        WriteInt16((short)UpValue[1]);
                    };
                };

                if (Instruction.IsJump)
                    WriteInt32((int)Chunk.InstructionMap[Instruction.JumpTo]);

                if (Instruction.RequiresCustomData)
                {
                    WriteByte((byte)(((List<int>)Instruction.CustomData).Count));
                    foreach (int Value in (List<int>)Instruction.CustomData) { WriteInt32(Value); };
                };
            };

            Chunk.Constants.Shuffle(); Chunk.UpdateMappings();

            foreach (Instruction Instruction in Chunk.Instructions)
            {
                Instruction.UpdateRegisters();

                if ((!Instruction.CustomInstructionData.Serialize) || (Instruction.InstructionType == InstructionType.Data)) { continue; };

                var CustomInstructionData = Instruction.CustomInstructionData;
                var VirtualOpcode = CustomInstructionData.OpCode;

                if (!CustomInstructionData.Mutated) { VirtualOpcode?.Mutate(Instruction); };
                CustomInstructionData.Mutated = true;
            };

            foreach (ChunkStep Step in ObfuscationContext.ChunkSteps)
            {
                switch (Step)
                {
                    case ChunkStep.StackSize: { WriteInt16(Chunk.StackSize); break; };
                    case ChunkStep.ParameterCount: { WriteByte(Chunk.ParameterCount); break; };

                    case ChunkStep.Instructions: // & ChunkStep.Constants
                        {
                            WriteInt32(Chunk.Constants.Count);
                            foreach (Constant Constant in Chunk.Constants)
                            {
                                bool optimizeString = false;

                                if (Constant.Type == ConstantType.String)
                                {
                                    string str = (string)Constant.Data;
                                    if (str.Length >= 100 && str.IndexOf("https://") == -1 && str.IndexOf("http://") == -1)
                                    {
                                        optimizeString = true;
                                    }
                                }

                                if (!optimizeString)
                                    WriteByte((byte)ObfuscationContext.ConstantMapping[(int)Constant.Type]);
                                else
                                    WriteByte((byte)ObfuscationContext.ConstantMapping[(int)ConstantType.FastString]);

                                switch (Constant.Type)
                                {
                                    case ConstantType.Int32:
                                        {
                                            WriteInt32((short)(Constant.Data + (1 << 16)));
                                            break;
                                        };
                                    case ConstantType.Int16:
                                        {
                                            WriteInt16((short)(Constant.Data + (1 << 8)));
                                            break;
                                        };
                                    case ConstantType.Boolean:
                                        {
                                            WriteBool(Constant.Data);
                                            break;
                                        };
                                    case ConstantType.Number:
                                        {
                                            WriteNumber(Constant.Data);
                                            break;
                                        };
                                    case ConstantType.String:
                                        {
                                            if (optimizeString)
                                                WriteStringRaw(Constant.Data);
                                            else
                                                WriteString(Constant.Data);
                                            break;
                                        };
                                };
                            };

                            WriteInt32(Chunk.Instructions.Count);
                            foreach (Instruction Instruction in Chunk.Instructions) { SerializeInstruction(Instruction); };

                            break;
                        };

                    case ChunkStep.Chunks: { WriteInt32(Chunk.Chunks.Count); foreach (Chunk SubChunk in Chunk.Chunks) { SerializeLChunk(SubChunk, Bytes); }; break; };
                };
            };
        }

        public List<byte> Serialize(Chunk HeadChunk)
        {
            List<byte> Bytes = new List<byte>(); SerializeLChunk(HeadChunk, Bytes); return (Bytes);
        }
    };
};