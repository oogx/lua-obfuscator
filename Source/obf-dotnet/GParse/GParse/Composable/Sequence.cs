using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a sequence of grammar rules
    /// </summary>
    public class Sequence<T> : GrammarNodeListContainer<Sequence<T>, T>
    {
        /// <summary>
        /// The grammar nodes that compose this sequence
        /// </summary>
        public IReadOnlyList<GrammarNode<T>> GrammarNodes => this.grammarNodes;

        /// <summary>
        /// Initializes a sequence
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Sequence ( params GrammarNode<T>[] grammarNodes ) : base(grammarNodes)
        {
        }
    }
}
