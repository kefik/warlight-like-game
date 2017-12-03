namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Common.Collections;
    using GameObjectsLib.GameMap;
    using Region = GameObjectsLib.GameMap.Region;

    /// <summary>
    ///     Represents low level visual representation of the map template,
    ///     represents the link between image and regions.
    /// </summary>
    internal class MapImageTemplateProcessor
    {
        /// <summary>
        ///     Image containing highlighted version of the game map.
        /// </summary>
        public Bitmap RegionHighlightedImage { get; }

        private readonly BidirectionalDictionary<Color, Region> regionsColorsMapped;

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
            RegionHighlightedImage = new Bitmap(regionHighlightedImage);

            if (map.Regions.Count != regionsWithColors.Count)
            {
                throw new ArgumentException();
            }

            regionsColorsMapped = new BidirectionalDictionary<Color, Region>();

            // initialize color
            foreach (KeyValuePair<Color, Region> item in regionsWithColors)
            {
                regionsColorsMapped.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        ///     Finds region corresponding to the given color and returns it.
        /// </summary>
        /// <param name="color">Color on the map specified in constructor</param>
        /// <returns>Region corresponding to the color.</returns>
        public Region GetRegion(Color color)
        {
            return regionsColorsMapped.TryGetValue(color, out Region region) ? region : null;
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
            if (color.R == Global.TextPlacementColor.R
                && color.G == Global.TextPlacementColor.G
                && color.B == Global.TextPlacementColor.B) // its color marking army writing position
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
            bool correct = regionsColorsMapped.TryGetValue(region, out Color color);
            return correct ? new Color?(color) : null;
        }
    }
}
