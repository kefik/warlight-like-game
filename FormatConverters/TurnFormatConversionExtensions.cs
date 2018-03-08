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
        /// <returns></returns>
        public static BotTurn ToBotTurn(this Turn turn)
        {
            switch (turn)
            {
                case GameBeginningTurn gameBeginingTurn:
                    return gameBeginingTurn.ToBotGameBeginningTurn();
                case GameTurn gameTurn:
                    return gameTurn.ToBotGameTurn();
                default:
                    throw new ArgumentException($"Invalid type of {nameof(turn)} for conversion.");
            }
        }

        /// <summary>
        /// Converts from <see cref="GameTurn"/> to <seealso cref="BotGameTurn"/>.
        /// </summary>
        /// <param name="gameTurn"></param>
        /// <returns></returns>
        public static BotGameTurn ToBotGameTurn(this GameTurn gameTurn)
        {
            var botGameTurn = new BotGameTurn(gameTurn.PlayerOnTurn.Id);
            
            // deploys
            foreach (Deployment deployment in gameTurn.Deploying.ArmiesDeployed)
            {
                botGameTurn.Deployments.Add((deployment.Region.Id, deployment.Army));
            }

            // attacks
            foreach (Attack attack in gameTurn.Attacking.Attacks)
            {
                botGameTurn.Attacks.Add((attack.AttackingPlayer.Id,
                    attack.Attacker.Id, attack.AttackingArmy,
                    attack.Defender.Id));
            }

            return botGameTurn;
        }

        /// <summary>
        /// Converts from <see cref="GameBeginningTurn"/> to <seealso cref="BotGameBeginningTurn"/>.
        /// </summary>
        /// <param name="gameBeginningTurn"></param>
        /// <returns></returns>
        public static BotGameBeginningTurn ToBotGameBeginningTurn(this GameBeginningTurn gameBeginningTurn)
        {
            var botGameBeginningTurn = new BotGameBeginningTurn(gameBeginningTurn.PlayerOnTurn.Id);
            foreach (Seize selectedRegion in gameBeginningTurn.SelectedRegions)
            {
                botGameBeginningTurn.SeizedRegionsIds.Add(selectedRegion.Region.Id);
            }
            return botGameBeginningTurn;
        }

        public static Turn ToTurn(this BotTurn botTurn, Map map, ICollection<Player> players)
        {
            switch (botTurn)
            {
                case BotGameBeginningTurn gameBeginningTurn:
                    return gameBeginningTurn.ToGameBeginningTurn(map, players);
                case BotGameTurn gameTurn:
                    return gameTurn.ToGameTurn(map, players);
                default:
                    throw new ArgumentException($"Invalid type of {nameof(botTurn)} for conversion.");
            }
        }

        public static GameTurn ToGameTurn(this BotGameTurn botGameTurn, Map map, ICollection<Player> players)
        {
            var gameTurn = new GameTurn(players.First(x => x.Id == botGameTurn.PlayerId));
            // deploying
            var deployments = new List<Deployment>();
            foreach ((int regionId, int army) in botGameTurn.Deployments)
            {
                deployments.Add(new Deployment(
                        map.Regions.First(x => x.Id == regionId),
                        army
                    ));
            }
            var deploying = new Deploying(deployments);

            // attacking
            var attacks = new List<Attack>();
            foreach ((int attackingPlayerId, int attackingRegionId, int attackingArmy, int defendingRegionId) in botGameTurn.Attacks)
            {
                attacks.Add(new Attack(
                        players.First(x => x.Id == attackingPlayerId),
                        map.Regions.First(x => x.Id == attackingRegionId),
                        attackingArmy,
                        map.Regions.First(x => x.Id == defendingRegionId)
                    ));
            }

            var attacking = new Attacking(attacks);

            gameTurn.Deploying = deploying;
            gameTurn.Attacking = attacking;

            return gameTurn;
        }

        public static GameBeginningTurn ToGameBeginningTurn(this BotGameBeginningTurn botGameBeginningTurn, Map map, ICollection<Player> players)
        {
            var gameBeginningTurn = new GameBeginningTurn(players.First(x => x.Id == botGameBeginningTurn.PlayerId));

            foreach (int regionId in botGameBeginningTurn.SeizedRegionsIds)
            {
                gameBeginningTurn.SelectedRegions.Add(
                    new Seize(players.First(x => x.Id == botGameBeginningTurn.PlayerId),
                        map.Regions.First(x => x.Id == regionId)));
            }

            return gameBeginningTurn;
        }
    }
}