﻿namespace GameAi
{
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using GameRecording;
    using Interfaces;

    public class WarlightAiBotHandler : IOnlineBotHandler<BotTurn>
    {
        private readonly IOnlineBot<BotTurn> onlineBot;
        private readonly IIdsTranslationUnit translationUnit;

        public WarlightAiBotHandler(Game game, Player player, GameBotType gameBotType)
        {
            onlineBot = new GameBotCreator().CreateFromGame(game, player, gameBotType, out var regionsIdsMappingDictionary);

            translationUnit = regionsIdsMappingDictionary;
        }

        public WarlightAiBotHandler(GameBotType gameBotType, MapMin mapMin, Difficulty difficulty, byte playerEncoded, bool isFogOfWar)
        {
            onlineBot = new GameBotCreator().Create(gameBotType, mapMin, difficulty, playerEncoded,
                isFogOfWar, out var regionsIdsMappingDictionary);

            translationUnit = regionsIdsMappingDictionary;
        }

        public BotTurn GetCurrentBestMove()
        {
            // TODO: translate format
            // TODO: return current best move
            throw new System.NotImplementedException();
        }

        public async Task<BotTurn> FindBestMoveAsync()
        {
            var turn = await onlineBot.FindBestMoveAsync();

            // TODO: translate format

            throw new System.NotImplementedException();
        }
    }
}