using System;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Modules;

namespace GParse.Lexing.Composable
{
    internal class ComposableLexerRuleInterpreter<TTokenType> : ILexerModule<TTokenType>
    {
        public String Name { get; }
        public String Prefix { get; }
        private GrammarNode<Char> RootNode { get; }

        public ComposableLexerRuleInterpreter ( String name, GrammarNode<Char> rootNode )
        {
            this.Name = name;
            this.Prefix = null!;
            this.RootNode = rootNode;
        }

        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader ) => throw new NotImplementedException ( );

        public Token<TTokenType> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter ) => throw new NotImplementedException ( );
    }
}