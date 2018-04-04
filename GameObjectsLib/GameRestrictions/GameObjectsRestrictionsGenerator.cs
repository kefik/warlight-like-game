namespace GameObjectsLib.GameRestrictions
{
    using System;
    using System.Collections.Generic;
    using Game;
    using GameMap;
    using Players;

    public class GameObjectsRestrictionsGenerator
    {
        private readonly Map map;
        private readonly IList<Player> players;
        private readonly int regionsToChooseCount;
        private readonly Random random;

        public GameObjectsRestrictionsGenerator(Map map, IList<Player> players, int regionsToChooseCount)
        {
            this.map = map;
            this.players = players;
            this.regionsToChooseCount = regionsToChooseCount;
            this.random = new Random();
        }

        public GameObjectsRestrictions Generate()
        {
            var gameObjectsRestrictions = new GameObjectsRestrictions();

            var superRegions = map.SuperRegions;

            var selectedRegions = new HashSet<Region>();
            

            foreach (Player player in players)
            {
                var gameBeginningRestriction = new GameObjectsBeginningRestriction()
                {
                    Player =  player,
                    RegionsToChooseCount = regionsToChooseCount
                };

                foreach (var superRegion in superRegions)
                {
                    var regions = superRegion.Regions;

                    Region chosenRegion;
                    do
                    {
                        int regionsIndex = random.Next(regions.Count);
                        chosenRegion = regions[regionsIndex];
                    } while (selectedRegions.Contains(chosenRegion));

                    selectedRegions.Add(chosenRegion);
                    gameBeginningRestriction.RegionsPlayersCanChoose.Add(chosenRegion);
                }

                gameObjectsRestrictions.GameBeginningRestrictions.Add(gameBeginningRestriction);
            }

            return gameObjectsRestrictions;
        }
    }
}