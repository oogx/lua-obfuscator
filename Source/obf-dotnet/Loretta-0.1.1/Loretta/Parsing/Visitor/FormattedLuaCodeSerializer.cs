using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using GParse;
using GParse.IO;
using GParse.Lexing;

using Loretta;
using Loretta.Lexing;
using Loretta.Parsing;
using Loretta.Utilities;

using Loretta.Parsing.AST;
using Loretta.Parsing.AST.Tables;

using System.Reflection;

using Loretta.ThirdParty;
//using Loretta.ThirdParty.Utility;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

namespace Loretta.Parsing.Visitor
{
    public class FormattedLuaCodeSerializer : ITreeVisitor
    {
        private Boolean MaxSecurityEnabled = false;
        private Boolean EncryptAllStrings = false;
        private Boolean ObfuscatedExpressions = false;
        private Boolean PremiumFormat = false;

        private Random Random = new Random ( );

        private Encoding LuaEncoding = Encoding.GetEncoding ( 28591 );

        private String Location = "";

        #region Variable Tracing

        public List<List<String>> Stack = new List<List<String>> { new List<String> ( ) };
        public List<Dictionary<String, String>> Labels = new List<Dictionary<String, String>> { new Dictionary<String, String> ( ) };

        public List<String> LastStack = new List<String> ( );
        public Dictionary<String, String> LastLabels = new Dictionary<String, String> ( );

        private void AddLocalVaraible ( String Variable ) { if ( !this.Stack.Last ( ).Contains ( Variable ) ) { this.Stack.Last ( ).Add ( Variable ); }; }

        private void HandleIdentifier ( IdentifierExpression IdentifierExpression ) { ( this.Labels.Last ( ) )[IdentifierExpression.Identifier] = IdentifierExpression.Identifier; }

        private void IncreaseStack ( ) { this.Stack.Add ( new List<String> ( this.Stack.Last ( ) ) ); this.Labels.Add ( new Dictionary<String, String> ( ) ); }

        private void DecreaseStack ( )
        {
            this.LastStack = this.Stack.Last ( );
            this.LastLabels = this.Labels.Last ( );

            this.Stack.Remove ( this.Stack.Last ( ) );
            this.Labels.Remove ( this.Labels.Last ( ) );

            foreach ( var Label in this.LastLabels.Keys )
                ( this.Labels.Last ( ) )[Label] = Label;
        }

        #endregion

        #region String Encryption

        public class StringDecryptor
        {
            public Int32[] Table;

