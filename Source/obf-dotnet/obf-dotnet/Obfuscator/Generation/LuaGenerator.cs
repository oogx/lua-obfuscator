using System;
using System.Collections.Generic;

namespace LuaGeneration
{
    public static class LuaGeneration
    {
        private static string Symbols = "`~!@#$%^&*()_-+={}|;:<,>.?/";
        private static string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string Lowercase = "abcdefghijklmnnopqrstuvwxyz";
        private static string Numbers = "0123456789";
        private static string Hexadecimal = "ABCDEFabcdef0123456789";
        private static string Alphabet = Uppercase + Lowercase;
        private static string Alphanumeric = Alphabet + Numbers;
        private static string Characters = Alphabet + Numbers + Symbols;

        private static string[] Operators = { " + ", " - ", " * ", " / ", " ^ ", " % ", " or ", " and ", " == ", " <= ", " >= ", " < ", " > " };

        private static Random Random = new Random();

        private static List<string> Variables = new List<string>();
        private static double GenerationIntensity = 10;
        private static string Source = ("");
        private static dynamic ValueLock = null;

        private static int DEFAULT_VARIABLE_NAME_LENGTH = 10;

        private const int DEFAULT_NUMBER_LENGTH = 5;

        private const int MAXIMUM_TABLE_LENGTH = 10;
        private const int MAXIMUM_STRING_LENGTH = 100;

        private const int MAXIMUM_DEPTH = 5;
        private const int MAXIMUM_FUNCTION_DEPTH = 2;

        private static string GenerateVariableName(int Length = 0)
        {
            Length = Length > 0 ? Length : DEFAULT_VARIABLE_NAME_LENGTH; string String = Alphabet[Random.Next(0, Alphabet.Length)].ToString(); for (int I = 0; I < (Length - 1); I++) { String += Alphanumeric[Random.Next(0, Alphanumeric.Length)]; }; return (String);
        }

        private static string GenerateNumber(int Length = DEFAULT_NUMBER_LENGTH, bool Integer = (true))
        {
            Integer = (true); string String = (""); for (int I = 0; I < Length; I++) { String += Numbers[Random.Next(0, Numbers.Length)]; }; if (!Integer) { String += "."; for (int I = 0; I < Length; I++) { String += Numbers[Random.Next(0, Numbers.Length)]; }; }; return (String);
        }

        private static string GenerateHexadecimal(int Length = DEFAULT_NUMBER_LENGTH)
        {
            string String = "0x"; for (int I = 0; I < Length; I++) { String += Hexadecimal[Random.Next(0, Hexadecimal.Length)]; }; return (String);
        }

        private static string GenerateString(int MaximumLength = MAXIMUM_STRING_LENGTH)
        {
            string String = (""); for (int I = 0; I < Random.Next(1, MaximumLength); I++) { String += Characters[Random.Next(0, Characters.Length)]; }; switch (Random.Next(0, 3)) { case (0): { return ("\"" + String + "\""); }; case (1): { return ("'" + String + "'"); }; case (2): { string Chunk = (new string('=', Random.Next(0, 10))); return ("[" + Chunk + "[" + String + "]" + Chunk + "]"); }; }; return (String);
        }

        private static string GenerateTable(int Depth = 0)
        {
            if (Depth > MAXIMUM_DEPTH) { return ("{}"); }; string String = ("{"); for (int I = 0; I < Random.Next(0, MAXIMUM_TABLE_LENGTH); I++) { switch (Random.Next(0, 2)) { case (0): { String += $"{GetRandomValue(Depth + 1)};"; break; }; case (1): { String += $"[({GetRandomValue(Depth + 1)})] = {GetRandomValue(Depth + 1)};"; break; }; }; }; return (String + "}");
        }

        private static string GetEquation(int Depth = 0)
        {
            string String = (GetRandomValue(Depth + 1) + Operators[Random.Next(0, Operators.Length)] + GetRandomValue(Depth + 1)); if (Depth <= MAXIMUM_DEPTH) { String += Operators[Random.Next(0, Operators.Length)] + GetEquation(Depth + 1); }; return (String);
        }

