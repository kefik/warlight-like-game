namespace GameObjectsLib.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Common.Extensions;
    using Game;
    using GameAi.Data;
    using GameMap;
    using GameRecording;
    using GameUser;
    using NUnit.Framework;
    using Players;
    using Region = GameMap.Region;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class GameTests
    {
        private Map map;
        private AiPlayer pc1;
        private HumanPlayer testUser;
        private Region czechia;
        private Region austria;
        private Region poland;

        [SetUp]
        public void Initialize()
        {
            // set current directory to be the test bin directory (resharper using its own)
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var mapXmlString = TestMaps.TestMap;

            using (var stream = mapXmlString.ToStream())
            {
                map = new Map(1, "TEST", 1, stream);
            }
            czechia = map.Regions.First(x => x.Name == "Czechia");
            poland = map.Regions.First(x => x.Name == "Poland");
            austria = map.Regions.First(x => x.Name == "Austria");

            pc1 = new AiPlayer(Difficulty.Hard, "PC1", KnownColor.Beige, GameBotType.MonteCarloTreeSearchBot);
            testUser = new HumanPlayer(new LocalUser("TestUser"), KnownColor.ActiveBorder);
        }

        [Test]
        public void ReconstructGraphTest()
        {
            var players = new List<Player>
            {
                pc1,
                testUser
            };
            var game = new GameFactory().CreateGame(0, GameType.SinglePlayer, map, players, true, null);

            #region Initialize game state

            // initial seizes
            var initialSeizes = new List<Seize>();
            austria = map.Regions.First(x => x.Name == "Austria");
            poland = map.Regions.First(x => x.Name == "Poland");
            initialSeizes.Add(new Seize(pc1, austria));
            initialSeizes.Add(new Seize(testUser, poland));
            var linearizedGameBeginningRound = new LinearizedGameBeginningRound(initialSeizes);
            game.AllRounds.Add(linearizedGameBeginningRound);

            // initial deploys and attacks
            var deploy = new List<Deployment>
            {
                new Deployment(austria, 7, pc1),
                new Deployment(poland, 7, testUser)
            };

            czechia = map.Regions.First(x => x.Name == "Czechia");
            var attacks = new List<Attack>
            {
                new Attack(pc1, austria, 6, czechia)
                {
                    // state after attack
                    PostAttackMapChange = new PostAttackMapChange()
                    {
                        DefendingRegionOwner = pc1,
                        AttackingRegionArmy = 1,
                        DefendingRegionArmy = 5
                    }
                },
                new Attack(testUser, poland, 3, czechia)
                {
                    PostAttackMapChange = new PostAttackMapChange()
                    {
                        DefendingRegionOwner = pc1,
                        DefendingRegionArmy = 4,
                        AttackingRegionArmy = 5
                    }
                }
            };
            game.AllRounds.Add(new LinearizedGameRound(
                new Deploying(deploy),
                new Attacking(attacks)));

            austria.Army = 1;
            austria.ChangeOwner(pc1);

            poland.Army = 5;
            poland.ChangeOwner(testUser);

            czechia.Army = 4;
            czechia.ChangeOwner(pc1);

            #endregion

            // copy the game
            var copiedGame = game.DeepCopy();
            var copiedMap = copiedGame.Map;
            
            // check players and regions
            
            // check player instance
            foreach (Player copiedGamePlayer in copiedGame.Players)
            {
                foreach (Region controlledRegion in copiedGamePlayer.ControlledRegions)
                {
                    AreSame(copiedGamePlayer, controlledRegion.Owner);
                }
            }

            // check region instances
            foreach (Region copiedMapRegion in copiedMap.Regions.Where(x => x.Owner != null))
            {
                var owner = copiedMapRegion.Owner;
                var ownerRegions = owner.ControlledRegions;

                var sameRegion = ownerRegions.First(x => x == copiedMapRegion);
                AreSame(copiedMapRegion, sameRegion);
            }

            // check rounds
            foreach (ILinearizedRound copiedGameAllRound in copiedGame.AllRounds)
            {
                switch (copiedGameAllRound)
                {
                    case LinearizedGameRound round:
                        var attacksDefenders = round.Attacking.Attacks.Select(x => x.Defender);
                        var attacksAttackers = round.Attacking.Attacks.Select(x => x.Attacker);
                        // all instances regions are connected
                        IsTrue(attacksDefenders.All(x => copiedMap.Regions.Any(y => object.ReferenceEquals(y, x))));
                        IsTrue(attacksAttackers.All(x => copiedMap.Regions.Any(y => object.ReferenceEquals(y, x))));
                        // all instances of players are connected
                        var attackingPlayers = round.Attacking.Attacks.Select(x => x.AttackingPlayer);
                        IsTrue(attackingPlayers.All(x => copiedGame.Players.Any(y => object.ReferenceEquals(y, x))));
                        IsTrue(round.Attacking.Attacks.All(x =>
                            copiedGame.Players.Any(y => object.ReferenceEquals(x.PostAttackMapChange.DefendingRegionOwner, y))));
                        break;
                    case LinearizedGameBeginningRound round:
                        var seizedRegions = round.SelectedRegions.Select(x => x.Region);
                        var seizingPlayers = round.SelectedRegions.Select(x => x.SeizingPlayer);

                        IsTrue(seizedRegions.All(x => copiedMap.Regions.Any(y => object.ReferenceEquals(y, x))));
                        IsTrue(seizingPlayers.All(x => copiedGame.Players.Any(y => object.ReferenceEquals(y, x))));
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            // TODO: more checks
        }
    }
}