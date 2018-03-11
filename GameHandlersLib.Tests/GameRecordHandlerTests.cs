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
            IsNull(currentAction);
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
            czechia = regions.First(x => x.Name == "Czechia");
            germany = regions.First(x => x.Name == "Germany");
            austria = regions.First(x => x.Name == "Austria");
            AreEqual(czechia, currentAction.Defender);
            AreEqual(germany, currentAction.Attacker);
            AreEqual(7, germany.Army);
            AreEqual(5, czechia.Army);
            AreEqual(pc1, czechia.Owner);
        }

        [Test]
        public void MoveToNextAndPreviousActionTest()
        {
            gameRecordHandler.Load(game);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            bool wasMoved = gameRecordHandler.MoveToNextAction();

            IsFalse(wasMoved);

            wasMoved = gameRecordHandler.MoveToPreviousAction();
            wasMoved = gameRecordHandler.MoveToNextAction();

            IsTrue(wasMoved);
            IsNull(gameRecordHandler.GetCurrentAction());
        }

        [Test]
        public void MoveToNextAndPreviousActionTest2()
        {
            gameRecordHandler.Load(game);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;

            // current position == after last action

            // revert attack germany -> czechia
            bool wasMoved = gameRecordHandler.MoveToPreviousAction();
            // revert attack austria -> czechia
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            // revert deploy germany
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            // revert deploy austria
            wasMoved = gameRecordHandler.MoveToPreviousAction();

            austria = regions.First(x => x.Name == "Austria");
            czechia = regions.First(x => x.Name == "Czechia");
            germany = regions.First(x => x.Name == "Germany");

            AreEqual(2, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(2, czechia.Army);
            IsNull(czechia.Owner);
            AreEqual(2, germany.Army);
            AreEqual(pc2, germany.Owner);

            // revert seize germany
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(2, germany.Army);
            IsNull(germany.Owner);

            // revert seize austria
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(2, austria.Army);
            IsNull(austria.Owner);

            // revert invalid one more back
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsFalse(wasMoved);

            // seize austria and germany
            wasMoved = gameRecordHandler.MoveToNextAction();
            wasMoved = gameRecordHandler.MoveToNextAction();

            AreEqual(2, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(2, czechia.Army);
            IsNull(czechia.Owner);
            AreEqual(2, germany.Army);
            AreEqual(pc2, germany.Owner);
        }

        [Test]
        public void MoveRoundsTest()
        {
            gameRecordHandler.Load(game);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            czechia = regions.First(x => x.Name == "Czechia");
            germany = regions.First(x => x.Name == "Germany");
            austria = regions.First(x => x.Name == "Austria");

            // reset deploy and attack round
            bool wasMoved = gameRecordHandler.MoveToPreviousRound();
            IsTrue(wasMoved);
            AreEqual(2, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(2, czechia.Army);
            IsNull(czechia.Owner);
            AreEqual(2, germany.Army);
            AreEqual(pc2, germany.Owner);

            // reset seizes
            wasMoved = gameRecordHandler.MoveToPreviousRound();
            IsTrue(wasMoved);
            AreEqual(2, germany.Army);
            IsNull(germany.Owner);
            IsTrue(wasMoved);
            AreEqual(2, austria.Army);
            IsNull(austria.Owner);

            // try to play one step back
            wasMoved = gameRecordHandler.MoveToPreviousRound();
            IsFalse(wasMoved);

            // play seizes, deploy and attacks
            wasMoved = gameRecordHandler.MoveToNextRound();
            IsTrue(wasMoved);
            wasMoved = gameRecordHandler.MoveToNextRound();
            IsTrue(wasMoved);
            AreEqual(1, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(4, czechia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(5, germany.Army);
            AreEqual(pc2, germany.Owner);

            // try to play one step forward
            wasMoved = gameRecordHandler.MoveToNextRound();
            IsFalse(wasMoved);
        }

        [Test]
        public void MoveToBeginningAndEndTest()
        {
            gameRecordHandler.Load(game);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            czechia = regions.First(x => x.Name == "Czechia");
            germany = regions.First(x => x.Name == "Germany");
            austria = regions.First(x => x.Name == "Austria");

            // to the beginning
            bool wasMoved = gameRecordHandler.MoveToBeginning();
            IsTrue(wasMoved);
            AreEqual(2, germany.Army);
            IsNull(germany.Owner);
            IsTrue(wasMoved);
            AreEqual(2, austria.Army);
            IsNull(austria.Owner);

            // try more to the beginning
            wasMoved = gameRecordHandler.MoveToBeginning();
            IsFalse(wasMoved);

            // to the end
            wasMoved = gameRecordHandler.MoveToEnd();
            IsTrue(wasMoved);
            AreEqual(1, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(4, czechia.Army);
            AreEqual(pc1, czechia.Owner);
            AreEqual(5, germany.Army);
            AreEqual(pc2, germany.Owner);

            // try one more
            wasMoved = gameRecordHandler.MoveToEnd();
            IsFalse(wasMoved);
        }
    }
}