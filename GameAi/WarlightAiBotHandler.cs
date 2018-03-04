namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using Interfaces;
    using InterFormatCommunication.GameRecording;
    using InterFormatCommunication.Restrictions;

    public class WarlightAiBotHandler : IOnlineBotHandler<BotTurn>
    {
        private readonly IOnlineBot<BotTurn> onlineBot;
        private readonly IIdsTranslationUnit translationUnit;

        public WarlightAiBotHandler(Game game, Player player, GameBotType gameBotType,
            Restrictions restrictions)
        {
            onlineBot = new GameBotCreator().CreateFromGame(game, player,
                gameBotType,
                out var regionsIdsMappingDictionary,
                restrictions);

            translationUnit = regionsIdsMappingDictionary;
        }

        public WarlightAiBotHandler(GameBotType gameBotType, MapMin mapMin, Difficulty difficulty, byte playerEncoded, bool isFogOfWar,
            Restrictions restrictions)
        {
            onlineBot = new GameBotCreator().Create(gameBotType, mapMin, difficulty, playerEncoded,
                isFogOfWar, out var regionsIdsMappingDictionary, restrictions);

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

            return turn;
        }

        public void StopEvaluation()
        {
            onlineBot.StopEvaluation();
        }

        public Task StopEvaluation(TimeSpan timeSpan)
        {
            return Task.Delay(timeSpan).ContinueWith(x => onlineBot.StopEvaluation());
        }
    }
}