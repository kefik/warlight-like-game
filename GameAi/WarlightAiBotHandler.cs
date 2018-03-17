namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces;

    public class WarlightAiBotHandler : IOnlineBotHandler<BotTurn>
    {
        private readonly IOnlineBot<BotTurn> onlineBot;
        private readonly RegionsIdsMappingHandler regionsIdsMappingHandler;
        
        public WarlightAiBotHandler(GameBotType gameBotType,
            MapMin mapMin, Difficulty difficulty,
            byte playerEncoded,
            byte[] playersIds,
            bool isFogOfWar,
            Restrictions restrictions)
        {
            regionsIdsMappingHandler = new RegionsIdsMappingHandler(
                    mapMin.RegionsMin.Select(x => x.Id),
                    mapMin.SuperRegionsMin.Select(x => x.Id)
                );

            // create remapped map
            mapMin = regionsIdsMappingHandler.TranslateToNew(mapMin);

            // remap restrictions
            var newRestrictions = regionsIdsMappingHandler.TranslateToNew(restrictions);
            
            onlineBot = new GameBotCreator().Create(gameBotType, mapMin, difficulty,
                playerEncoded, playersIds,
                isFogOfWar, newRestrictions);
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

            return regionsIdsMappingHandler.TranslateToOriginal(turn);
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