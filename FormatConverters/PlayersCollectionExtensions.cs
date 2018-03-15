namespace FormatConverters
{
    using System.Collections.Generic;
    using GameAi.Data;
    using GameObjectsLib.Players;

    public static class PlayersCollectionExtensions
    {
        public static IIdsMapper CreateCompressedAiMapper(this ICollection<Player> gamePlayers)
        {
            var playerIdsMapper = new IdsMappingDictionary();

            // no owner == 0
            playerIdsMapper.GetMappedIdOrInsert(0);

            // play
            foreach (Player gamePlayer in gamePlayers)
            {
                playerIdsMapper.GetMappedIdOrInsert(gamePlayer.Id);
            }

            return playerIdsMapper;
        }
    }
}