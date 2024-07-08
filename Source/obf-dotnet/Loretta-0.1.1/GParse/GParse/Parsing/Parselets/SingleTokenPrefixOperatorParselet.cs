using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a prefix expression node
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    /// <param name="operator"></param>
    /// <param name="operand"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public delegate Boolean PrefixNodeFactory<TokenTypeT, ExpressionNodeT> ( Token<TokenTypeT> @operator, ExpressionNodeT operand, [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TokenTypeT : notnull;

    /// <summary>
    /// A module for single-token prefix operators
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT> : IPrefixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        private readonly Int32 precedence;
        private readonly PrefixNodeFactory<TokenTypeT, ExpressionNodeT> factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public SingleTokenPrefixOperatorParselet ( Int32 precedence, PrefixNodeFactory<TokenTypeT, ExpressionNodeT> factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence must be a value greater than 0." );

            this.precedence = precedence;
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, IProgress<Diagnostic> diagnosticReporter, [NotNullWhen ( true )] out ExpressionNodeT parsedExpression )
        {
            parsedExpression = default!;
            Token<TokenTypeT> prefix = parser.TokenReader.Consume ( );
            if ( parser.TryParseExpression ( this.precedence, out ExpressionNodeT expression ) )
                return this.factory ( prefix, expression, out parsedExpression );
            else
                return false;
        }
    }
}
