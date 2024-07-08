using System;
using GParse.Composable;
using GParse.Lexing.Modules;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// The interface for a composable rule compiler
    /// </summary>
    public interface IComposableLexerRuleCompiler<TTokenType>
    {
        /// <summary>
        /// Compiles a <see cref="GrammarNode{T}"/> into a <see cref="ILexerModule{TokenTypeT}"/>.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns></returns>
        ILexerModule<TTokenType> CompileRule ( GrammarNode<Char> grammarNode );
    }
}