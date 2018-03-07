namespace FormatConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameAi.Data.Restrictions;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.Players;

    public static class RestrictionsFormatConversionExtensions
    {
        /// <summary>
        /// Converts from <see cref="Restrictions"/> format
        /// to <seealso cref="GameObjectsRestrictions"/> format.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="players"></param>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        public static GameObjectsRestrictions ToGameRestrictions(this Restrictions restrictions, Map map,
            ICollection<Player> players)
        {
            GameObjectsRestrictions gameObjectsRestrictions = new GameObjectsRestrictions();
            var gameObjectsGameBeginningRestrictions = new List<GameObjectsBeginningRestriction>();
            foreach (var gameBeginningRestriction in restrictions.GameBeginningRestrictions)
            {
                var gameObjectsBeginningRestriction = new GameObjectsBeginningRestriction();
                gameObjectsBeginningRestriction.Player
                    = players.First(x => x.Id == gameBeginningRestriction.PlayerId);
                gameObjectsBeginningRestriction.RegionsToChooseCount =
                    gameBeginningRestriction.RegionsPlayerCanChooseCount;
                gameObjectsBeginningRestriction.RegionsPlayersCanChoose = new List<Region>();

                foreach (int restrictedRegion in gameBeginningRestriction.RestrictedRegions)
                {
                    gameObjectsBeginningRestriction.RegionsPlayersCanChoose.Add(map.Regions.First(x => x.Id == restrictedRegion));
                }
                gameObjectsGameBeginningRestrictions.Add(gameObjectsBeginningRestriction);
            }
            gameObjectsRestrictions.GameBeginningRestrictions = gameObjectsGameBeginningRestrictions;

            return gameObjectsRestrictions;
        }

        public static Restrictions ToRestrictions(this GameObjectsRestrictions gameObjectsRestrictions)
        {
            throw new NotImplementedException();
        }
    }
}