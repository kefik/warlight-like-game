namespace FormatConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Collections;

    /// <summary>
    /// Wrapper around BidirectionalDictionary, whose purpose is to map Ids of structures
    /// both ways with more friendly interface.
    /// </summary>
    public class IdsMappingDictionary : IIdsMapper
    {
        /// <summary>
        /// First parameter is Original Id, second parameter is Mapped Id.
        /// </summary>
        private readonly BidirectionalDictionary<int, int> bidirectionalDictionary;

        public IdsMappingDictionary()
        {
            bidirectionalDictionary = new BidirectionalDictionary<int, int>();
        }

        public IdsMappingDictionary(IEnumerable<int> ids) : this()
        {
            foreach (int id in ids)
            {
                GetMappedIdOrInsert(id);
            }
        }

        public void Clear()
        {
            bidirectionalDictionary.Clear();
        }

        /// <summary>
        /// Gets mapped ID based on <see cref="originalId"/>. If the <seealso cref="originalId"/>
        /// doesnt exist, creates new entry with it. Starts from 0.
        /// </summary>
        /// <param name="originalId"></param>
        /// <returns></returns>
        public int GetMappedIdOrInsert(int originalId)
        {
            // dictionary contains the value => return it
            if (bidirectionalDictionary.TryGetValue(originalId, second: out int mappedId))
            {
                return mappedId;
            }

            // doesnt contain the value => add it to the dictionary and return the value
            int currentMax = bidirectionalDictionary.Count == 0 ? -1 : bidirectionalDictionary.Max(x => x.Value);

            int newMax = currentMax + 1;

            bidirectionalDictionary.Add(originalId, newMax);

            return newMax;
        }

        public bool TryGetNewId(int originalId, out int mappedId)
        {
            return bidirectionalDictionary.TryGetValue(originalId, second: out mappedId);
        }

        public bool TryGetOriginalId(int mappedId, out int originalId)
        {
            return bidirectionalDictionary.TryGetValue(mappedId, first: out originalId);
        }

        public int GetOriginalId(int mappedId)
        {
            if (!bidirectionalDictionary.TryGetValue(mappedId,
                first: out int originalId))
            {
                throw new IndexOutOfRangeException();
            }

            return originalId;
        }

        public int GetNewId(int originalId)
        {
            if (!bidirectionalDictionary.TryGetValue(originalId, second: out int mappedId))
            {
                throw new IndexOutOfRangeException();
            }

            return mappedId;
        }
    }
}