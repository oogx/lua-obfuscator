namespace GParse.Math
{
    using System;

    /// <summary>
    /// Extensions for the <see cref="Range{T}" /> struct
    /// </summary>
    public static class RangeExtensions
    {
        #region IsNeighbourOf

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<Char> lhs, in Range<Char> rhs ) =>
            Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<Byte> lhs, in Range<Byte> rhs ) =>
            Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<SByte> lhs, in Range<SByte> rhs ) =>
            Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<Int32> lhs, in Range<Int32> rhs ) =>
            Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<UInt32> lhs, in Range<UInt32> rhs ) =>
            ( lhs.End - rhs.Start ) == 1
                || ( rhs.Start - lhs.End ) == 1
                || ( lhs.Start - rhs.End ) == 1
                || ( rhs.End - lhs.Start ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<Int64> lhs, in Range<Int64> rhs ) =>
            Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<UInt64> lhs, in Range<UInt64> rhs ) =>
            ( lhs.End - rhs.Start ) == 1
                || ( rhs.Start - lhs.End ) == 1
                || ( lhs.Start - rhs.End ) == 1
                || ( rhs.End - lhs.Start ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<Double> lhs, in Range<Double> rhs ) =>
            Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;

        /// <summary>
        /// Checks whether a range is neighbour to another
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean IsNeighbourOf ( in this Range<Single> lhs, in Range<Single> rhs ) =>
            Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;

        #endregion IsNeighbourOf
    }
}
