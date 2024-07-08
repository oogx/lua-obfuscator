using System;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a non-terminal
    /// </summary>
    public class NonTerminal<T> : GrammarNode<T>
    {
        /// <summary>
        /// The name of the production this references
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Initializes a non-terminal
        /// </summary>
        /// <param name="name"></param>
        public NonTerminal ( String name )
        {
            this.Name = name;
        }
    }
}
