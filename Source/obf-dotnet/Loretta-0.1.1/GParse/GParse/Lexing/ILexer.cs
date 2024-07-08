namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a lexer
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexer<TokenTypeT> : IReadOnlyLexer<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Consumes the next token in the stream
        /// </summary>
        /// <returns></returns>
        Token<TokenTypeT> Consume ( );

        /// <summary>
        /// Returns to a given location
        /// </summary>
        /// <param name="location"></param>
        void Rewind ( SourceLocation location );
    }
}
