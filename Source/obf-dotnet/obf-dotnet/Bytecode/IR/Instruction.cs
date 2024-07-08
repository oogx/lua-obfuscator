using Obfuscator.Obfuscation;
using System;
using System.Collections.Generic;

namespace Obfuscator.Bytecode.IR
{
    [Serializable]
    public class Instruction
    {
        public OpCode OpCode;
        public InstructionType InstructionType;
        public InstructionConstantType ConstantType;

        public List<dynamic> References;
        public List<Instruction> BackReferences;

        public Chunk Chunk;

        public int A;
        public int B;
        public int C;
        public int D;

        public int PC;

        public int Data;
        public int Line;

        public bool IsConstantA = false;
        public bool IsConstantB = false;
        public bool IsConstantC = false;

        public bool RequiresCustomData = false;
        public dynamic CustomData;

        public BasicBlock Block;

        public bool IsJump;
        public Instruction JumpTo;

        public bool IgnoreInstruction = false;

        public int? WrittenVIndex = null;

        public int? WrittenA = null;
        public int? WrittenB = null;
        public int? WrittenC = null;

        public Func<Instruction, Instruction> RegisterHandler = null;

        public CustomInstructionData CustomInstructionData = new CustomInstructionData();

        public Instruction(Instruction Instruction)
        {
            this.OpCode = Instruction.OpCode;
            this.InstructionType = Instruction.InstructionType;
            this.ConstantType = Instruction.ConstantType;

            this.References = new List<dynamic>(Instruction.References);
            this.BackReferences = new List<Instruction>(Instruction.BackReferences);

            this.Chunk = Instruction.Chunk;

            this.A = Instruction.A;
            this.B = Instruction.B;
            this.C = Instruction.C;

            this.Data = Instruction.Data;
            this.Line = Instruction.Line;

            this.IsJump = Instruction.IsJump;
            this.JumpTo = Instruction.JumpTo;

            this.IsConstantA = Instruction.IsConstantA;
            this.IsConstantB = Instruction.IsConstantB;
            this.IsConstantC = Instruction.IsConstantC;
        }

        public Instruction(Chunk Chunk, OpCode OpCode, params object[] References)
        {
            this.OpCode = OpCode;

            if (Deserializer.InstructionMappings.TryGetValue(OpCode, out InstructionType Type)) { InstructionType = Type; }
            else { InstructionType = InstructionType.ABC; };

            this.ConstantType = InstructionConstantType.NK;

            this.References = new List<dynamic> { null, null, null, null, null };
            this.BackReferences = new List<Instruction>();

            this.Chunk = Chunk;

            this.A = 0;
            this.B = 0;
            this.C = 0;

            this.Data = 0;
            this.Line = 0;

            this.IsConstantA = false;
            this.IsConstantB = false;
            this.IsConstantC = false;

            for (int I = 0; I < References.Length; I++) { var Reference = References[I]; this.References[I] = Reference; if (Reference is Instruction Instruction) { Instruction.BackReferences.Add(this); }; };
        }

