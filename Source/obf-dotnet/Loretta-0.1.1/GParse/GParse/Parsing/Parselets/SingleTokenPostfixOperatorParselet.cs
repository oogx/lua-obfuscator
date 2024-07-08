using System;
using System.Diagnostics.CodeAnalysis;
using GParse;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a postfix expression node
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    /// <param name="operand"></param>
    /// <param name="operator"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public delegate Boolean PostfixNodeFactory<TokenTypeT, ExpressionNodeT> ( ExpressionNodeT operand, Token<TokenTypeT> @operator, [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TokenTypeT : notnull;

    /// <summary>
    /// A module that can parse a postfix operation with an
    /// operator that is composed of a single token
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT> : IInfixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <inheritdoc />
        public Int32 Precedence { get; }

        private readonly PostfixNodeFactory<TokenTypeT, ExpressionNodeT> factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public SingleTokenPostfixOperatorParselet ( Int32 precedence, PostfixNodeFactory<TokenTypeT, ExpressionNodeT> factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence of the operator must be greater than 0" );

            this.Precedence = precedence;
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT expression, IProgress<Diagnostic> diagnosticEmitter, [NotNullWhen ( true )] out ExpressionNodeT parsedExpression ) =>
            this.factory ( expression, parser.TokenReader.Consume ( ), out parsedExpression );
    }
}
