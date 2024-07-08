using System;

namespace GParse
{
    /// <summary>
    /// Represents a diagnostic emmited by the compiler, such as an error, warning, suggestion, etc.
    /// </summary>
    public class Diagnostic
    {
        /// <summary>
        /// The ID of the emitted diagnostic
        /// </summary>
        public String Id { get; }

        /// <summary>
        /// The location that the diagnostic is reffering to in the code
        /// </summary>
        public SourceRange Range { get; }

        /// <summary>
        /// The severity of the diagnostic
        /// </summary>
        public DiagnosticSeverity Severity { get; }

        /// <summary>
        /// The description of this diagnostic
        /// </summary>
        public String Description { get; }

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="range"></param>
        /// <param name="severity"></param>
        /// <param name="description"></param>
        public Diagnostic ( String id, SourceRange range, DiagnosticSeverity severity, String description )
        {
            this.Id = id;
            this.Range = range;
            this.Severity = severity;
            this.Description = description;
        }

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <param name="severity"></param>
        /// <param name="description"></param>
        protected Diagnostic ( String id, SourceLocation location, DiagnosticSeverity severity, String description ) : this ( id, location.To ( location ), severity, description )
        {
        }
    }
}
