using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a infix expression node
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    /// <param name="left"></param>
    /// <param name="op"></param>
    /// <param name="right"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public delegate Boolean InfixNodeFactory<TokenTypeT, ExpressionNodeT> ( ExpressionNodeT left, Token<TokenTypeT> op, ExpressionNodeT right, [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TokenTypeT : notnull;

    /// <summary>
    /// A module that can parse an infix operation with an operator composed of a single token
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class SingleTokenInfixOperatorParselet<TokenTypeT, ExpressionNodeT> : IInfixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <inheritdoc />
        public Int32 Precedence { get; }

        private readonly Boolean isRightAssociative;
        private readonly InfixNodeFactory<TokenTypeT, ExpressionNodeT> factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public SingleTokenInfixOperatorParselet ( Int32 precedence, Boolean isRightAssociative, InfixNodeFactory<TokenTypeT, ExpressionNodeT> factory )
        {
            this.Precedence = precedence;
            this.isRightAssociative = isRightAssociative;
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT expression, IProgress<Diagnostic> diagnosticEmitter, [NotNullWhen ( true )] out ExpressionNodeT parsedExpression )
        {
            parsedExpression = default!;
            Token<TokenTypeT> op = parser.TokenReader.Consume ( );

            // We decrease the precedence by one on right-associative operators because the minimum
            // precedence passed to TryParseExpression is exclusive (meaning that the precedence of the
            // infix parselets must be higher than the one we pass it.
            // TODO: Check if this cannot create bugs with other operators that have the same precedence.
            Int32 minPrecedence;
            if ( this.isRightAssociative )
                minPrecedence = this.Precedence - 1;
            else
                minPrecedence = this.Precedence;

            if ( parser.TryParseExpression ( minPrecedence, out ExpressionNodeT nextExpr ) )
                return this.factory ( expression, op, nextExpr, out parsedExpression );
            else
                return false;
        }
    }
}
