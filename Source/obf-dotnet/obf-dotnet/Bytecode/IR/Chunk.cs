using System;
using System.Collections.Generic;

namespace Obfuscator.Bytecode.IR
{
    [Serializable]
    public class Chunk
    {
        public string Name = "";

        public byte ParameterCount;

        public byte VarArgFlag;

        public byte StackSize;

        public int Line;
        public int LastLine;

        public int CurrentOffset;
        public int CurrentParameterOffset;

        public byte UpValueCount;
        public List<string> UpValues;

        public List<Instruction> Instructions;
        public Dictionary<Instruction, int> InstructionMap;

        public List<Constant> Constants;
        public Dictionary<Constant, int> ConstantMap;

        public List<Chunk> Chunks;
        public Dictionary<Chunk, int> ChunkMap;

        public int XORKey = new Random().Next(0, 256);

        public void UpdateMappings()
        {
            this.InstructionMap.Clear();
            this.ConstantMap.Clear();
            this.ChunkMap.Clear();

            for (int I = 0; I < this.Instructions.Count; I++) { this.InstructionMap.Add(this.Instructions[I], I); };
            for (int I = 0; I < this.Constants.Count; I++) { this.ConstantMap.Add(this.Constants[I], I); };
            for (int I = 0; I < this.Chunks.Count; I++) { this.ChunkMap.Add(this.Chunks[I], I); };
        }
    };
};