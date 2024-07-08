using System;

namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a read-only lexer
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface IReadOnlyLexer<TokenTypeT>
        where TokenTypeT : notnull
    {
        // Let user have access to reader maybe(?)

        /// <summary>
        /// The location that the lexer is at in the stream
        /// </summary>
        SourceLocation Location { get; }

        /// <summary>
        /// Whether the lexer is at the end of the file
        /// </summary>
        Boolean EOF { get; }

        /// <summary>
        /// Returns the next token without advancing in the stream
        /// </summary>
        /// <returns></returns>
        Token<TokenTypeT> Peek ( );
    }
}
