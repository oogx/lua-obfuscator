using Obfuscator.Obfuscation;
using System;
using System.IO;
using System.Linq;

namespace obf_cli
{
    internal class Program
    {
        private static bool DEBUGMODE = false;

        private static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            if (DEBUGMODE)
            {
                var settings = new ObfuscationSettings();
                //settings.PremiumFormat = true;
                settings.DebugMode = false;
                //settings.CompressedOutput = true;
                settings.MaximumSecurityEnabled = true;

                var obf = new Obfuscator.Obfuscator(settings, Directory.GetCurrentDirectory());

                string ret = obf.ObfuscateString(File.ReadAllText("Process/realinput2.lua"));
                //Console.WriteLine(ret);
            }
            else
            {
                string[] options =
                {
                    "DisableSuperOperators",
                    "MaximumSecurityEnabled",
                    "ControlFlowObfuscation",
                    "ConstantEncryption",
                    "EncryptAllStrings",
                    "DisableAllMacros",
                    "EnhancedOutput",
                    "EnhancedConstantEncryption",
                    "CompressedOutput",
                    "PremiumFormat",
                    "ByteCodeMode",
                    "FakeConstants",
                    "AntiEQHook",
                };

                var settings = new ObfuscationSettings();

                bool wanthelp = false;
                if (args.Length >= 1)
                {
                    wanthelp = args[0] == "-h" || args[0] == "--help" || args[0] == "-?";
                }
                bool missingargs = args.Length < 2 + options.Length;

                if (missingargs || wanthelp)
                {
                    if (missingargs && !wanthelp)
                        Console.WriteLine("ERR: expected " + (3 + options.Length).ToString() + " arguments");

                    Console.WriteLine("usage: obf-cli [input file] [directory] [opts]");
                    Console.WriteLine("options (in order) (bool and default = false unless otherwise specified):");

                    foreach (string opt in options)
                    {
                        Console.Write("\t" + opt);
                        var type = typeof(ObfuscationSettings);
                        var field = type.GetField(opt);
                        if (field.FieldType.Name != "Boolean")
                        {
                            Console.Write(" (" + field.FieldType.Name + ", default = \"" + field.GetValue(settings).ToString() + "\")");
                        }
                        else if (field.GetValue(settings).Equals(true))
                        {
                            Console.Write(" (default = true)");
                        }
                        Console.WriteLine();
                    }
                    return;
                }

                string file = args[0];
                string dir = args[1];

                int i = 0;



                foreach (string opt in options)
                {
                    string argin = args[2 + i];
                    var type = typeof(ObfuscationSettings);
                    var field = type.GetField(opt);

                    if (field.FieldType.Name == "String")
                    {
                        field.SetValue(settings, argin);
                    }
                    else if (field.FieldType.Name == "Boolean")
                    {
                        field.SetValue(settings, argin == "true");
                    }
                    else if (field.FieldType.Name == "Int32") 
                    {
                        Int32 n;
                        try {
                            n = Int32.Parse(argin);
                        
                        } catch {n = 2000;}
                        field.SetValue(settings, n);
                    }
                    else
                    {
                        Console.Error.WriteLine("no handler for option type: " + field.FieldType.Name);
                    }


                    i++;
                }

                //settings.DebugMode = true;
                Console.WriteLine(settings.AntiEQHook);
                Console.WriteLine(settings.PremiumFormat);
                var obf = new Obfuscator.Obfuscator(settings, dir);
                obf.ObfuscateString(File.ReadAllText(file));
                if (args.Contains("-D"))
                        Console.WriteLine("Obfuscation Complete");
            }
        }
    }
}