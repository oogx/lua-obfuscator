using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Obfuscator;
using Obfuscator.Obfuscation;
namespace DiscordBot {
    public class Program {
        static void Main(string[] Arguments) => new Program().Main().GetAwaiter().GetResult();
        public static bool Debug = false;
        private static Encoding LuaEncoding = Encoding.GetEncoding(28591);
        public async Task Main() {
            StartProcess();
        }
        public struct ObfuscationProcess { public string UserId; public Obfuscator.Obfuscator Obfuscator; public bool Obfuscating; public Process Process; public bool PrivateUser; public bool PublicUser; };
        public struct UserDataTemplate { public long Time; public short Obfuscations; public long ObfuscationTime; };
        public static Dictionary<string, ObfuscationProcess> Processes = new Dictionary<string, ObfuscationProcess>();
        public static Dictionary<ulong, UserDataTemplate> UserData = new Dictionary<ulong, UserDataTemplate>();
        private async Task StartProcess() {
            string FakeId = "";
            string filePath = @"";
            string Settings = @"Storage\Settings.txt";

            if (File.Exists(Settings)) {
                string[] lines = File.ReadAllLines(Settings);
                foreach (string line in lines) {
                    if (line.Contains("Discord_Id")) {
                        FakeId = line.Split('"')[1];
                    }
                    if (line.Contains("File_Path")) {
                        filePath = line.Split('"')[1];
                    }
                }
            }

            Program.Processes[FakeId] = (new Program.ObfuscationProcess { UserId = FakeId, PrivateUser = true });
            if (true) {
                ObfuscationProcess ObfuscationProcess = Processes[FakeId];
                if (ObfuscationProcess.Obfuscating) { return; };
                ObfuscationProcess.Obfuscating = true;
                async void StartObfuscationProcess() {
                    string Content = "";
                    Content = File.ReadAllText(filePath);
                    string Folder = $@"Storage\Obfuscation\{FakeId}";
                    if (Directory.Exists(Folder)) { Directory.Delete(Folder, true); };
                    Directory.CreateDirectory(Folder);
                    try {
                        string Input = Path.Combine(Folder, "Input.lua");
                        File.WriteAllText(Input, Content, LuaEncoding);
                        FileInfo FileInfo = new FileInfo(Input);
                        ObfuscationSettings ObfuscationSettings;
                        ObfuscationSettings = new ObfuscationSettings {
                            EncryptAllStrings = false,
                            DisableSuperOperators = true,
                            ControlFlowObfuscation = false,
                            ConstantEncryption = false,
                            MaximumSecurityEnabled = false,
                            DisableAllMacros = true,
                            EnhancedOutput = false,
                            PremiumFormat = false,
                            ByteCodeMode = "Default"
                        };
                        ObfuscationSettings.DebugMode = Debug;
                        ObfuscationProcess.Obfuscator = new Obfuscator.Obfuscator(ObfuscationSettings, Folder);
                        async void Obfuscate() {
                            string ErrorMessage = "";
                            ObfuscationProcess.Obfuscator.Compile(out ErrorMessage);
                            ObfuscationProcess.Obfuscator.Deserialize(out ErrorMessage);
                            ObfuscationProcess.Obfuscator.Obfuscate(out ErrorMessage);
                        }
                        Thread Thread = new Thread(Obfuscate); Thread.Start();
                    }
                    catch (Exception Exception) { Console.WriteLine(Exception); return; };
                }
                new Thread(StartObfuscationProcess).Start();
            };
        }
    };
};