        private static string GenerateFunction(int Depth = 0)
        {
            if (Depth > MAXIMUM_DEPTH) { return ("(function(...) return; end)"); }; int VariableCount = Variables.Count; string String = "(function("; int Parameters = Random.Next(0, 10); for (int I = 0; I < Parameters; I++) { string Parameter = GenerateVariableName(); String += Parameter + ", "; Variables.Add(Parameter); }; String += "...)"; GenerateBody(Depth + 1); String += "return "; int R = Random.Next(0, 10); for (int I = 0; I < R; I++) { String += GetRandomValue(Depth + 1) + ((I < (R - 1)) ? ", " : ""); }; Variables.RemoveRange(VariableCount, Variables.Count - VariableCount); return (String + "; end)");
        }

        private static string GetRandomValue(int Depth = 0, dynamic Lock = null)
        {
            string String = ("");

            if (ValueLock != null)
            {
                switch (Random.Next(0, 11))
                {
                    case (0) when (Variables.Count > 0): { String = Variables[Random.Next(0, Variables.Count)]; break; };
                    case (7) when (Depth < MAXIMUM_DEPTH): { String = GenerateTable(Depth + 1); break; };
                    case (8) when (Depth < MAXIMUM_DEPTH): { String = GetEquation(Depth + 1); break; };
                    case (9) when (Depth < MAXIMUM_DEPTH): { String = GenerateFunction(Depth + 1); break; };
                    default: { String = Alphabet[Random.Next(0, Alphabet.Length)].ToString(); break; };
                };
            }
            else
            {
                if (Depth > MAXIMUM_DEPTH)
                {
                    switch (Random.Next(0, 7))
                    {
                        case (0) when (Variables.Count > 0): { String = Variables[Random.Next(0, Variables.Count)]; break; };
                        case (1): { String = GenerateString(); break; };
                        case (2): { String = GenerateNumber(); break; };
                        case (3): { String = GenerateHexadecimal(); break; };
                        case (4): { String = ("..."); break; };
                        case (5): { String = (Random.Next(0, 2) == 0) ? ("false") : ("true"); break; };
                        case (6): { String = ("nil"); break; };
                        default: { String = GenerateString(); break; };
                    };
                }
                else
                {
                    switch (Random.Next(0, 11))
                    {
                        case (0) when (Variables.Count > 0): { String = Variables[Random.Next(0, Variables.Count)]; break; };
                        case (1): { String = GenerateString(); break; };
                        case (2): { String = GenerateNumber(); break; };
                        case (3): { String = GenerateHexadecimal(); break; };
                        case (4): { String = ("..."); break; };
                        case (5): { String = (Random.Next(0, 2) == 0) ? ("false") : ("true"); break; };
                        case (6): { String = ("nil"); break; };
                        case (7) when (Depth < MAXIMUM_DEPTH): { String = GenerateTable(Depth + 1); break; };
                        case (8) when (Depth < MAXIMUM_DEPTH): { String = GetEquation(Depth + 1); break; };
                        case (9) when (Depth < MAXIMUM_DEPTH): { String = GenerateFunction(Depth + 1); break; };
                        default: { String = GenerateString(); break; };
                    };
                }
            }

            if (Random.Next(0, 2) == 0) { String = "(not " + String + ")"; };
            if (Random.Next(0, 2) == 0) { String = "#" + String + ""; };
            if (Random.Next(0, 2) == 0) { String = "(-" + String + ")"; };
            if (Random.Next(0, 2) == 0) { String = "(" + String + ")" + "." + GenerateVariableName(); };
            if (Random.Next(0, 2) == 0) { String = "(" + String + ")()"; };

            return (String);
        }

        private static void GenerateBody(int Depth = 1)
        {
            if (Depth > MAXIMUM_FUNCTION_DEPTH) { return; }

            int VariableCount = Variables.Count;

            for (int I = 0; I < GenerationIntensity - Depth; I++)
            {
                switch (Random.Next(0, 6))
                {
                    case (0): { string VariableName = GenerateVariableName(); Source += $"local {VariableName} = {GetRandomValue(Depth + 1)};"; Variables.Add(VariableName); break; };
                    case (1): { Source += $"if ({GetEquation(Depth + 1)}) then "; GenerateBody(Depth + 1); Source += " end;"; break; };
                    case (2): { Source += $"while ({GetEquation(Depth + 1)}) do "; GenerateBody(Depth + 1); Source += " end;"; break; };
                    case (3): { string VariableName = GenerateVariableName(); Source += $"for {VariableName} = {GetEquation(Depth + 1)}, {GetEquation(Depth + 1)}, {GetEquation(Depth + 1)} do "; Variables.Add(VariableName); GenerateBody(Depth + 1); Source += $" end;"; Variables.Remove(VariableName); break; };
                    case (4): { string VariableName = GenerateVariableName(); Source += $"local function {VariableName}(...) "; Variables.Add(VariableName); GenerateBody(Depth + 1); Source += $" end;"; break; };
                };
            };

            Variables.RemoveRange(VariableCount, Variables.Count - VariableCount);
        }

