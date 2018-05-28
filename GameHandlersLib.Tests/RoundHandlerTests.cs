namespace GameHandlersLib.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common.Extensions;
    using Common.Interfaces;
    using GameAi.Data;
    using GameHandlers;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.Players;
    using MapHandlers;
    using Moq;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;
    using Region = GameObjectsLib.GameMap.Region;

    [TestFixture]
    public class RoundHandlerTests
    {
        private RoundHandler roundHandler;

        private Game game;

        private Region czechia;
        private Region austria;
        private Region poland;

        private AiPlayer pc1;
        private AiPlayer pc2;

        [SetUp]
        public void Initialize()
        {
            // set current directory to be the test bin directory (resharper using its own)
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var mapXmlString = Maps.TestMap;

            Map map;
            using (var stream = mapXmlString.ToStream())
            {
                map = new Map(1, "TEST", 1, stream);
            }

            pc1 = new AiPlayer(Difficulty.Hard, "PC1", KnownColor.AliceBlue, GameBotType.MonteCarloTreeSearchBot);
            pc2 = new AiPlayer(Difficulty.Hard, "PC2", KnownColor.Red, GameBotType.MonteCarloTreeSearchBot);
            var players = new List<Player>()
            {
                pc1,
                pc2
            };

            game = new GameFactory().CreateGame(GameType.Simulator, map, players, true,
                new GameObjectsRestrictions());

            austria = map.Regions.First(x => x.Name == "Austria");
            poland = map.Regions.First(x => x.Name == "Poland");
            czechia = map.Regions.First(x => x.Name == "Czechia");

            roundHandler = new RoundHandler(game);

            // inject moq random into list extensions
            var randomFieldInfo = typeof(ListExtensions).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .First(x => x.FieldType == typeof(Random));

            var randomMoq = new Mock<Random>();
            randomMoq.Setup(x => x.Next(It.IsAny<int>())).Returns(1);

            randomFieldInfo.SetValue(null, randomMoq.Object);
        }

        [Test]
        public void PlayGameRoundTest()
        {
            // assign moq to random
            var randomMoq = new Mock<Random>();
            randomMoq.Setup(x => x.NextDouble()).Returns(0);

            var randomInstance = randomMoq.Object;
            ((IRandomInjectable)roundHandler).Inject(randomInstance);
            
            #region Initialize seizes
            var initialSeizes = new List<Seize>();
            initialSeizes.Add(new Seize(pc1, austria));
            initialSeizes.Add(new Seize(pc2, poland));
            var linearizedGameBeginningRound = new LinearizedGameBeginningRound(initialSeizes);
            game.AllRounds.Add(linearizedGameBeginningRound);

            austria.ChangeOwner(pc1);
            poland.ChangeOwner(pc2);
            #endregion

            var gameRound = new GameRound()
            {
                Turns = new List<Turn>()
                {
                    new GameTurn(new Deploying(new List<Deployment>()
                    {
                        new Deployment(austria, 7, pc1)
                    }), new Attacking(new List<Attack>()
                        {
                            new Attack(pc1, austria, 6, czechia)
                        }), pc1),
                    new GameTurn(new Deploying(new List<Deployment>()
                    {
                        new Deployment(poland, 7, pc2)
                    }), new Attacking(new List<Attack>()
                    {
                        new Attack(pc2, poland, 3, czechia)
                    }), pc2)
                }
            };
            roundHandler.PlayRound(gameRound);

            var lastLinearizedRound = (LinearizedGameRound)game.AllRounds.Last();
            var attacks = lastLinearizedRound.Attacking.Attacks;

            var pc1Attack = attacks.First(x => x.AttackingPlayer == pc1);
            var pc2Attack = attacks.First(x => x.AttackingPlayer == pc2);

            AreEqual(1, czechia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(1, austria.Army);
            AreEqual(4, poland.Army);

            AreEqual(czechia, pc2Attack.Defender);
            AreEqual(1, pc2Attack.PostAttackMapChange.DefendingRegionArmy);
            AreEqual(4, pc2Attack.PostAttackMapChange.AttackingRegionArmy);
            AreEqual(pc1, pc2Attack.PostAttackMapChange.DefendingRegionOwner);

            AreEqual(austria, pc1Attack.Attacker);
            AreEqual(czechia, pc1Attack.Defender);
            AreEqual(4, pc1Attack.PostAttackMapChange.DefendingRegionArmy);
            AreEqual(1, pc1Attack.PostAttackMapChange.AttackingRegionArmy);
            AreEqual(pc1, pc1Attack.PostAttackMapChange.DefendingRegionOwner);
        }

        [Test]
        public void PlayGameRoundNoKillTest()
        {
            // assign moq to random via reflection
            // assign moq to random
            var randomMoq = new Mock<Random>();
            randomMoq.Setup(x => x.NextDouble()).Returns(1);

            var randomInstance = randomMoq.Object;
            ((IRandomInjectable)roundHandler).Inject(randomInstance);

            #region Initialize seizes
            var initialSeizes = new List<Seize>();
            initialSeizes.Add(new Seize(pc1, austria));
            initialSeizes.Add(new Seize(pc2, poland));
            var linearizedGameBeginningRound = new LinearizedGameBeginningRound(initialSeizes);
            game.AllRounds.Add(linearizedGameBeginningRound);

            austria.ChangeOwner(pc1);
            poland.ChangeOwner(pc2);
            #endregion

            var gameRound = new GameRound()
            {
                Turns = new List<Turn>()
                {
                    new GameTurn(new Deploying(new List<Deployment>()
                    {
                        new Deployment(austria, 7, pc1)
                    }), new Attacking(new List<Attack>()
                    {
                        new Attack(pc1, austria, 6, czechia)
                    }), pc1),
                    new GameTurn(new Deploying(new List<Deployment>()
                    {
                        new Deployment(poland, 7, pc2)
                    }), new Attacking(new List<Attack>()
                    {
                        new Attack(pc2, poland, 3, czechia)
                    }), pc2)
                }
            };

            roundHandler.PlayRound(gameRound);


            var linearizedGameRound = (LinearizedGameRound)game.AllRounds.Last();

            var attacks = linearizedGameRound.Attacking.Attacks;

            var pc1Attack = attacks.First(x => x.AttackingPlayer == pc1);
            var pc2Attack = attacks.First(x => x.AttackingPlayer == pc2);

            AreEqual(2, czechia.Army);
            AreEqual(null, czechia.Owner);
            AreEqual(7, austria.Army);
            AreEqual(7, poland.Army);

            AreEqual(austria, pc1Attack.Attacker);
            AreEqual(czechia, pc1Attack.Defender);
            AreEqual(7, pc1Attack.PostAttackMapChange.AttackingRegionArmy);
            AreEqual(2, pc1Attack.PostAttackMapChange.DefendingRegionArmy);
            AreEqual(null, pc1Attack.PostAttackMapChange.DefendingRegionOwner);

            AreEqual(czechia, pc2Attack.Defender);
            AreEqual(2, pc2Attack.PostAttackMapChange.DefendingRegionArmy);
            AreEqual(7, pc2Attack.PostAttackMapChange.AttackingRegionArmy);
            AreEqual(null, pc2Attack.PostAttackMapChange.DefendingRegionOwner);           
        }

        [Test]
        public void PlayGameBeginningRound()
        {
            #region Initialize game state

            // initial seizes
            var seizeRound = new GameBeginningRound();
            seizeRound.Turns.Add(new GameBeginningTurn(new List<Seize>()
            {
                new Seize(pc1, austria)
            }, pc1));
            seizeRound.Turns.Add(new GameBeginningTurn(new List<Seize>()
            {
                new Seize(pc2, poland)
            }, pc2));
            #endregion
            
            roundHandler.PlayRound(seizeRound);
        }
    }
}