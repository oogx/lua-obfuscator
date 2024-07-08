using Obfuscator.Bytecode.IR;
using Obfuscator.Extensions;
using Obfuscator.Obfuscation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Obfuscator.Utility
{
    public static class Utility
    {
        [DllImport("ObfuscatorDLL.dll")]
        private static extern double StringToDouble(string String, ref bool Success);

        private static List<string> VMStrings = new List<string>
        {
            "Vestra.Tech",
            "People Malding Over Psu Fork",
            "Vestra OnTop",
            "Roblox Ewwwww",
        };

        public static void VoidInstruction(Instruction Instruction)
        {
            Instruction.OpCode = OpCode.None;
            Instruction.A = 0;
            Instruction.B = 0;
            Instruction.C = 0;

            Instruction.ConstantType = InstructionConstantType.NK;
            Instruction.InstructionType = InstructionType.ABC;

            foreach (dynamic Reference in Instruction.References)
            {
                if (Reference is Instruction ReferencedInstruction)
                {
                    ReferencedInstruction.BackReferences.Remove(Instruction);
                }
                else if (Reference is Constant ReferencedConstant)
                {
                    ReferencedConstant.BackReferences.Remove(Instruction);
                };
            };

            Instruction.References = new List<dynamic> { null, null, null, null, null };
        }

        public static void SwapBackReferences(Instruction Original, Instruction Instruction)
        {
            foreach (Instruction BackReference in Original.BackReferences)
            {
                Instruction.BackReferences.Add(BackReference);

                if (BackReference.JumpTo == Original) { BackReference.JumpTo = Instruction; }
                else { BackReference.References[BackReference.References.IndexOf(Original)] = Instruction; };
            };

            Original.BackReferences.Clear();
        }

        public static Constant GetOrAddConstant(Chunk Chunk, ConstantType Type, dynamic Constant, out int ConstantIndex)
        {
            var Current = Chunk.Constants.FirstOrDefault(C => ((C.Type == Type) && (C.Data == Constant))); if (Current != null) { ConstantIndex = Chunk.Constants.IndexOf(Current); return (Current); }; Constant nConstant = new Constant { Type = Type, Data = Constant }; ConstantIndex = Chunk.Constants.Count; Chunk.Constants.Add(nConstant); Chunk.ConstantMap.Add(nConstant, ConstantIndex); return (nConstant);
        }

        public static int[] GetRegistersThatInstructionWritesTo(Instruction Instruction, int StackTop = 256)
        {
            int A = Instruction.A;
            int B = Instruction.B;
            int C = Instruction.C;

            switch (Instruction.OpCode)
            {
                case OpCode.OpMove: { return new int[] { A }; };
                case OpCode.OpLoadK: { return new int[] { A }; };
                case OpCode.OpUnm: { return new int[] { A }; };
                case OpCode.OpLoadBool: { return new int[] { A }; };
                case OpCode.OpLoadNil: { return Enumerable.Range(A, (B - A) + 1).ToArray(); };
                case OpCode.OpLen: { return new int[] { A }; };
                case OpCode.OpNot: { return new int[] { A }; };
                case OpCode.OpGetGlobal: { return new int[] { A }; };
                case OpCode.OpGetUpValue: { return new int[] { A }; };
                case OpCode.OpGetTable: { return new int[] { A }; };
                case OpCode.OpAdd: { return new int[] { A }; };
                case OpCode.OpSub: { return new int[] { A }; };
                case OpCode.OpMul: { return new int[] { A }; };
                case OpCode.OpDiv: { return new int[] { A }; };
                case OpCode.OpMod: { return new int[] { A }; };
                case OpCode.OpPow: { return new int[] { A }; };
                case OpCode.OpConcat: { return new int[] { A }; };
                case OpCode.OpCall when (C >= 2): { return Enumerable.Range(A, C + 1).ToArray(); };
                case OpCode.OpCall when (C == 0): { return new int[] { A }; };
                case OpCode.OpVarArg when (B >= 2): { return Enumerable.Range(A, B - 1).ToArray(); };
                case OpCode.OpVarArg when (B == 0): { return new int[] { A }; };
                case OpCode.OpSelf: { return new int[] { A, A + 1 }; };
                case OpCode.OpTestSet: { return new int[] { A }; };
                case OpCode.OpForPrep: { return new int[] { A }; };
                case OpCode.OpForLoop: { return new int[] { A, A + 3 }; };
                case OpCode.OpTForLoop: { return Enumerable.Range(A + 2, ((A + 2 + C) - A) + 1).ToArray(); };
                case OpCode.OpNewTable: { return new int[] { A }; };
                case OpCode.OpClosure: { return new int[] { A }; };
                case OpCode.OpClose: { return Enumerable.Range(0, StackTop).ToArray(); };
                default: { return new int[] { }; };
            };
        }

        public static int[] GetRegistersThatInstructionReadsFrom(Instruction Instruction, int StackTop = 256)
        {
            int A = Instruction.A;
            int B = Instruction.B;
            int C = Instruction.C;

            switch (Instruction.OpCode)
            {
                case OpCode.OpMove: { return new int[] { B }; };
                case OpCode.OpUnm: { return new int[] { B }; };
                case OpCode.OpLen: { return new int[] { B }; };
                case OpCode.OpNot: { return new int[] { B }; };
                case OpCode.OpSetGlobal: { return new int[] { A }; };
                case OpCode.OpSetUpValue: { return new int[] { A }; };
                case OpCode.OpGetTable: { List<int> List = new List<int> { B }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpSetTable: { List<int> List = new List<int> { A }; if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpAdd: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpSub: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpMul: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpDiv: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpMod: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpPow: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpConcat: { return Enumerable.Range(B, (C - B) + 1).ToArray(); };
                case OpCode.OpCall when (B >= 2): { return Enumerable.Range(A, B).ToArray(); };
                case OpCode.OpCall when (B == 0): { return Enumerable.Range(A, StackTop - A).ToArray(); };
                case OpCode.OpCall when (B == 1): { return new int[] { A }; };
                case OpCode.OpReturn when (B >= 2): { return Enumerable.Range(A, B - 1).ToArray(); };
                case OpCode.OpReturn when (B == 0): { return Enumerable.Range(A, StackTop - A).ToArray(); };
                case OpCode.OpTailCall when (B == 0): { return Enumerable.Range(A, StackTop - A).ToArray(); };
                case OpCode.OpTailCall when (B >= 2): { return Enumerable.Range(A, B).ToArray(); };
                case OpCode.OpSelf: { int[] List = new int[] { B }; if (C < ObfuscationSettings.ConstantOffset) { List[1] = C; }; return (List.ToArray()); };
                case OpCode.OpEq: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpLt: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpLe: { List<int> List = new List<int>(); if (B < ObfuscationSettings.ConstantOffset) { List.Add(B); }; if (C < ObfuscationSettings.ConstantOffset) { List.Add(C); }; return (List.ToArray()); };
                case OpCode.OpTest: { return new int[] { A }; };
                case OpCode.OpTestSet: { return new int[] { A, B }; };
                case OpCode.OpForPrep: { return new int[] { A + 2 }; };
                case OpCode.OpForLoop: { return new int[] { A, A + 1, A + 2, A + 3 }; };
                case OpCode.OpTForLoop: { return new int[] { A, A + 1, A + 2, A + 3 }; };
                case OpCode.OpSetList: { return Enumerable.Range(A, StackTop - A).ToArray(); };
                default: { return new int[] { }; };
            };
        }

        public static Random Random = new Random();

        public static List<char> HexDecimal = ("abcdefABCDEF0123456789").ToCharArray().ToList();
        public static List<char> Decimal = ("0123456789").ToCharArray().ToList();
        public static List<char> Characters = ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJLMNOPQRSTUVWXYZ").ToCharArray().ToList();
        public static List<char> AlphaNumeric = ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJLMNOPQRSTUVWXYZ01234567890").ToCharArray().ToList();

        public static bool OverrideStrings = false;
        public static bool NoExtraString = false;
        public static List<string> extra = new List<string>();

        public static void GetExtraStrings(string source)
        {
            NoExtraString = false;
            OverrideStrings = false;
            extra.Clear();

            Match mtch = Regex.Match(source, @"^\s*--\[(=*)\[([\S\s]*?)\]\1\]");
            if (!mtch.Success) return;
            if (mtch.Groups.Count < 3) return;

            string comment = mtch.Groups[2].Value.Trim();
            string[] lines = comment.Split('\n');

            if (lines.Length == 0) return;
            string line1 = lines[0].Trim().ToLower();
            if (line1 != "strings" && line1 != "strings-override") return;
            if (line1 == "strings-override")
                OverrideStrings = true;

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                extra.Add(line);
                //Console.WriteLine(line + " " + line.Length + " " + Encoding.ASCII.GetString(Encoding.Default.GetBytes(line)).Length);
            }
        }

        public static int CompatLength(string str)
        {
            return Encoding.ASCII.GetString(Encoding.Default.GetBytes(str)).Length;
        }

        public static string FinalReplaceStrings(string source)
        {
            return Regex.Replace(source, @"EXTRASTRING(\d+)", mtch =>
            {
                return extra[Int32.Parse(mtch.Groups[1].Value)].Replace("\"", "\\\"");
            });
        }

        public static string IntegerToHex(int Integer) => ("0x" + Integer.ToString("X3"));

        public static string IntegerToString(int Integer, int Minimum = 0)
        {
            switch (Random.Next(Minimum, 4))
            {
                case 0:
                    return (Integer.ToString());

                case 1:
                    return (IntegerToHex(Integer));

                case 2:
                    return (IntegerToTable(Integer));

                case 3:
                    {
                        Random rand = new Random();
                        if (extra.Count == 0 || (extra.Count == 1 && OverrideStrings) || NoExtraString)
                        {
                            string String = VMStrings.Random();
                            return $"({Integer + String.Length} - #(\"{String}\"))";
                        }
                        else if (OverrideStrings)
                        {
                            int idx = rand.Next(0, extra.Count);
                            string String = extra[idx];
                            return $"({Integer + CompatLength(String)} - #(\"EXTRASTRING{idx}\"))";
                        }
                        else
                        {
                            if (rand.Next(0, 2) == 0)
                            {
                                int idx = rand.Next(0, extra.Count);
                                string String = extra[idx];
                                return $"({Integer + CompatLength(String)} - #(\"EXTRASTRING{idx}\"))";
                            }
                            else
                            {
                                string String = VMStrings.Random();
                                return $"({Integer + String.Length} - #(\"{String}\"))";
                            }
                        }
                    }
            };
            return (Integer.ToString());
        }

        public static string IntegerToStringBasic(int Integer)
        {
            switch (Random.Next(0, 2)) { case 0: return (Integer.ToString()); case 1: return (IntegerToHex(Integer)); }; return (Integer.ToString());
        }

        public static string IntegerToTable(int Value)
        {
            string Table = "(#{";
            int Values = Random.Next(0, 5);
            Value -= Values;
            int Count = 0;
            int Indexes = 0;
            while (Count < Values)
            {
                /*if ((Indexes < 5) && (Random.Next(0, 5) == 0))
                {
                    // using # on holed table not good
                    Table += $"{IntegerToStringBasic(Random.Next(0, 1000))};";
                    //Table += $"[{IntegerToStringBasic(Random.Next(Values + 10, Values + 1000))}]={IntegerToStringBasic(Random.Next(0, 1000))};";
                    Indexes += 1;
                    continue;
                };*/
                Table += IntegerToStringBasic(Random.Next(0, 1000)) + ";";
                Count += 1;
            };
            if (Random.Next(0, 2) == 0)
            {
                int ReturnValues = Random.Next(0, 5);
                int ReturnCount = 0;
                Value -= ReturnValues;
                Table += "(function(...)return ";
                while (ReturnCount < ReturnValues)
                {
                    Table += IntegerToStringBasic(Random.Next(0, 1000));
                    if (ReturnCount < (ReturnValues - 1))
                    {
                        Table += ",";
                    };
                    ReturnCount += 1;
                };
                bool HasVarArg = Random.Next(0, 3) == 0;
                if (HasVarArg)
                {
                    if (ReturnValues > 0)
                    {
                        Table += ",";
                    };
                    Table += "...";
                };
                Table += ";end)(";
                if (HasVarArg)
                {
                    int VarArgValues = Random.Next(0, 5);
                    int VarArgCount = 0;
                    Value -= VarArgValues;
                    while (VarArgCount < VarArgValues)
                    {
                        Table += IntegerToStringBasic(Random.Next(0, 1000));
                        if (VarArgCount < (VarArgValues - 1))
                        {
                            Table += ",";
                        };
                        VarArgCount += 1;
                    };
                };
                Table += ")";
            };
            return (Table + $"}}{(Math.Sign(Value) < 0 ? " - " : " + ")}{IntegerToStringBasic(Math.Abs(Value))})");
        }

        public static List<string> GetIndexList()
        {
            List<string> Indicies = new List<string>();

            switch (Random.Next(0, 2))
            {
                case (0): { double Index = Random.Next(0, 1000000000); Indicies.Add($"[{Index}]"); break; };
                //case (1): { double Index = Random.NextDouble(); Indicies.Add($"[{Index}]"); break; };
                case (1): { int Length = Random.Next(4, 10); string Index = Characters.Random().ToString(); for (int I = 0; I < Length; I++) { Index += AlphaNumeric.Random(); }; Indicies.Add($".{Index}"); Indicies.Add($"[\"{Index}\"]"); Indicies.Add($"['{Index}']"); break; };
            };

            return (Indicies);
        }

        public static List<string> GetIndexListNoBrackets()
        {
            List<string> Indicies = new List<string>();

            switch (Random.Next(0, 2))
            {
                case (0): { double Index = 0d; Index += Random.Next(0, 1000000); if (Random.Next(0, 4) == 0) { Index += Random.NextDouble(); }; if (Random.Next(0, 2) == 0) { Index = -(Index); }; Indicies.Add($"{Index}"); break; };
                case (1): { int Length = Random.Next(2, 10); string Index = Characters.Random().ToString(); for (int I = 0; I < Length; I++) { Index += AlphaNumeric.Random(); }; Indicies.Add($"\"{Index}\""); Indicies.Add($"'{Index}'"); break; };
            };

            return (Indicies);
        }
    };
};