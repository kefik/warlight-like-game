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

    /// <summary>
    /// Handles mapping between formats.
    /// E.g. <see cref="MapMin.GetRegion"/> is using specific format.
    /// This component ensures its mapping.
    /// </summary>
    public class WarlightAiBotHandler : IOnlineBotHandler<BotTurn>
    {
        private readonly IOnlineBot<BotTurn> onlineBot;
        private readonly RegionsIdsMappingHandler regionsIdsMappingHandler;
        
        public WarlightAiBotHandler(GameBotType gameBotType,
            MapMin mapMin, Difficulty difficulty,
            byte myPlayerId,
            byte[] enemyPlayersIds,
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
                myPlayerId, enemyPlayersIds,
                isFogOfWar, newRestrictions);
        }

        public BotTurn GetCurrentBestMove()
        {
            var turn = onlineBot.GetCurrentBestMove();

            if (regionsIdsMappingHandler != null)
            {
                var originalTurn =
                    regionsIdsMappingHandler
                        .TranslateToOriginal(turn);
                return originalTurn;
            }

            return turn;
        }

        public async Task<BotTurn> FindBestMoveAsync()
        {
            var turn = await onlineBot.FindBestMoveAsync();

            // remap if there's any mapping
            if (regionsIdsMappingHandler != null)
            {
                var originalTurn =
                    regionsIdsMappingHandler
                        .TranslateToOriginal(turn);
                return originalTurn;
            }

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

        public void UseFixedDeploy(IEnumerable<BotDeployment> deploymentsToUse)
        {
            onlineBot.UseFixedDeploy(regionsIdsMappingHandler.TranslateToNew(deploymentsToUse));
        }
    }
}