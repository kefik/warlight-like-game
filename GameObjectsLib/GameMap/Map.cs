using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using GameObjectsLib.GameMap;
using ProtoBuf;

namespace GameObjectsLib.GameMap
{
    /// <summary>
    /// Instance of this class represents map of the game.
    /// </summary>
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class Map
    {
        public string Name { get; }

        /// <summary>
        /// Returns maximum number of players for the given map.
        /// </summary>
        public int PlayersLimit { get; }

        /// <summary>
        /// Represents regions of the map that player can conquer.
        /// </summary>
        public ICollection<Region> Regions { get; } = new HashSet<Region>();
        /// <summary>
        /// Represents region groups this map has.
        /// </summary>
        public ICollection<SuperRegion> SuperRegions { get; } = new HashSet<SuperRegion>();
        
        private Map(string name, int playersLimit)
        {
            this.Name = name;
            this.PlayersLimit = playersLimit;
        }
        Map() { }

        /// <summary>
        /// Creates instance of map, initializes it,
        /// loads all objects related to its given model,
        /// getting the map ready for the start of the game.
        /// </summary>
        public static Map Create(string name, int playersLimit, string templatePath)
        {
            Map map = new Map(name, playersLimit);
            // set validation against xsd settings
            XmlReaderSettings settings = new XmlReaderSettings()
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
                                case "SuperRegion":
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
                                case "Region":
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
                                case "SuperRegion":
                                    // reset
                                    isSuperRegionElement = false;
                                    break;
                                case "Region":
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
                                case "SuperRegion":
                                    isSuperRegion = true;
                                    break;
                                case "Region":
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
                                case "SuperRegion":
                                    isSuperRegion = false;
                                    break;
                                case "Region":
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
