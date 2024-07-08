using System;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Stores all modules that compose a <see cref="PrattParser{TokenTypeT, ExpressionNodeT}" />
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParserBuilder<TokenTypeT, ExpressionNodeT> : IPrattParserBuilder<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        #region Modules

        /// <summary>
        /// The registered <see cref="IPrefixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> prefixModuleTree = new PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> ( );

        /// <summary>
        /// The registered <see cref="IInfixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> infixModuleTree = new PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> ( );

        #endregion Modules

        #region Register

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="module"></param>
        public virtual void Register ( TokenTypeT tokenType, IPrefixParselet<TokenTypeT, ExpressionNodeT> module ) =>
            this.prefixModuleTree.AddModule ( tokenType, module );

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="module"></param>
        public virtual void Register ( TokenTypeT tokenType, String id, IPrefixParselet<TokenTypeT, ExpressionNodeT> module ) =>
            this.prefixModuleTree.AddModule ( tokenType, id, module );

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="module"></param>
        public void Register ( TokenTypeT tokenType, IInfixParselet<TokenTypeT, ExpressionNodeT> module ) =>
            this.infixModuleTree.AddModule ( tokenType, module );

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="module"></param>
        public void Register ( TokenTypeT tokenType, String id, IInfixParselet<TokenTypeT, ExpressionNodeT> module ) =>
            this.infixModuleTree.AddModule ( tokenType, id, module );

        #endregion Register

        #region RegisterLiteral

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TokenTypeT tokenType, LiteralNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new LiteralParselet<TokenTypeT, ExpressionNodeT> ( factory ) );

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TokenTypeT tokenType, String ID, LiteralNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new LiteralParselet<TokenTypeT, ExpressionNodeT> ( factory ) );

        #endregion RegisterLiteral

        #region RegisterSingleTokenPrefixOperator

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, Int32 precedence, PrefixNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, PrefixNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPrefixOperator

        #region RegisterSingleTokenInfixOperator

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, Int32 precedence, Boolean isRightAssociative, InfixNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new SingleTokenInfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, Boolean isRightAssociative, InfixNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new SingleTokenInfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        #endregion RegisterSingleTokenInfixOperator

        #region RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, Int32 precedence, PostfixNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, PostfixNodeFactory<TokenTypeT, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Creates a parser that will read from the <paramref name="reader" /> provided
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public virtual IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader, IProgress<Diagnostic> diagnosticEmitter ) =>
            new PrattParser<TokenTypeT, ExpressionNodeT> ( reader, this.prefixModuleTree, this.infixModuleTree, diagnosticEmitter );
    }
}
