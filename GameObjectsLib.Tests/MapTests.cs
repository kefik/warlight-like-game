using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectsLib.Tests
{
    using System.Configuration;
    using System.IO;
    using Common.Extensions;
    using GameMap;

    using static Assert;

    [TestFixture]
    public class MapTests
    {
        [SetUp]
        public void Initialize()
        {
            // set current directory to be the test bin directory (resharper using its own)
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void InitializeTestMapTest()
        {
            var mapXmlString = TestMaps.TestMap;

            Map map;
            using (var stream = mapXmlString.ToStream())
            {
                map = new Map(1, "TEST", 1, stream);
            }

            // assert regions
            var regions = map.Regions;
            
            var slovakiaRegion = regions.First(x => x.Name == "Slovakia");

            AreEqual(3, slovakiaRegion.NeighbourRegions.Count);
            AreSame(regions.First(x => x.Name == "Czechia"), slovakiaRegion.NeighbourRegions.First(x => x.Name == "Czechia"));

            // assert super regions
            var superRegions = map.SuperRegions;

            AreEqual(1, superRegions.Count);

            var superRegion = superRegions.First();
            AreEqual(5, superRegion.Regions.Count);
        }
    }
}
