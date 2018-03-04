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

    public class WarlightAiBotHandler : IOnlineBotHandler<BotTurn>
    {
        private readonly IOnlineBot<BotTurn> onlineBot;
        private readonly IIdsTranslationUnit translationUnit;

        public WarlightAiBotHandler(Game game, Player player, GameBotType gameBotType,
            IEnumerable<IGameBeginningRestriction> gameBeginningRestrictions = null,
            IEnumerable<IGameRestriction> gameRestrictions = null)
        {
            onlineBot = new GameBotCreator().CreateFromGame(game, player,
                gameBotType,
                out var regionsIdsMappingDictionary,
                gameBeginningRestrictions, gameRestrictions);

            translationUnit = regionsIdsMappingDictionary;
        }

        public WarlightAiBotHandler(GameBotType gameBotType, MapMin mapMin, Difficulty difficulty, byte playerEncoded, bool isFogOfWar,
            IEnumerable<IGameBeginningRestriction> gameBeginningRestrictions = null,
            IEnumerable<IGameRestriction> gameRestrictions = null)
        {
            onlineBot = new GameBotCreator().Create(gameBotType, mapMin, difficulty, playerEncoded,
                isFogOfWar, out var regionsIdsMappingDictionary, gameBeginningRestrictions, gameRestrictions);

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