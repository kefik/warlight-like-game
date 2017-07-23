using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ProtoBuf;

namespace GameObjectsLib.GameMap
{
    /// <summary>
    /// Instance of this class represents map of the game.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public sealed class Map
    {
        [ProtoMember(1)]
        public int Id { get; }
        [ProtoMember(2)]
        public string Name { get; }

        /// <summary>
        /// Returns maximum number of players for the given map.
        /// </summary>
        [ProtoMember(3)]
        public int PlayersLimit { get; }

        /// <summary>
        /// Represents regions of the map that player can conquer.
        /// </summary>
        [ProtoMember(4, AsReference = true)]
        public IList<Region> Regions { get; } = new List<Region>();
        /// <summary>
        /// Represents region groups this map has.
        /// </summary>
        [ProtoMember(5)]
        public IList<SuperRegion> SuperRegions { get; } = new List<SuperRegion>();
        
        private Map(int id, string name, int playersLimit)
        {
            Id = id;
            Name = name;
            PlayersLimit = playersLimit;
        }
        Map() { }

        /// <summary>
        /// Creates instance of map, initializes it,
        /// loads all objects related to its given model,
        /// getting the map ready for the start of the game.
        /// </summary>
        public static Map Create(int id, string name, int playersLimit, string templatePath)
        {
            Map map = new Map(id, name, playersLimit);
            // set validation against xsd settings
            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, args) =>
            {
                switch (args.Severity)
                {
                    case XmlSeverityType.Error:
                        throw new XmlSchemaValidationException();
                    case XmlSeverityType.Warning:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            // verify xml against loaded xsd, read everything except for neighbours
            using (XmlReader reader = XmlReader.Create(templatePath, settings))
            {
                #region SuperRegion stats
                int superRegionCounter = 1;
                bool isSuperRegionElement = false;
                #endregion

                #region Region stats
                int regionCounter = 1;
                bool isRegionElement = false;
                
                #endregion

                bool isNeighbours = false;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case nameof(SuperRegion):
                                    isSuperRegionElement = true;
                                    if (!isRegionElement && !isNeighbours) // is SuperRegion attribute
                                    {
                                        string superRegionName = reader.GetAttribute("Name");
                                        int superRegionBonus = int.Parse(reader.GetAttribute("Bonus"));
                                        var superRegion = new SuperRegion(superRegionCounter++, superRegionName,
                                            superRegionBonus);
                                        map.SuperRegions.Add(superRegion);
                                    }
                                    break;
                                case nameof(Region):
                                    isRegionElement = true;
                                    if (isSuperRegionElement && !isNeighbours) // is Region element
                                    {
                                        string regionName = reader.GetAttribute("Name");
                                        // TODO: may drop
                                        int army = int.Parse(reader.GetAttribute("Army"));
                                        var region =
                                            new Region(regionCounter++, regionName, map.SuperRegions.Last())
                                            {
                                                Army = army
                                            };
                                        map.Regions.Add(region);
                                        map.SuperRegions.Last().Regions.Add(region);
                                    }
                                    break;
                                case "Neighbours":
                                    isNeighbours = true;
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case nameof(SuperRegion):
                                    // reset
                                    isSuperRegionElement = false;
                                    break;
                                case nameof(Region):
                                    // reset
                                    isRegionElement = false;
                                    break;
                                case "Neighbours":
                                    isNeighbours = false;
                                    break;
                            }
                            break;
                    }
                }
            }
            // dont verify, just read neighbours and load them appropriately
            using (XmlReader reader = XmlReader.Create(templatePath))
            {
                bool isSuperRegion = false;
                
                int isRegion = 0;
                Region givenRegion = null;

                bool isNeighbours = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case nameof(SuperRegion):
                                    isSuperRegion = true;
                                    break;
                                case nameof(Region):
                                    isRegion++;
                                    if (isSuperRegion && isRegion == 1 && !isNeighbours)
                                    {
                                        givenRegion = (from region in map.Regions
                                                       where region.Name == reader.GetAttribute("Name")
                                                       select region).First();
                                    }
                                    else if (isSuperRegion && isRegion == 2 && isNeighbours)
                                    {
                                        // find region with this name and add it to given regions neighbours
                                        // TODO: slow
                                        var regionsNeighbour = (from region in map.Regions
                                                                where region.Name == reader.GetAttribute("Name")
                                                                select region).First();
                                        givenRegion.NeighbourRegions.Add(regionsNeighbour);
                                    }
                                    break;
                                case "Neighbours":
                                    isNeighbours = true;
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case nameof(SuperRegion):
                                    isSuperRegion = false;
                                    break;
                                case nameof(Region):
                                    isRegion--;
                                    if (isSuperRegion && isRegion == 0 && !isNeighbours) givenRegion = null;
                                    break;
                                case "Neighbours":
                                    isNeighbours = false;
                                    break;
                            }
                            break;
                    }
                }
            }
            return map;
        }
        
        public override string ToString()
        {
            string name = string.Format($"{nameof(Name)}: {Name}");
            string playersLimit = string.Format($"{nameof(PlayersLimit)}: {PlayersLimit}");
            string superRegions;
            {
                var sb = new StringBuilder();
                foreach (SuperRegion superRegion in SuperRegions)
                {
                    sb.Append(superRegion.Name + ", ");
                }
                superRegions = string.Format($"{nameof(SuperRegions)}: {sb}");
            }

            return name + ", " + playersLimit + ", " + superRegions;

        }
    }
}
