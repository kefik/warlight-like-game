using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Instance of this class represents map of the game.
    /// </summary>
    public sealed class Map // TODO: rework to non-abstract so the map can be dynamically loaded, remove some properties, mb add visual representation in it
    {
        readonly string name;

        public string Name
        {
            get { return name; }
        }
        readonly int playersLimit;

        /// <summary>
        /// Returns maximum number of players for the given map.
        /// </summary>
        public int PlayersLimit
        {
            get { return playersLimit; }
        }
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
            this.name = name;
            this.playersLimit = playersLimit;
        }

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
                if (args.Severity == XmlSeverityType.Error)
                    throw new XmlSchemaValidationException();
            };
            // verify xml against loaded xsd, read everything except for neighbours
            using (XmlReader reader = XmlReader.Create(templatePath, settings))
            {
                #region SuperRegion stats
                int superRegionCounter = 1;
                bool isSuperRegionElement = false;

                string superRegionName = null;
                int superRegionBonus = 0;
                #endregion

                #region Region stats
                int regionCounter = 1;
                bool isRegionElement = false;

                string regionName = null;
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
                                    break;
                                case "Region":
                                    isRegionElement = true;
                                    break;
                                case "Neighbours":
                                    isNeighbours = true;
                                    break;
                            }
                            break;
                        case XmlNodeType.Attribute:
                            if (isSuperRegionElement
                                && !isRegionElement && !isNeighbours) // is SuperRegion attribute
                            {
                                switch (reader.Name)
                                {
                                    case "Name":
                                        superRegionName = reader.Value;
                                        break;
                                    case "Bonus":
                                        superRegionBonus = int.Parse(reader.Value);
                                        break;
                                }
                            }
                            else if (isSuperRegionElement
                                && isRegionElement && !isNeighbours) // is Region element
                            {
                                if (reader.Name == "Name")
                                {
                                    regionName = reader.Value;
                                }
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case "SuperRegion":
                                    var superRegion =
                                        new SuperRegion(superRegionCounter, superRegionName, superRegionBonus);
                                    map.SuperRegions.Add(superRegion);
                                    // reset
                                    isSuperRegionElement = false;
                                    superRegionCounter++;
                                    superRegionName = null;
                                    superRegionBonus = 0;
                                    break;
                                case "Region":
                                    var region =
                                        new Region(regionCounter, regionName, map.SuperRegions.Last());
                                    map.Regions.Add(region);
                                    // reset
                                    isRegionElement = false;
                                    regionCounter++;
                                    regionName = null;
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
                                                       where region.Name == reader.Value
                                                       select region).First();
                                    }
                                    else if (isSuperRegion && isRegion == 2 && isNeighbours)
                                    {
                                        // find region with this name and add it to given regions neighbours
                                        // TODO: slow
                                        var regionsNeighbour = (from region in map.Regions
                                                                where region.Name == reader.Value
                                                                select region).First();
                                        givenRegion.NeighbourRegions.Add(regionsNeighbour);
                                    }
                                    break;
                                case "Neighbours":
                                    isNeighbours = true;
                                    break;
                            }
                            break;
                        case XmlNodeType.Attribute:
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

        
    }
}
