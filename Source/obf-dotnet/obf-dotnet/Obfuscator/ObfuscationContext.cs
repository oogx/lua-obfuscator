using Obfuscator.Extensions;
using Obfuscator.Obfuscation.OpCodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Obfuscation
{
    public enum ChunkStep { ParameterCount, Instructions, Chunks, Constants, StackSize, StepCount };

    public enum InstructionStep { Enum, A, B, C, StepCount };

    public struct InstructionMap
    {
        public List<string> Enum;
        public List<string> Data;
        public List<string> A;
        public List<string> B;
        public List<string> C;
        public List<string> D;
        public List<string> E;
    };

    public struct ChunkMap
    {
        public List<string> ParameterCount;
        public List<string> Instructions;
        public List<string> Chunks;
        public List<string> Constants;
        public List<string> StackSize;
        public List<string> InstructionPoint;
    };

    public struct DeserializerInstructionStep { public int Value; public string Enum; public string A; public string B; public string C; };

    public class ObfuscationContext
    {
        public Bytecode.IR.Chunk HeadChunk;

        public Dictionary<Bytecode.IR.OpCode, VOpCode> InstructionMapping;

        public ChunkStep[] ChunkSteps;

        public InstructionStep[] InstructionStepsABC;
        public InstructionStep[] InstructionStepsABx;
        public InstructionStep[] InstructionStepsAsBx;
        public InstructionStep[] InstructionStepsAsBxC;
        public InstructionStep[] InstructionSteps;

        public int[] ConstantMapping;

        public int PrimaryXORKey;
        public int InitialPrimaryXORKey;

        public int[] XORKeys;

        public Obfuscator Obfuscator;

        private Random Random = new Random();

        public InstructionMap Instruction = new InstructionMap();
        public ChunkMap Chunk = new ChunkMap();

        public List<DeserializerInstructionStep> DeserializerInstructionSteps;

        public List<string> PrimaryIndicies;

        public string ByteCode;
        public string FormatTable;

        public Dictionary<long, Generation.NumberEquation> NumberEquations;

        public ObfuscationContext(Bytecode.IR.Chunk HeadChunk)
        {
            this.HeadChunk = HeadChunk;

            InstructionMapping = new Dictionary<Bytecode.IR.OpCode, VOpCode>();

            ChunkSteps = Enumerable.Range(0, (int)ChunkStep.StepCount).Select(I => (ChunkStep)I).ToArray();
            ChunkSteps.Shuffle();

            InstructionSteps = Enumerable.Range(0, (int)InstructionStep.StepCount).Select(I => (InstructionStep)I).ToArray();
            InstructionSteps.Shuffle();

            InstructionStepsABC = InstructionSteps.ToArray().Shuffle().ToArray();
            InstructionStepsABx = InstructionSteps.ToArray().Shuffle().ToArray();
            InstructionStepsAsBx = InstructionSteps.ToArray().Shuffle().ToArray();
            InstructionStepsAsBxC = InstructionSteps.ToArray().Shuffle().ToArray();

            PrimaryXORKey = Random.Next(0, 256);
            InitialPrimaryXORKey = PrimaryXORKey;

            XORKeys = new int[] { Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256) };

            Instruction.A = Utility.Utility.GetIndexListNoBrackets();
            Instruction.B = Utility.Utility.GetIndexListNoBrackets();
            Instruction.C = Utility.Utility.GetIndexListNoBrackets();
            Instruction.D = Utility.Utility.GetIndexListNoBrackets();
            Instruction.E = Utility.Utility.GetIndexListNoBrackets();

            Instruction.Enum = Utility.Utility.GetIndexListNoBrackets();
            Instruction.Data = Utility.Utility.GetIndexListNoBrackets();

            Chunk.ParameterCount = Utility.Utility.GetIndexListNoBrackets();
            Chunk.Instructions = Utility.Utility.GetIndexListNoBrackets();
            Chunk.Chunks = Utility.Utility.GetIndexListNoBrackets();
            Chunk.Constants = Utility.Utility.GetIndexListNoBrackets();
            Chunk.StackSize = Utility.Utility.GetIndexListNoBrackets();
            Chunk.InstructionPoint = Utility.Utility.GetIndexListNoBrackets();

            DeserializerInstructionSteps = new List<DeserializerInstructionStep> { };

            PrimaryIndicies = (new List<string> { "A", "B", "C" }).Shuffle().ToList();

            NumberEquations = new Dictionary<long, Generation.NumberEquation> { };
            int Count = Random.Next(5, 15); for (int I = 0; I < Count; I++) { NumberEquations.Add(Random.Next(0, 1000000000), new Generation.NumberEquation(Random.Next(3, 6))); };

            int _ = Random.Next(0, 16); ConstantMapping = new int[] { _, _ += Random.Next(1, 16), _ += Random.Next(1, 16), _ += Random.Next(1, 16), _ += Random.Next(1, 16), _ += Random.Next(1, 16) }; ConstantMapping.Shuffle();
        }
    };
};