using System;
using System.Collections.Generic;

namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a token reader
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ITokenReader<TokenTypeT> : IEnumerable<Token<TokenTypeT>>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The location of the token reader
        /// </summary>
        SourceLocation Location { get; }

        /// <summary>
        /// Whether we're at the end of the file
        /// </summary>
        public Boolean EOF { get; }

        /// <summary>
        /// Consumes the token at <paramref name="offset" /> in the stream without moving
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        Token<TokenTypeT> Lookahead ( Int32 offset = 0 );

        /// <summary>
        /// Consumes the next token in the stream
        /// </summary>
        /// <returns></returns>
        Token<TokenTypeT> Consume ( );

        /// <summary>
        /// Skips a certain amount of tokens
        /// </summary>
        /// <param name="count">The amount of tokens to skip</param>
        void Skip ( Int32 count );

        /// <summary>
        /// Returns to a given location
        /// </summary>
        /// <param name="location"></param>
        void Rewind ( SourceLocation location );

        #region IsAhead

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TokenTypeT}.Type" /> equal to <paramref name="tokenType" />
        /// </summary>
        /// <param name="tokenType">The wanted type</param>
        /// <param name="offset">The offset</param>
        /// <returns></returns>
        Boolean IsAhead ( TokenTypeT tokenType, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TokenTypeT}.Type" /> in the given <paramref name="tokenTypes" />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TokenTypeT}.Id" /> equal to <paramref name="ID" />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( String ID, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TokenTypeT}.Id" /> in the given <paramref name="ids" />
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( IEnumerable<String> ids, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TokenTypeT}.Type" /> equal to <paramref name="tokenType" /> and the
        /// <see cref="Token{TokenTypeT}.Id" /> equal to <paramref name="id" />
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( TokenTypeT tokenType, String id, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TokenTypeT}.Type" /> in the given <paramref name="tokenTypes" /> and
        /// has the <see cref="Token{TokenTypeT}.Id" /> in the given <paramref name="ids" />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, IEnumerable<String> ids, Int32 offset = 0 );

        #endregion IsAhead

        #region Accept

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="ID" />
        /// </summary>
        /// <param name="ID">The ID to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( String ID, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has one of the required <paramref name="IDs" />
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<String> IDs, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has one of the required <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<TokenTypeT> types, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="ID" /> and
        /// <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="ID">The ID to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type, String ID, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the one of the required <paramref name="IDs" />
        /// and <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="IDs"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs, out Token<TokenTypeT> token );

        #endregion Accept

        #region FatalExpect

        /// <summary>
        /// Throws an exception if the next token in the stream does not have the <paramref name="ID" />
        /// required
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Token<TokenTypeT> FatalExpect ( String ID );

        /// <summary>
        /// Throws an exception if the next token in the stream does not have one of the required
        /// <paramref name="IDs" />
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        Token<TokenTypeT> FatalExpect ( IEnumerable<String> IDs );

        /// <summary>
        /// Throws an exception if the next token in the stream does not have the <paramref name="type" />
        /// required
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Token<TokenTypeT> FatalExpect ( TokenTypeT type );

        /// <summary>
        /// Throws an excepton if the next token in the stream does not have one of the required
        /// <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        Token<TokenTypeT> FatalExpect ( IEnumerable<TokenTypeT> types );

        /// <summary>
        /// Throws an exception if the next token in the stream does not have the <paramref name="ID" />
        /// and <paramref name="type" /> required
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        Token<TokenTypeT> FatalExpect ( TokenTypeT type, String ID );

        /// <summary>
        /// Throws an exception if the next token in the stream does not have one of the required
        /// <paramref name="IDs" /> or <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        Token<TokenTypeT> FatalExpect ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs );

        #endregion FatalExpect
    }
}
