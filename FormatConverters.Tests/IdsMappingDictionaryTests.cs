namespace FormatConverters.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class IdsMappingDictionaryTests
    {
        private IdsMappingDictionary dictionary;

        [SetUp]
        public void Initialize()
        {
            dictionary = new IdsMappingDictionary();
        }

        [Test]
        public void Test1()
        {
            var regionIds = new List<int>()
            {
                1, 2, 3, 4, 5, 6
            };

            // check insert
            foreach (int regionId in regionIds)
            {
                Assert.AreEqual(regionId - 1, dictionary.GetMappedIdOrInsert(regionId));
            }

            // check getting new value
            foreach (int regionId in regionIds)
            {
                Assert.AreEqual(regionId - 1, dictionary.GetNewId(regionId));
            }

            // check getting old values
            foreach (int regionId in regionIds)
            {
                Assert.AreEqual(regionId, dictionary.GetOriginalId(regionId - 1));
            }
        }
    }
}