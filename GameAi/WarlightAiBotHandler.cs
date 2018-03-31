#if DEBUG
#define LOG_HANDLER
#endif

namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

#if LOG_HANDLER
            Debug.WriteLine("WARLIGHT AI BOT HANDLER INSTANCE CREATED");
#endif
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
            
            Debug.WriteLine($"RETURNING TURN (BOT {turn.PlayerId})");

            var originalTurn = regionsIdsMappingHandler.TranslateToOriginal(turn);

            Debug.WriteLine($"RETURNING CONVERTED REGIONS TURN (BOT {originalTurn.PlayerId})");

            return originalTurn;
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