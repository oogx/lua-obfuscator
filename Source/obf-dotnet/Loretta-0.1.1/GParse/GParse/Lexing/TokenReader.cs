using System;
using System.Collections;
using System.Collections.Generic;
using GParse.Errors;

namespace GParse.Lexing
{
    /// <summary>
    /// Implements the token reader interface
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class TokenReader<TokenTypeT> : ITokenReader<TokenTypeT>
        where TokenTypeT : notnull
    {
        private readonly List<Token<TokenTypeT>> TokenCache;
        private readonly Object TokenCacheLock = new Object ( );

        /// <summary>
        /// The parser's Lexer instance
        /// </summary>
        protected readonly ILexer<TokenTypeT> Lexer;

        /// <summary>
        /// Initializes a token with a cache of lookahead of 1 token
        /// </summary>
        /// <param name="lexer"></param>
        public TokenReader ( ILexer<TokenTypeT> lexer ) : this ( lexer, 1 )
        {
        }

        /// <summary>
        /// Initializes a token with a lookahead cache size of <paramref name="maxLookaheadOffset" />
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="maxLookaheadOffset"></param>
        public TokenReader ( ILexer<TokenTypeT> lexer, Int32 maxLookaheadOffset )
        {
            this.Lexer = lexer;
            this.TokenCache = new List<Token<TokenTypeT>> ( maxLookaheadOffset );
        }

        /// <summary>
        /// Saves N tokens from the lexer on the readahead cache
        /// </summary>
        /// <param name="count"></param>
        protected void CacheTokens ( Int32 count )
        {
            while ( count-- > 0 )
                this.TokenCache.Add ( this.Lexer.Consume ( ) );
        }

        #region ITokenReader<TokenTypeT>

        /// <inheritdoc />
        public SourceLocation Location
        {
            get
            {
                lock ( this.TokenCacheLock )
                    return this.TokenCache.Count > 0 ? this.TokenCache[0].Range.Start : this.Lexer.Location;
            }
        }

        /// <inheritdoc/>
        public Boolean EOF => this.Lexer.EOF;

        /// <inheritdoc />
        public Token<TokenTypeT> Lookahead ( Int32 offset = 0 )
        {
            lock ( this.TokenCacheLock )
            {
                if ( this.TokenCache.Count <= offset )
                    this.CacheTokens ( offset - this.TokenCache.Count + 1 );

                return this.TokenCache[offset];
            }
        }

        /// <inheritdoc />
        public Token<TokenTypeT> Consume ( )
        {
            lock ( this.TokenCacheLock )
            {
                if ( this.TokenCache.Count < 1 )
                    this.CacheTokens ( 1 );

                Token<TokenTypeT> tok = this.TokenCache[0];
                this.TokenCache.RemoveAt ( 0 );
                return tok;
            }
        }

        /// <inheritdoc />
        public void Skip ( Int32 count )
        {
            lock ( this.TokenCacheLock )
            {
                if ( this.TokenCache.Count <= count )
                {
                    count -= this.TokenCache.Count;
                    this.TokenCache.Clear ( );
                    while ( count-- > 0 )
                        this.Lexer.Consume ( );
                }
                else
                {
                    this.TokenCache.RemoveRange ( 0, count );
                }
            }
        }

        /// <inheritdoc/>
        public void Rewind ( SourceLocation location )
        {
            lock ( this.TokenCacheLock )
            {
                this.TokenCache.Clear ( );
                this.Lexer.Rewind ( location );
            }
        }

        #region IsAhead

        /// <inheritdoc />
        public Boolean IsAhead ( TokenTypeT tokenType, Int32 offset = 0 ) =>
            EqualityComparer<TokenTypeT>.Default.Equals ( this.Lookahead ( offset ).Type, tokenType );

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, Int32 offset = 0 )
        {
            TokenTypeT type = this.Lookahead(offset).Type;
            foreach ( TokenTypeT wtype in tokenTypes )
            {
                if ( EqualityComparer<TokenTypeT>.Default.Equals ( wtype, type ) )
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public Boolean IsAhead ( String ID, Int32 offset = 0 ) =>
            this.Lookahead ( offset ).Id == ID;

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<String> ids, Int32 offset = 0 )
        {
            var aheadId = this.Lookahead ( offset ).Id;
            foreach ( var id in ids )
            {
                if ( id == aheadId )
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public Boolean IsAhead ( TokenTypeT tokenType, String id, Int32 offset = 0 ) =>
            this.IsAhead ( tokenType, offset ) && this.IsAhead ( id, offset );

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, IEnumerable<String> ids, Int32 offset = 0 ) =>
            this.IsAhead ( tokenTypes, offset ) && this.IsAhead ( ids, offset );

        #endregion IsAhead

        #region Accept

        /// <inheritdoc />
        public Boolean Accept ( String ID, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( ID ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( IEnumerable<String> IDs, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( IDs ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( type ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( IEnumerable<TokenTypeT> types, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( types ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( TokenTypeT type, String ID, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( type, ID ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( types, IDs ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        #endregion Accept

        #region FatalExpect

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( String ID )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( ID, out _ ) )
                throw new FatalParsingException ( next.Range, $"Expected a {ID} but got {next.Id} instead." );
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( IEnumerable<String> IDs )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( IDs, out _ ) )
                throw new FatalParsingException ( next.Range, $"Expected any ({String.Join ( ", ", IDs )}) but got {next.Id}" );
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( TokenTypeT type )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( type, out _ ) )
                throw new FatalParsingException ( next.Range, $"Expected a {type} but got {next.Type} instead." );
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( IEnumerable<TokenTypeT> types )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( types, out _ ) )
                throw new FatalParsingException ( next.Range, $"Expected any ({String.Join ( ", ", types )}) but got {next.Type}" );
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( TokenTypeT type, String ID )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( type, ID, out _ ) )
                throw new FatalParsingException ( next.Range, $"Expected a {ID}+{type} but got a {next.Id}+{next.Type}" );
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( types, IDs, out _ ) )
                throw new FatalParsingException ( next.Range, $"Expected any ({String.Join ( ", ", IDs )})+({String.Join ( ", ", types )}) but got {next.Id}+{next.Type}" );
            return next;
        }

        #endregion FatalExpect

        #endregion ITokenReader<TokenTypeT>

        /// <summary>
        /// Returns an enumerator that uses <see cref="ITokenReader{TokenTypeT}.Lookahead(Int32)" /> to
        /// enumerate all tokens
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Token<TokenTypeT>> GetEnumerator ( ) => new TokenReaderEnumerator<TokenTypeT> ( this );

        /// <summary>
        /// Returns an enumerator that uses <see cref="ITokenReader{TokenTypeT}.Lookahead(Int32)" /> to
        /// enumerate all tokens
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator ( ) => new TokenReaderEnumerator<TokenTypeT> ( this );
    }
}
