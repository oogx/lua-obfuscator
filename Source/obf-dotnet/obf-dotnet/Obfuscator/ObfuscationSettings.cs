namespace Obfuscator.Obfuscation
{
    public class ObfuscationSettings
    {
        public const int ConstantOffset = 1048;
        public const int MaximumVirtuals = 250;
        public const int MaximumSuperOperatorLength = 50;
        public const int MaximumSuperOperators = 120;

        public bool DisableSuperOperators = false;
        public bool MaximumSecurityEnabled = false;
        public bool ControlFlowObfuscation = true;
        public bool ConstantEncryption = false;
        public bool EncryptAllStrings = false;
        public bool DisableAllMacros = false;
        public bool EnhancedOutput = false;
        public bool EnhancedConstantEncryption = false;
        public bool CompressedOutput = false;
        public bool PremiumFormat = false;
        public bool AntiEQHook = false;
        public int FakeConstants = 2000;
        public bool DebugMode = false;

        public string ByteCodeMode = "Default";

        public ObfuscationSettings()
        {
        }

        public ObfuscationSettings(ObfuscationSettings ObfuscationSettings)
        {
            this.DisableSuperOperators = ObfuscationSettings.DisableSuperOperators;
            this.MaximumSecurityEnabled = ObfuscationSettings.MaximumSecurityEnabled;
            this.ControlFlowObfuscation = ObfuscationSettings.ControlFlowObfuscation;
            this.ConstantEncryption = ObfuscationSettings.ConstantEncryption;
            this.EncryptAllStrings = ObfuscationSettings.EncryptAllStrings;
            this.DisableAllMacros = ObfuscationSettings.DisableAllMacros;
            this.EnhancedOutput = ObfuscationSettings.EnhancedOutput;
            this.EnhancedConstantEncryption = ObfuscationSettings.EnhancedConstantEncryption;
            this.CompressedOutput = ObfuscationSettings.CompressedOutput;
            this.PremiumFormat = ObfuscationSettings.PremiumFormat;
            this.ByteCodeMode = ObfuscationSettings.ByteCodeMode;
            this.AntiEQHook = ObfuscationSettings.AntiEQHook;
        }
    };
};