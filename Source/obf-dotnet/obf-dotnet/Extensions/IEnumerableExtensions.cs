using System;
using System.Collections.Generic;

namespace Obfuscator.Extensions
{
    public static class IEnumerableExtensions
    {
        private static Random Generator = new Random();

        public static IList<T> Shuffle<T>(this IList<T> List)
        {
            for (var I = 0; I < List.Count; I++) { List.Swap(I, Generator.Next(I, List.Count)); }; return (List);
        }

        public static void Swap<T>(this IList<T> List, int I, int J)
        {
            var Temp = List[I]; List[I] = List[J]; List[J] = Temp;
        }

        public static T Random<T>(this IList<T> List) => List[Generator.Next(0, List.Count)];
    };
};