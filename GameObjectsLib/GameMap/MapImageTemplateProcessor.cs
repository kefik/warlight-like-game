namespace GameObjectsLib.GameMap
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    ///     Represents low level visual representation of the map template,
    ///     represents the link between image and regions.
    /// </summary>
    internal class MapImageTemplateProcessor
    {
        private readonly Map map;

        /// <summary>
        ///     Image containing highlighted version of the game map.
        /// </summary>
        public Bitmap RegionHighlightedImage { get; }

        private readonly Color textPlacementColor = Color.FromArgb(78, 24, 86);

        private readonly Dictionary<Color, Region> regionsMapped;
        private readonly Dictionary<Region, Color> colorsMapped;

        /// <summary>
        ///     Constructs MapImage instance.
        /// </summary>
        /// <param name="map">Map with regions.</param>
        /// <param name="regionHighlightedImage">Image containing the map in proper format.</param>
        /// <param name="regionsWithColors">
        ///     Tuples mapping color of region to corresponding region. Regions must correspond to
        ///     those in the map.
        /// </param>
        public MapImageTemplateProcessor(Map map, Image regionHighlightedImage,
            Dictionary<Color, Region> regionsWithColors)
        {
            this.map = map;
            RegionHighlightedImage = new Bitmap(regionHighlightedImage);

            if (map.Regions.Count != regionsWithColors.Count)
            {
                throw new ArgumentException();
            }

            regionsMapped = regionsWithColors;

            // initialize color
            colorsMapped = new Dictionary<Region, Color>();
            foreach (KeyValuePair<Color, Region> item in regionsWithColors)
            {
                colorsMapped.Add(item.Value, item.Key);
            }
        }

        /// <summary>
        ///     Finds region corresponding to the given color and returns it.
        /// </summary>
        /// <param name="color">Color on the map specified in constructor</param>
        /// <returns>Region corresponding to the color.</returns>
        public Region GetRegion(Color color)
        {
            return regionsMapped.TryGetValue(color, out Region region) ? region : null;
        }

        /// <summary>
        ///     Based on the image, returns region corresponding to those coordinates.
        /// </summary>
        /// <param name="x">Coordinate x on image specified in constructor.</param>
        /// <param name="y">Coordinate y on image specified in constructor.</param>
        /// <returns>Region corresponding to the coordinates</returns>
        public Region GetRegion(int x, int y)
        {
            Color color = RegionHighlightedImage.GetPixel(x, y);
            if (color.R == textPlacementColor.R
                && color.G == textPlacementColor.G
                && color.B == textPlacementColor.B) // its color marking army writing position
            {
                // that color is only one pixel sized, so we get other pixel
                return GetRegion(RegionHighlightedImage.GetPixel(x - 1, y));
            }
            return GetRegion(RegionHighlightedImage.GetPixel(x, y));
        }

        /// <summary>
        ///     Returns color mapped to region. If no such regions is found, null is returned.
        /// </summary>
        /// <param name="region">Region to match the color.</param>
        /// <returns>Color matching the region.</returns>
        public Color? GetColor(Region region)
        {
            bool correct = colorsMapped.TryGetValue(region, out Color color);
            return correct ? new Color?(color) : null;
        }
    }
}
