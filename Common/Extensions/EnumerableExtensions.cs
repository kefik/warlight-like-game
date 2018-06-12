namespace Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Collections;

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts enumerable to HashSet.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TSetType"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static HashSet<TSetType> ToHashSet<TType, TSetType>(this IEnumerable<TType> enumerable, Func<TType, TSetType> selector)
        {
            HashSet<TSetType> hashSet = new HashSet<TSetType>();

            foreach (var item in enumerable)
            {
                hashSet.Add(selector(item));
            }

            return hashSet;
        }

        /// <summary>
        /// Converts enumerable to HashSet.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static HashSet<TType> ToHashSet<TType>(this IEnumerable<TType> enumerable)
        {
            return ToHashSet(enumerable, x => x);
        }

        /// <summary>
        /// Repeats in round specified <see cref="enumerable"/>.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<TType> RepeatInRound<TType>(
            this IEnumerable<TType> enumerable)
        {
            while (true)
            {
                foreach (TType item in enumerable)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns true, if two enumerables are equal ignoring order.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="otherEnumerable"></param>
        /// <returns></returns>
        public static bool UnorderedEquals<TType>(
            this IEnumerable<TType> enumerable, IEnumerable<TType> otherEnumerable)
        {
            var set = otherEnumerable.ToHashSet();

            return enumerable.All(item => set.Contains(item));
        }
    }
}