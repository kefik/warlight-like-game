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
    using GameObjectsLib.GameRestrictions;
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
        private Region poland;

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

            game = new GameFactory().CreateGame(1, GameType.Simulator, map, players, true, new GameObjectsRestrictions());
            
            gameRecordHandler = new GameRecordHandler(mapProcessorMoq.Object, game, null);
            
            #region Initialize game state

            // initial seizes
            var initialSeizes = new List<Seize>();
            austria = map.Regions.First(x => x.Name == "Austria");
            poland = map.Regions.First(x => x.Name == "Poland");
            initialSeizes.Add(new Seize(pc1, austria));
            initialSeizes.Add(new Seize(pc2, poland));
            var linearizedGameBeginningRound = new LinearizedGameBeginningRound(initialSeizes);
            game.AllRounds.Add(linearizedGameBeginningRound);

            // initial deploys and attacks
            var deploy = new List<Deployment>
            {
                new Deployment(austria, 7, pc1),
                new Deployment(poland, 7, pc2)
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
                new Attack(pc2, poland, 3, czechia)
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
            poland.ChangeOwner(pc2);

            czechia.Army = 4;
            czechia.ChangeOwner(pc1);

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
            gameRecordHandler.Load(game, null);

            AreNotEqual(previousGame, game);
        }

        [Test]
        public void MoveToPreviousActionTest()
        {
            bool wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsFalse(wasMoved);
            
            gameRecordHandler.Load(game, null);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            wasMoved = gameRecordHandler.MoveToPreviousAction();

            IsTrue(wasMoved);
            var currentAction = (Attack)gameRecordHandler.GetCurrentAction();

            // refreshed countries
            czechia = regions.First(x => x.Name == "Czechia");
            poland = regions.First(x => x.Name == "Poland");
            austria = regions.First(x => x.Name == "Austria");
            AreEqual(czechia, currentAction.Defender);
            AreEqual(poland, currentAction.Attacker);
            AreEqual(7, poland.Army);
            AreEqual(5, czechia.Army);
            AreEqual(pc1, czechia.Owner);
        }

        [Test]
        public void MoveToNextAndPreviousActionTest()
        {
            gameRecordHandler.Load(game, null);
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
            gameRecordHandler.Load(game, null);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;

            // current position == after last action

            // revert attack Poland -> czechia
            bool wasMoved = gameRecordHandler.MoveToPreviousAction();
            AreEqual(2, pc1.ControlledRegions.Count);
            AreEqual(1, pc2.ControlledRegions.Count);
            // revert attack austria -> czechia
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            // revert deploy Poland
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            // revert deploy austria
            wasMoved = gameRecordHandler.MoveToPreviousAction();

            austria = regions.First(x => x.Name == "Austria");
            czechia = regions.First(x => x.Name == "Czechia");
            poland = regions.First(x => x.Name == "Poland");
            pc1 = (AiPlayer)copiedGame.Players.First(x => x.Name == "PC1");
            pc2 = (AiPlayer)copiedGame.Players.First(x => x.Name == "PC2");

            AreEqual(2, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(2, czechia.Army);
            IsNull(czechia.Owner);
            AreEqual(2, poland.Army);
            AreEqual(pc2, poland.Owner);
            AreEqual(1, pc1.ControlledRegions.Count);
            AreEqual(1, pc2.ControlledRegions.Count);

            // revert seize Poland
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(2, poland.Army);
            IsNull(poland.Owner);
            AreEqual(1, pc1.ControlledRegions.Count);

            // revert seize austria
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(2, austria.Army);
            IsNull(austria.Owner);
            AreEqual(0, pc1.ControlledRegions.Count);
            AreEqual(0, pc2.ControlledRegions.Count);

            // revert invalid one more back
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsFalse(wasMoved);

            // seize austria and Poland
            wasMoved = gameRecordHandler.MoveToNextAction();
            IsTrue(wasMoved);
            wasMoved = gameRecordHandler.MoveToNextAction();
            IsTrue(wasMoved);

            AreEqual(2, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(2, czechia.Army);
            IsNull(czechia.Owner);
            AreEqual(2, poland.Army);
            AreEqual(pc2, poland.Owner);
        }

        [Test]
        public void MoveRoundsTest()
        {
            gameRecordHandler.Load(game, null);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            czechia = regions.First(x => x.Name == "Czechia");
            poland = regions.First(x => x.Name == "Poland");
            austria = regions.First(x => x.Name == "Austria");

            // reset deploy and attack round
            bool wasMoved = gameRecordHandler.MoveToPreviousRound();
            IsTrue(wasMoved);
            AreEqual(2, austria.Army);
            AreEqual(pc1, austria.Owner);
            AreEqual(2, czechia.Army);
            IsNull(czechia.Owner);
            AreEqual(2, poland.Army);
            AreEqual(pc2, poland.Owner);

            // reset seizes
            wasMoved = gameRecordHandler.MoveToPreviousRound();
            IsTrue(wasMoved);
            AreEqual(2, poland.Army);
            IsNull(poland.Owner);
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
            AreEqual(5, poland.Army);
            AreEqual(pc2, poland.Owner);

            // try to play one step forward
            wasMoved = gameRecordHandler.MoveToNextRound();
            IsFalse(wasMoved);
        }

        [Test]
        public void MoveToBeginningAndEndTest()
        {
            gameRecordHandler.Load(game, null);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            czechia = regions.First(x => x.Name == "Czechia");
            poland = regions.First(x => x.Name == "Poland");
            austria = regions.First(x => x.Name == "Austria");

            // to the beginning
            bool wasMoved = gameRecordHandler.MoveToBeginning();
            IsTrue(wasMoved);
            AreEqual(2, poland.Army);
            IsNull(poland.Owner);
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
            AreEqual(5, poland.Army);
            AreEqual(pc2, poland.Owner);

            // try one more
            wasMoved = gameRecordHandler.MoveToEnd();
            IsFalse(wasMoved);
        }

        [Test]
        public void MoveActionsWithPerspectiveTest()
        {
            gameRecordHandler.Load(game, pc1);
            var copiedGame = gameRecordHandler.Game;
            var regions = copiedGame.Map.Regions;
            czechia = regions.First(x => x.Name == "Czechia");
            poland = regions.First(x => x.Name == "Poland");
            austria = regions.First(x => x.Name == "Austria");
            pc1 = (AiPlayer)copiedGame.Players.First(x => x.Name == "PC1");
            pc2 = (AiPlayer)copiedGame.Players.First(x => x.Name == "PC2");

            // revert attack Poland -> czechia
            bool wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(2, pc1.ControlledRegions.Count);
            AreEqual(1, pc2.ControlledRegions.Count);

            // revert attack austria -> czechia
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(1, pc1.ControlledRegions.Count);
            AreEqual(1, pc2.ControlledRegions.Count);

            // revert deploy Poland
            // pc1 cannot see Poland => should revert
            // to deploy austria
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(1, pc1.ControlledRegions.Count);
            AreEqual(1, pc2.ControlledRegions.Count);
            var currentAction = (Deployment) gameRecordHandler.GetCurrentAction();
            AreEqual(pc1, currentAction.DeployingPlayer);

            // revert to pc1 seize
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsTrue(wasMoved);
            AreEqual(0, pc1.ControlledRegions.Count);
            var pc1Seize = (Seize) gameRecordHandler.GetCurrentAction();
            AreEqual(pc1, pc1Seize.SeizingPlayer);
            AreEqual(austria, pc1Seize.Region);

            // revert one more
            wasMoved = gameRecordHandler.MoveToPreviousAction();
            IsFalse(wasMoved);

            // next -> after seizing austria
            wasMoved = gameRecordHandler.MoveToNextAction();
            IsTrue(wasMoved);
            AreEqual(1, pc1.ControlledRegions.Count);
            AreEqual(0, pc2.ControlledRegions.Count);

            // next -> before deploying to poland
            wasMoved = gameRecordHandler.MoveToNextAction();
            IsTrue(wasMoved);
            currentAction = (Deployment)gameRecordHandler.GetCurrentAction();
            AreEqual(pc2, currentAction.DeployingPlayer);

            // next -> after austria attack, before poland attack
            wasMoved = gameRecordHandler.MoveToNextAction();
            IsTrue(wasMoved);
            var currentAttack = (Attack)gameRecordHandler.GetCurrentAction();
            AreEqual(poland, currentAttack.Attacker);

            // next after poland attack
            wasMoved = gameRecordHandler.MoveToNextAction();
            IsTrue(wasMoved);
            AreEqual(null, gameRecordHandler.GetCurrentAction());
        }
    }
}