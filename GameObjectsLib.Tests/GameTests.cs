namespace GameObjectsLib.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Common.Extensions;
    using Game;
    using GameAi.Data;
    using GameMap;
    using GameUser;
    using NUnit.Framework;
    using Players;

    [TestFixture]
    public class GameTests
    {
        private Map map;
        private IList<Player> players;

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

            players = new List<Player>
            {
                new AiPlayer(Difficulty.Hard, "PC1", KnownColor.AliceBlue, GameBotType.MonteCarloTreeSearchBot),
                new HumanPlayer(new LocalUser("TestUser"), KnownColor.Red)
            };
        }

        [Test]
        public void HotseatGameTest()
        {
            Game game = new GameFactory().CreateGame(1, GameType.MultiplayerHotseat, map, players, fogOfWar: true, restrictions: null);
        }
    }
}