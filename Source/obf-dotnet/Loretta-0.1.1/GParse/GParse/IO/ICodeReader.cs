using System;
using System.Text.RegularExpressions;

namespace GParse.IO
{
    /// <summary>
    /// Defines a stream reader meant for reading code, which provides line and column location info.
    /// </summary>
    public interface ICodeReader : IReadOnlyCodeReader
    {
        /// <summary>
        /// Advances in the stream by a given <paramref name="offset"/>.
        /// </summary>
        /// <remarks>
        /// Line endings are considered a single character and skipped over as such. The following
        /// are considered line endings:
        /// <list type="bullet">
        /// <item>LF (\n)</item>
        /// </list>
        /// </remarks>
        /// <param name="offset"></param>
        void Advance ( Int32 offset );

        #region Mutable operations

        #region Read

        /// <summary>
        /// Returns the next character from the stream or null if the reader is at the end of the stream.
        /// </summary>
        /// <returns></returns>
        Char? Read ( );

        /// <summary>
        /// Returns the character at the given <paramref name="offset"/> from the stream or null if
        /// the reader is at the end of the stream.
        /// </summary>
        /// <remarks>
        /// This also skips all characters between the current position of the reader and the
        /// provided <paramref name="offset"/>.
        /// </remarks>
        /// <param name="offset"></param>
        /// <returns></returns>
        Char? Read ( Int32 offset );

        #endregion Read

        #region ReadLine

        /// <summary>
        /// Reads a line from the stream. The returned string does not contain the end-of-line character.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The following are considered line endings:
        /// <list type="bullet">
        /// <item>CR + LF (\r\n)</item>
        /// <item>LF (\n)</item>
        /// <item>CR (\r)</item>
        /// <item><see cref="Environment.NewLine"/></item>
        /// <item>EOF</item>
        /// </list>
        /// </remarks>
        String ReadLine ( );

        #endregion ReadLine

        #region ReadString

        /// <summary>
        /// Reads a string of the given length from the stream.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        String? ReadString ( Int32 length );

        #endregion ReadString

        #region ReadStringUntil

        /// <summary>
        /// Reads the contents from the stream until the provided <paramref name="delim"/> is found
        /// or the end of the stream is hit.
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        String ReadStringUntil ( Char delim );

        /// <summary>
        /// Reads the contents from the stream until the provided <paramref name="delim"/> is found
        /// or the end of the stream is hit.
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        String ReadStringUntil ( String delim );

        /// <summary>
        /// Reads the contents from the stream until a character passes the provided <paramref
        /// name="filter"/> or the end of the stream is hit.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        String ReadStringUntil ( Predicate<Char> filter );

        #endregion ReadStringUntil

        #region ReadStringWhile

        /// <summary>
        /// Reads the contents from the stream while the characters pass the provided <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        String ReadStringWhile ( Predicate<Char> filter );

        #endregion ReadStringWhile

        #region ReadToEnd

        /// <summary>
        /// Reads the contents from the stream until the end of the stream.
        /// </summary>
        /// <returns></returns>
        String ReadToEnd ( );

        #endregion ReadToEnd

#if HAS_SPAN

        #region ReadSpanLine

        /// <summary>
        /// Reads a line from the stream. The returned span does not contain the end-of-line character.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The following are considered line endings:
        /// <list type="bullet">
        /// <item>CR + LF (\r\n)</item>
        /// <item>LF (\n)</item>
        /// <item>CR (\r)</item>
        /// <item><see cref="Environment.NewLine"/></item>
        /// <item>EOF</item>
        /// </list>
        /// </remarks>
        ReadOnlySpan<Char> ReadSpanLine ( );

        #endregion ReadLine

        #region ReadSpan

        /// <summary>
        /// Reads a span of the given length from the stream.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        ReadOnlySpan<Char> ReadSpan ( Int32 length );

        #endregion ReadSpan

        #region ReadSpanUntil

        /// <summary>
        /// Reads the contents from the stream until the provided <paramref name="delim"/> is found
        /// or the end of the stream is hit.
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        ReadOnlySpan<Char> ReadSpanUntil ( Char delim );

        /// <summary>
        /// Reads the contents from the stream until the provided <paramref name="delim"/> is found
        /// or the end of the stream is hit.
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        ReadOnlySpan<Char> ReadSpanUntil ( String delim );

        /// <summary>
        /// Reads the contents from the stream until a character passes the provided <paramref
        /// name="filter"/> or the end of the stream is hit.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        ReadOnlySpan<Char> ReadSpanUntil ( Predicate<Char> filter );

        #endregion ReadSpanUntil

        #region ReadSpanWhile

        /// <summary>
        /// Reads the contents from the stream while the characters pass the provided <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        ReadOnlySpan<Char> ReadSpanWhile ( Predicate<Char> filter );

        #endregion ReadSpanWhile

        #region ReadSpanToEnd

        /// <summary>
        /// Reads the contents from the stream until the end of the stream.
        /// </summary>
        /// <returns></returns>
        ReadOnlySpan<Char> ReadSpanToEnd ( );

        #endregion

#endif

        #region MatchRegex

        /// <summary>
        /// Attempts to match a regex <paramref name="expression"/> but does not advance if it fails.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Match MatchRegex ( String expression );

        /// <summary>
        /// Attempts to match a regex but does not advance if it fails.
        /// </summary>
        /// <param name="regex">
        /// <para>
        /// A <see cref="Regex"/> instance that contains an expression starting with the \G modifier.
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
        /// strings are stored in an internal cache and the instances are initialized with <see cref="RegexOptions.Compiled"/>.
        /// </remarks>
        Match MatchRegex ( Regex regex );

        #endregion MatchRegex

        #region Position Manipulation

        /// <summary>
        /// Seeks back to the beggining of the stream.
        /// </summary>
        void Reset ( );

        /// <summary>
        /// Restores the reader's location to a provided <paramref name="location"/>.
        /// </summary>
        /// <param name="location"></param>
        /// <remarks>
        /// No validation is done to check that the provided line and column numbers are correct.
        /// </remarks>
        void Restore ( SourceLocation location );

        #endregion Position Manipulation

        #endregion Mutable operations
    }
}