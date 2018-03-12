namespace GameAi.Data.Tests
{
    using System;
    using EvaluationStructures;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class MapMinTests
    {
        private MapMin mapMin;

        [SetUp]
        public void Initialize()
        {
            RegionMin[] regionsMin = {
                new RegionMin(0, 0, 2)
                {
                    NeighbourRegionsIds = new []{1}
                }, 
                new RegionMin(1, 0, 4, 1)
                {
                    NeighbourRegionsIds = new []{0}
                }, 
            };

            SuperRegionMin[] superRegionsMin =
            {
                new SuperRegionMin(0, 5)
                {
                    RegionsIds = new [] {0, 1}
                }
            };

            mapMin = new MapMin(regionsMin, superRegionsMin);
        }

        [Test]
        public void DeepCopyTest()
        {
            var newMapMin = mapMin.DeepCopy();

            // is copy test
            IsCopyTest(newMapMin);

            // test that each object is not of same instance (try to change id on once, bcuz its static field)
            for (int i = 0; i < newMapMin.RegionsMin.Length; i++)
            {
                newMapMin.RegionsMin[i].Id = 10;
                AreNotEqual(mapMin.RegionsMin[i].Id, newMapMin.RegionsMin[i].Id);
            }

            for (int i = 0; i < newMapMin.SuperRegionsMin.Length; i++)
            {
                var superRegion = newMapMin.SuperRegionsMin[i];
                superRegion.Id = 10;
                AreNotEqual(superRegion.Id, mapMin.SuperRegionsMin[i].Id);
            }
        }

        [Test]
        public void ShallowCopyTest()
        {
            var newMapMin = mapMin.ShallowCopy();

            IsCopyTest(newMapMin);

            // test that each object is not of same instance (try to change id on once, bcuz its static field)
            for (int i = 0; i < newMapMin.RegionsMin.Length; i++)
            {
                newMapMin.RegionsMin[i].Id = 10;
                AreEqual(mapMin.RegionsMin[i].Id, newMapMin.RegionsMin[i].Id);
            }

            for (int i = 0; i < newMapMin.SuperRegionsMin.Length; i++)
            {
                var superRegion = newMapMin.SuperRegionsMin[i];
                superRegion.Id = 10;
                AreEqual(superRegion.Id, mapMin.SuperRegionsMin[i].Id);
            }
        }



        private void IsCopyTest(MapMin newMapMin)
        {
            // test regions
            for (int i = 0; i < Math.Max((int) newMapMin.RegionsMin.Length, (int) mapMin.RegionsMin.Length); i++)
            {
                var newMapMinRegion = newMapMin.RegionsMin[i];
                var mapMinRegion = mapMin.RegionsMin[i];

                AreEqual(mapMinRegion.Id, newMapMinRegion.Id);
                AreEqual(mapMinRegion.Army, newMapMinRegion.Army);
                AreEqual(mapMinRegion.OwnerId, newMapMinRegion.OwnerId);

                // have same neighbours
                for (int j = 0; j < Math.Max((int) newMapMinRegion.NeighbourRegionsIds.Length, (int) mapMinRegion.NeighbourRegionsIds.Length); j++)
                {
                    AreEqual(mapMinRegion.NeighbourRegionsIds[j], newMapMinRegion.NeighbourRegionsIds[j]);
                }
            }

            for (int i = 0; i < Math.Max((int) newMapMin.SuperRegionsMin.Length, (int) mapMin.SuperRegionsMin.Length); i++)
            {
                var oldSuperRegion = mapMin.SuperRegionsMin[i];
                var newSuperRegion = newMapMin.SuperRegionsMin[i];

                AreEqual(oldSuperRegion.Id, newSuperRegion.Id);
                AreEqual(oldSuperRegion.Bonus, newSuperRegion.Bonus);
                AreEqual(oldSuperRegion.OwnerId, newSuperRegion.OwnerId);

                // have same regions
                for (int j = 0; j < Math.Max((int) oldSuperRegion.RegionsIds.Length, (int) newSuperRegion.RegionsIds.Length); j++)
                {
                    AreEqual(oldSuperRegion.RegionsIds[j], newSuperRegion.RegionsIds[j]);
                }
            }
        }
    }
}