using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpCall : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B > 2) && (Instruction.C > 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results = ({ Stack[A](UnPack(Stack, A + 1, Instruction[OP_B])) }); local Limit = Instruction[OP_C]; local K = 0; for I = A, Limit, 1 do K = K + 1; Stack[I] = Results[K]; end; for I = Limit + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B += Instruction.A - 1; Instruction.C += Instruction.A - 2;
        }
    };

    public class OpCallB2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 2) && (Instruction.C > 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results = { Stack[A](Stack[A + 1]); }; local Limit = Instruction[OP_C]; local K = 0; for I = A, Limit do K = K + 1; Stack[I] = Results[K]; end; for I = Limit + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.C += Instruction.A - 2; Instruction.B = 0;
        }
    };

    public class OpCallB0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 0) && (Instruction.C > 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results = { Stack[A](UnPack(Stack, A + 1, Top)); }; local Limit = Instruction[OP_C]; local K = 0; for I = A, Limit do K = K + 1; Stack[I] = Results[K]; end; for I = Limit + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.C += Instruction.A - 2; Instruction.B = 0;
        }
    };

    public class OpCallB1 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 1) && (Instruction.C > 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results = { Stack[A](); }; local Limit = Instruction[OP_C]; local K = 0; for I = A, Limit do K = K + 1; Stack[I] = Results[K]; end; for I = Limit + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.C += Instruction.A - 2; Instruction.B = 0;
        }
    };

    public class OpCallC0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B > 2) && (Instruction.C == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results, Limit = _R(Stack[A](UnPack(Stack, A + 1, Instruction[OP_B]))); Top = Limit + A - 1; local K = 0; for I = A, Top do K = K + 1; Stack[I] = Results[K]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B += Instruction.A - 1; Instruction.C = 0;
        }
    };

    public class OpCallC0B2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 2) && (Instruction.C == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results, Limit = _R(Stack[A](Stack[A + 1])); Top = Limit + A - 1; local K = 0; for I = A, Top do K = K + 1; Stack[I] = Results[K]; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };

    public class OpCallC1 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B > 2) && (Instruction.C == 1));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A](UnPack(Stack, A + 1, Instruction[OP_B])); for I = A + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B += Instruction.A - 1; Instruction.C = 0;
        }
    };

    public class OpCallC1B2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 2) && (Instruction.C == 1));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A](Stack[A + 1]); for I = A, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };

    public class OpCallB0C0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 0) && (Instruction.C == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results, Limit = _R(Stack[A](UnPack(Stack, A + 1, Top))); Top = Limit + A - 1; local K = 0; for I = A, Top do K = K + 1; Stack[I] = Results[K]; end; for I = Top + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };

    public class OpCallB0C1 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 0) && (Instruction.C == 1));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A](UnPack(Stack, A + 1, Top)); for I = A + 1, Top do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };

    public class OpCallB1C0 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 1) && (Instruction.C == 0));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; local Results, Limit = _R(Stack[A]()); Top = Limit + A - 1; local K = 0; for I = A, Top do K = K + 1; Stack[I] = Results[K]; end; for I = Top + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    }

    public class OpCallB1C1 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 1) && (Instruction.C == 1));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "Stack[Instruction[OP_A]]();";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };

    public class OpCallC2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B > 2) && (Instruction.C == 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A] = Stack[A](UnPack(Stack, A + 1, Instruction[OP_B])); for I = A + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B += Instruction.A - 1; Instruction.C = 0;
        }
    };

    public class OpCallC2B2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 2) && (Instruction.C == 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A] = Stack[A](Stack[A + 1]); for I = A + 1, StackSize do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };

    public class OpCallB0C2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 0) && (Instruction.C == 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A] = Stack[A](UnPack(Stack, A + 1, Top)); for I = A + 1, Top do Stack[I] = nil; end;";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };

    public class OpCallB1C2 : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => ((Instruction.OpCode == OpCode.OpCall) && (Instruction.B == 1) && (Instruction.C == 2));

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => @"local A = Instruction[OP_A]; Stack[A] = Stack[A]();";

        public override void Mutate(Instruction Instruction)
        {
            Instruction.B = 0; Instruction.C = 0;
        }
    };
};