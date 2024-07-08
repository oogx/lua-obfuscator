using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Implements the modular pratt expression parser
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParser<TokenTypeT, ExpressionNodeT> : IPrattParser<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The this holds the tree of <see cref="IPrefixParselet{TokenTypeT, ExpressionNodeT}" /> to be used while parsing expressions.
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> prefixModuleTree;

        /// <summary>
        /// This holds the tree of <see cref="IInfixParselet{TokenTypeT, ExpressionNodeT}"/> to be used while parsing expressions.
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> infixModuleTree;

        /// <summary>
        /// This is the <see cref="IProgress{T}"/> reporter to which the parser should send <see cref="Diagnostic">Diagnostics</see> to.
        /// </summary>
        protected readonly IProgress<Diagnostic> diagnosticReporter;

        /// <inheritdoc />
        public ITokenReader<TokenTypeT> TokenReader { get; }

        /// <summary>
        /// Initializes a pratt parser
        /// </summary>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModuleTree"></param>
        /// <param name="infixModuleTree"></param>
        /// <param name="diagnosticEmitter"></param>
        protected internal PrattParser ( ITokenReader<TokenTypeT> tokenReader, PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> prefixModuleTree, PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> infixModuleTree, IProgress<Diagnostic> diagnosticEmitter )
        {
            this.TokenReader        = tokenReader;
            this.prefixModuleTree   = prefixModuleTree;
            this.infixModuleTree    = infixModuleTree;
            this.diagnosticReporter = diagnosticEmitter;
        }

        #region PrattParser<TokenTypeT, ExpressionNodeT>

        #region ParseExpression

        /// <inheritdoc />
        public virtual Boolean TryParseExpression ( Int32 minPrecedence, [NotNullWhen ( true )] out ExpressionNodeT expression )
        {
            expression = default!;
            var foundExpression = false;
            foreach ( IPrefixParselet<TokenTypeT, ExpressionNodeT> module in this.prefixModuleTree.GetSortedCandidates ( this.TokenReader ) )
            {
                SourceLocation start = this.TokenReader.Location;
                if ( module.TryParse ( this, this.diagnosticReporter, out expression ) )
                {
                    foundExpression = true;
                    break;
                }
                if ( this.TokenReader.Location != start )
                    this.TokenReader.Rewind ( start );
            }
            if ( !foundExpression )
                return false;

            Boolean couldParse;
            do
            {
                couldParse = false;
                foreach ( IInfixParselet<TokenTypeT, ExpressionNodeT> module in this.infixModuleTree.GetSortedCandidates ( this.TokenReader ) )
                {
                    SourceLocation start = this.TokenReader.Location;
                    if ( minPrecedence < module.Precedence
                        && module.TryParse ( this, expression, this.diagnosticReporter, out ExpressionNodeT tmpExpr ) )
                    {
                        couldParse = true;
                        expression = tmpExpr;
                        break;
                    }
                    if ( this.TokenReader.Location != start )
                        this.TokenReader.Rewind ( start );
                }
            }
            while ( couldParse );

            return true;
        }

        /// <inheritdoc />
        public virtual Boolean TryParseExpression ( [NotNullWhen ( true )] out ExpressionNodeT expression ) =>
            this.TryParseExpression ( 0, out expression );

        #endregion ParseExpression

        #endregion PrattParser<TokenTypeT, ExpressionNodeT>
    }
}
