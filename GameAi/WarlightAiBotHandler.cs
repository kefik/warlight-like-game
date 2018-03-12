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
        private readonly IIdsTranslationUnit regionIdsTranslationUnit;
        private readonly IIdsTranslationUnit playerIdsTranslationUnit;

        public WarlightAiBotHandler(Game game, HumanPlayer player, GameBotType gameBotType,
            Restrictions restrictions)
        {
            onlineBot = new GameBotCreator().CreateFromGame(game, player,
                gameBotType, 
                out var regionsIdsMappingDictionary,
                out var playersIdsMappingDictionary,
                restrictions);

            regionIdsTranslationUnit = regionsIdsMappingDictionary;
            this.playerIdsTranslationUnit = playersIdsMappingDictionary;
        }
        public WarlightAiBotHandler(Game game, AiPlayer player,
            Restrictions restrictions)
        {
            onlineBot = new GameBotCreator().CreateFromGame(game, player,
                player.BotType,
                out var regionsIdsMappingDictionary,
                out var playersIdsMappingDictionary,
                restrictions);

            regionIdsTranslationUnit = regionsIdsMappingDictionary;
            this.playerIdsTranslationUnit = playersIdsMappingDictionary;
        }

        public WarlightAiBotHandler(GameBotType gameBotType, MapMin mapMin, Difficulty difficulty, byte playerEncoded, bool isFogOfWar,
            Restrictions restrictions)
        {
            onlineBot = new GameBotCreator().Create(gameBotType, mapMin, difficulty, playerEncoded,
                isFogOfWar, out var regionsIdsMappingDictionary, restrictions);

            regionIdsTranslationUnit = regionsIdsMappingDictionary;
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
            if (!playerIdsTranslationUnit.TryGetOriginalId(turn.PlayerId, out int playerId))
            {
                throw new ArgumentException();
            }

            switch (turn)
            {
                case BotGameBeginningTurn gameBeginningTurn:
                    var translatedRegions = gameBeginningTurn.SeizedRegionsIds.Select(x =>
                    {
                        if (regionIdsTranslationUnit.TryGetOriginalId(x, out int originalId))
                        {
                            return originalId;
                        }

                        throw new ArgumentException("Exception");
                    }).ToList();

                    return new BotGameBeginningTurn(playerId)
                    {
                        SeizedRegionsIds = translatedRegions
                    };
                case BotGameTurn gameTurn:
                    var attacks = gameTurn.Attacks.Select(x =>
                    {
                        if (!regionIdsTranslationUnit.TryGetOriginalId(x.AttackingRegionId,
                            out int originalAttackingRegion))
                        {
                            throw new ArgumentException();
                        }

                        if (!regionIdsTranslationUnit.TryGetOriginalId(x.DefendingRegionId,
                            out int originalDefendingRegion))
                        {
                            throw new ArgumentException();
                        }
                        
                        if (!playerIdsTranslationUnit.TryGetOriginalId(x.AttackingPlayerId, out int attackingPlayerId))
                        {
                            throw new ArgumentException();
                        }

                        return (attackingPlayerId, originalAttackingRegion, x.AttackingArmy, originalDefendingRegion);
                    }).ToList();

                    var deploys = gameTurn.Deployments.Select(x =>
                    {
                        if (!regionIdsTranslationUnit.TryGetOriginalId(x.RegionId, out int originalRegionId))
                        {
                            throw new ArgumentException();
                        }

                        if (!regionIdsTranslationUnit.TryGetOriginalId(x.DeployingPlayerId, out int deployingPlayerId))
                        {
                            throw new ArgumentException();
                        }

                        return (originalRegionId, x.Army, deployingPlayerId);
                    }).ToList();

                    return new BotGameTurn(playerId)
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
    }
}