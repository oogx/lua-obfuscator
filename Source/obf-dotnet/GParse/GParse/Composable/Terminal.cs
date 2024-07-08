using System;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a terminal
    /// </summary>
    public class Terminal<T> : GrammarNode<T>
    {
        /// <summary>
        /// The value of the terminal
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new terminal
        /// </summary>
        /// <param name="value"></param>
        public Terminal ( T value )
        {
            this.Value = value;
        }
    }
}
