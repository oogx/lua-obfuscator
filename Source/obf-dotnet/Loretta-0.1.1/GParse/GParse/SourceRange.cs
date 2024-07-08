using System;
using System.Collections.Generic;

namespace GParse
{
    /// <summary>
    /// Defines a range in source code
    /// </summary>
    public readonly struct SourceRange : IEquatable<SourceRange>
    {
        /// <summary>
        /// Zero
        /// </summary>
        public static readonly SourceRange Zero = new SourceRange ( SourceLocation.Zero, SourceLocation.Zero );

        /// <summary>
        /// Starting location
        /// </summary>
        public readonly SourceLocation Start;

        /// <summary>
        /// Ending location
        /// </summary>
        public readonly SourceLocation End;

        /// <summary>
        /// Initializes this range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public SourceRange ( SourceLocation start, SourceLocation end )
        {
            this.End = end;
            this.Start = start;
        }

        /// <inheritdoc />
        public override String ToString ( ) => $"{this.Start} - {this.End}";

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is SourceRange range && this.Equals ( range );

        /// <inheritdoc />
        public Boolean Equals ( SourceRange other ) => this.End.Equals ( other.End )
                    && this.Start.Equals ( other.Start );

        /// <inheritdoc />
        public override Int32 GetHashCode ( )
        {
            var hashCode = 945720665;
            hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.End );
            return ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.Start );
        }

        /// <inheritdoc />
        public static Boolean operator == ( SourceRange lhs, SourceRange rhs ) => lhs.Equals ( rhs );

        /// <inheritdoc />
        public static Boolean operator != ( SourceRange lhs, SourceRange rhs ) => !( lhs == rhs );

        #endregion Generated Code
    }
}
