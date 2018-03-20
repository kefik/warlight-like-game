namespace FormatConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameAi.Data.GameRecording;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;

    public static class TurnFormatConversionExtensions
    {
        /// <summary>
        /// Converts from <see cref="Turn"/> to <seealso cref="BotTurn"/>.
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="playerIdsesMapper"></param>
        /// <returns></returns>
        public static BotTurn ToBotTurn(this Turn turn,
            IIdsMapper playerIdsesMapper)
        {
            switch (turn)
            {
                case GameBeginningTurn gameBeginingTurn:
                    return gameBeginingTurn
                        .ToBotGameBeginningTurn(playerIdsesMapper);
                case GameTurn gameTurn:
                    return gameTurn.ToBotGameTurn(playerIdsesMapper);
                default:
                    throw new ArgumentException($"Invalid type of {nameof(turn)} for conversion.");
            }
        }

        /// <summary>
        /// Converts from <see cref="GameTurn"/> to <seealso cref="BotGameTurn"/>.
        /// </summary>
        /// <param name="gameTurn"></param>
        /// <param name="playerIdsesMapper"></param>
        /// <returns></returns>
        public static BotGameTurn ToBotGameTurn(this GameTurn gameTurn,
            IIdsMapper playerIdsesMapper)
        {
            var botGameTurn = new BotGameTurn(playerIdsesMapper
                .GetNewId(gameTurn.PlayerOnTurn.Id));
            
            // deploys
            foreach (Deployment deployment in gameTurn.Deploying
                .ArmiesDeployed)
            {
                botGameTurn.Deployments
                    .Add(new BotDeployment(deployment.Region.Id, deployment.Army,
                        playerIdsesMapper.GetNewId(deployment.DeployingPlayer.Id)));
            }

            // attacks
            foreach (Attack attack in gameTurn.Attacking.Attacks)
            {
                botGameTurn.Attacks.Add(new BotAttack(attack.AttackingPlayer.Id,
                    attack.Attacker.Id, attack.AttackingArmy,
                    attack.Defender.Id));
            }

            return botGameTurn;
        }

        /// <summary>
        /// Converts from <see cref="GameBeginningTurn"/> to <seealso cref="BotGameBeginningTurn"/>.
        /// </summary>
        /// <param name="gameBeginningTurn"></param>
        /// <param name="playerIdsesMapper"></param>
        /// <returns></returns>
        public static BotGameBeginningTurn ToBotGameBeginningTurn(this GameBeginningTurn gameBeginningTurn,
            IIdsMapper playerIdsesMapper)
        {
            var botGameBeginningTurn = new BotGameBeginningTurn(
                playerIdsesMapper
                .GetNewId(gameBeginningTurn.PlayerOnTurn.Id));
            foreach (Seize selectedRegion in gameBeginningTurn.SelectedRegions)
            {
                botGameBeginningTurn.SeizedRegionsIds.Add(selectedRegion.Region.Id);
            }
            return botGameBeginningTurn;
        }

        public static Turn ToTurn(this BotTurn botTurn, Map map, ICollection<Player> players,
            IIdsMapper playerIdsesMapper)
        {
            switch (botTurn)
            {
                case BotGameBeginningTurn gameBeginningTurn:
                    return gameBeginningTurn.ToGameBeginningTurn(map, players, playerIdsesMapper);
                case BotGameTurn gameTurn:
                    return gameTurn.ToGameTurn(map, players, playerIdsesMapper);
                default:
                    throw new ArgumentException($"Invalid type of {nameof(botTurn)} for conversion.");
            }
        }

        public static GameTurn ToGameTurn(this BotGameTurn botGameTurn,
            Map map, ICollection<Player> players,
            IIdsMapper playerIdsesMapper)
        {
            var gameTurn = new GameTurn(players
                .First(x => x.Id == playerIdsesMapper
                    .GetOriginalId(botGameTurn.PlayerId)));
            // deploying
            var deployments = new List<Deployment>();
            foreach (var deployment in botGameTurn.Deployments)
            {
                deployments.Add(new Deployment(
                        map.Regions.First(x => x.Id == deployment.RegionId),
                        deployment.Army,
                        players.First(x => x.Id == playerIdsesMapper.GetOriginalId(deployment.DeployingPlayerId))
                    ));
            }
            var deploying = new Deploying(deployments);

            // attacking
            var attacks = new List<Attack>();
            foreach (var attack in botGameTurn.Attacks)
            {
                attacks.Add(new Attack(
                        players.First(x => x.Id == playerIdsesMapper.GetOriginalId(attack.AttackingPlayerId)),
                        map.Regions.First(x => x.Id == attack.AttackingRegionId),
                        attack.AttackingArmy,
                        map.Regions.First(x => x.Id == attack.DefendingRegionId)
                    ));
            }

            var attacking = new Attacking(attacks);

            gameTurn.Deploying = deploying;
            gameTurn.Attacking = attacking;

            return gameTurn;
        }

        public static GameBeginningTurn ToGameBeginningTurn(this BotGameBeginningTurn botGameBeginningTurn,
            Map map, ICollection<Player> players,
            IIdsMapper playerIdsesMapper)
        {
            var gameBeginningTurn = new GameBeginningTurn(players
                .First(x => x.Id == playerIdsesMapper
                    .GetOriginalId(botGameBeginningTurn.PlayerId)));

            foreach (int regionId in botGameBeginningTurn.SeizedRegionsIds)
            {
                gameBeginningTurn.SelectedRegions.Add(
                    new Seize(players.First(x => x.Id == playerIdsesMapper
                        .GetOriginalId(botGameBeginningTurn.PlayerId)),
                        map.Regions.First(x => x.Id == regionId)));
            }

            return gameBeginningTurn;
        }
    }
}