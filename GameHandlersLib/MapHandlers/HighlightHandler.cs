namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.GameMap;
    using Region = GameObjectsLib.GameMap.Region;

    /// <summary>
    /// Handler for highlighting region.
    /// </summary>
    internal class HighlightHandler
    {
        private readonly Bitmap mapImage;
        private MapImageTemplateProcessor templateProcessor;
        private TextDrawingHandler textDrawingHandler;
        private ColoringHandler coloringHandler;

        /// <summary>
        /// List of region, its army and its previous color.
        /// </summary>
        private readonly IList<Tuple<Region, int?, Color>> highlightedRegions = new List<Tuple<Region, int?, Color>>();

        public HighlightHandler(Bitmap mapImage, MapImageTemplateProcessor templateProcessor, TextDrawingHandler textDrawingHandler, ColoringHandler coloringHandler)
        {
            this.mapImage = mapImage;
            this.templateProcessor = templateProcessor;
            this.textDrawingHandler = textDrawingHandler;
            this.coloringHandler = coloringHandler;
        }

        /// <summary>
        /// Gets highlighted regions count.
        /// </summary>
        public int HighlightedRegionsCount
        {
            get { return highlightedRegions.Count; }
        }

        /// <summary>
        /// Is region selected?
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool IsRegionHighlighted(Region region)
        {
            return highlightedRegions.Any(x => x.Item1 == region);
        }

        /// <summary>
        /// Highlights region.
        /// </summary>
        /// <param name="region">Region to be highlighted.</param>
        /// <param name="originalRegionColor">Original color of region.</param>
        /// <param name="army">Army occupying the reigon. Null if the army is not visible.</param>
        internal void HighlightRegion(Region region, Color originalRegionColor, int? army)
        {
            // region template color
            Color color = templateProcessor.GetColor(region).Value;

            Bitmap regionHighlightedImage = templateProcessor.RegionHighlightedImage;
            Bitmap mapImage = this.mapImage;

            // lock the bits and change format to rgb 
            BitmapData regionHighlightedImageData =
                regionHighlightedImage.LockBits(
                    new Rectangle(0, 0, regionHighlightedImage.Width, regionHighlightedImage.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
            try
            {
                BitmapData imageData =
                    mapImage.LockBits(new Rectangle(0, 0, mapImage.Width, mapImage.Height), ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);

                unsafe
                {
                    var regionHighlightedMapPtr = (byte*)regionHighlightedImageData.Scan0;
                    var mapPtr = (byte*)imageData.Scan0;

                    int bytes = Math.Abs(regionHighlightedImageData.Stride) * regionHighlightedImage.Height;

                    for (int i = 0; i < bytes; i += 6)
                    {
                        // get colors from highlighted one
                        byte* blue = regionHighlightedMapPtr;
                        byte* green = regionHighlightedMapPtr + 1;
                        byte* red = regionHighlightedMapPtr + 2;

                        Color regionColor = Color.FromArgb(*red, *green, *blue);
                        if (regionColor == color)
                        {
                            *(mapPtr + 2) = Global.HighlightColor.R;
                            *(mapPtr + 1) = Global.HighlightColor.G;
                            *mapPtr = Global.HighlightColor.B;
                        }


                        regionHighlightedMapPtr += 6;
                        mapPtr += 6;
                    }
                }

                mapImage.UnlockBits(imageData);
            }
            finally
            {
                regionHighlightedImage.UnlockBits(regionHighlightedImageData);
            }

            if (army != null)
            {
                // draw army number
                textDrawingHandler.DrawArmyNumber(region, army.Value);
            }
            
            highlightedRegions.Add(new Tuple<Region, int?, Color>(region, army, originalRegionColor));
        }

        /// <summary>
        /// Highlights region.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="army"></param>
        internal void HighlightRegion(int x, int y, int? army)
        {
            HighlightRegion(templateProcessor.GetRegion(x, y), mapImage.GetPixel(x, y), army);
        }

        /// <summary>
        /// Unhighlights region recoloring it to previous color.
        /// If the region was not highlighted, throws <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="region"></param>
        internal void UnhighlightRegion(Region region)
        {
            var highlightedRegionTuple =
                highlightedRegions.FirstOrDefault(x => x.Item1 == region);
            if (highlightedRegionTuple == null)
            {
                throw new ArgumentException($"Region {region.Name} was not highlighted previously, cannot be unhighlighted.");
            }
            // is neighbour to player on turn => recolor to visible color
            coloringHandler.Recolor(region, highlightedRegionTuple.Item3);

            // draw army if there was any
            if (highlightedRegionTuple.Item2 != null)
            {
                textDrawingHandler.DrawArmyNumber(region, highlightedRegionTuple.Item2.Value);
            }

            highlightedRegions.Remove(highlightedRegionTuple);
        }

        /// <summary>
        /// Unhighlights region, recoloring it to previous color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void UnhighlightRegion(int x, int y)
        {
            UnhighlightRegion(templateProcessor.GetRegion(x, y));
        }

        /// <summary>
        /// Resets highlighting.
        /// </summary>
        internal void ResetHighlight()
        {
            for (int i = HighlightedRegionsCount - 1; i >= 0; i--)
            {
                UnhighlightRegion(highlightedRegions[i].Item1);
            }
        }
    }
}