            public String Encrypt ( Byte[] Bytes ) { var Encrypted = new List<Byte> ( ); var Length = this.Table.Length; for ( var Index = 0; Index < Bytes.Length; Index++ ) { Encrypted.Add ( ( Byte ) ( Bytes[Index] ^ this.Table[Index % Length] ) ); }; return $@"(((function(F) local cTable = setmetatable({{}}, {{ __tostring = (function(...) while (true) do end; end); __concat = (function(...) return (...); end); }}); local BitXOR = ((function(A, B) local P, C = 1, 0; while ((A > 0) and (B > 0)) do local X, Y = A % 2, B % 2; if X ~= Y then C = C + P; end; A, B, P = (A - X) / 2, (B - Y) / 2, P * 2; end; if A < B then A = B; end; while A > 0 do local X = A % 2; if X > 0 then C = C + P; end; A, P =(A - X) / 2, P * 2; end; return (C); end)); local Char = (string.char); local Dictionary = ({{}}); for Index = 0, 255 do local Value = Char(Index); Dictionary[Index] = Value; Dictionary[Value] = Index; end; local A, B = (""""), (""{String.Join ( "", this.Table.Select ( T => "\\" + T.ToString ( ) ) )}""); local C = (string.sub); for D = 1, #F do local E = (D - 1) % {this.Table.Length} + 1; A = A .. Dictionary[BitXOR(Dictionary[C(F, D, D)], Dictionary[C(B, E, E)])] .. cTable; end; return (A); end))(""{String.Join ( "", Encrypted.Select ( T => "\\" + T.ToString ( ) ) )}""))"; }

            public StringDecryptor ( Int32 MaxLength ) { var Random = new Random ( ); this.Table = Enumerable.Repeat ( 0, MaxLength ).Select ( I => Random.Next ( 0, 256 ) ).ToArray ( ); }

            public static String EncryptString ( String String ) => new StringDecryptor ( String.Length ).Encrypt ( Encoding.GetEncoding ( 28591 ).GetBytes ( String ) );
        };

        #endregion

        private CodeWriter Writer;

        public LuaOptions LuaOptions { get; }

        public FormattedLuaCodeSerializer ( LuaOptions LuaOptions, String Indentation = "\t" ) { this.Writer = new CodeWriter ( Indentation ); this.LuaOptions = LuaOptions; }

        #region Code Serialization Helpers

        private void WriteStatementLineEnd ( Statement Statement ) { if ( Statement.Semicolon is Token<LuaTokenType> ) { this.Writer.WriteLine ( ";" ); } else { this.Writer.WriteLine ( ); }; }

        private void WriteSeparatedNodeList<T> ( String Separator, ImmutableArray<T> Nodes ) where T : LuaASTNode { for ( var I = 0; I < Nodes.Length; I++ ) { this.VisitNode ( Nodes[I] ); if ( I != Nodes.Length - 1 ) { this.Writer.Write ( Separator ); }; }; }

        #endregion Code Serialization Helpers

        #region ITreeVisitor

        public virtual void VisitNode ( LuaASTNode Node ) => Node.Accept ( this );

        #region Expressions

        private Dictionary<String, String> BitwiseOperators = new Dictionary<String, String> { ["&"] = "band", ["|"] = "bor", [">>"] = "rshift", ["<<"] = "lshift", ["~"] = "bxor" };
        private Dictionary<String, String> CompoundOperators = new Dictionary<String, String> { ["+="] = "+", ["-="] = "-", ["*="] = "*", ["/="] = "/", ["^="] = "^", ["%="] = "%", ["..="] = "..", ["&="] = "&", ["|="] = "|", [">>="] = ">>", ["<<="] = "<<", ["//="] = "//" };

        private List<String> inlinedFunctions = new List<String>();
        private Dictionary<String, FunctionDefinitionStatement> definedFunctions = new Dictionary<String, FunctionDefinitionStatement>();
        public virtual void VisitBinaryOperation ( BinaryOperationExpression BinaryOperation )
        {
            if ( ( String ) BinaryOperation.Operator.Value == "//" ) { this.Writer.Write ( "math.floor(" ); this.VisitNode ( BinaryOperation.Left ); this.Writer.Write ( " / " ); this.VisitNode ( BinaryOperation.Right ); this.Writer.Write ( ")" ); return; }
            if ( this.BitwiseOperators.ContainsKey ( ( String ) BinaryOperation.Operator.Value ) ) { this.Writer.Write ( $"((bit) or (bit32)).{this.BitwiseOperators[( String ) BinaryOperation.Operator.Value]}(" ); this.VisitNode ( BinaryOperation.Left ); this.Writer.Write ( ", " ); this.VisitNode ( BinaryOperation.Right ); this.Writer.Write ( ")" ); return; };

            this.VisitNode ( BinaryOperation.Left );
            this.Writer.Write ( ' ' );
            this.Writer.Write ( BinaryOperation.Operator.Value );
            this.Writer.Write ( ' ' );
            this.VisitNode ( BinaryOperation.Right );
        }

        public virtual void VisitBoolean ( BooleanExpression BooleanExpression ) => this.Writer.Write ( BooleanExpression.Value ? "true" : "false" );

        public virtual void VisitFunctionCall ( FunctionCallExpression Node )
        {
            if ( Node.Function is IdentifierExpression IdentifierExpression )
            {
                if ( IdentifierExpression.Identifier == "PSU_CRASH" ) { this.Writer.Write ( "PSU_WRAP(function(...) while (true) do end; repeat until (false); for _ = 0, 1, 0 do end; end)()" ); return; }
                else if ( IdentifierExpression.Identifier == "PSU_ENCRYPT_STRING" ) { if ( Node.Arguments.Count ( ) > 0 ) { var Encrypted = new List<String> ( ); foreach ( Expression Expression in Node.Arguments ) { if ( Expression is StringExpression stringExpression ) { var String = stringExpression.Value; var Bytes = this.LuaEncoding.GetBytes ( String ); Encrypted.Add ( new StringDecryptor ( String.Length ).Encrypt ( Bytes ) ); }; }; this.Writer.Write ( $"PSU_WRAP(function(...) return {String.Join ( ", ", Encrypted )}; end)()" ); return; }; }
                else if ( IdentifierExpression.Identifier == "PSU_MAX_SECURITY_START" ) { this.MaxSecurityEnabled = true; }
                else if ( IdentifierExpression.Identifier == "PSU_MAX_SECURITY_END" ) { this.MaxSecurityEnabled = false; }
                else if ( IdentifierExpression.Identifier == "PSU_EXECUTE" )
                {
                    if ( Node.Arguments.Count ( ) > 0 )
                    {
                        if ( Node.Arguments.First ( ) is StringExpression StringExpression )
                        {
                            this.Writer.Write ( $"PSU_WRAP(function(...) {this.ObfuscateString ( StringExpression.Value )} end)" );

                            return;
                        };
                    };
                }
                else if ( IdentifierExpression.Identifier == "PSU_LOADSTRING" )
                {
                    if ( Node.Arguments.Count ( ) > 0 )
                    {
                        if ( Node.Arguments.First ( ) is AnonymousFunctionExpression AnonymousFunction )
                        {
                            var Serializer = new FormattedLuaCodeSerializer ( this.LuaOptions, "\t" );
                            Serializer.Stack.Add ( new List<String> ( this.Stack.Last ( ) ) );
                            Serializer.ObfuscatedExpressions = true;

                            Serializer.VisitNode ( AnonymousFunction.Body );

                            var UsedLabels = new List<String> ( );

                            foreach ( var Label in Serializer.Labels.Last ( ).Keys )
                            {
                                if ( this.Stack.Last ( ).Contains ( Label ) )
                                    UsedLabels.Add ( Label );
                            };

                            var Body = $"return(function(...){Serializer.ToString ( )}end);";
                            if ( UsedLabels.Count > 0 )
                                Body = $"local {String.Join ( ",", UsedLabels )}=...;" + Body;

                            File.WriteAllText ( Path.Combine ( this.Location, "VM.lua" ), Body );
                            File.WriteAllText ( Path.Combine ( this.Location, "Output.lua" ), Body );

                            var Process = new Process { StartInfo = { FileName = $"Lua/LuaJIT.exe", Arguments = "Lua/lua/LuaSrcDiet.lua --maximum --opt-entropy --opt-emptylines --opt-eols --opt-numbers --opt-whitespace --opt-locals --noopt-strings \"" + Path.Combine ( this.Location, "VM.lua" ) + "\" -o \"" + Path.Combine ( this.Location, "Output.lua" ) + "\"", UseShellExecute = false, RedirectStandardError = false, RedirectStandardOutput = false } };
                            Process.Start ( ); Process.WaitForExit ( ); if ( !File.Exists ( Path.Combine ( this.Location, "Output.lua" ) ) ) { return; };

                            Body = File.ReadAllText ( Path.Combine ( this.Location, "Output.lua" ), this.LuaEncoding ).Replace ( "\n", " " );

                            this.Writer.Write ( $"loadstring([=====[{Body}]=====])({String.Join ( ",", UsedLabels )})" );

                            return;
                        };
                    };
                }
                else if ( IdentifierExpression.Identifier == "PSU_INLINE_FUNCTION" )
                {
                    if ( Node.Arguments.Count ( ) > 0 )
                    {
                        if ( Node.Arguments.First ( ) is IdentifierExpression idExpression )
                        {
                            this.inlinedFunctions.Add ( idExpression.Identifier );
                            return;
                        }
                    }
                }
                else if (this.inlinedFunctions.Contains(IdentifierExpression.Identifier))
                {
                    if (this.definedFunctions.TryGetValue(IdentifierExpression.Identifier, out FunctionDefinitionStatement func))
                    {
                        var Serializer = new FormattedLuaCodeSerializer ( this.LuaOptions, "\t" );
                        Serializer.Stack.Add ( new List<String> ( this.Stack.Last ( ) ) );
                        Serializer.ObfuscatedExpressions = true;

                        Serializer.VisitNode ( func.Body );

                        var funcBody = Serializer.ToString ( );
                        var body = funcBody;
                        if (Node.Arguments.Length > 0)
                        {
                            body = "do\n";
                            var limit = Node.Arguments.Length > func.Arguments.Length ? func.Arguments.Length : Node.Arguments.Length;

                            if ( Node.Arguments.Length > 0 && Node.Arguments.Last ( ) is VarArgExpression varArg && func.Arguments.Length > Node.Arguments.Length )
                            {
                                var line = "local ";
                                for (var i = limit - 1; i < func.Arguments.Length; i++ )
                                {
                                    if ( func.Arguments[i] is IdentifierExpression funcArg )
                                    {
                                        line += funcArg.Identifier + ", ";
                                    }
                                }

                                if ( line.EndsWith ( ", " ) )
                                {
                                    line = line.Substring ( 0, line.Length - 2 );
                                    line += " = ...\n";
                                    limit--;
                                    body += line;
                                }
                            }

                            for (var i = 0; i < limit; i++ )
                            {
                                if (func.Arguments[i] is IdentifierExpression funcArg)
                                {
                                    var Serializer2 = new FormattedLuaCodeSerializer ( this.LuaOptions, "\t" );
                                    Serializer2.Stack.Add ( new List<String> ( this.Stack.Last ( ) ) );
                                    Serializer2.ObfuscatedExpressions = true;

                                    Serializer2.VisitNode ( Node.Arguments[i] );

                                    body += "local " + funcArg.Identifier + " = " + Serializer2.ToString ( ) + "\n";
                                }
                            }

                            body += funcBody;
                            body += "\nend";
                        }

                        this.Writer.Write ( body );
                        return;
                    }
                }
            };

            this.VisitNode ( Node.Function );
            this.Writer.Write ( " ( " );
            this.WriteSeparatedNodeList ( ", ", Node.Arguments );
            this.Writer.Write ( " )" );
        }

        public virtual void VisitGroupedExpression ( GroupedExpression Node ) { this.Writer.Write ( "( " ); this.VisitNode ( Node.InnerExpression ); this.Writer.Write ( " )" ); }

        public virtual void VisitIdentifier ( IdentifierExpression Identifier ) { this.Writer.Write ( Identifier.Identifier ); this.HandleIdentifier ( Identifier ); }

        public virtual void VisitIndex ( IndexExpression Node )
        {
            this.VisitNode ( Node.Indexee );

            switch ( Node.Type )
            {
                case IndexType.Indexer: { this.Writer.Write ( "[" ); this.VisitNode ( Node.Indexer ); this.Writer.Write ( "]" ); break; };
                case IndexType.Member: { this.Writer.Write ( "." ); this.VisitNode ( Node.Indexer ); break; };
                case IndexType.Method: { this.Writer.Write ( ":" ); this.VisitNode ( Node.Indexer ); break; };
            };
        }

        public virtual void VisitNil ( NilExpression NilExpression ) => this.Writer.Write ( "nil" );

        public virtual void VisitNumber ( NumberExpression NumberExpression )
        {
            this.Writer.Write ( NumberExpression.Tokens.Single ( ).Raw );
        }

        public virtual void VisitString ( StringExpression Node )
        {
            if ( this.EncryptAllStrings )
            {
                var String = Node.Value; var Bytes = this.LuaEncoding.GetBytes ( String );
                this.Writer.Write ( $"{new StringDecryptor ( String.Length ).Encrypt ( Bytes )}" );
                return;
            }
            else if ( this.ObfuscatedExpressions )
            {
                var RawString = Node.Value; var Bytes = this.LuaEncoding.GetBytes ( RawString );
                this.Writer.Write ( "\"" + ( RawString.Length > 0 ? "\\" : "" ) + $"{String.Join ( "\\", Bytes )}" + "\"" );
                return;
            };

            this.Writer.Write ( Node.Tokens.Single ( ).Raw );
        }

        public virtual void VisitTableConstructor ( TableConstructorExpression Node )
        {
            this.Writer.WriteLine ( "{" );
            this.Writer.WithIndentation ( ( ) => { for ( var I = 0; I < Node.Fields.Length; I++ ) { this.Writer.WriteIndentation ( ); this.VisitNode ( Node.Fields[I] ); this.Writer.WriteLine ( ); }; } );
            this.Writer.WriteIndented ( "}" );
        }

        public virtual void VisitTableField ( TableField Node )
        {
            switch ( Node.KeyType )
            {
                case TableFieldKeyType.Expression: { this.Writer.Write ( "[" ); this.VisitNode ( Node.Key! ); this.Writer.Write ( "] = " ); break; };
                case TableFieldKeyType.Identifier: { this.VisitNode ( Node.Key! ); this.Writer.Write ( " = " ); break; };
            };

            this.VisitNode ( Node.Value );

            if ( Node.Delimiter != default ) { this.Writer.Write ( Node.Delimiter.Raw ); };
        }

        public virtual void VisitUnaryOperation ( UnaryOperationExpression Node )
        {
            switch ( Node.Fix )
            {
                case UnaryOperationFix.Prefix:
                {
                    if ( ( String ) Node.Operator.Value == "~" ) { this.Writer.Write ( "((bit) or (bit32)).bnot(" ); this.VisitNode ( Node.Operand ); this.Writer.Write ( ")" ); break; };

                    this.Writer.Write ( Node.Operator.Value );

                    if ( StringUtils.IsIdentifier ( this.LuaOptions.UseLuaJitIdentifierRules, Node.Operator.Raw ) ) { this.Writer.Write ( ' ' ); };

                    this.VisitNode ( Node.Operand );

                    break;
                };

                case UnaryOperationFix.Postfix:
                {
                    this.VisitNode ( Node.Operand );

                    if ( StringUtils.IsIdentifier ( this.LuaOptions.UseLuaJitIdentifierRules, Node.Operator.Raw ) ) { this.Writer.Write ( ' ' ); };

                    this.Writer.Write ( Node.Operator.Value );

                    break;
                };
            };
        }

        public virtual void VisitVarArg ( VarArgExpression VarArg ) => this.Writer.Write ( "..." );

        #endregion Expressions

        public virtual void VisitAssignment ( AssignmentStatement AssignmentStatement )
        {
            this.Writer.WriteIndentation ( );

            this.WriteSeparatedNodeList ( ", ", AssignmentStatement.Variables );
            this.Writer.Write ( " = " );
            this.WriteSeparatedNodeList ( ", ", AssignmentStatement.Values );

            this.WriteStatementLineEnd ( AssignmentStatement );
        }

        public void VisitCompoundAssignmentStatement ( CompoundAssignmentStatement CompoundAssignmentStatement )
        {
            this.Writer.WriteIndentation ( );

            if ( CompoundAssignmentStatement.Assignee is IndexExpression CompoundIndexExpression )
            {
                this.Writer.Write ( " local VM_COMPOUND_ASSIGNMENT_INDEXEE = " );
                this.VisitNode ( CompoundIndexExpression.Indexee );
                this.Writer.Write ( " local VM_COMPOUND_ASSIGNMENT_INDEXER = " );
                this.VisitNode ( CompoundIndexExpression.Indexer );

                this.Writer.Write ( " VM_COMPOUND_ASSIGNMENT_INDEXEE[VM_COMPOUND_ASSIGNMENT_INDEXER] = " );

                if ( CompoundAssignmentStatement.OperatorToken.Id == "//=" )
                {
                    this.Writer.Write ( "math.floor(VM_COMPOUND_ASSIGNMENT_INDEXEE[VM_COMPOUND_ASSIGNMENT_INDEXER] / " );
                    this.VisitNode ( CompoundAssignmentStatement.ValueExpression );
                    this.Writer.Write ( ") " );

                    this.WriteStatementLineEnd ( CompoundAssignmentStatement );

                    return;
                }

                if ( this.BitwiseOperators.ContainsKey ( this.CompoundOperators[CompoundAssignmentStatement.OperatorToken.Id] ) )
                {
                    this.Writer.Write ( $"((bit) or (bit32)).{this.BitwiseOperators[this.CompoundOperators[CompoundAssignmentStatement.OperatorToken.Id]]}(VM_COMPOUND_ASSIGNMENT_INDEXEE[VM_COMPOUND_ASSIGNMENT_INDEXER], " );
                    this.VisitNode ( CompoundAssignmentStatement.ValueExpression );
                    this.Writer.Write ( ") " );

                    this.WriteStatementLineEnd ( CompoundAssignmentStatement );

                    return;
                }

                this.Writer.Write ( "VM_COMPOUND_ASSIGNMENT_INDEXEE[VM_COMPOUND_ASSIGNMENT_INDEXER]" );
                this.Writer.Write ( $" {this.CompoundOperators[CompoundAssignmentStatement.OperatorToken.Id]} " );
                this.VisitNode ( CompoundAssignmentStatement.ValueExpression );

                this.WriteStatementLineEnd ( CompoundAssignmentStatement );

                return;
            }
            else
            {
                if ( CompoundAssignmentStatement.OperatorToken.Id == "//=" )
                {
                    this.VisitNode ( CompoundAssignmentStatement.Assignee );
                    this.Writer.Write ( $" = math.floor(" );
                    this.VisitNode ( CompoundAssignmentStatement.Assignee );
                    this.Writer.Write ( " / " );
                    this.VisitNode ( CompoundAssignmentStatement.ValueExpression );
                    this.Writer.Write ( ") " );

                    this.WriteStatementLineEnd ( CompoundAssignmentStatement );

                    return;
                }

                if ( this.BitwiseOperators.ContainsKey ( this.CompoundOperators[CompoundAssignmentStatement.OperatorToken.Id] ) )
                {
                    this.VisitNode ( CompoundAssignmentStatement.Assignee );
                    this.Writer.Write ( $" = ((bit) or (bit32)).{this.BitwiseOperators[this.CompoundOperators[CompoundAssignmentStatement.OperatorToken.Id]]}(" );
                    this.VisitNode ( CompoundAssignmentStatement.Assignee );
                    this.Writer.Write ( ", " );
                    this.VisitNode ( CompoundAssignmentStatement.ValueExpression );
                    this.Writer.Write ( ") " );

                    this.WriteStatementLineEnd ( CompoundAssignmentStatement );

                    return;
                }

                this.VisitNode ( CompoundAssignmentStatement.Assignee );
                this.Writer.Write ( " = " );
                this.VisitNode ( CompoundAssignmentStatement.Assignee );
                this.Writer.Write ( $" {this.CompoundOperators[CompoundAssignmentStatement.OperatorToken.Id]} " );
                this.VisitNode ( CompoundAssignmentStatement.ValueExpression );

                this.WriteStatementLineEnd ( CompoundAssignmentStatement );

                return;
            }
        }

        public virtual void VisitBreak ( BreakStatement BreakStatement )
        {
            this.Writer.WriteIndented ( "break" );
            this.WriteStatementLineEnd ( BreakStatement );
        }

        public virtual void VisitContinue ( ContinueStatement ContinueStatement )
        {
            this.Writer.WriteIndented ( "continue" );
            this.WriteStatementLineEnd ( ContinueStatement );
        }

        public virtual void VisitDo ( DoStatement DoStatement )
        {
            this.Writer.WriteLineIndented ( "do" );

            this.IncreaseStack ( );

            this.Writer.WithIndentation ( ( ) => this.VisitNode ( DoStatement.Body ) );

            this.DecreaseStack ( );

            this.Writer.WriteIndented ( "end" );

            this.WriteStatementLineEnd ( DoStatement );
        }

        public virtual void VisitExpressionStatement ( ExpressionStatement ExpressionStatement )
        {
            this.Writer.WriteIndentation ( );
            this.VisitNode ( ExpressionStatement.Expression );
            this.WriteStatementLineEnd ( ExpressionStatement );
        }

        public virtual void VisitAnonymousFunction ( AnonymousFunctionExpression AnonymousFunction )
        {
            this.Writer.Write ( "function ( " );
            this.WriteSeparatedNodeList ( ", ", AnonymousFunction.Arguments );
            this.Writer.WriteLine ( " )" );

            this.IncreaseStack ( );

            foreach ( Expression Expression in AnonymousFunction.Arguments )
            {
                if ( Expression is IdentifierExpression Identifier )
                    this.AddLocalVaraible ( Identifier.Identifier );
            };

            this.Writer.WithIndentation ( ( ) => this.VisitNode ( AnonymousFunction.Body ) );
            this.Writer.WriteIndented ( "end" );

            this.DecreaseStack ( );
        }

        public virtual void VisitFunctionDefinition ( FunctionDefinitionStatement FunctionDeclaration )
        {
            if ( FunctionDeclaration.IsLocal )
            {
                this.IncreaseStack ( );
                this.AddLocalVaraible ( ( ( IdentifierExpression ) FunctionDeclaration.Name ).Identifier );
                this.Writer.WriteIndented ( "local function " );
            }
            else
            {
                this.IncreaseStack ( );
                this.Writer.WriteIndented ( "function " );
            };

            if (FunctionDeclaration.Name is IdentifierExpression nameExpression)
                this.definedFunctions.TryAdd ( nameExpression.Identifier, FunctionDeclaration );

            this.VisitNode ( FunctionDeclaration.Name );
            this.Writer.Write ( " ( " );
            this.WriteSeparatedNodeList ( ", ", FunctionDeclaration.Arguments );

            foreach ( Expression Expression in FunctionDeclaration.Arguments )
            {
                if ( Expression is IdentifierExpression Identifier )
                    this.AddLocalVaraible ( Identifier.Identifier );
            };

            this.Writer.WriteLine ( " ) " );
            this.Writer.WithIndentation ( ( ) => this.VisitNode ( FunctionDeclaration.Body ) );
            this.Writer.WriteIndented ( "end" );

            this.DecreaseStack ( );

            this.WriteStatementLineEnd ( FunctionDeclaration );
        }

        public virtual void VisitGotoLabel ( GotoLabelStatement GotoLabelStatement )
        {
            this.Writer.WriteIndented ( "::" );
            this.Writer.Write ( GotoLabelStatement.Label.Identifier );
            this.Writer.Write ( "::" );
            this.WriteStatementLineEnd ( GotoLabelStatement );
        }

        public virtual void VisitGoto ( GotoStatement GotoStatement )
        {
            this.Writer.WriteIndented ( "goto " );
            this.Writer.Write ( GotoStatement.Target.Identifier );
            this.WriteStatementLineEnd ( GotoStatement );
        }

        public virtual void VisitIfStatement ( IfStatement IfStatement )
        {
            for ( var Index = 0; Index < IfStatement.Clauses.Length; Index++ )
            {
                IfClause Clause = IfStatement.Clauses[Index];

                if ( Index == 0 ) { this.Writer.WriteIndented ( "if " ); }
                else { this.Writer.WriteIndented ( "elseif " ); }

                this.VisitNode ( Clause.Condition );

                this.Writer.WriteLine ( " then" );

                this.IncreaseStack ( );

                this.Writer.WithIndentation ( ( ) => this.VisitNode ( Clause.Body ) );

                this.DecreaseStack ( );
            };

            if ( IfStatement.ElseBlock is StatementList )
            {
                this.Writer.WriteLineIndented ( "else" );

                this.IncreaseStack ( );

                this.Writer.WithIndentation ( ( ) => this.VisitNode ( IfStatement.ElseBlock ) );

                this.DecreaseStack ( );
            };

            this.Writer.WriteIndented ( "end" );
            this.WriteStatementLineEnd ( IfStatement );
        }

        public virtual void VisitGenericFor ( GenericForLoopStatement GenericForLoop )
        {
            this.Writer.WriteIndented ( "for " );
            this.WriteSeparatedNodeList ( ", ", GenericForLoop.Variables );
            this.Writer.Write ( " in " );
            this.WriteSeparatedNodeList ( ", ", GenericForLoop.Expressions );
            this.Writer.WriteLine ( " do" );

            this.IncreaseStack ( );

            foreach ( IdentifierExpression Identifier in GenericForLoop.Variables )
                this.AddLocalVaraible ( Identifier.Identifier );

            this.Writer.WithIndentation ( ( ) => this.VisitNode ( GenericForLoop.Body ) );

            this.DecreaseStack ( );

            this.Writer.WriteIndented ( "end" );
            this.WriteStatementLineEnd ( GenericForLoop );
        }

        public virtual void VisitLocalVariableDeclaration ( LocalVariableDeclarationStatement LocalVariableDeclaration )
        {
            this.Writer.WriteIndented ( "local " );
            this.WriteSeparatedNodeList ( ", ", LocalVariableDeclaration.Identifiers );

            foreach ( IdentifierExpression Identifier in LocalVariableDeclaration.Identifiers )
                this.AddLocalVaraible ( Identifier.Identifier );

            if ( LocalVariableDeclaration.Values.Any ( ) )
            {
                this.Writer.Write ( " = " );
                this.WriteSeparatedNodeList ( ", ", LocalVariableDeclaration.Values );
            };

            this.WriteStatementLineEnd ( LocalVariableDeclaration );
        }

        public virtual void VisitNumericFor ( NumericForLoopStatement NumericForLoop )
        {
            this.Writer.WriteIndented ( "for " );
            this.VisitNode ( NumericForLoop.Variable );
            this.Writer.Write ( " = " );
            this.VisitNode ( NumericForLoop.Initial );
            this.Writer.Write ( ", " );
            this.VisitNode ( NumericForLoop.Final );

            if ( NumericForLoop.Step is Expression )
            {
                this.Writer.Write ( ", " );
                this.VisitNode ( NumericForLoop.Step );
            };

            this.Writer.WriteLine ( " do" );

            this.IncreaseStack ( );

            this.AddLocalVaraible ( NumericForLoop.Variable.Identifier );

            this.Writer.WithIndentation ( ( ) => this.VisitNode ( NumericForLoop.Body ) );

            this.DecreaseStack ( );

            this.Writer.WriteIndented ( "end" );
            this.WriteStatementLineEnd ( NumericForLoop );
        }

        public virtual void VisitRepeatUntil ( RepeatUntilStatement RepeatUntilLoop )
        {
            this.Writer.WriteLineIndented ( "repeat" );

            this.IncreaseStack ( );

            this.Writer.WithIndentation ( ( ) => this.VisitNode ( RepeatUntilLoop.Body ) );
            this.Writer.WriteIndented ( "until " );

            this.VisitNode ( RepeatUntilLoop.Condition );

            this.WriteStatementLineEnd ( RepeatUntilLoop );

            this.DecreaseStack ( );
        }

        public virtual void VisitReturn ( ReturnStatement ReturnStatement )
        {
            this.Writer.WriteIndented ( "return " );
            this.WriteSeparatedNodeList ( ", ", ReturnStatement.Values );
            this.WriteStatementLineEnd ( ReturnStatement );
        }

        public virtual void VisitStatementList ( StatementList Node )
        {
            foreach ( Statement Statement in Node.Body )
                this.VisitNode ( Statement );
        }

        public virtual void VisitWhileLoop ( WhileLoopStatement WhileLoop )
        {
            this.Writer.WriteIndented ( "while " );

            this.VisitNode ( WhileLoop.Condition );

            this.Writer.WriteLine ( " do" );

            this.IncreaseStack ( );

            this.Writer.WithIndentation ( ( ) => this.VisitNode ( WhileLoop.Body ) );

            this.DecreaseStack ( );

            this.Writer.WriteIndented ( "end" );
            this.WriteStatementLineEnd ( WhileLoop );
        }

        public void VisitEmptyStatement ( EmptyStatement EmptyStatement )
        {
            this.Writer.WriteIndentation ( );
            this.WriteStatementLineEnd ( EmptyStatement );
        }

        #endregion ITreeVisitor

        public void Clear ( ) => this.Writer.Reset ( );

        public override String ToString ( ) => this.Writer.ToString ( );

        private Func<String, String>? ObfuscateString;

        public static String Format ( LuaOptions LuaOptions, LuaASTNode Node, Func<String, String> ObfuscateString, Boolean EncryptAllStrings = false, String Location = "", Boolean PremiumFormat = false )
        {
            var Serializer = new FormattedLuaCodeSerializer ( LuaOptions );

            Serializer.EncryptAllStrings = EncryptAllStrings;
            Serializer.ObfuscateString = ObfuscateString;
            Serializer.Location = Location;
            Serializer.PremiumFormat = PremiumFormat;

            Serializer.Writer.Write ( "local PSU_WRAP = (function(...) return ...; end);" );

            Serializer.VisitNode ( Node );

            return Serializer.ToString ( );
        }
    }
}