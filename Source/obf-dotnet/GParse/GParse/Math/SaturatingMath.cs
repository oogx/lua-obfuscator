using System;
using System.Runtime.CompilerServices;

namespace GParse.Math
{
    /// <summary>
    ///    The saturating math class.
    /// Implements all fundamental arithmetic operations with saturating logic.
    /// </summary>
    public class SaturatingMath
    {
        #region Int32

        #region Overflow/Underflow checks

        /// <summary>
        ///    Whether the addition of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillAdditionOverflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs > 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the addition of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillAdditionUnderflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs < 0 && lhs <= min - rhs /* ∧ rhs < 0 */;
        }

        /// <summary>
        ///    Whether the subtraction of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillSubtractionOverflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs > 0 && lhs >= max + rhs /* ∧ rhs < 0 */;
        }

        /// <summary>
        ///    Whether the subtraction of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillSubtractionUnderflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs < 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the multiplication of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillMultiplicationOverflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max / rhs;
        }

        /// <summary>
        ///    Whether the multiplication of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillMultiplicationUnderflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min / rhs;
        }

        /// <summary>
        ///    Whether the division of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillDivisionOverflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max * rhs;
        }

        /// <summary>
        ///    Whether the division of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillDivisionUnderflow ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min * rhs;
        }

        #endregion Overflow/Underflow checks

        #region Math operations

        /// <summary>
        ///    Adds both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int32 Add ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillAdditionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs + rhs;
        }

        /// <summary>
        ///    Subtracts both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int32 Subtract ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( WillSubtractionOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs - rhs;
        }

        /// <summary>
        ///    Multiplies both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int32 Multiply ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillMultiplicationUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs * rhs;
        }

        /// <summary>
        ///    Divides both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int32 Divide ( Int32 lhs, Int32 rhs, Int32 min, Int32 max )
        {
            if ( WillDivisionOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs / rhs;
        }

        #endregion Math operations

        #endregion Int32

        #region Int64

        #region Overflow/Underflow checks

        /// <summary>
        ///    Whether the addition of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillAdditionOverflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs > 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the addition of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillAdditionUnderflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs < 0 && lhs <= min - rhs /* ∧ rhs < 0 */;
        }

        /// <summary>
        ///    Whether the subtraction of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillSubtractionOverflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs > 0 && lhs >= max + rhs /* ∧ rhs < 0 */;
        }

        /// <summary>
        ///    Whether the subtraction of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillSubtractionUnderflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs < 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the multiplication of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillMultiplicationOverflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max / rhs;
        }

        /// <summary>
        ///    Whether the multiplication of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillMultiplicationUnderflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min / rhs;
        }

        /// <summary>
        ///    Whether the division of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillDivisionOverflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max * rhs;
        }

        /// <summary>
        ///    Whether the division of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillDivisionUnderflow ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min * rhs;
        }

        #endregion Overflow/Underflow checks

        #region Math operations

        /// <summary>
        ///    Adds both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int64 Add ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillAdditionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs + rhs;
        }

        /// <summary>
        ///    Subtracts both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int64 Subtract ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( WillSubtractionOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs - rhs;
        }

        /// <summary>
        ///    Multiplies both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int64 Multiply ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillMultiplicationUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs * rhs;
        }

        /// <summary>
        ///    Divides both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Int64 Divide ( Int64 lhs, Int64 rhs, Int64 min, Int64 max )
        {
            if ( WillDivisionOverflow ( lhs, rhs, min, max ) )
                return max;
            else if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs / rhs;
        }

        #endregion Math operations

        #endregion Int64

        #region UInt32

        #region Overflow/Underflow check

        /// <summary>
        ///    Whether the addition of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillAdditionOverflow ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the subtraction of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillSubtractionUnderflow ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the multiplication of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillMultiplicationOverflow ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max / rhs;
        }

        /// <summary>
        ///    Whether the division of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillDivisionUnderflow ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min * rhs;
        }

        #endregion Overflow/Underflow check

        #region Math operations

        /// <summary>
        /// Adds both elements with the lower bound being <see cref="UInt32.MinValue" /> and
        /// the upper bound being <see cref="UInt32.MaxValue" />. This is a branchless optimized
        /// implementation.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Add ( UInt32 lhs, UInt32 rhs )
        {
            // Algorithm from: https://web.archive.org/web/20190213215419/https://locklessinc.com/articles/sat_arithmetic/
            unchecked
            {
                var res = lhs + rhs;
                var check = res < lhs;
                unsafe
                {
                    res |= ( UInt32 ) ( -*( Byte* ) &check );
                }
                return res;
            }
        }

        /// <summary>
        /// Adds both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Add ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
                return max;
            else
                return lhs + rhs;
        }

        /// <summary>
        /// Subtracts both elements with the lower bound being <see cref="UInt32.MinValue" /> and
        /// the upper bound being <see cref="UInt32.MaxValue" />. This is a branchless optimized
        /// implementation.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Subtract ( UInt32 lhs, UInt32 rhs )
        {
            // Algorithm from: https://web.archive.org/web/20190213215419/https://locklessinc.com/articles/sat_arithmetic/
            unchecked
            {
                var res = lhs - rhs;
                var check = res <= lhs;
                unsafe
                {
                    res &= ( UInt32 ) ( -*( Byte* ) &check );
                }
                return res;
            }
        }

        /// <summary>
        /// Subtracts both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Subtract ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs - rhs;
        }

        /// <summary>
        /// Multiplies both elements with the lower bound being <see cref="UInt32.MinValue" /> and
        /// the upper bound being <see cref="UInt32.MaxValue" />. This is a branchless optimized
        /// implementation.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Multiply ( UInt32 lhs, UInt32 rhs )
        {
            // Algorithm from: https://web.archive.org/web/20190213215419/https://locklessinc.com/articles/sat_arithmetic/
            unchecked
            {
                var res = ( UInt64 ) lhs * rhs;
                var hi = ( UInt32 ) ( res >> 32 );
                var lo = ( UInt32 ) res;
                var check = hi == 0;
                unsafe
                {
                    return lo | ( UInt32 ) ( -*( Byte* ) &check );
                }
            }
        }

        /// <summary>
        /// Multiplies both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Multiply ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
                return max;
            else
                return lhs * rhs;
        }

        /// <summary>
        /// Divides both elements with the lower bound being <see cref="UInt32.MinValue" /> and
        /// the upper bound being <see cref="UInt32.MaxValue" />. This is a branchless optimized
        /// implementation.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Divide ( UInt32 lhs, UInt32 rhs ) =>
            lhs / rhs;

        /// <summary>
        /// Divides both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt32 Divide ( UInt32 lhs, UInt32 rhs, UInt32 min, UInt32 max )
        {
            if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs / rhs;
        }

        #endregion Math operations

        #endregion UInt32

        #region UInt64

        #region Overflow/Underflow check

        /// <summary>
        ///    Whether the addition of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillAdditionOverflow ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the subtraction of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillSubtractionUnderflow ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
        }

        /// <summary>
        ///    Whether the multiplication of these elements will overflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillMultiplicationOverflow ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs >= max / rhs;
        }

        /// <summary>
        ///    Whether the division of these elements will underflow
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean WillDivisionUnderflow ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( max < min )
                throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

            return lhs != 0 && rhs != 0 && lhs <= min * rhs;
        }

        #endregion Overflow/Underflow check

        #region Math operations

        /// <summary>
        /// Adds both elements with the lower bound being <see cref="UInt64.MinValue" /> and
        /// the upper bound being <see cref="UInt64.MaxValue" />. This is a branchless optimized
        /// implementation.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Add ( UInt64 lhs, UInt64 rhs )
        {
            // Algorithm from: https://web.archive.org/web/20190213215419/https://locklessinc.com/articles/sat_arithmetic/
            unchecked
            {
                var res = lhs + rhs;
                var check = res < lhs;
                unsafe
                {
                    res |= ( UInt64 ) ( -*( Byte* ) &check );
                }
                return res;
            }
        }

        /// <summary>
        /// Adds both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Add ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
                return max;
            else
                return lhs + rhs;
        }

        /// <summary>
        /// Subtracts both elements with the lower bound being <see cref="UInt64.MinValue" /> and
        /// the upper bound being <see cref="UInt64.MaxValue" />. This is a branchless optimized
        /// implementation.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Subtract ( UInt64 lhs, UInt64 rhs )
        {
            // Algorithm from: https://web.archive.org/web/20190213215419/https://locklessinc.com/articles/sat_arithmetic/
            unchecked
            {
                var res = lhs - rhs;
                var check = res <= lhs;
                unsafe
                {
                    res &= ( UInt64 ) ( -*( Byte* ) &check );
                }
                return res;
            }
        }

        /// <summary>
        /// Subtracts both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Subtract ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs - rhs;
        }


        /// <summary>
        /// Multiplies both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Multiply ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
                return max;
            else
                return lhs * rhs;
        }

        /// <summary>
        /// Divides both elements with the lower bound being <see cref="UInt64.MinValue" /> and
        /// the upper bound being <see cref="UInt64.MaxValue" />. This is a branchless optimized
        /// implementation.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Divide ( UInt64 lhs, UInt64 rhs ) =>
            lhs / rhs;

        /// <summary>
        /// Divides both elements
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static UInt64 Divide ( UInt64 lhs, UInt64 rhs, UInt64 min, UInt64 max )
        {
            if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
                return min;
            else
                return lhs / rhs;
        }

        #endregion Math operations

        #endregion UInt64
    }
}
