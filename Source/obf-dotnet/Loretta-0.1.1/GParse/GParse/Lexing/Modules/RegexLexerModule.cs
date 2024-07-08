using System;
using System.Text.RegularExpressions;
using GParse.Errors;
using GParse.IO;

namespace GParse.Lexing.Modules
{
    /// <summary>
    /// A module that defines a token through a regex pattern
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class RegexLexerModule<TokenTypeT> : ILexerModule<TokenTypeT>
        where TokenTypeT : notnull
    {
        private readonly String Id;
        private readonly TokenTypeT Type;
        private readonly String? Expression;
        private readonly Regex? Regex;
        private readonly Func<Match, Object>? Converter;
        private readonly Boolean IsTrivia;

        /// <inheritdoc />
        public String Name => $"Regex Lexer Module: {this.Expression}";

        /// <inheritdoc />
        public String? Prefix { get; }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        /// <param name="isTrivia"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, Object>? converter, Boolean isTrivia )
        {
            this.Converter = converter;
            this.Expression = regex;
            this.Id = id;
            this.IsTrivia = isTrivia;
            this.Prefix = prefix;
            this.Type = type;
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        /// <param name="isTrivia"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, Object>? converter, Boolean isTrivia )
        {
            this.Converter = converter;
            this.Regex = regex;
            this.Id = id;
            this.IsTrivia = isTrivia;
            this.Prefix = prefix;
            this.Type = type;
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, Object>? converter ) : this ( id, type, regex, prefix, converter, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, Object>? converter ) : this ( id, type, regex, prefix, converter, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String? prefix ) : this ( id, type, regex, prefix, null )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String? prefix ) : this ( id, type, regex, prefix, null )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex ) : this ( id, type, regex, null )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex ) : this ( id, type, regex, null )
        {
        }

        /// <inheritdoc />
        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader )
        {
            Match res = this.Expression != null
                ? reader.PeekRegex ( this.Expression )
                : reader.PeekRegex ( this.Regex! );
            return res.Success;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            SourceLocation start = reader.Location;
            Match result = this.Expression != null
                ? reader.MatchRegex ( this.Expression )
                : reader.MatchRegex ( this.Regex! );
            if ( result.Success )
            {
                return new Token<TokenTypeT> ( this.Id, result.Value, this.Converter != null ? this.Converter ( result ) : result.Value, this.Type, start.To ( reader.Location ), this.IsTrivia );
            }
            else
            {
                throw new FatalParsingException ( reader.Location, "Cannot consume a token when check wasn't successful." );
            }
        }
    }
}
