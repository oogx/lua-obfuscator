using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// The base class for all grammar nodes
    /// </summary>
    public abstract class GrammarNode<T>
    {
        /// <summary>
        /// Creates an alternation node
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Alternation<T> operator | ( GrammarNode<T> left, GrammarNode<T> right )
        {
            var nodes = new List<GrammarNode<T>> ( );
            if ( left is Alternation<T> leftAlternation )
                nodes.AddRange ( leftAlternation.GrammarNodes );
            else
                nodes.Add ( left );
            if ( right is Alternation<T> rightAlternation )
                nodes.AddRange ( rightAlternation.GrammarNodes );
            else
                nodes.Add ( right );
            return new Alternation<T> ( nodes.ToArray ( ) );
        }

        /// <summary>
        /// Creates a sequence node
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Sequence<T> operator & ( GrammarNode<T> left, GrammarNode<T> right )
        {
            var nodes = new List<GrammarNode<T>> ( );
            if ( left is Sequence<T> leftAlternation )
                nodes.AddRange ( leftAlternation.GrammarNodes );
            else
                nodes.Add ( left );
            if ( right is Sequence<T> rightAlternation )
                nodes.AddRange ( rightAlternation.GrammarNodes );
            else
                nodes.Add ( right );
            return new Sequence<T> ( nodes.ToArray ( ) );
        }
    }
}
