using System;
using System.Text.RegularExpressions;

namespace GParse.IO
{
    /// <summary>
    /// Defines a read-only stream reader meant for reading code, which provides line and column
    /// location info.
    /// </summary>
    public interface IReadOnlyCodeReader
    {
        #region Position Management

        /// <summary>
        /// Current line.
        /// </summary>
        Int32 Line { get; }

        /// <summary>
        /// Current column.
        /// </summary>
        Int32 Column { get; }

        /// <summary>
        /// Current position.
        /// </summary>
        Int32 Position { get; }

        /// <summary>
        /// The full location of the reader.
        /// </summary>
        SourceLocation Location { get; }

        #endregion Position Management

        /// <summary>
        /// The size of the stream of code being read.
        /// </summary>
        Int32 Length { get; }

        #region Non-mutable operations

        #region FindOffset

        /// <summary>
        /// Returns the offset of the given <paramref name="character" /> or -1 if not found.
        /// </summary>
        /// <param name="character">The character to look for.</param>
        /// <returns>The offset of the given <paramref name="character" /> or -1 if not found.</returns>
        Int32 FindOffset ( Char character );

        /// <summary>
        /// <inheritdoc cref="FindOffset(Char)" />
        /// </summary>
        /// <param name="character"><inheritdoc cref="FindOffset(Char)" /></param>
        /// <param name="offset">The offset to start searching at.</param>
        /// <returns><inheritdoc cref="FindOffset(Char)" /></returns>
        Int32 FindOffset ( Char character, Int32 offset );

        /// <summary>
        /// Finds the offset of the first character that passes the provided <paramref
        /// name="predicate" /> or -1 if not found.
        /// </summary>
        /// <param name="predicate">The predicate that checks each character.</param>
        /// <returns>
        /// The offset of the first character that passes the provided <paramref name="predicate" />
        /// or -1 if not found.
        /// </returns>
        Int32 FindOffset ( Predicate<Char> predicate );

        /// <summary>
        /// <inheritdoc cref="FindOffset(Predicate{Char})" />
        /// </summary>
        /// <param name="predicate"><inheritdoc cref="FindOffset(Predicate{Char})" /></param>
        /// <param name="offset">The offset to start searching at.</param>
        /// <returns><inheritdoc cref="FindOffset(Predicate{Char})" /></returns>
        Int32 FindOffset ( Predicate<Char> predicate, Int32 offset );

        /// <summary>
        /// Finds the offset of a given <paramref name="str" /> or -1 if not found.
        /// </summary>
        /// <param name="str">The string to search for.</param>
        /// <returns>The offset of a given <paramref name="str" /> or -1 if not found.</returns>
        Int32 FindOffset ( String str );

        /// <summary>
        /// <inheritdoc cref="FindOffset(String)" />
        /// </summary>
        /// <param name="str"><inheritdoc cref="FindOffset(String)" /></param>
        /// <param name="offset">The offset to start searching at.</param>
        /// <returns><inheritdoc cref="FindOffset(String)" /></returns>
        Int32 FindOffset ( String str, Int32 offset );

#if HAS_SPAN
        /// <summary>
        /// Finds the offset of the given <paramref name="span" /> or -1 if not found.
        /// </summary>
        /// <param name="span">The span to search for.</param>
        /// <returns>The offset of the given <paramref name="span" /> or -1 if not found.</returns>
        Int32 FindOffset ( ReadOnlySpan<Char> span );

        /// <summary>
        /// <inheritdoc cref="FindOffset(ReadOnlySpan{Char})" />
        /// </summary>
        /// <param name="span"><inheritdoc cref="FindOffset(ReadOnlySpan{Char})" /></param>
        /// <param name="offset">The offset to start searching at.</param>
        /// <returns><inheritdoc cref="FindOffset(ReadOnlySpan{Char})" /></returns>
        Int32 FindOffset ( ReadOnlySpan<Char> span, Int32 offset );
#endif

        #endregion FindOffset

        #region IsNext

        /// <summary>
        /// Returns whether the provided <paramref name="character" /> is at the <see
        /// cref="Position" /> the reader is at.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <returns></returns>
        Boolean IsNext ( Char character );

        /// <summary>
        /// Returns whether the provided <paramref name="str" /> is at the <see cref="Position" />
        /// the reader is at.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns></returns>
        Boolean IsNext ( String str );

#if HAS_SPAN
        /// <summary>
        /// Returns whether the provided <paramref name="span" /> is at the <see cref="Position" />
        /// the reader is at.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        Boolean IsNext ( ReadOnlySpan<Char> span );
#endif

