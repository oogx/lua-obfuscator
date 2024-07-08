using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Errors;
using GParse.IO;
using GParse.Lexing.Modules;

namespace GParse.Lexing
{
    /// <summary>
    /// A modular lexer created by the <see cref="ModularLexerBuilder{TokenTypeT}" />
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class ModularLexer<TokenTypeT> : ILexer<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// This lexer's module tree
        /// </summary>
        protected readonly LexerModuleTree<TokenTypeT> ModuleTree;

        /// <summary>
        /// The reader being used by the lexer
        /// </summary>
        protected readonly ICodeReader Reader;

        /// <summary>
        /// The <see cref="Diagnostic" /> emmiter.
        /// </summary>
        protected readonly IProgress<Diagnostic> DiagnosticEmitter;

        /// <summary>
        /// Initializes a new lexer
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        protected internal ModularLexer ( LexerModuleTree<TokenTypeT> tree, ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            this.ModuleTree = tree ?? throw new ArgumentNullException ( nameof ( tree ) );
            this.Reader = reader ?? throw new ArgumentNullException ( nameof ( reader ) );
            this.DiagnosticEmitter = diagnosticEmitter ?? throw new ArgumentNullException ( nameof ( diagnosticEmitter ) );
        }

        /// <summary>
        /// Consumes a token accounting for trivia tokens.
        /// </summary>
        /// <returns></returns>
        protected virtual Token<TokenTypeT> InternalConsumeToken ( )
        {
            SourceLocation loc = this.Reader.Location;
            try
            {
                if ( this.Reader.Position == this.Reader.Length )
                    return new Token<TokenTypeT> ( "EOF", String.Empty, String.Empty, default, this.Reader.Location.To ( this.Reader.Location ) );

                foreach ( ILexerModule<TokenTypeT> module in this.ModuleTree.GetSortedCandidates ( this.Reader ) )
                {
                    if ( module.CanConsumeNext ( this.Reader ) )
                    {
                        try
                        {
                            return module.ConsumeNext ( this.Reader, this.DiagnosticEmitter );
                        }
                        catch ( Exception ex ) when ( !( ex is FatalParsingException ) )
                        {
                            throw new FatalParsingException ( loc.To ( this.Reader.Location ), ex.Message, ex );
                        }
                    }

                    if ( this.Reader.Location != loc )
                        throw new FatalParsingException ( loc, $"Lexing module '{module.Name}' modified state on CanConsumeNext and did not restore it." );
                }

                throw new FatalParsingException ( this.Reader.Location, $"No registered modules can consume the rest of the input." );
            }
            catch
            {
                this.Reader.Restore ( loc );
                throw;
            }
        }

        /// <summary>
        /// Retrieves the first meaningful token while accumulating trivia
        /// </summary>
        /// <returns></returns>
        protected virtual Token<TokenTypeT> GetFirstMeaningfulToken ( )
        {
            Token<TokenTypeT> tok;
            var triviaAccumulator = new List<Token<TokenTypeT>> ( );
            while ( ( tok = this.InternalConsumeToken ( ) ).IsTrivia )
                triviaAccumulator.Add ( tok );

            return new Token<TokenTypeT> ( tok.Id, tok.Raw, tok.Value, tok.Type, tok.Range, false, triviaAccumulator.ToArray ( ) );
        }

        #region Lexer

        /// <inheritdoc />
        public Token<TokenTypeT> Consume ( ) => this.GetFirstMeaningfulToken ( );

        /// <summary>
        /// Returns to the given location
        /// </summary>
        /// <param name="location"></param>
        public void Rewind ( SourceLocation location ) => this.Reader.Restore ( location );

        #region IReadOnlyLexer

        /// <inheritdoc />
        public Boolean EOF => this.Reader.Position == this.Reader.Length;

        /// <inheritdoc />
        public SourceLocation Location => this.Reader.Location;

        /// <inheritdoc />
        public Token<TokenTypeT> Peek ( )
        {
            SourceLocation loc = this.Reader.Location;
            try { return this.Consume ( ); }
            finally { this.Reader.Restore ( loc ); }
        }

        #endregion IReadOnlyLexer

        #endregion Lexer
    }
}
