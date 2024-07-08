using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a node that contains other nodes as it's children
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TElem"></typeparam>
    public abstract class GrammarNodeListContainer<TNode, TElem> : GrammarNode<TElem>
        where TNode : GrammarNodeListContainer<TNode, TElem>
    {
        /// <summary>
        /// The list of grammar nodes
        /// </summary>
        protected readonly List<GrammarNode<TElem>> grammarNodes;

        /// <summary>
        /// Initializes a new <see cref="GrammarNodeListContainer{TNode,TElem}" />
        /// </summary>
        /// <param name="grammarNodes"></param>
        protected GrammarNodeListContainer ( GrammarNode<TElem>[] grammarNodes )
        {
            this.grammarNodes = new List<GrammarNode<TElem>> ( grammarNodes );
        }

        /// <summary>
        /// Appends a node to this container's children list
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns></returns>
        public virtual TNode AppendNode ( GrammarNode<TElem> grammarNode )
        {
            this.grammarNodes.Add ( grammarNode );
            return ( TNode ) this;
        }

        /// <summary>
        /// Appends a collection of nodes to this container's children list
        /// </summary>
        /// <param name="grammarNodes"></param>
        /// <returns></returns>
        public virtual TNode AppendNodes ( IEnumerable<GrammarNode<TElem>> grammarNodes )
        {
            this.grammarNodes.AddRange ( grammarNodes );
            return ( TNode ) this;
        }
    }
}
