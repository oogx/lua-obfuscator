using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using GParse.IO;
using GUtils.Expressions;
using GUtils.StateMachines.Transducers;

namespace GParse.Extensions.StateMachines
{
    /// <summary>
    /// Represents a compiled <see cref="Transducer{InputT, OutputT}" /> that acts upon a
    /// <see cref="StringCodeReader" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate Boolean ICodeReaderTransducer<T> ( StringCodeReader reader, out T value );

    /// <summary>
    /// Extensions to the <see cref="Transducer{InputT, OutputT}" /> class for operating on
    /// <see cref="StringCodeReader" /> instances
    /// </summary>
    public static class SourceCodeReaderTransducerExtensions
    {
        /// <summary>
        /// Attempts to execute a transducer with content from a <see cref="StringCodeReader" />. If the
        /// state reached is not a terminal state, the reader is rewinded to it's initial position.
        /// </summary>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <param name="reader"></param>
        /// <param name="output"></param>
        /// <returns>Whether the state reached was a terminal state.</returns>
        public static Boolean TryExecute<OutputT> ( this Transducer<Char, OutputT> transducer, ICodeReader reader, [MaybeNull] out OutputT output )
        {
            if ( transducer is null )
                throw new ArgumentNullException ( nameof ( transducer ) );
            if ( reader is null )
                throw new ArgumentNullException ( nameof ( reader ) );

            SourceLocation startLocation = reader.Location;
            TransducerState<Char, OutputT> state = transducer.InitialState;
            while ( reader.Position != reader.Length )
            {
                if ( !state.TransitionTable.TryGetValue ( reader.Peek ( )!.Value, out TransducerState<Char, OutputT>? tmp ) )
                    break;
                state = tmp;
                reader.Advance ( 1 );
            }

            if ( state.IsTerminal )
            {
                output = state.Output;
                return true;
            }

            reader.Restore ( startLocation );
            // Since the analyzer doesn't seems to obey [MaybeNull], we ignore the warning
            output = default;
            return false;
        }

        private static SwitchExpression CompileState<OutputT> ( TransducerState<Char, OutputT> state, ParameterExpression reader, ParameterExpression output, LabelTarget @return, Int32 depth )
        {
            var idx = 0;
            var cases = new SwitchCase[state.TransitionTable.Count];
            foreach ( KeyValuePair<Char, TransducerState<Char, OutputT>> statePair in state.TransitionTable )
            {
                cases[idx++] = Expression.SwitchCase (
                    CompileState ( statePair.Value, reader, output, @return, depth + 1 ),
                    Expression.Constant ( ( Char? ) statePair.Key )
                );
            }

            return Expression.Switch (
                GExpression.MethodCall<ICodeReader> ( reader, r => r.Peek ( depth ), depth ),
                state.IsTerminal
                    ? Expression.Block (
                        GExpression.MethodCall<ICodeReader> ( reader, r => r.Advance ( 0 ), depth + 1 ),
                        Expression.Assign ( output, Expression.Constant ( state.Output ) ),
                        Expression.Return ( @return, Expression.Constant ( true ) )
                    )
                    : ( Expression ) Expression.Return ( @return, Expression.Constant ( false ) ),
                cases
            );
        }

        /// <summary>
        /// Compiles a <see cref="Transducer{InputT, OutputT}" /> that takes a
        /// <see cref="StringCodeReader" /> as an input provider
        /// </summary>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <returns></returns>
        public static ICodeReaderTransducer<OutputT> CompileWithCodeReaderAsInput<OutputT> ( this Transducer<Char, OutputT> transducer )
        {
            ParameterExpression reader = Expression.Parameter ( typeof ( ICodeReader ), "reader");
            ParameterExpression output = Expression.Parameter ( typeof ( OutputT ).MakeByRefType ( ), "output");
            LabelTarget @return = Expression.Label ( typeof ( Boolean ) );
            return Expression.Lambda<ICodeReaderTransducer<OutputT>> (
                Expression.Block (
                    CompileState ( transducer.InitialState, reader, output, @return, 0 ),
                    Expression.Label ( @return, Expression.Constant ( false ) )
                ),
                reader,
                output
            ).Compile ( );
        }
    }
}
