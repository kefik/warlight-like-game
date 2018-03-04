namespace Common.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Finds out whether given array is null or empty.
        /// </summary>
        /// <typeparam name="TType">Type of the array.</typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<TType>(this TType[] array)
        {
            return array == null || array.Length == 0;
        }
    }
}