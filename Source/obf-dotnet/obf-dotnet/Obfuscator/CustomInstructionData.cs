using Obfuscator.Obfuscation.OpCodes;
using System;

namespace Obfuscator.Obfuscation
{
    [Serializable]
    public class CustomInstructionData
    {
        public VOpCode OpCode;
        public VOpCode WrittenOpCode;

        public bool Mutated = false;
        public bool Serialize = true;

        public CustomInstructionData()
        {
        }
    };
};