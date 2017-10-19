﻿namespace GameHandlersLib.MapHandlers
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
        private MapImageTemplateProcessor templateProcessor;
        private ColoringHandler coloringHandler;
        internal Bitmap MapImage;

        /// <summary>
        /// Draws army number onto screen.
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

            Graphics gr = Graphics.FromImage(MapImage);
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
        ///     Draws army number into the map. If its been drawed previously, it overdraws it
        ///     resetting color to regions owner color. If not, it draws it.
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
                bool isNeighbour = (from item in region.NeighbourRegions
                                    where item == region
                                    select item).Any();
                if (!isNeighbour)
                {
                    coloringHandler.Recolor(sourceColor, Global.RegionNotVisibleColor);
                }
                else
                {
                    coloringHandler.Recolor(sourceColor, Global.RegionVisibleUnoccupiedColor);
                }
            }

            DrawArmyNumber(region, army);
        }
    }
}