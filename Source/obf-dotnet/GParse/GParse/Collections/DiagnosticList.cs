using System;
using System.Collections;
using System.Collections.Generic;

namespace GParse.Collections
{
    /// <summary>
    /// A class that implements both <see cref="IProgress{T}" /> and <see cref="IReadOnlyList{T}" /> for
    /// <see cref="Diagnostic" /> for use with components that require an <see cref="IProgress{T}" />
    /// </summary>
    public class DiagnosticList : IReadOnlyList<Diagnostic>, IProgress<Diagnostic>
    {
        private List<Diagnostic> Diagnostics { get; }

        /// <inheritdoc />
        public Int32 Count =>
            this.Diagnostics.Count;

        /// <inheritdoc />
        public Diagnostic this[Int32 index] =>
            this.Diagnostics[index];

        /// <summary>
        /// Initializes this <see cref="DiagnosticList" />
        /// </summary>
        public DiagnosticList ( )
        {
            this.Diagnostics = new List<Diagnostic> ( );
        }

        /// <summary>
        /// Reports a diagnostic
        /// </summary>
        /// <param name="item"></param>
        public void Report ( Diagnostic item ) =>
            this.Diagnostics.Add ( item );

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Diagnostic> GetEnumerator ( ) =>
            this.Diagnostics.GetEnumerator ( );

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator ( ) =>
            this.Diagnostics.GetEnumerator ( );
    }
}
