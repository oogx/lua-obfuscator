using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using GParse.Lexing;
using GUtils.Expressions;
using GUtils.StateMachines.Transducers;

namespace GParse.Extensions.StateMachines
{
    /// <summary>
    /// Represents a compiled <see cref="Transducer{InputT, OutputT}" /> that accepts a
    /// <see cref="ITokenReader{TokenTypeT}" /> as an input provider
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="OutputT"></typeparam>
    /// <param name="reader"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public delegate Boolean TokenReaderTransducer<TokenTypeT, OutputT> ( ITokenReader<TokenTypeT> reader, [AllowNull] out OutputT output )
        where TokenTypeT : notnull;

    /// <summary>
    /// Extensions for a <see cref="Transducer{InputT, OutputT}" /> to work with a
    /// <see cref="TokenReader{TokenTypeT}" />
    /// </summary>
    public static class ITokenReaderTransducerExtensions
    {
        /// <summary>
        /// Attempts to execute the state machine against the <see cref="TokenReader{TokenTypeT}" />
        /// </summary>
        /// <typeparam name="TokenTypeT"></typeparam>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <param name="reader"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Boolean TryExecute<TokenTypeT, OutputT> ( this Transducer<Token<TokenTypeT>, OutputT> transducer, ITokenReader<TokenTypeT> reader, [MaybeNull] out OutputT output )
            where TokenTypeT : notnull
        {
            if ( reader == null )
                throw new ArgumentNullException ( nameof ( reader ) );

            var offset = 0;
            TransducerState<Token<TokenTypeT>, OutputT> state = transducer.InitialState;
            while ( !reader.EOF )
            {
                if ( !state.TransitionTable.TryGetValue ( reader.Lookahead ( offset ), out TransducerState<Token<TokenTypeT>, OutputT>? tmp ) )
                    break;
                state = tmp;
                offset++;
            }

            if ( state.IsTerminal )
            {
                reader.Skip ( offset + 1 );
                output = state.Output;
                return true;
            }

            output = default;
            return false;
        }

        private static SwitchExpression CompileState<TokenTypeT, OutputT> ( TransducerState<Token<TokenTypeT>, OutputT> state, ParameterExpression reader, ParameterExpression output, LabelTarget @return, Int32 depth )
            where TokenTypeT : notnull
        {
            var idx = 0;
            var cases = new SwitchCase[state.TransitionTable.Count];
            foreach ( KeyValuePair<Token<TokenTypeT>, TransducerState<Token<TokenTypeT>, OutputT>> statePair in state.TransitionTable )
            {
                cases[idx++] = Expression.SwitchCase (
                    CompileState ( statePair.Value, reader, output, @return, depth + 1 ),
                    Expression.Constant ( statePair.Key )
                );
            }

            return Expression.Switch (
                GExpression.MethodCall<ITokenReader<TokenTypeT>> ( reader, r => r.Lookahead ( depth ), depth ),
                state.IsTerminal
                    ? Expression.Block (
                        GExpression.MethodCall<ITokenReader<TokenTypeT>> ( reader, r => r.Skip ( 0 ), depth + 1 ),
                        Expression.Assign ( output, Expression.Constant ( state.Output ) ),
                        Expression.Return ( @return, Expression.Constant ( true ) )
                    )
                    : ( Expression ) Expression.Constant ( false ),
                cases
            );
        }

        /// <summary>
        /// Compiles this <see cref="Transducer{InputT, OutputT}" />
        /// </summary>
        /// <typeparam name="TokenTypeT"></typeparam>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <returns></returns>
        public static TokenReaderTransducer<TokenTypeT, OutputT> CompileWithTokenReaderAsInput<TokenTypeT, OutputT> ( this Transducer<Token<TokenTypeT>, OutputT> transducer )
            where TokenTypeT : notnull
        {
            ParameterExpression reader = Expression.Parameter ( typeof ( ITokenReader<TokenTypeT> ), "reader" );
            ParameterExpression output = Expression.Parameter ( typeof ( OutputT ).MakeByRefType ( ), "output" );
            LabelTarget @return = Expression.Label ( typeof ( Boolean ) );

            return Expression.Lambda<TokenReaderTransducer<TokenTypeT, OutputT>> (
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
