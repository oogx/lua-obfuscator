using System;
using GParse;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Defines the interface of a
    /// <see cref="IPrattParser{TokenTypeT, ExpressionNodeT}" /> builder
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IPrattParserBuilder<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, IPrefixParselet<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, String ID, IPrefixParselet<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, String ID, IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Initializes a new Pratt Parser
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader, IProgress<Diagnostic> diagnosticEmitter );
    }
}
