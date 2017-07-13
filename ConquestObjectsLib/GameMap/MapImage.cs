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
    /// Represents visual representation of the map and functionality linked with it.
    /// </summary>
    class MapImage
    {
        readonly Map map;
        readonly Bitmap image;
        readonly Dictionary<Color, Region> regionsMapped = new Dictionary<Color, Region>();

        /// <summary>
        /// Constructs MapImage instance.
        /// </summary>
        /// <param name="map">Map with regions.</param>
        /// <param name="image">Image containing the map in proper format.</param>
        /// <param name="regionsWithColors">Tuples mapping color of region to corresponding region. Regions must correspond to those in the map.</param>
        public MapImage(Map map, Bitmap image, ICollection<Tuple<Color, Region>> regionsWithColors)
        {
            this.map = map;
            this.image = image;
            
            if (map.Regions.Count != regionsWithColors.Count) throw new ArgumentException();

            HashSet<Region> regions = new HashSet<Region>(); // for value in dictionary to be unique
            foreach (var item in regionsWithColors)
            {
                if (!regions.Add(item.Item2)) throw new ArgumentException();
                if (!map.Regions.Contains(item.Item2)) throw new ArgumentException(); // TODO: slow, it shouldnt matter tho

                regionsMapped.Add(item.Item1, item.Item2);
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
            return !regionsMapped.TryGetValue(color, out region) ? null : region;
        }

        /// <summary>
        /// Based on coordinates on the image, returns region corresponding to those coordinates.
        /// </summary>
        /// <param name="x">Coordinate x on image specified in constructor.</param>
        /// <param name="y">Coordinate y on image specified in constructor.</param>
        /// <returns>Region corresponding to the coordinates</returns>
        public Region GetRegion(int x, int y)
        {
            return GetRegion(image.GetPixel(x, y));
        }
        
    }
}
