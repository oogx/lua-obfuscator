using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a literal node expression
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    /// <param name="token"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public delegate Boolean LiteralNodeFactory<TokenTypeT, ExpressionNodeT> ( Token<TokenTypeT> token, [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TokenTypeT : notnull;

    /// <summary>
    /// A module for single token literals
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class LiteralParselet<TokenTypeT, ExpressionNodeT> : IPrefixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        private readonly LiteralNodeFactory<TokenTypeT, ExpressionNodeT> factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="factory"></param>
        public LiteralParselet ( LiteralNodeFactory<TokenTypeT, ExpressionNodeT> factory )
        {
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, IProgress<Diagnostic> diagnosticReporter, [NotNullWhen ( true )] out ExpressionNodeT parsedExpression ) =>
            this.factory ( parser.TokenReader.Consume ( ), out parsedExpression );
    }
}
