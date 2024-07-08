namespace Obfuscator.Bytecode.IR
{
    public enum OpCode
    {
        OpMove,
        OpLoadK,
        OpLoadBool,
        OpLoadNil,
        OpGetUpValue,
        OpGetGlobal,
        OpGetTable,
        OpSetGlobal,
        OpSetUpValue,
        OpSetTable,
        OpNewTable,
        OpSelf,
        OpAdd,
        OpSub,
        OpMul,
        OpDiv,
        OpMod,
        OpPow,
        OpUnm,
        OpNot,
        OpLen,
        OpConcat,
        OpJump,
        OpEq,
        OpLt,
        OpLe,
        OpTest,
        OpTestSet,
        OpCall,
        OpTailCall,
        OpReturn,
        OpForLoop,
        OpForPrep,
        OpTForLoop,
        OpSetList,
        OpClose,
        OpClosure,
        OpVarArg,

        None,
        Custom,

        OpGetStack,
        OpNewStack,

        OpLoadJump,
        OpDynamicJump
    };

    public enum ConstantType
    {
        Nil,
        Boolean,
        Number,
        String,
        FastString,

        Int16,
        Int32,
    };

    public enum InstructionType
    {
        ABC = 0,
        ABx = 1,
        AsBx = 2,
        AsBxC = 3,
        Data = 4,
        Closure = 5,
        Compressed = 6
    };

    public enum InstructionConstantType
    {
        NK = 0b000000,
        RA = 0b001000,
        RB = 0b010000,
        RC = 0b100000
    };
};