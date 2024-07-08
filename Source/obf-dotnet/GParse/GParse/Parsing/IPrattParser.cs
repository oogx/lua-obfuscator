using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing
{
    /// <summary>
    /// Defines the interface of a modular pratt expression parser
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IPrattParser<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The parser's token reader instance
        /// </summary>
        ITokenReader<TokenTypeT> TokenReader { get; }

        /// <summary>
        /// Attempts to parse an expression with a minimum precedence of
        /// <paramref name="minPrecedence" />.
        /// </summary>
        /// <remarks>
        /// The minimum precedence is used to enforce the precedence of operators as well as
        /// associativity.
        ///
        /// The <see cref="Parselets.SingleTokenInfixOperatorParselet{TokenTypeT, ExpressionNodeT}" />
        /// uses the <paramref name="minPrecedence" /> parameter to implement associativity by passing in
        /// the associativity of the operator subtracted by one so that the operator itself is in the set
        /// of possible parselets.
        /// </remarks>
        /// <param name="minPrecedence"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Boolean TryParseExpression ( Int32 minPrecedence, [NotNullWhen ( true )] out ExpressionNodeT expression );

        /// <summary>
        /// Attempts to parse an expression
        /// </summary>
        /// <returns></returns>
        Boolean TryParseExpression ( [NotNullWhen ( true )] out ExpressionNodeT expression );
    }
}
