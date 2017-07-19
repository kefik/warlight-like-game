using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using ConquestObjectsLib.Properties;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents low level visual representation of the map template,
    /// represents the link between image and regions.
    /// </summary>
    class MapImageTemplateProcessor
    {
        readonly Map map;
        /// <summary>
        /// Image containing highlighted version of the game map.
        /// </summary>
        public Bitmap RegionHighlightedImage { get; }

        readonly Dictionary<Color, Region> regionsMapped;

        /// <summary>
        /// Constructs MapImage instance.
        /// </summary>
        /// <param name="map">Map with regions.</param>
        /// <param name="regionHighlightedImage">Image containing the map in proper format.</param>
        /// <param name="regionsWithColors">Tuples mapping color of region to corresponding region. Regions must correspond to those in the map.</param>
        public MapImageTemplateProcessor(Map map, Image regionHighlightedImage,
            Dictionary<Color, Region> regionsWithColors)
        {
            this.map = map;
            this.RegionHighlightedImage = new Bitmap(regionHighlightedImage);

            if (map.Regions.Count != regionsWithColors.Count) throw new ArgumentException();

            regionsMapped = regionsWithColors;
        }
        /// <summary>
        /// Finds region corresponding to the given color and returns it.
        /// </summary>
        /// <param name="color">Color on the map specified in constructor</param>
        /// <returns>Region corresponding to the color.</returns>
        public Region GetRegion(Color color)
        {
            return regionsMapped.TryGetValue(color, out Region region) ? region : null;
        }

        /// <summary>
        /// Based on the image, returns region corresponding to those coordinates.
        /// </summary>
        /// <param name="x">Coordinate x on image specified in constructor.</param>
        /// <param name="y">Coordinate y on image specified in constructor.</param>
        /// <returns>Region corresponding to the coordinates</returns>
        public Region GetRegion(int x, int y)
        {
            return GetRegion(RegionHighlightedImage.GetPixel(x, y));
        }
        
        
    }
    
    /// <summary>
    /// Represents visual representation of the game map and functionality linked with it.
    /// </summary>
    public class MapImageProcessor
    {
        Bitmap MapImage { get; }
        readonly MapImageTemplateProcessor templateProcessor;

        private MapImageProcessor(MapImageTemplateProcessor mapImageTemplateProcessor, Bitmap gameMapMapImage)
        {
            MapImage = gameMapMapImage;
            templateProcessor = mapImageTemplateProcessor;
        }
        /// <summary>
        /// Recolors every pixel of the original color in the map to the new color.
        /// </summary>
        /// <param name="sourceColor">Source color.</param>
        /// <param name="targetColor">Color to recolor the region to.</param>
        public void Recolor(Color sourceColor, Color targetColor)
        {
            var templateImage = templateProcessor.RegionHighlightedImage;
            var regionHighlightedImage = MapImage;

            // TODO: iterate through every pixel on "templateImage" and replace same positioned pixels with old color with new color on the "regionHighlightedImage"


        }
        /// <summary>
        /// Initializes an instance of MapImageProcessor.
        /// </summary>
        /// <param name="map">Map of the future game.</param>
        /// <param name="regionHighlightedImagePath">Path of image whose role is to map region to certain color to recognize what image has been clicked on by the user.</param>
        /// <param name="regionColorMappingPath">Path of file mapping color to certain existing map region.</param>
        /// <param name="mapImagePath">Map of image that will be used as map and displayed to the user.</param>
        /// <returns>Initialized instance.</returns>
        public static MapImageProcessor Create(Map map, string regionHighlightedImagePath, string regionColorMappingPath, string mapImagePath)
        {
            // TODO: check
            var regionHighlightedImage = new Bitmap(regionHighlightedImagePath);

            // read the file
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

            Dictionary<Color, Region> dictionary = new Dictionary<Color, Region>();
            using (XmlReader reader = XmlReader.Create(regionColorMappingPath, settings))
            {
                Color color = default(Color); 
                Region region = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Entry":
                                    break;
                                case nameof(Color):
                                    // TODO : maybe better way is possible
                                    byte red = Convert.ToByte(reader.GetAttribute("Red"));
                                    byte green = Convert.ToByte(reader.GetAttribute("Green"));
                                    byte blue = Convert.ToByte(reader.GetAttribute("Blue"));
                                    byte alpha = Convert.ToByte(reader.GetAttribute("Alpha"));
                                    color = Color.FromArgb(alpha, red, green, blue);
                                    break;
                                case nameof(Region):
                                    region = (from item in map.Regions
                                             where item.Name == reader.GetAttribute("Name")
                                             select item).First();
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Entry")
                            {
                                dictionary.Add(color, region);
                            }
                            break;
                    }
                }
            }

            MapImageTemplateProcessor mapImageTemplateProcessor = new MapImageTemplateProcessor(map, regionHighlightedImage, dictionary);

            var image = new Bitmap(mapImagePath);

            return new MapImageProcessor(mapImageTemplateProcessor, image);
        }

    }
}
