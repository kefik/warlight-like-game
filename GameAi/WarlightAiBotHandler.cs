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
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using Interfaces;

    public class WarlightAiBotHandler : IOnlineBotHandler<BotTurn>
    {
        private readonly IOnlineBot<BotTurn> onlineBot;
        private readonly IIdMapper regionIdMapper;
        
        public WarlightAiBotHandler(GameBotType gameBotType,
            MapMin mapMin, Difficulty difficulty,
            byte playerEncoded, bool isFogOfWar,
            Restrictions restrictions)
        {
            onlineBot = new GameBotCreator().Create(gameBotType, mapMin, difficulty, playerEncoded,
                isFogOfWar, out var regionsIdsMappingDictionary, restrictions);

            regionIdMapper = regionsIdsMappingDictionary;

            // remap restrictions (region ids)
            RemapRestrictions(restrictions);
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

            return TranslateTurn(turn);
        }

        private BotTurn TranslateTurn(BotTurn turn)
        {
            switch (turn)
            {
                case BotGameBeginningTurn gameBeginningTurn:
                    var translatedRegions = gameBeginningTurn.SeizedRegionsIds.Select(x =>
                    {
                        if (regionIdMapper.TryGetOriginalId(x, out int originalId))
                        {
                            return originalId;
                        }

                        throw new ArgumentException("Exception");
                    }).ToList();

                    return new BotGameBeginningTurn(turn.PlayerId)
                    {
                        SeizedRegionsIds = translatedRegions
                    };
                case BotGameTurn gameTurn:
                    var attacks = gameTurn.Attacks.Select(x =>
                    {
                        if (!regionIdMapper.TryGetOriginalId(x.AttackingRegionId,
                            out int originalAttackingRegion))
                        {
                            throw new ArgumentException();
                        }

                        if (!regionIdMapper.TryGetOriginalId(x.DefendingRegionId,
                            out int originalDefendingRegion))
                        {
                            throw new ArgumentException();
                        }

                        return (x.AttackingPlayerId, originalAttackingRegion, x.AttackingArmy, originalDefendingRegion);
                    }).ToList();

                    var deploys = gameTurn.Deployments.Select(x =>
                        (regionIdMapper.GetOriginalId(x.RegionId),
                            x.Army, x.DeployingPlayerId)).ToList();

                    return new BotGameTurn(turn.PlayerId)
                    {
                        Attacks = attacks,
                        Deployments = deploys
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StopEvaluation()
        {
            onlineBot.StopEvaluation();
        }

        public Task StopEvaluation(TimeSpan timeSpan)
        {
            return Task.Delay(timeSpan).ContinueWith(x => onlineBot.StopEvaluation());
        }

        private void RemapRestrictions(Restrictions restrictions)
        {
            foreach (GameBeginningRestriction gameBeginningRestriction
                in restrictions.GameBeginningRestrictions)
            {
                gameBeginningRestriction.RestrictedRegions =
                    gameBeginningRestriction.RestrictedRegions
                    .Select(x => regionIdMapper.GetNewId(x))
                    .ToList();
            }
        }
    }
}