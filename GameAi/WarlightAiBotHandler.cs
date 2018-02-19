namespace GameAi
{
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using Interfaces;

    public class WarlightAiBotHandler : IOnlineBotHandler<Turn>
    {
        private readonly IOnlineBot<Turn> onlineBot;
        private readonly IIdsTranslationUnit translationUnit;

        public WarlightAiBotHandler(Game game, Player player, GameBotType gameBotType)
        {
            onlineBot = new GameBotCreator().CreateFromGame(game, player, gameBotType, out var regionsIdsMappingDictionary);

            translationUnit = regionsIdsMappingDictionary;
        }

        public WarlightAiBotHandler(GameBotType gameBotType, RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, Difficulty difficulty, byte playerEncoded, bool isFogOfWar)
        {
            onlineBot = new GameBotCreator().Create(gameBotType, regionsMin, superRegionsMin, difficulty, playerEncoded,
                isFogOfWar, out var regionsIdsMappingDictionary);

            translationUnit = regionsIdsMappingDictionary;
        }

        public Turn GetCurrentBestMove()
        {
            // TODO: translate format
            // TODO: return current best move
            throw new System.NotImplementedException();
        }

        public async Task<Turn> FindBestMoveAsync()
        {
            var turn = await onlineBot.FindBestMoveAsync();

            // TODO: translate format

            throw new System.NotImplementedException();
        }
    }
}