        public void UpdateRegisters()
        {
            if (InstructionType == InstructionType.Data) { return; };

            this.PC = Chunk.InstructionMap[this];

            switch (OpCode)
            {
                case OpCode.OpLoadK:
                case OpCode.OpGetGlobal:
                case OpCode.OpSetGlobal: { IsConstantB = true; B = Chunk.ConstantMap[(Constant)References[0]]; break; };

                case OpCode.OpJump:
                case OpCode.OpForLoop:
                case OpCode.OpLoadJump:
                case OpCode.OpForPrep: { B = Chunk.InstructionMap[(Instruction)References[0]]; break; };

                case OpCode.OpClosure: { B = Chunk.ChunkMap[(Chunk)References[0]]; break; };

                case OpCode.OpGetTable:
                case OpCode.OpSetTable:
                case OpCode.OpAdd:
                case OpCode.OpSub:
                case OpCode.OpMul:
                case OpCode.OpDiv:
                case OpCode.OpMod:
                case OpCode.OpPow:
                case OpCode.OpEq:
                case OpCode.OpLt:
                case OpCode.OpLe:
                case OpCode.OpSelf:
                    {
                        if (References[0] is Constant ConstantB) { IsConstantB = true; B = Chunk.ConstantMap[ConstantB]; } else { IsConstantB = false; };
                        if (References[1] is Constant ConstantC) { IsConstantC = true; C = Chunk.ConstantMap[ConstantC]; } else { IsConstantC = false; };

                        break;
                    };

                case OpCode.Custom:
                    {
                        if (References[0] is Constant ConstantA) { IsConstantA = true; A = Chunk.ConstantMap[ConstantA]; } else { IsConstantA = false; };
                        if (References[1] is Constant ConstantB) { IsConstantB = true; B = Chunk.ConstantMap[ConstantB]; } else { IsConstantB = false; };
                        if (References[2] is Constant ConstantC) { IsConstantC = true; C = Chunk.ConstantMap[ConstantC]; } else { IsConstantC = false; };

                        if (References[0] is Instruction InstructionA) { A = Chunk.InstructionMap[InstructionA]; };
                        if (References[1] is Instruction InstructionB) { B = Chunk.InstructionMap[InstructionB]; };
                        if (References[2] is Instruction InstructionC) { C = Chunk.InstructionMap[InstructionC]; };

                        break;
                    };
            };
        }

        public void SetupReferences()
        {
            switch (OpCode)
            {
                case OpCode.OpLoadK:
                case OpCode.OpGetGlobal:
                case OpCode.OpSetGlobal: { Constant Reference = Chunk.Constants[B]; References[0] = Reference; Reference.BackReferences.Add(this); break; };

                case OpCode.OpJump:
                case OpCode.OpForLoop:
                case OpCode.OpForPrep: { Instruction Reference = Chunk.Instructions[Chunk.InstructionMap[this] + B + 1]; References[0] = Reference; Reference.BackReferences.Add(this); break; };

                case OpCode.OpClosure: { References[0] = Chunk.Chunks[B]; break; };

                case OpCode.OpEq:
                case OpCode.OpLt:
                case OpCode.OpLe:
                    {
                        if (B > 255) { this.IsConstantB = true; References[0] = Chunk.Constants[B -= 256]; References[0].BackReferences.Add(this); };
                        if (C > 255) { this.IsConstantC = true; References[1] = Chunk.Constants[C -= 256]; References[1].BackReferences.Add(this); };

                        Instruction Reference = Chunk.Instructions[Chunk.InstructionMap[this] + Chunk.Instructions[Chunk.InstructionMap[this] + 1].B + 1 + 1];
                        References[2] = Reference; Reference.BackReferences.Add(this);

                        break;
                    };

                case OpCode.OpTest:
                case OpCode.OpTestSet:
                case OpCode.OpTForLoop:
                    {
                        Instruction Reference = Chunk.Instructions[Chunk.InstructionMap[this] + Chunk.Instructions[Chunk.InstructionMap[this] + 1].B + 1 + 1];
                        References[2] = Reference; Reference.BackReferences.Add(this);

                        break;
                    };

                case OpCode.OpGetTable:
                case OpCode.OpSetTable:
                case OpCode.OpAdd:
                case OpCode.OpSub:
                case OpCode.OpMul:
                case OpCode.OpDiv:
                case OpCode.OpMod:
                case OpCode.OpPow:
                case OpCode.OpSelf:
                    {
                        if (B > 255) { this.IsConstantB = true; References[0] = Chunk.Constants[B -= 256]; References[0].BackReferences.Add(this); };
                        if (C > 255) { this.IsConstantC = true; References[1] = Chunk.Constants[C -= 256]; References[1].BackReferences.Add(this); };

                        break;
                    };
            };
        }
    };
};