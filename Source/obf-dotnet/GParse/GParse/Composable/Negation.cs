namespace GParse.Composable
{
    /// <summary>
    /// Represents a negation of a grammar node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Negation<T> : GrammarNode<T>
    {
        /// <summary>
        /// The grammar node to be negated
        /// </summary>
        public GrammarNode<T> GrammarNode { get; }

        /// <summary>
        /// Initializes a new grammar node
        /// </summary>
        /// <param name="grammarNode"></param>
        public Negation ( GrammarNode<T> grammarNode )
        {
            this.GrammarNode = grammarNode;
        }
    }
}