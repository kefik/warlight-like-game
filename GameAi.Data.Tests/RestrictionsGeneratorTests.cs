namespace GameAi.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Restrictions;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class RestrictionsGeneratorTests
    {
        private RestrictionsGenerator generator;
        private List<int> regionIds;

        [SetUp]
        public void Initialize()
        {
            regionIds = new List<int>()
            {
                1, 2, 3, 4, 5
            };

            var playerIds = new List<int>()
            {
                1, 2
            };

            generator = new RestrictionsGenerator(regionIds, playerIds);
        }

        [Test]
        public void GenerateTest()
        {
            // iterate test 50 times
            for (int i = 0; i < 50; i++)
            {
                var restrictions = generator.Generate();

                var beginningRestrictions = restrictions
                    .GameBeginningRestrictions;

                AreEqual(2, beginningRestrictions.Count);

                // check player ids
                var playerIdsSelected =
                    beginningRestrictions.Select(x => x.PlayerId).ToList();
                Contains(1, playerIdsSelected);
                Contains(2, playerIdsSelected);

                // check regions have no intersect
                var selectedRegions = (from restriction in beginningRestrictions
                                       from regionId in restriction.RestrictedRegions
                                       select regionId).ToList();
                That(selectedRegions, Is.EquivalentTo(selectedRegions.Distinct()));

                // check that regions are valid
                foreach (int selectedRegion in selectedRegions)
                {
                    Contains(selectedRegion, regionIds);
                }

                // chose same number of regions for each
                AreEqual(1, beginningRestrictions
                    .Select(x => x.RegionsPlayerCanChooseCount)
                    .Distinct()
                    .Count());
            }
        }
    }
}