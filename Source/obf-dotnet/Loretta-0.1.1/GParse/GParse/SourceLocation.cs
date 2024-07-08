using System;

namespace GParse
{
    /// <summary>
    /// Defines a point in a source code file
    /// </summary>
    public readonly struct SourceLocation : IEquatable<SourceLocation>
    {
        /// <summary>
        /// The start of a file
        /// </summary>
        public static readonly SourceLocation Zero = new SourceLocation ( 1, 1, 0 );

        /// <summary>
        /// Maximum possible value
        /// </summary>
        public static readonly SourceLocation Max = new SourceLocation ( Int32.MaxValue, Int32.MaxValue, Int32.MaxValue );

        /// <summary>
        /// Minimum possible value (invalid)
        /// </summary>
        public static readonly SourceLocation Min = new SourceLocation ( Int32.MinValue, Int32.MinValue, Int32.MinValue );

        /// <summary>
        /// Standard invalid location
        /// </summary>
        public static readonly SourceLocation Invalid = new SourceLocation ( -1, -1, -1 );

        /// <summary>
        /// The byte offset of this location
        /// </summary>
        public readonly Int32 Byte;

        /// <summary>
        /// The line of this location
        /// </summary>
        public readonly Int32 Line;

        /// <summary>
        /// The column of this location
        /// </summary>
        public readonly Int32 Column;

        /// <summary>
        /// Initializes this location
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <param name="pos"></param>
        public SourceLocation ( Int32 line, Int32 column, Int32 pos )
        {
            this.Line = line;
            this.Column = column;
            this.Byte = pos;
        }

        /// <summary>
        /// Creates a range with this as start and
        /// <paramref name="end" /> as end
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public SourceRange To ( SourceLocation end ) => new SourceRange ( this, end );

        /// <inheritdoc />
        public override String ToString ( ) => $"{this.Line}:{this.Column}";

        /// <summary>
        /// Deconstructs this source position
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="Column"></param>
        public void Deconstruct ( out Int32 Line, out Int32 Column )
        {
            Line = this.Line;
            Column = this.Column;
        }

        /// <summary>
        /// Deconstructs this source position
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="Column"></param>
        /// <param name="Byte"></param>
        public void Deconstruct ( out Int32 Line, out Int32 Column, out Int32 Byte )
        {
            Line = this.Line;
            Column = this.Column;
            Byte = this.Byte;
        }

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is SourceLocation location && this.Equals ( location );

        /// <inheritdoc />
        public Boolean Equals ( SourceLocation other ) =>
            this.Column == other.Column && this.Line == other.Line && this.Byte == other.Byte;

        /// <inheritdoc />
        public override Int32 GetHashCode ( )
        {
            var hashCode = 412437926;
            hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + this.Column.GetHashCode ( );
            return ( hashCode * -1521134295 ) + this.Line.GetHashCode ( );
        }

        /// <inheritdoc />
        public static Boolean operator == ( SourceLocation lhs, SourceLocation rhs ) => lhs.Equals ( rhs );

        /// <inheritdoc />
        public static Boolean operator != ( SourceLocation lhs, SourceLocation rhs ) => !( lhs == rhs );

        #endregion Generated Code
    }
}
