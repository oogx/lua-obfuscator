using System;
using System.Text;

namespace GParse.IO
{
    /// <summary>
    /// Defines a code writer
    /// </summary>
    public class CodeWriter
    {
        private readonly StringBuilder _builder;
        private readonly String _indentationSequence;
        private String _cachedIndentation;

        /// <summary>
        /// The indentation level of the writer
        /// </summary>
        public Int32 Indentation { get; private set; }

        /// <summary>
        /// Initializes this class
        /// </summary>
        public CodeWriter ( String indentationSequence = "\t" )
        {
            this._indentationSequence = indentationSequence;
            this.Indentation = 0;
            this._cachedIndentation = String.Empty;
            this._builder = new StringBuilder ( );
        }

        /// <summary>
        /// Resets the writer
        /// </summary>
        public void Reset ( )
        {
            this.Indentation = 0;
            this._cachedIndentation = String.Empty;
            this._builder.Clear ( );
        }

        /// <summary>
        /// Increases the indentation level
        /// </summary>
        public void Indent ( )
        {
            this.Indentation++;
            this._cachedIndentation += this._indentationSequence;
        }

        /// <summary>
        /// Decreases the indentation level
        /// </summary>
        public void Outdent ( )
        {
            this.Indentation--;
            this._cachedIndentation = this._cachedIndentation.Substring ( 0, this.Indentation * this._indentationSequence.Length );
        }

        /// <summary>
        /// Decreases the indentation level
        /// </summary>
        public void Unindent ( ) => this.Outdent ( );

        #region Write(Indented)

        /// <summary>
        /// Writes a value
        /// </summary>
        /// <param name="value"></param>
        public void Write ( Object? value ) => this._builder.Append ( value );

        /// <summary>
        /// Writes a value
        /// </summary>
        /// <param name="value"></param>
        public void Write ( String? value ) => this._builder.Append ( value );

        /// <summary>
        /// Writes a formatted value
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Write ( String format, params Object?[] args ) => this._builder.AppendFormat ( format, args );

        /// <summary>
        /// Writes the indentation prefix
        /// </summary>
        public void WriteIndentation ( ) => this._builder.Append ( this._cachedIndentation );

        /// <summary>
        /// Writes prefixed by indentation
        /// </summary>
        /// <param name="value"></param>
        public void WriteIndented ( Object? value ) => this._builder.Append ( this._cachedIndentation ).Append ( value );

        /// <summary>
        /// Writes prefixed by indentation
        /// </summary>
        /// <param name="value"></param>
        public void WriteIndented ( String? value ) => this._builder.Append ( this._cachedIndentation ).Append ( value );

        /// <summary>
        /// WRites formatted prefixed by indentation
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteIndented ( String format, params Object?[] args ) => this._builder.Append ( this._cachedIndentation ).AppendFormat ( format, args );

        #endregion Write(Indented)

        #region WriteLine(Indented)

        /// <summary>
        /// Writes an empty line
        /// </summary>
        public void WriteLine ( ) => this._builder.AppendLine ( );

        /// <summary>
        /// Writes a value followed by the line terminator
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine ( Object? value ) => this._builder.Append ( value ).AppendLine ( );

        /// <summary>
        /// Writes a value followed by the line terminator
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine ( String? value ) => this._builder.AppendLine ( value );

        /// <summary>
        /// Writes a formatted value followed by the line terminator
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLine ( String format, params Object?[] args ) => this._builder.AppendFormat ( format, args ).AppendLine ( );

        /// <summary>
        /// Writes a value followed by the line terminator and
        /// prefixed by the indetation
        /// </summary>
        /// <param name="value"></param>
        public void WriteLineIndented ( Object? value ) => this._builder.Append ( this._cachedIndentation ).Append ( value ).AppendLine ( );

        /// <summary>
        /// Writes a value followed by the line terminator and
        /// prefixed by the indetation
        /// </summary>
        /// <param name="value"></param>
        public void WriteLineIndented ( String? value ) => this._builder.Append ( this._cachedIndentation ).AppendLine ( value );

        /// <summary>
        /// Writes a formatted value followed by the line
        /// terminator and prefixed by the indetation
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLineIndented ( String format, params Object?[] args ) => this._builder.Append ( this._cachedIndentation ).AppendFormat ( format, args ).AppendLine ( );

        #endregion WriteLine(Indented)

        /// <summary>
        /// Increases the indentation before the callback and
        /// decreases it after
        /// </summary>
        /// <param name="cb"></param>
        public void WithIndentation ( Action cb )
        {
            if ( cb == null )
                throw new ArgumentNullException ( nameof ( cb ) );

            this.Indent ( );
            cb ( );
            this.Outdent ( );
        }

        /// <summary>
        /// Gets the entire code as a string
        /// </summary>
        /// <returns></returns>
        public String GetCode ( ) => this._builder.ToString ( );

        /// <inheritdoc />
        public override String ToString ( ) => this.GetCode ( );
    }
}
