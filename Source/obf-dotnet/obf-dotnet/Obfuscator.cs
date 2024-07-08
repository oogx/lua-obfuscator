// -------------------------------------------------- //

using GParse;
using GParse.Collections;
using GParse.Lexing;

// -------------------------------------------------- //

using Loretta;
using Loretta.Lexing;
using Loretta.Parsing;
using Loretta.Parsing.Visitor;
using Obfuscator.Bytecode;
using Obfuscator.Bytecode.IR;

// -------------------------------------------------- //

using Obfuscator.Extensions;
using Obfuscator.Obfuscation;
using Obfuscator.Obfuscation.Generation;
using Obfuscator.Obfuscation.Generation.Macros;
using Obfuscator.Obfuscation.OpCodes;
using Obfuscator.Obfuscation.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Obfuscator
{
    public class Obfuscator
    {
        private Encoding LuaEncoding = Encoding.GetEncoding(28591);
        private Random Random = new Random();

        private ObfuscationContext ObfuscationContext;
        private ObfuscationSettings ObfuscationSettings;
        private Chunk HeadChunk;

        private string Location = "";

        public bool Obfuscating = true;

        public Obfuscator(ObfuscationSettings ObfuscationSettings, string Location)
        {
            this.ObfuscationSettings = ObfuscationSettings; this.Location = Location;
        }

        // -------------------------------------------------- //

        private bool IsUsed(Chunk Chunk, VOpCode Virtual)
        {
            bool Return = false; foreach (Instruction Instruction in Chunk.Instructions) { if (Virtual.IsInstruction(Instruction)) { if (!ObfuscationContext.InstructionMapping.ContainsKey(Instruction.OpCode)) { ObfuscationContext.InstructionMapping.Add(Instruction.OpCode, Virtual); }; Instruction.CustomInstructionData = new CustomInstructionData { OpCode = Virtual }; Return = (true); }; }; foreach (Chunk sChunk in Chunk.Chunks) { Return |= IsUsed(sChunk, Virtual); }; return (Return);
        }

        // -------------------------------------------------- //

        public string ObfuscateString(string Source)
        {
            if (!Directory.Exists(Location)) { Directory.CreateDirectory(Location); };
            File.WriteAllText(Path.Combine(Location, "Input.lua"), Source);
            File.WriteAllText(Path.Combine(Location, "Output.lua"), "");

            Obfuscator Obfuscator = new Obfuscator(new ObfuscationSettings(ObfuscationSettings), Location);
            string Error = "";

            Obfuscator.Compile(out Error);
            if (Error.Length > 0)
	            throw new SystemException(Error);
            Obfuscator.Deserialize(out Error);
            if (Error.Length > 0)
	            throw new SystemException(Error);
            Obfuscator.Obfuscate(out Error);
            if (Error.Length > 0)
	            throw new SystemException(Error);

            //try { if (!Obfuscator.Compile(out Error)) { return (""); }; } catch { Console.WriteLine("1 " + Error); return (""); };
            //try { if (!Obfuscator.Deserialize(out Error)) { return (""); }; } catch { Console.WriteLine("2 " + Error); return (""); };
            //if (!Obfuscator.Obfuscate(out Error)) { return (""); };

            // string String = File.ReadAllText(Path.Combine(Location, "Output.lua"));

            // Directory.Delete(Path.Combine(Location, "Process"), true);

            return File.ReadAllText(Path.Combine(Location, "Output.lua"));
        }

        // -------------------------------------------------- //
        

        public bool Compile(out string Error)
        {
            Error = "";
            bool b = false;

            if (!Directory.Exists(Location)) { Error = "[Error #1] File Directory Does Not Exist!"; return (false); };
            if (!File.Exists(Path.Combine(Location, "Input.lua"))) { Error = "[Error #2] Input File Does Not Exist!"; return (false); };
            string Input = Path.Combine(Location, "Input.lua");
            string ByteCode = Path.Combine(Location, "LuaC.out");
            string ExtraHeader = @"local numberoffakes =" + $"{ObfuscationSettings.FakeConstants}" + ( @"
local fakes = {'DefaultChatSystemChatEvents','secrun','is_beta','secure_call','cache_replace','get_thread_identity','request','protect_gui','run_secure_lua','cache_invalidate','queue_on_teleport','is_cached','set_thread_identity','write_clipboard','run_secure_function','crypto','websocket','unprotect_gui','create_secure_function','crypt','syn','request','SayMessageRequest','FireServer','InvokeServer','tick','pcall','spawn','print','warn','game','GetService','getgc','getreg','getrenv','getgenv','debug','require','ModuleScript','LocalScript','GetChildren','GetDescendants','function','settings','GameSettings','RenderSettings','string','sub','service','IsA','Parent','Name','RunService','Stepped','wait','Changed','FindFirstChild','Terrain','Lighting','Enabled','getconnections','firesignal','workspace','true','false','tostring','table','math','random','floor','Humanoid','Character','LocalPlayer','plr','Players','Player','WalkSpeed','Enum','KeyCode','_G','BreakJoints','Health','Chatted','RemoteEvent','RemoteFunction','getrawmetatable','make_writable','setreadonly','PointsService','JointsService','VRService','Ragdoll','SimulationRadiusLocaleId','gethiddenproperty','sethiddenproperty','syn','Zombies','GameId','JobId','Tool','Accessory','RightGrip','Weld','HumanoidRootPart','GuiService','CoreGui','BindableEvent','fire','BodyForce','Chat','PlayerGui','NetworkMarker','Geometry','TextService','LogService','error','LuaSettings','UserInputService','fireclickdetector','Trail','Camera','CurrentCamera','FOV','Path','InputObject','Frame','TextBox','ScreenGui','hookfunction','Debris','ReplicatedStorage','ReplicatedFirst','decompile','saveinstance','TweenService','SoundService','Teams','Tween','BasePart','Seat','Decal','Instance','new','Ray','TweenInfo','Color3','CFrame','Vector3','Vector2','UDim','UDim2','NumberRange','NumberSequence','Handle','Gravity','HopperBin','Shirt','Pants','Mouse','IntValue','StringValue','Value','VirtualUser','MouseButton1Click','Activated','FileMesh','TeleportService','Teleport','userdata','string','int','number','bool','BodyGyro','Backpack','SkateboardPlatform','FilteringEnabled','Shoot','Shell','Asset','checkifgay','create','god','BrianSucksVexu','checkifalive','getteams','getnearest','getcross','autoshoot','chatspam','changeupvalues','modifyguns','infammo','godmode','aimbot','esp','crashserver','antiaim','usertype','type'}
local faked = {}
for i = 1,numberoffakes do
table.insert(faked, 'a34534345 = \'' ..tostring(fakes[math.random(1,#fakes)])..'\'')
end
table.concat(faked,'\n')");
            if (ObfuscationSettings.AntiEQHook)
            {
            }
            // ExtraHeader += $"print('hi2')";
            string src = File.ReadAllText(Input);
            Utility.Utility.GetExtraStrings(src);
            File.WriteAllText(Input, ExtraHeader + "\n" + src);

            if (!ObfuscationSettings.DisableAllMacros)
            {
                LuaOptions LuaOptions = new LuaOptions(true, true, true, false, true, true, true, true, true,
                    true, true, true, ContinueType.ContextualKeyword);

                LuaLexerBuilder LexerBuilder = new LuaLexerBuilder(LuaOptions);
                LuaParserBuilder ParserBuilder = new LuaParserBuilder(LuaOptions);

                var Diagnostics = new DiagnosticList();
                var Lexer = LexerBuilder.CreateLexer(File.ReadAllText(Input), Diagnostics);
                var TokenReader = new TokenReader<LuaTokenType>(Lexer);
                var Parser = ParserBuilder.CreateParser(TokenReader, Diagnostics);
                var Tree = Parser.Parse();

                if (Diagnostics.Any(Diagnostic => Diagnostic.Severity == DiagnosticSeverity.Error))
                {
	                Diagnostic FirstError =
		                Diagnostics.First(Diagnostic => Diagnostic.Severity == DiagnosticSeverity.Error);
	                string ActualRange =
		                $"{FirstError.Range.Start.Line - 7}:{FirstError.Range.Start.Column} - {FirstError.Range.End.Line - 7}:{FirstError.Range.End.Column}";
                    Error = $"[{FirstError.Id}] {FirstError.Description} @ {ActualRange}";
                    return (false);
                }

                

                File.WriteAllText(Input,
                    contents: FormattedLuaCodeSerializer.Format(LuaOptions.All, Tree, ObfuscateString,
                        ObfuscationSettings.EncryptAllStrings, Location, ObfuscationSettings.PremiumFormat));
            }

            ;

           
            string FileNameP = $"Lua/LuaC.exe";
            OperatingSystem os = Environment.OSVersion;

            //Console.WriteLine(os.Platform);

            if (os.Platform.ToString() == "Unix")
                FileNameP="luac";


            Process Process = new Process
            {
                StartInfo =
                {
                    FileName = FileNameP, Arguments = "-o \"" + ByteCode + "\" \"" + Input + "\"",
                    UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true
                }
            };
            Process.Start();
            Process.WaitForExit();
            Console.WriteLine(Process.ExitCode);
            if (Process.ExitCode >= 1)
            {
	            Error = Process.StandardError.ReadToEnd();
	            return (false);
            }
            if (!File.Exists(String.Format("{0}/LuaC.out", Location)))
            {
                Error = "[Error #8] LuaC Has no output (Syntax Error?)";
                return (false);
            }

            if (!Obfuscating)
            {
                Error = "[?] Process Terminated.";
                return (false);
            }

            ;
            if (!File.Exists(ByteCode))
            {
                Error = "[Error #3] Lua Error: Error While Compiling Script! (Syntax Error?)";
                return (false);
            }

            b = true;

            return b;
        }

        public bool Deserialize(out string Error)
        {
            Error = "";

            Deserializer Deserializer = new Deserializer(File.ReadAllBytes(Path.Combine(Location, "LuaC.out")));
            try { HeadChunk = Deserializer.DecodeFile(); } catch { Error = "[Error #4] Error While Deserializing File!"; return (false); };

            if (!Obfuscating) { Error = "[?] Process Terminated."; return (false); };

            ObfuscationContext = new ObfuscationContext(HeadChunk);
            ObfuscationContext.Obfuscator = this;

            return (true);
        }

        public bool Obfuscate(out string Error)
        {
            Error = "";

            List<VOpCode> AdditionalVirtuals = new List<VOpCode>();

            if (!ObfuscationSettings.DisableAllMacros)
                (new BytecodeSecurity(HeadChunk, ObfuscationSettings, ObfuscationContext, AdditionalVirtuals)).DoChunks();

            if (!ObfuscationSettings.DisableAllMacros)
                (new LuaMacros(HeadChunk, AdditionalVirtuals)).DoChunks();

            // Shuffle Control Flow Blocks:
            void ShuffleControlFlow(Chunk Chunk)
            {
                foreach (Chunk SubChunk in Chunk.Chunks) { ShuffleControlFlow(SubChunk); };

                List<BasicBlock> BasicBlocks = new BasicBlock().GenerateBasicBlocks(Chunk);
                Instruction EntryPoint = Chunk.Instructions.First();

                Dictionary<int, BasicBlock> BlockMap = new Dictionary<int, BasicBlock>();

                BasicBlocks.Shuffle();

                int InstructionPoint = 0;

                foreach (BasicBlock Block in BasicBlocks)
                {
                    foreach (Instruction Instruction in Block.Instructions)
                    {
                        BlockMap[InstructionPoint] = Block;

                        InstructionPoint++;
                    };
                };

                foreach (BasicBlock Block in BasicBlocks)
                {
                    if (Block.Instructions.Count == 0) { continue; }; // ???

                    Instruction Instruction = Block.Instructions.Last();

                    switch (Instruction.OpCode)
                    {
                        case (OpCode.OpForPrep):
                        case (OpCode.OpForLoop):
                            {
                                Block.Instructions.Add(new Instruction(Chunk, OpCode.OpJump, Block.References[0].Instructions[0]));

                                break;
                            };

                        case (OpCode.OpEq):
                        case (OpCode.OpLt):
                        case (OpCode.OpLe):
                        case (OpCode.OpTest):
                        case (OpCode.OpTestSet):
                        case (OpCode.OpTForLoop):
                            {
                                Block.Instructions.Add(new Instruction(Chunk, OpCode.OpJump, Block.References[0].Instructions[0]));

                                break;
                            };

                        case (OpCode.OpReturn): { break; };

                        case (OpCode.OpJump): { break; };

                        default:
                            {
                                Block.Instructions.Add(new Instruction(Chunk, OpCode.OpJump, Block.References[0].Instructions[0]));

                                break;
                            };
                    };
                };

                Chunk.Instructions.Clear();

                Chunk.Instructions.Add(new Instruction(Chunk, OpCode.OpJump, EntryPoint));

                foreach (BasicBlock Block in BasicBlocks)
                {
                    foreach (Instruction Instruction in Block.Instructions)
                    {
                        Chunk.Instructions.Add(Instruction);
                    };
                };

                Chunk.UpdateMappings();
            };

            if (ObfuscationSettings.ControlFlowObfuscation) { ShuffleControlFlow(HeadChunk); };

            List<VOpCode> Virtuals = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.IsSubclassOf(typeof(VOpCode))).Select(Activator.CreateInstance).Cast<VOpCode>().Where(T => IsUsed(HeadChunk, T)).ToList();
            foreach (VOpCode Virtual in AdditionalVirtuals) { Virtuals.Add(Virtual); };

            //if (ObfuscationSettings.ConstantEncryption)
            //    (new ConstantEncryption(ObfuscationContext, ObfuscationSettings, HeadChunk)).EncryptAllConstants(Virtuals);

            if (!ObfuscationSettings.DisableSuperOperators)
                (new SuperOperators()).DoChunk(HeadChunk, Virtuals);

            Virtuals.Shuffle(); for (int I = 0; I < Virtuals.Count; I++) { Virtuals[I].VIndex = I; };

            //////////////////////////////////////////////////

            if (ObfuscationSettings.PremiumFormat)
            {
                Utility.Utility.NoExtraString = true;
                PremiumScriptBuilder ScriptBuilder = new PremiumScriptBuilder(HeadChunk, ObfuscationContext, ObfuscationSettings, Virtuals);
                string Source = ScriptBuilder.BuildScript(Location);
            }
            else
            {
                ScriptBuilder ScriptBuilder = new ScriptBuilder(HeadChunk, ObfuscationContext, ObfuscationSettings, Virtuals);
                string Source = ScriptBuilder.BuildScript(Location);
            };

            return (true);
        }
    };
};