using System;
using System.Linq;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a sequence that defines the prefix of this rule
    /// </summary>
    public class PrefixNode : GrammarNode<Char>
    {
        private static Boolean HasPrefixNodeInTree ( GrammarNode<Char> grammarNode )
        {
            return grammarNode switch
            {
                PrefixNode _ => true,
                Sequence<Char> seq => seq.GrammarNodes.Any ( HasPrefixNodeInTree ),
                Alternation<Char> alt => alt.GrammarNodes.Any ( HasPrefixNodeInTree ),
                Negation<Char> neg => HasPrefixNodeInTree ( neg.GrammarNode ),
                Repetition<Char> rep => HasPrefixNodeInTree ( rep.GrammarNode ),
                _ => false,
            };
        }

        /// <summary>
        /// Returns whether a grammar node has a proper <see cref="PrefixNode"/>
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns></returns>
        public static Boolean HasProperPrefixSequence ( GrammarNode<Char> grammarNode )
        {
            return grammarNode switch
            {
                PrefixNode _ => true,
                Sequence<Char> seq => seq.GrammarNodes.Count > 0
                                      && HasProperPrefixSequence ( seq.GrammarNodes[0] )
                                      && seq.GrammarNodes.Skip ( 1 ).All ( g => !HasProperPrefixSequence ( g ) ),
                Alternation<Char> alt => alt.GrammarNodes.Count > 0
                                         && alt.GrammarNodes.All ( HasProperPrefixSequence ),
                _ => false,
            };
        }

        /// <summary>
        /// The grammar node that represents the prefix
        /// </summary>
        public GrammarNode<Char> GrammarNode { get; }

        /// <summary>
        /// Initializes a prefix sequence
        /// </summary>
        /// <param name="grammarNode"></param>
        public PrefixNode ( GrammarNode<Char> grammarNode )
        {
            this.GrammarNode = grammarNode;
        }
    }
}