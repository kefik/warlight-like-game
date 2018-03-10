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


    public class GameRecordHandlerTests
    {
        private GameRecordHandler gameRecordHandler;

        private Game game;

        private Region czechia;
        private Region austria;
        private Region germany;

        private AiPlayer pc1;
        private AiPlayer pc2;

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
            
            gameRecordHandler = new GameRecordHandler(mapProcessorMoq.Object, game);
            
            #region Initialize game state

            // initial seizes
            var initialSeizes = new List<Seize>();
            austria = map.Regions.First(x => x.Name == "Austria");
            germany = map.Regions.First(x => x.Name == "Germany");
            initialSeizes.Add(new Seize(pc1, austria));
            initialSeizes.Add(new Seize(pc2, germany));
            var linearizedGameBeginningRound = new LinearizedGameBeginningRound(initialSeizes);
            game.AllRounds.Add(linearizedGameBeginningRound);

            // initial deploys and attacks
            var deploy = new List<Deployment>
            {
                new Deployment(austria, 7, pc1),
                new Deployment(germany, 7, pc2)
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
                new Attack(pc2, germany, 3, czechia)
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
            austria.Owner = pc1;

            germany.Army = 5;
            germany.Owner = pc2;

            czechia.Army = 4;
            czechia.Owner = pc1;

            #endregion
        }

        /// <summary>
        /// Game from <see cref="gameRecordHandler"/>
        /// should be with no rounds so far.
        /// This test will verify its current state
        /// (and test deep copying by that).
        /// </summary>
        [Test]
        public void GameInstancesAreIndependentTest()
        {
            var currentAction = gameRecordHandler.GetCurrentAction();
            AreSame(null, currentAction);
            IsEmpty(gameRecordHandler.Rounds);
        }

        /// <summary>
        /// Tests loading current instance of the game.
        /// </summary>
        [Test]
        public void LoadAndCurrentActionTest()
        {
            var previousGame = gameRecordHandler.Game;
            gameRecordHandler.Load(game);

            AreNotEqual(previousGame, game);
        }

        [Test]
        public void MoveToPreviousActionTest()
        {
            bool wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsFalse(wasMoved);
            
            gameRecordHandler.Load(game);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            wasMoved = gameRecordHandler.MoveToPreviousAction();

            IsTrue(wasMoved);
            var currentAction = (Attack)gameRecordHandler.GetCurrentAction();

            // refreshed countries
            var czechia = regions.First(x => x.Name == "Czechia");
            var germany = regions.First(x => x.Name == "Germany");
            var austria = regions.First(x => x.Name == "Austria");
            AreEqual(currentAction.Defender, czechia);
            AreEqual(currentAction.Attacker, germany);
            AreEqual(germany.Army, 7);
            AreEqual(czechia.Army, 5);
            AreEqual(czechia.Owner, pc1);
        }
    }
}