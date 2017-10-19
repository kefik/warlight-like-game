namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using GameObjectsLib.GameMap;
    using Region = GameObjectsLib.GameMap.Region;

    /// <summary>
    /// Low level control for recoloring regions to specified colors.
    /// </summary>
    internal class ColoringHandler
    {
        private readonly MapImageTemplateProcessor templateProcessor;
        private readonly Bitmap mapImage;

        public ColoringHandler(Bitmap mapImage, MapImageTemplateProcessor templateProcessor)
        {
            if (templateProcessor == null || mapImage == null)
            {
                throw new ArgumentException();
            }

            this.templateProcessor = templateProcessor;
            this.mapImage = mapImage;
        }

        /// <summary>
        ///     Recolors given region to target color.
        /// </summary>
        /// <param name="region">Given region.</param>
        /// <param name="targetColor">Color that given region will be recolored to.</param>
        public void Recolor(Region region, Color targetColor)
        {
            if (region == null)
            {
                throw new ArgumentException("Region must not be null.");
            }

            Color? colorOrNull = templateProcessor.GetColor(region);
            if (colorOrNull == null)
            {
                throw new ArgumentException("There is no color matching region passed as parameter.");
            }

            Recolor(colorOrNull.Value, targetColor);
        }

        public void Recolor(Region region, KnownColor targetColor)
        {
            Recolor(region, Color.FromKnownColor(targetColor));
        }

        /// <summary>
        ///     Recolors every pixel of the original color from the map template
        ///     in the map to the new color.
        /// </summary>
        /// <param name="sourceColor">Source color in region highlighted image.</param>
        /// <param name="targetColor">Color to recolor the region to.</param>
        public void Recolor(Color sourceColor, Color targetColor)
        {
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

                    bool isTheArea = false;
                    for (int i = 0; i < bytes; i += 3)
                    {
                        // get colors from highlighted one
                        byte* blue = regionHighlightedMapPtr;
                        byte* green = regionHighlightedMapPtr + 1;
                        byte* red = regionHighlightedMapPtr + 2;


                        // if that color is equal to source color
                        if (*red == sourceColor.R && *green == sourceColor.G && *blue == sourceColor.B)
                        {
                            // recolor it in map image
                            *(mapPtr + 2) = targetColor.R;
                            *(mapPtr + 1) = targetColor.G;
                            *mapPtr = targetColor.B;
                            isTheArea = true;
                        }
                        else if (isTheArea && *red == Global.TextPlacementColor.R
                                 && *green == MapHandlers.Global.TextPlacementColor.G
                                 && *blue == MapHandlers.Global.TextPlacementColor.B)
                        {
                            *(mapPtr + 2) = targetColor.R;
                            *(mapPtr + 1) = targetColor.G;
                            *mapPtr = targetColor.B;
                            isTheArea = false;
                        }
                        else
                        {
                            isTheArea = false;
                        }

                        regionHighlightedMapPtr += 3;
                        mapPtr += 3;
                    }
                }

                mapImage.UnlockBits(imageData);
            }
            finally
            {
                regionHighlightedImage.UnlockBits(regionHighlightedImageData);
            }
        }
    }
}