using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a grammar node that matches an inclusive range
    /// </summary>
    public class CharRangeNode : GrammarNode<Char>
    {
        /// <summary>
        /// The first char this range will match
        /// </summary>
        public Char Start { get; }

        /// <summary>
        /// The last char this range will match
        /// </summary>
        public Char End { get; }

        /// <summary>
        /// Initializes this character range grammar node
        /// </summary>
        /// <param name="start">The first char this range will match</param>
        /// <param name="end">The last char this range will match</param>
        public CharRangeNode ( Char start, Char end )
        {
            this.Start = start;
            this.End = end;
        }
    }
}