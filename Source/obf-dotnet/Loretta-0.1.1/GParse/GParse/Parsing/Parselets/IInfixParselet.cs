using System;
using System.Diagnostics.CodeAnalysis;
using GParse;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses infix operations
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IInfixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The precedence of this module
        /// </summary>
        Int32 Precedence { get; }

        /// <summary>
        /// Attempts to parse an infix/postfix expression. (state will be restored by the caller on failure)
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="expression"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <param name="parsedExpression"></param>
        /// <returns></returns>
        Boolean TryParse ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT expression, IProgress<Diagnostic> diagnosticEmitter, [NotNullWhen ( true )] out ExpressionNodeT parsedExpression );
    }
}
