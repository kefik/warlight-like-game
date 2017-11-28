namespace Common.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using Common.Collections;
    using NUnit;
    using NUnit.Framework;

    [TestFixture]
    public class BidirectionalDictionaryTest
    {
        private BidirectionalDictionary<string, int> testDictionary;

        [SetUp]
        public void Setup()
        {
            testDictionary = new BidirectionalDictionary<string, int> {{"test1", 1}, {"test2", 2}, {"test3", 3}};
        }

        [Test]
        public void ContainsKeyTest()
        {
            Assert.True(!testDictionary.ContainsKey(5));

            bool contains1 = testDictionary.ContainsKey(1);
            bool contains2 = testDictionary.ContainsKey("test1");
            bool contains3 = testDictionary.ContainsKey(2);

            Assert.True(contains1 && contains2 && contains3);
        }

        [Test]
        public void AddTest()
        {
            Assert.Throws<ArgumentException>(() => testDictionary.Add(null, 7));
            Assert.Throws<ArgumentException>(() => testDictionary.Add("testt", 2));
            Assert.Throws<ArgumentException>(() => testDictionary.Add("test1", 10));
            
            testDictionary.Add("test4", 4);
            testDictionary.Add("222", int.MaxValue);
        }

        [Test]
        public void CountTest()
        {
            Assert.AreEqual(3, testDictionary.Count);
        }

        [Test]
        public void TryGetValueTest()
        {
            bool doesContain = testDictionary.TryGetValue(2, out string test);
            Assert.True(doesContain);
            Assert.AreEqual(test, "test2");

            bool doesNotContain = testDictionary.TryGetValue(4, out string _);
            Assert.AreEqual(doesNotContain, false);
        }

        [Test]
        public void IndexerTest()
        {
            // get
            Assert.Throws<KeyNotFoundException>(() =>
            {
                int x = testDictionary[""];
            });

            int test1 = testDictionary["test1"];
            Assert.AreEqual(test1, 1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                int x1 = testDictionary[null];
            });

            string test2 = testDictionary[2];
            Assert.AreEqual(test2, "test2");

            // set
            testDictionary[2] = "test10";
            testDictionary[1] = "test1";

            Assert.Throws<ArgumentNullException>(() => { testDictionary[1] = null; });
        }

        [Test]
        public void ForEachTest()
        {
            foreach (var keyValuePair in testDictionary)
            {
                if (keyValuePair.Value == 1 && keyValuePair.Key == "test1")
                { }
                else if (keyValuePair.Key == "test2" && keyValuePair.Value == 2) { }
                else if (keyValuePair.Key == "test3" && keyValuePair.Value == 3)
                { }
                else
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}