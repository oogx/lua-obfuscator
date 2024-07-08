using System;
using System.Diagnostics.CodeAnalysis;
using GParse;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses a prefix expression
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IPrefixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Attempts to parse a prefix expression. (state will be restored by the caller)
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="diagnosticReporter"></param>
        /// <param name="parsedExpression"></param>
        /// <returns></returns>
        Boolean TryParse ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, IProgress<Diagnostic> diagnosticReporter, [NotNullWhen ( true )] out ExpressionNodeT parsedExpression );
    }
}
