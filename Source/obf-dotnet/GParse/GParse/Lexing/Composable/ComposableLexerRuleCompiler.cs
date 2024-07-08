using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using GParse.Composable;
using GParse.Lexing.Modules;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A <see cref="GrammarNode{T}"/>-based lexer rule compiler
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public class ComposableLexerRuleCompiler<TTokenType> : IComposableLexerRuleCompiler<TTokenType>
    {
        /// <inheritdoc/>
        public ILexerModule<TTokenType> CompileRule ( GrammarNode<Char> grammarNode )
        {
            throw new NotImplementedException ( );
        }
    }
}
