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
    public class ActionEnumeratorTests
    {
        private ActionEnumerator actionEnumerator;

        private AiPlayer pc1;
        private AiPlayer pc2;

        private Region czechia;
        private Region austria;
        private Region germany;

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

            var game = new GameFactory().CreateGame(1, GameType.Simulator, map, players, true, null);

            
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

            actionEnumerator = new ActionEnumerator(game.AllRounds);
        }

        [Test]
        public void CurrentTest()
        {
            IsNull(actionEnumerator.GetCurrentAction());
        }

        [Test]
        public void MovePreviousAndNextTest()
        {
            // initially after second attack

            // on second attack
            bool wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);
            AreEqual(1, actionEnumerator.RoundIndex);
            AreEqual(3, actionEnumerator.ActionIndex);

            // on first attack
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);
            AreEqual(1, actionEnumerator.RoundIndex);
            AreEqual(2, actionEnumerator.ActionIndex);

            // on second deployment
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);
            AreEqual(1, actionEnumerator.RoundIndex);
            AreEqual(1, actionEnumerator.ActionIndex);

            // on first deployment
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);
            var current = actionEnumerator.GetCurrentAction();
            var currentDeploy = (Deployment) current;
            AreEqual(pc1, currentDeploy.DeployingPlayer);
            AreEqual(austria, currentDeploy.Region);
            AreEqual(1, actionEnumerator.RoundIndex);
            AreEqual(0, actionEnumerator.ActionIndex);

            // on second deployment
            wasMoved = actionEnumerator.MoveNext();
            IsTrue(wasMoved);
            current = actionEnumerator.GetCurrentAction();
            currentDeploy = (Deployment) current;
            AreEqual(pc2, currentDeploy.DeployingPlayer);
            AreEqual(germany, currentDeploy.Region);
            AreEqual(1, actionEnumerator.RoundIndex);
            AreEqual(1, actionEnumerator.ActionIndex);

            // on first attack
            wasMoved = actionEnumerator.MoveNext();
            IsTrue(wasMoved);
            AreEqual(1, actionEnumerator.RoundIndex);
            AreEqual(2, actionEnumerator.ActionIndex);

            // on second attack
            wasMoved = actionEnumerator.MoveNext();
            IsTrue(wasMoved);
            current = actionEnumerator.GetCurrentAction();
            var currentAttack = (Attack) current;
            AreEqual(czechia, currentAttack.Defender);
            AreEqual(germany, currentAttack.Attacker);
            AreEqual(pc2, currentAttack.AttackingPlayer);
            AreEqual(1, actionEnumerator.RoundIndex);
            AreEqual(3, actionEnumerator.ActionIndex);

            // after second attack
            wasMoved = actionEnumerator.MoveNext();
            IsTrue(wasMoved);
            IsNull(actionEnumerator.GetCurrentAction());
            AreEqual(2, actionEnumerator.RoundIndex);
            AreEqual(0, actionEnumerator.ActionIndex);

            // trying to go further
            wasMoved = actionEnumerator.MoveNext();
            IsFalse(wasMoved);
            AreEqual(2, actionEnumerator.RoundIndex);
            AreEqual(0, actionEnumerator.ActionIndex);
        }

        [Test]
        public void MovePreviousAndNextTest2()
        {
            // initially after second attack

            // on second attack
            bool wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);

            // on first attack
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);

            // on second deployment
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);

            // on first deployment
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);

            // on second seize
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);

            // on first seize
            wasMoved = actionEnumerator.MovePrevious();
            IsTrue(wasMoved);
            AreEqual(0, actionEnumerator.RoundIndex);
            AreEqual(0, actionEnumerator.ActionIndex);

            // trying to go even more back
            wasMoved = actionEnumerator.MovePrevious();
            IsFalse(wasMoved);
            AreEqual(0, actionEnumerator.RoundIndex);
            AreEqual(0, actionEnumerator.ActionIndex);
        }
    }
}