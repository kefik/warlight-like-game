namespace GameHandlersLib.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Common.Extensions;
    using GameAi.Data;
    using GameHandlers;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using MapHandlers;
    using Moq;
    using NUnit.Framework;
    using static NUnit.Framework.Assert;
    using Region = GameObjectsLib.GameMap.Region;

    [TestFixture]
    public class GameRecordHandlerTests2
    {
        private GameRecordHandler gameRecordHandler;

        private Game game;

        private Region czechia;
        private Region austria;
        private Region poland;
        private Region slovakia;

        private Player pc1;
        private Player pc2;

        [SetUp]
        public void Initialize()
        {
            // setup image processor moq
            var mapProcessorMoq = new Mock<IMapImageProcessor>();
            mapProcessorMoq.Setup(x => x.RedrawMap(It.IsAny<Game>(),
                It.IsAny<Player>()));
            mapProcessorMoq.Setup(x => x.Deploy(It.IsAny<GameObjectsLib.GameMap.Region>(), It.IsAny<int>()));

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

            game = new GameFactory().CreateGame(1, GameType.Simulator, map, players, true, null);


            austria = map.Regions.First(x => x.Name == "Austria");
            poland = map.Regions.First(x => x.Name == "Poland");
            czechia = map.Regions.First(x => x.Name == "Czechia");
            slovakia = map.Regions.First(x => x.Name == "Slovakia");

            gameRecordHandler = new GameRecordHandler(mapProcessorMoq.Object, game, null);
        }

        [Test]
        public void MoveToPreviousAndNextActionTest()
        {
            #region Initialize game state

            // initial seizes
            var initialSeizes = new List<Seize>();

            initialSeizes.Add(new Seize(pc1, czechia));
            initialSeizes.Add(new Seize(pc2, slovakia));
            var linearizedGameBeginningRound = new LinearizedGameBeginningRound(initialSeizes);
            game.AllRounds.Add(linearizedGameBeginningRound);

            // initial deploys and attacks
            var deploy = new List<Deployment>
            {
                new Deployment(czechia, 7, pc1),
                new Deployment(slovakia, 3, pc2)
            };
            var attacks = new List<Attack>
            {
                new Attack(pc1, czechia, 6, slovakia)
                {
                    PostAttackMapChange = new PostAttackMapChange()
                    {
                        DefendingRegionOwner = pc1,
                        AttackingRegionArmy = 1,
                        DefendingRegionArmy = 4
                    }
                },
                new Attack(pc2, slovakia, 2, poland)
                {
                    PostAttackMapChange = null
                }
            };
            game.AllRounds.Add(new LinearizedGameRound(
                new Deploying(deploy),
                new Attacking(attacks)));
            
            czechia.Army = 1;
            czechia.ChangeOwner(pc1);

            slovakia.Army = 4;
            slovakia.ChangeOwner(pc1);

            #endregion

            gameRecordHandler.Load(game, null);

            Map map = gameRecordHandler.Game.Map;
            game = gameRecordHandler.Game;

            austria = map.Regions.First(x => x.Name == "Austria");
            poland = map.Regions.First(x => x.Name == "Poland");
            czechia = map.Regions.First(x => x.Name == "Czechia");
            slovakia = map.Regions.First(x => x.Name == "Slovakia");

            pc1 = game.Players.First(x => x.Name == "PC1");
            pc2 = game.Players.First(x => x.Name == "PC2");

            // assert
            AreEqual(1, czechia.Army);
            AreEqual(4, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc1, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToPreviousAction());
            AreEqual(7, czechia.Army);
            AreEqual(3, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc2, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToPreviousAction());
            AreEqual(7, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc2, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToPreviousAction());
            AreEqual(2, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc2, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToPreviousAction());
            AreEqual(2, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(null, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToPreviousAction());
            AreEqual(2, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(null, czechia.Owner);
            AreEqual(null, slovakia.Owner);

            IsFalse(gameRecordHandler.MoveToPreviousAction());
            AreEqual(2, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(null, czechia.Owner);
            AreEqual(null, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToNextAction());
            AreEqual(2, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(null, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToNextAction());
            AreEqual(2, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc2, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToNextAction());
            AreEqual(7, czechia.Army);
            AreEqual(2, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc2, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToNextAction());
            AreEqual(7, czechia.Army);
            AreEqual(3, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc2, slovakia.Owner);

            IsTrue(gameRecordHandler.MoveToNextAction());
            AreEqual(1, czechia.Army);
            AreEqual(4, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc1, slovakia.Owner);

            IsFalse(gameRecordHandler.MoveToNextAction());
            AreEqual(1, czechia.Army);
            AreEqual(4, slovakia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(pc1, slovakia.Owner);
        }
    }
}