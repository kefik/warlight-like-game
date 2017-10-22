namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.InteropServices;
    using GameObjectsLib.GameMap;
    using Region = GameObjectsLib.GameMap.Region;

    /// <summary>
    /// Handler for drawing text onto specified BitMap.
    /// </summary>
    internal class TextDrawingHandler
    {
        private readonly MapImageTemplateProcessor templateProcessor;
        private readonly ColoringHandler coloringHandler;
        private SelectRegionHandler selectRegionHandler;
        private readonly Bitmap mapImage;

        public TextDrawingHandler(Bitmap mapImage, MapImageTemplateProcessor templateProcessor, ColoringHandler coloringHandler)
        {
            if (templateProcessor == null || coloringHandler == null || mapImage == null)
            {
                throw new ArgumentException();
            }

            this.templateProcessor = templateProcessor;
            this.coloringHandler = coloringHandler;
            this.mapImage = mapImage;
        }

        /// <summary>
        /// Initializes <see cref="TextDrawingHandler"/>.
        /// Must be called before any other method.
        /// </summary>
        /// <param name="selectRegionHandler"></param>
        public void Initialize(SelectRegionHandler selectRegionHandler)
        {
            this.selectRegionHandler = selectRegionHandler;
        }

        /// <summary>
        /// Draws army number onto screen. Respects highlighting.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="army"></param>
        public void DrawArmyNumber(Region region, int army)
        {
            // get color that match the region
            Color? colorOrNull = templateProcessor.GetColor(region);
            if (colorOrNull == null)
            {
                return;
            }
            // source color
            Color sourceColor = colorOrNull.Value;

            Rectangle rect = new Rectangle(0, 0, templateProcessor.RegionHighlightedImage.Width, templateProcessor.RegionHighlightedImage.Height);
            BitmapData bmpData =
                templateProcessor.RegionHighlightedImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            PointF point = default(PointF);
            try
            {
                IntPtr ptr = bmpData.Scan0;

                int stride = bmpData.Stride;
                var rgbValues = new byte[bmpData.Stride * bmpData.Height];

                Marshal.Copy(ptr, rgbValues, 0, rgbValues.Length);

                PointF GetMatchingPoint()
                {
                    for (int column = 0; column < bmpData.Height; column++)
                    {
                        Color previousColor = default(Color);
                        for (int row = 0; row < bmpData.Width; row++)
                        {
                            byte red = rgbValues[column * stride + row * 3 + 2];
                            byte green = rgbValues[column * stride + row * 3 + 1];
                            byte blue = rgbValues[column * stride + row * 3];
                            Color color = Color.FromArgb(red, green, blue);
                            // if it is point to draw and its in the correct region, get point coordinates
                            if (color == Global.TextPlacementColor && previousColor == sourceColor)
                            {
                                return new PointF(row, column);
                            }
                            previousColor = color;
                        }
                    }
                    throw new ArgumentException();
                }

                // get point where to draw the number of armies
                point = GetMatchingPoint();
            }
            finally
            {
                templateProcessor.RegionHighlightedImage.UnlockBits(bmpData);
            }

            Graphics gr = Graphics.FromImage(mapImage);
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            // draw the string onto map
            gr.DrawString(army.ToString(),
                new Font("Tahoma", 8), Brushes.Black,
                point);
            gr.Flush();
        }

        /// <summary>
        ///     Draws army number into the map. TODO: respect to highlighting
        /// </summary>
        /// <param name="region">Region to draw it into.</param>
        /// <param name="army">Army number to draw.</param>
        public void OverDrawArmyNumber(Region region, int army)
        {
            // get color that match the region
            Color? colorOrNull = templateProcessor.GetColor(region);
            if (colorOrNull == null)
            {
                return;
            }
            // source color
            Color sourceColor = colorOrNull.Value;

            // recolor back to the previous color
            if (region.Owner != null)
            {
                coloringHandler.Recolor(sourceColor, Color.FromKnownColor(region.Owner.Color));
            }
            else
            {
                // doesnt have owner => recolor to visible color
                coloringHandler.Recolor(sourceColor, Global.RegionVisibleUnoccupiedColor);
            }

            DrawArmyNumber(region, army);
        }
    }
}