        #endregion IsNext

        #region IsAt

        /// <summary>
        /// Returns whether the provided <paramref name="character" /> is at the provided <paramref
        /// name="offset" />.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <param name="offset">The offset to check at.</param>
        /// <returns>
        /// Whether the provided <paramref name="character" /> is at the provided <paramref
        /// name="offset" />.
        /// </returns>
        Boolean IsAt ( Char character, Int32 offset );

        /// <summary>
        /// Returns whether the provided <paramref name="str" /> is at the provided <paramref
        /// name="offset" />.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="offset">The offset to check at.</param>
        /// <returns>
        /// Whether the provided <paramref name="str" /> is at the provided <paramref name="offset" />.
        /// </returns>
        Boolean IsAt ( String str, Int32 offset );

#if HAS_SPAN
        /// <summary>
        /// Returns whether the provided <paramref name="span" /> is at the provided <paramref
        /// name="offset" />.
        /// </summary>
        /// <param name="span">The span to check.</param>
        /// <param name="offset">The offset to check at.</param>
        /// <returns>
        /// Whether the provided <paramref name="span" /> is at the provided <paramref name="offset" />.
        /// </returns>
        Boolean IsAt ( ReadOnlySpan<Char> span, Int32 offset );
#endif

        #endregion IsAt

        #region Peek

        /// <summary>
        /// Returns the next character without advancing in the stream or null if the reader is at
        /// the end of the stream.
        /// </summary>
        /// <returns></returns>
        Char? Peek ( );

        /// <summary>
        /// Returns the character at the given offset without advancing in the stream or null if the
        /// reader is at the end of the stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        Char? Peek ( Int32 offset );

        #endregion Peek

        #region PeekRegex

        /// <summary>
        /// Attempts to match a regex without advancing the stream.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Match PeekRegex ( String expression );

        /// <summary>
        /// Attempts to match a regex without advancing the stream.
        /// </summary>
        /// <param name="regex">
        /// <para>
        /// A <see cref="Regex" /> instance that contains an expression starting with the \G modifier.
        /// </para>
        /// <para>
        /// An exception will be thrown if the match does not start at the same position the reader
        /// is located at.
        /// </para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method is offered purely for the performance benefits of regular expressions
        /// generated with Regex.CompileToAssembly
        /// (https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.compiletoassembly).
        /// It is not meant to be used with anything else, since all regexes passed in the form of
        /// strings are stored in an internal cache and the instances are initialized with <see
        /// cref="RegexOptions.Compiled" />.
        /// </remarks>
        Match PeekRegex ( Regex regex );

        #endregion PeekRegex

        #region PeekString

        /// <summary>
        /// Reads a string of the provided length without advancing the stream.
        /// </summary>
        /// <param name="length">The length of the string to be read.</param>
        /// <returns>
        /// The string that was read or null if there weren't enough characters left to be read.
        /// </returns>
        String? PeekString ( Int32 length );

        /// <summary>
        /// <inheritdoc cref="PeekString(Int32)" />
        /// </summary>
        /// <param name="length"><inheritdoc cref="PeekString(Int32)" /></param>
        /// <param name="offset">The offset to get the string from.</param>
        /// <returns><inheritdoc cref="PeekString(Int32)" /></returns>
        String? PeekString ( Int32 length, Int32 offset );

        #endregion PeekString

#if HAS_SPAN

        #region PeekSpan

        /// <summary>
        /// Reads a span with a maximum size of <paramref name="length" /> without advancing the
        /// stream (might be smaller than the requested length if there aren't enough characters
        /// left to be read).
        /// </summary>
        /// <param name="length">The maximum length of the span to peek.</param>
        /// <returns>
        /// The read span (might be smaller than the requested length if there aren't enough
        /// characters left to be read).
        /// </returns>
        ReadOnlySpan<Char> PeekSpan ( Int32 length );

        /// <summary>
        /// <inheritdoc cref="PeekSpan(Int32)" />
        /// </summary>
        /// <param name="length"><inheritdoc cref="PeekSpan(Int32)" /></param>
        /// <param name="offset">The offset to get the span from.</param>
        /// <returns><inheritdoc cref="PeekSpan(Int32)" /></returns>
        ReadOnlySpan<Char> PeekSpan ( Int32 length, Int32 offset );

        #endregion PeekSpan

#endif

        #endregion Non-mutable operations
    }
}