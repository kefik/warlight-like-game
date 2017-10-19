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
    /// Handler for all region selections.
    /// </summary>
    internal class SelectRegionHandler
    {
        private readonly MapImageTemplateProcessor templateProcessor;
        private readonly ColoringHandler coloringHandler;
        private readonly TextDrawingHandler textDrawingHandler;
        private readonly Bitmap mapImage;

        private readonly Dictionary<Region, int> highlightedRegions = new Dictionary<Region, int>();

        public Region FirstSelectedRegion { get; private set; }
        public Region SecondSelectedRegion { get; private set; }

        private readonly bool isFogOfWar;
        internal Func<Player> GetPlayerOnTurn = () => null;

        public SelectRegionHandler(Bitmap mapImage, MapImageTemplateProcessor templateProcessor, ColoringHandler coloringHandler,
            TextDrawingHandler textDrawingHandler, bool isFogOfWar)
        {
            if (templateProcessor == null || coloringHandler == null || textDrawingHandler == null || mapImage == null)
            {
                throw new ArgumentException();
            }

            this.textDrawingHandler = textDrawingHandler;
            this.coloringHandler = coloringHandler;
            this.templateProcessor = templateProcessor;
            this.isFogOfWar = isFogOfWar;
            this.mapImage = mapImage;
        }

        internal void HighlightRegion(Region region, int army)
        {
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

            // draw army number
            textDrawingHandler.DrawArmyNumber(region, army);
        }
        internal void UnhighlightRegion(Region region)
        {
            if (region.Owner == null)
            {
                if (isFogOfWar)
                {
                    coloringHandler.Recolor(region, Global.RegionVisibleUnoccupiedColor);
                    int value;
                    {
                        if (!highlightedRegions.TryGetValue(region, out value))
                        {
                            throw new ArgumentException("Region is not highlighted.");
                        }
                    }
                    textDrawingHandler.DrawArmyNumber(region, value);
                }
                else
                {
                    // is it neighbour of some of players region ?
                    bool isNeighbour = region.IsNeighbourOf(GetPlayerOnTurn());
                    if (isNeighbour)
                    {
                        coloringHandler.Recolor(region, Global.RegionVisibleUnoccupiedColor);
                        int value;
                        {
                            if (!highlightedRegions.TryGetValue(region, out value))
                            {
                                throw new ArgumentException("Region is not highlighted.");
                            }
                        }
                        textDrawingHandler.DrawArmyNumber(region, value);
                    }
                    else
                    {
                        coloringHandler.Recolor(region, Global.RegionNotVisibleColor);
                    }
                }
            }
            else
            {
                coloringHandler.Recolor(region, Color.FromKnownColor(region.Owner.Color));
                int value;
                {
                    highlightedRegions.TryGetValue(region, out value);
                }
                textDrawingHandler.DrawArmyNumber(region, value);
            }
        }

        /// <summary>
        /// Resets highlight on all regions.
        /// </summary>
        private void ResetHighlight()
        {
            foreach (var item in highlightedRegions)
            {
                UnhighlightRegion(item.Key);
            }
        }

        /// <summary>
        /// Selects region, highlights it. If incorrect region is selected, all previously selected regions are resetted.
        /// </summary>
        /// <param name="x">X image coordinate</param>
        /// <param name="y">Y image coordinate</param>
        /// <param name="army">Army size of the (x,y) specified region.</param>
        /// <returns>How many regions are selected at the moment.</returns>
        public int SelectRegion(int x, int y, int army)
        {
            if (highlightedRegions.Count == 2)
            {
                return 2;
            }

            var region = templateProcessor.GetRegion(x, y);

            if (region == null || region.Owner != GetPlayerOnTurn())
            {
                ResetSelection();

                return 0;
            }

            HighlightRegion(region, army);
            // add to highlighted regions list
            if (highlightedRegions.Count == 0)
            {
                FirstSelectedRegion = region;
            }
            else if (highlightedRegions.Count == 1)
            {
                SecondSelectedRegion = region;
            }
            highlightedRegions.Add(region, army);

            return highlightedRegions.Count;
        }

        /// <summary>
        /// Resets selection and returns number regions hit by this method.
        /// </summary>
        /// <returns></returns>
        public int ResetSelection()
        {
            int resettedSelection = highlightedRegions.Count;

            ResetHighlight();

            FirstSelectedRegion = null;
            SecondSelectedRegion = null;
            highlightedRegions.Clear();

            return resettedSelection;
        }
    }
}