        public static string GenerateSingleVariable()
        {
            switch (Random.Next(0, 4)) { case (0): { return ($"local _ = {GenerateNumber()};"); }; case (1): { return ($"local _ = {GenerateString()};"); }; case (2): { return ($"local _ = {GenerateHexadecimal()};"); }; case (3): { return ($"local _ = ({{}});"); }; }; return ($"local _ = {GenerateString()};");
        }

        public static string GenerateRandomFile(int Iterations, int GenerationIntensity)
        {
            List<string> Chunks = new List<string>();

            Alphabet = "_";
            Alphanumeric = "_";
            DEFAULT_VARIABLE_NAME_LENGTH = 1;

            Source = ("");

            for (double I = 0; I < Iterations; I++)
            {
                Source = ("");

                switch (Random.Next(0, 5))
                {
                    case (0): { string VariableName = GenerateVariableName(); Source += $"local {VariableName} = {GetEquation(1)};"; Variables.Add(VariableName); break; };
                    case (1): { Source += $"if ({GetEquation(1)}) then "; GenerateBody(1); Source += " end;"; break; };
                    case (2): { Source += $"while ({GetEquation(1)}) do "; GenerateBody(1); Source += " end;"; break; };
                    case (3): { string VariableName = GenerateVariableName(); Source += $"for {VariableName} = {GetEquation(1)}, {GetEquation(1)}, {GetEquation(1)} do "; Variables.Add(VariableName); GenerateBody(1); Source += $" end;"; Variables.Remove(VariableName); break; };
                    case (4): { string VariableName = GenerateVariableName(); Source += $"local function {VariableName}(...) "; Variables.Add(VariableName); GenerateBody(1); Source += $" end;"; break; };
                };

                Source += "\n";

                Chunks.Add(Source);
            };

            Source = ("");
            Source = string.Join("", Chunks);

            return (Source);
        }

        public static string VarArgSpam(int Iterations, int GenerationIntensity)
        {
            ValueLock = new string[] { "..." };
            // Alphabet = "_";
            // Alphanumeric = "_";
            DEFAULT_VARIABLE_NAME_LENGTH = 1;

            List<string> Chunks = new List<string>();

            Source = ("");

            for (double I = 0; I < Iterations; I++)
            {
                Source = ("");

                switch (Random.Next(0, 5))
                {
                    case (0): { string VariableName = GenerateVariableName(); Source += $"local {VariableName} = {GetEquation(1)};"; Variables.Add(VariableName); break; };
                    case (1): { Source += $"if ({GetEquation(1)}) then "; GenerateBody(1); Source += " end;"; break; };
                    case (2): { Source += $"while ({GetEquation(1)}) do "; GenerateBody(1); Source += " end;"; break; };
                    case (3): { string VariableName = GenerateVariableName(); Source += $"for {VariableName} = {GetEquation(1)}, {GetEquation(1)}, {GetEquation(1)} do "; Variables.Add(VariableName); GenerateBody(1); Source += $" end;"; Variables.Remove(VariableName); break; };
                    case (4): { string VariableName = GenerateVariableName(); Source += $"local function {VariableName}(...) "; Variables.Add(VariableName); GenerateBody(1); Source += $" end;"; break; };
                };

                Source += "\n";

                Chunks.Add(Source);
            };

            Source = ("");
            Source = string.Join("", Chunks);

            Variables.Clear();

            return (Source);
        }

        public static string OperatorSpam(int Iterations, int GenerationIntensity)
        {
            DEFAULT_VARIABLE_NAME_LENGTH = 1;

            Source = ("");

            List<string> Chunks = new List<string>();

            for (double I = 0; I < Iterations; I++)
            {
                string Chunk = ("");
                Chunk += $"local {GenerateVariableName(2)} = {GenerateVariableName()}";

                for (int K = 0; K < GenerationIntensity; K++)
                    Chunk += $"{Operators[Random.Next(0, Operators.Length)]}{GenerateVariableName()}";

                Chunk += ";\n";

                Chunks.Add(Chunk);
            };

            Source = string.Join("", Chunks);

            Variables.Clear();

            return (Source);
        }
    };
};