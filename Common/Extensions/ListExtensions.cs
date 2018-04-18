namespace Common.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class ListExtensions
    {
        [ThreadStatic]
        private static Random random;

        private static Random Random
        {
            get
            {
                return random ??
                       (random = new Random());
            }
        }

        /// <summary>
        /// Randomly reorders specified collection of type <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <remarks>Is thread safe.</remarks>
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = ListExtensions.Random;

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}