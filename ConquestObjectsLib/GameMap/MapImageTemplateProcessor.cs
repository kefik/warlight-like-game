using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquestObjectsLib.Properties;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents visual representation of the map template and functionality linked with it.
    /// </summary>
    class MapImageTemplateProcessor
    {
        readonly Map map;
        /// <summary>
        /// Image containing highlighted version of the game map.
        /// </summary>
        public Bitmap RegionHighlightedImage { get; }
        readonly Dictionary<Color, Region> regionsMapped = new Dictionary<Color, Region>();

        /// <summary>
        /// Constructs MapImage instance.
        /// </summary>
        /// <param name="map">Map with regions.</param>
        /// <param name="colorHighlightedImage">Image containing the map in proper format.</param>
        /// <param name="regionsWithColors">Tuples mapping color of region to corresponding region. Regions must correspond to those in the map.</param>
        public MapImageTemplateProcessor(Map map, Image colorHighlightedImage,
            ICollection<Tuple<Color, Region>> regionsWithColors)
        {
            this.map = map;
            this.RegionHighlightedImage = new Bitmap(colorHighlightedImage);

            if (map.Regions.Count != regionsWithColors.Count) throw new ArgumentException();

            var regions = new HashSet<Region>(); // forces value in dictionary to be unique
            foreach (var item in regionsWithColors)
            {
                if (!regions.Add(item.Item2)) throw new ArgumentException(); // is already in HashSet
                if (!map.Regions.Contains(item.Item2)) throw new ArgumentException(); 
                regionsMapped.Add(item.Item1, item.Item2); // adds the pair to regions, if its unique
            }
        }
        /// <summary>
        /// Finds region corresponding to the given color and returns it.
        /// </summary>
        /// <param name="color">Color on the map specified in constructor</param>
        /// <returns>Region corresponding to the color.</returns>
        public Region GetRegion(Color color)
        {
            Region region;
            return regionsMapped.TryGetValue(color, out region) ? region : null;
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
    class MapImageProcessor
    {
        Bitmap MapImage { get; }
        readonly MapImageTemplateProcessor templateProcessor;

        public MapImageProcessor(MapImageTemplateProcessor mapImageTemplateProcessor, Bitmap gameMapMapImage)
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

    }
}
