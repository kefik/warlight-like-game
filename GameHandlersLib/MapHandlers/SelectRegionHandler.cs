namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using GameObjectsLib.GameMap;
    using Region = GameObjectsLib.GameMap.Region;

    internal class SelectRegionHandler
    {
        private MapImageTemplateProcessor templateProcessor;
        private Bitmap MapImage;
        private Color highlightColor = Color.Gold;

        private Dictionary<Region, int> highlightedRegions = new Dictionary<Region, int>();

        public void HighlightRegion(Region region, int army)
        {
            Color color;
            {
                Color? colorOrNull = templateProcessor.GetColor(region);
                if (colorOrNull == null)
                {
                    return;
                }

                color = colorOrNull.Value;
            }

            Bitmap regionHighlightedImage = templateProcessor.RegionHighlightedImage;
            Bitmap mapImage = MapImage;

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
                            *(mapPtr + 2) = highlightColor.R;
                            *(mapPtr + 1) = highlightColor.G;
                            *mapPtr = highlightColor.B;
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

            highlightedRegions.Add(region, army);
            //DrawArmyNumber(region, army);
        }
        //public void UnhighlightRegion(Region region)
        //{
        //    if (region == null)
        //    {
        //        return;
        //    }
        //    if (region.Owner == null)
        //    {
        //        // is it neighbour of some of players region?
        //        bool isNeighbour = (from controlledRegion in playerOnTurn.ControlledRegions
        //                            where controlledRegion.IsNeighbourOf(region)
        //                            select controlledRegion).Any();
        //        if (isNeighbour)
        //        {
        //            Recolor(region, regionVisibleUnoccupiedColor);
        //            DrawArmyNumber(region, army);
        //        }
        //        else
        //        {
        //            Recolor(region, regionNotVisibleColor);
        //        }
        //    }
        //    else
        //    {
        //        Recolor(region, Color.FromKnownColor(region.Owner.Color));
        //        DrawArmyNumber(region, army);
        //    }
        //}
    }
}