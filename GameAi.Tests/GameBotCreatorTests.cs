namespace GameAi.Tests
{
    using EvaluationStructures;
    using GameObjectsLib;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class GameBotCreatorTests
    {
        private GameBotCreator creator;

        [SetUp]
        public void Initialize()
        {
            creator = new GameBotCreator();
        }

        [Test]
        public void CreateFromMinifiedTest()
        {
            RegionMin[] regionsMin = {
                new RegionMin(8, 10, 2)
                {
                    NeighbourRegionsIds = new []{1}
                },
                new RegionMin(5, 10, 4, 1)
                {
                    NeighbourRegionsIds = new []{0}
                },
            };

            SuperRegionMin[] superRegionsMin =
            {
                new SuperRegionMin(10, 5)
                {
                    RegionsIds = new []{8, 5}
                }
            };

            var mapMin = new MapMin(regionsMin, superRegionsMin);

            var bot = creator.Create(GameBotType.MonteCarloTreeSearchBot, mapMin, Difficulty.Hard, playerEncoded: 1,
                isFogOfWar: true, regionsIdsMappingDictionary: out IdsMappingDictionary regionsIdsMappingDictionary);

            // validate dictionary
            {
                bool hasFive = regionsIdsMappingDictionary.TryGetNewId(5, out int newId);
                IsTrue(hasFive);

                bool hasEight = regionsIdsMappingDictionary.TryGetNewId(8, out int newId2);
                IsTrue(hasEight);

                IsTrue(newId == 0 || newId == 1);
                IsTrue(newId2 == 0 || newId2 == 1);
                AreNotEqual(newId2, newId);
            }
        }
    }
}