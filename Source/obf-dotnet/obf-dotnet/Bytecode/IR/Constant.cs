using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Bytecode.IR
{
    [Serializable]
    public class Constant
    {
        public List<Instruction> BackReferences;

        public ConstantType Type;
        public dynamic Data;

        public Constant()
        {
            BackReferences = new List<Instruction>();
        }

        public Constant(Constant Constant)
        {
            Type = Constant.Type; Data = Constant.Data; BackReferences = Constant.BackReferences.ToList();
        }
    };
};