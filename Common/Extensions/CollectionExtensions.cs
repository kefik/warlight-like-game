namespace Common.Extensions
{
    using System.Collections.Generic;

    public static class CollectionExtensions
    {
        /// <summary>
        /// Finds out whether given array is null or empty.
        /// </summary>
        /// <typeparam name="TType">Type of the array.</typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<TType>(this ICollection<TType> collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}