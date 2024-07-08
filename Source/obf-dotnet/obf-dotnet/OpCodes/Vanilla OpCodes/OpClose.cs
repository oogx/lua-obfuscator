using Obfuscator.Bytecode.IR;

namespace Obfuscator.Obfuscation.OpCodes
{
    public class OpClose : VOpCode
    {
        public override bool IsInstruction(Instruction Instruction) => (Instruction.OpCode == OpCode.OpClose);

        public override string GetObfuscated(ObfuscationContext ObfuscationContext) => "local A = Instruction[OP_A]; local Close = {}; for Index = 1, #lUpValues, 1 do local List = lUpValues[Index]; for Index = 0, #List, 1 do local UpValue = List[Index]; local oStack = UpValue[1]; local Position = UpValue[2]; if ((oStack == Stack) and (Position >= A)) then Close[Position] = oStack[Position]; UpValue[1] = Close; end; end; end;";
    };
};