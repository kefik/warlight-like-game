namespace Common.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents two-way dictionary which is able to either <see cref="TFirst"/> and <see cref="TSecond"/> fast.
    /// </summary>
    /// <remarks>Both <see cref="TFirst"/> and <see cref="TSecond"/> must be unique and not null.</remarks>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public class BidirectionalDictionary<TFirst, TSecond> : ICollection<KeyValuePair<TFirst, TSecond>>
    {
        /// <summary>
        /// Maps key to value.
        /// </summary>
        private readonly Dictionary<TFirst, TSecond> firstToSecondMap;

        /// <summary>
        /// Maps value to key.
        /// </summary>
        private readonly Dictionary<TSecond, TFirst> secondToFirstMap;

        /// <summary>
        /// Obtains object matching the <see cref="TSecond"/> <see cref="key"/>.
        /// </summary>
        /// <param name="key">Index</param>
        /// <remarks>Set is slower due to two-way dictionary.</remarks>
        /// <returns></returns>
        public TFirst this[TSecond key]
        {
            get { return secondToFirstMap[key]; }
            set
            {
                var previousValue = secondToFirstMap[key];

                secondToFirstMap[key] = value;

                firstToSecondMap.Remove(previousValue);
                firstToSecondMap.Add(value, key);
            }
        }

        /// <summary>
        /// Obtains object matching the <see cref="TFirst"/> <see cref="key"/>.
        /// </summary>
        /// <param name="key">Index</param>
        /// <remarks>Set is slower due to two-way dictionary.</remarks>
        /// <returns></returns>
        public TSecond this[TFirst key]
        {
            get { return firstToSecondMap[key]; }
            set
            {
                var previousValue = firstToSecondMap[key];

                firstToSecondMap[key] = value;

                secondToFirstMap.Remove(previousValue);
                secondToFirstMap.Add(value, key);
            }
        }

        public int Count
        {
            get { return firstToSecondMap.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public BidirectionalDictionary()
        {
            firstToSecondMap = new Dictionary<TFirst, TSecond>();
            secondToFirstMap = new Dictionary<TSecond, TFirst>();
        }

        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
        {
            foreach (var secondType in firstToSecondMap)
            {
                yield return secondType;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TFirst first, TSecond second)
        {
            if (first == null || second == null)
            {
                throw new ArgumentNullException();
            }

            firstToSecondMap.Add(first, second);
            secondToFirstMap.Add(second, first);
        }

        public void Add(KeyValuePair<TFirst, TSecond> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TFirst, TSecond>>.Remove(KeyValuePair<TFirst, TSecond> item)
        {
            firstToSecondMap.Remove(item.Key);
            return secondToFirstMap.Remove(item.Value);
        }

        public void Clear()
        {
            firstToSecondMap.Clear();
            secondToFirstMap.Clear();
        }

        bool ICollection<KeyValuePair<TFirst, TSecond>>.Contains(KeyValuePair<TFirst, TSecond> item)
        {
            // null not supported
            if (item.Key == null || item.Value == null)
            {
                return false;
            }

            // its enough to test one
            // if one dictionary contains
            if (firstToSecondMap.TryGetValue(item.Key, out TSecond value))
            {
                return value.Equals(item.Value);
            }

            return false;
        }

        /// <summary>
        /// Attempts to get value, returning false on unsuccessful attempt.
        /// </summary>
        /// <remarks>O(1) time.</remarks>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TFirst key, out TSecond value)
        {
            bool returnValue = firstToSecondMap.TryGetValue(key, out TSecond val);
            value = val;
            return returnValue;
        }

        /// <summary>
        /// Attempts to get value, returning false on unsuccessful attempt.
        /// </summary>
        /// <remarks>O(1) time.</remarks>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TSecond key, out TFirst value)
        {
            bool returnValue = secondToFirstMap.TryGetValue(key, out TFirst val);
            value = val;
            return returnValue;
        }

        public bool ContainsKey(TFirst key)
        {
            return firstToSecondMap.ContainsKey(key);
        }

        public bool ContainsKey(TSecond key)
        {
            return secondToFirstMap.ContainsKey(key);
        }

        void ICollection<KeyValuePair<TFirst, TSecond>>.CopyTo(KeyValuePair<TFirst, TSecond>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }
}