namespace GameObjectsLib.Tests
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Common.Extensions;
    using GameMap;
    using GameUser;
    using NUnit.Framework;
    using Players;
    using Region = GameMap.Region;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class RegionTests
    {
        private Map map;
        private Region czechia;
        private Region austria;
        private Region poland;
        private Player player;

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

            player = new HumanPlayer(new LocalUser("TestUser"), KnownColor.AliceBlue);
        }

        [Test]
        public void ChangeOwnerTest()
        {
            IsNull(czechia.Owner);
            AreEqual(0, player.ControlledRegions.Count);

            // change owner to player
            czechia.ChangeOwner(player);
            AreEqual(player, czechia.Owner);
            AreEqual(1, player.ControlledRegions.Count);

            // change owner to player again
            czechia.ChangeOwner(player);
            AreEqual(player, czechia.Owner);
            AreEqual(1, player.ControlledRegions.Count);

            // change owner to player again
            czechia.ChangeOwner(null);
            AreEqual(null, czechia.Owner);
            AreEqual(0, player.ControlledRegions.Count);
        }

        [Test]
        public void IsNeighbourOfTest()
        {
            IsTrue(austria.IsNeighbourOf(czechia));
            IsFalse(austria.IsNeighbourOf(poland));
            IsFalse(poland.IsNeighbourOf(austria));
            IsFalse(poland.IsNeighbourOf(poland));
        }
    }
}