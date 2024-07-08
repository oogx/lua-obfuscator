using System;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a repetition of a grammar node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repetition<T> : GrammarNode<T>
    {
        /// <summary>
        /// The grammar node that is to be repeated
        /// </summary>
        public GrammarNode<T> GrammarNode { get; }

        /// <summary>
        /// The number of repetitions required and permitted.
        /// </summary>
        public (UInt32? Minimum, UInt32? Maximum) Repetitions { get; }

        /// <summary>
        /// Creates a new repetition node
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="repetitions"></param>
        public Repetition ( GrammarNode<T> grammarNode, (UInt32? Minimum, UInt32? Maximum) repetitions )
        {
            this.GrammarNode = grammarNode;
            this.Repetitions = repetitions;
        }
    }
}