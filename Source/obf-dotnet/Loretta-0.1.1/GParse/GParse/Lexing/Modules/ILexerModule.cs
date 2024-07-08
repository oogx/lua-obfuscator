using System;
using GParse.IO;

namespace GParse.Lexing.Modules
{
    /// <summary>
    /// Defines the interface of a lexer module
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexerModule<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Parser module name
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The module prefix
        /// </summary>
        String? Prefix { get; }

        /// <summary>
        /// Whether this module can consume what's left in the reader
        /// </summary>
        /// <param name="reader">The reader that the module should use</param>
        /// <returns></returns>
        Boolean CanConsumeNext ( IReadOnlyCodeReader reader );

        /// <summary>
        /// Consume the next element in the reader
        /// </summary>
        /// <param name="reader">The reader that the module should use</param>
        /// <param name="diagnosticEmitter">The emmiter for diagnostics</param>
        /// <returns></returns>
        Token<TokenTypeT> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter );
    }
}
