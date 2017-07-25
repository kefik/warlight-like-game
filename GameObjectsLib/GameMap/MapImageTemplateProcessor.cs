using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;

namespace GameObjectsLib.GameMap
{
    /// <summary>
    /// Represents low level visual representation of the map template,
    /// represents the link between image and regions.
    /// </summary>
    class MapImageTemplateProcessor
    {
        readonly Map map;
        /// <summary>
        /// Image containing highlighted version of the game map.
        /// </summary>
        public Bitmap RegionHighlightedImage { get; }

        readonly Color textPlacementColor = Color.FromArgb(78, 24, 86);

        readonly Dictionary<Color, Region> regionsMapped;
        readonly Dictionary<Region, Color> colorsMapped;

        /// <summary>
        /// Constructs MapImage instance.
        /// </summary>
        /// <param name="map">Map with regions.</param>
        /// <param name="regionHighlightedImage">Image containing the map in proper format.</param>
        /// <param name="regionsWithColors">Tuples mapping color of region to corresponding region. Regions must correspond to those in the map.</param>
        public MapImageTemplateProcessor(Map map, Image regionHighlightedImage,
            Dictionary<Color, Region> regionsWithColors)
        {
            this.map = map;
            this.RegionHighlightedImage = new Bitmap(regionHighlightedImage);

            if (map.Regions.Count != regionsWithColors.Count) throw new ArgumentException();

            regionsMapped = regionsWithColors;

            // initialize color
            colorsMapped = new Dictionary<Region, Color>();
            foreach (var item in regionsWithColors)
            {
                colorsMapped.Add(item.Value, item.Key);
            }
        }
        /// <summary>
        /// Finds region corresponding to the given color and returns it.
        /// </summary>
        /// <param name="color">Color on the map specified in constructor</param>
        /// <returns>Region corresponding to the color.</returns>
        public Region GetRegion(Color color)
        {
            return regionsMapped.TryGetValue(color, out Region region) ? region : null;
        }

        /// <summary>
        /// Based on the image, returns region corresponding to those coordinates.
        /// </summary>
        /// <param name="x">Coordinate x on image specified in constructor.</param>
        /// <param name="y">Coordinate y on image specified in constructor.</param>
        /// <returns>Region corresponding to the coordinates</returns>
        public Region GetRegion(int x, int y)
        {
            var color = RegionHighlightedImage.GetPixel(x, y);
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
        /// Returns color mapped to region. If no such regions is found, null is returned.
        /// </summary>
        /// <param name="region">Region to match the color.</param>
        /// <returns>Color matching the region.</returns>
        public Color? GetColor(Region region)
        {
            bool correct = colorsMapped.TryGetValue(region, out Color color);
            return correct ? new Color?(color) : null;
        }


    }

    /// <summary>
    /// Represents visual representation of the game map and functionality linked with it.
    /// </summary>
    public class MapImageProcessor : IRefreshable<Game.Game>
    {
        readonly Color regionNotVisibleColor = Color.FromArgb(155, 150, 122);
        readonly Color regionVisibleUnoccupiedColor = Color.White;
        readonly Color textPlacementColor = Color.FromArgb(78, 24, 86);

        public Bitmap TemplateImage
        {
            get { return templateProcessor.RegionHighlightedImage; }
        }
        public Bitmap MapImage { get; }
        readonly MapImageTemplateProcessor templateProcessor;

        private MapImageProcessor(MapImageTemplateProcessor mapImageTemplateProcessor, Bitmap gameMapMapImage)
        {
            MapImage = gameMapMapImage;
            templateProcessor = mapImageTemplateProcessor;
        }

        /// <summary>
        /// Recolors every pixel of the original color from the map template
        /// in the map to the new color.
        /// </summary>
        /// <param name="sourceColor">Source color in region highlighted image.</param>
        /// <param name="targetColor">Color to recolor the region to.</param>
        public void Recolor(Color sourceColor, Color targetColor)
        {
            var regionHighlightedImage = templateProcessor.RegionHighlightedImage;
            var mapImage = MapImage;

            // lock the bits and change format to rgb 
            BitmapData regionHighlightedImageData =
                regionHighlightedImage.LockBits(new Rectangle(0, 0, regionHighlightedImage.Width, regionHighlightedImage.Height), ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
            try
            {
                BitmapData imageData =
                    mapImage.LockBits(new Rectangle(0, 0, mapImage.Width, mapImage.Height), ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* regionHighlightedMapPtr = (byte*)regionHighlightedImageData.Scan0;
                    byte* mapPtr = (byte*)imageData.Scan0;

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
                        else if (isTheArea && *red == textPlacementColor.R
                            && *green == textPlacementColor.G
                            && *blue == textPlacementColor.B)
                        {
                            *(mapPtr + 2) = targetColor.R;
                            *(mapPtr + 1) = targetColor.G;
                            *mapPtr = targetColor.B;
                            isTheArea = false;
                        }
                        else isTheArea = false;

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

        /// <summary>
        /// Recolors given region to target color.
        /// </summary>
        /// <param name="region">Given region.</param>
        /// <param name="targetColor">Color that given region will be recolored to.</param>
        public void Recolor(Region region, Color targetColor)
        {
            if (region == null) return;

            var colorOrNull = templateProcessor.GetColor(region);
            if (colorOrNull == null) return;

            Recolor(colorOrNull.Value, targetColor);
        }


        public Region GetRegion(int x, int y)
        {
            return templateProcessor.GetRegion(x, y);
        }

        /// <summary>
        /// Refreshes the bitmaps, redrawing all the content according to the
        /// information player possesses.
        /// </summary>
        /// <param name="game">Game from which it has source.</param>
        public void Refresh(Game.Game game)
        {
            if (game.GetType() == typeof(SingleplayerGame)) Redraw((SingleplayerGame)game);
        }

        /// <summary>
        /// Resets round, recoloring region selected by player to the default color.
        /// </summary>
        /// <param name="gameBeginningRound">What happened in the game round.</param>
        public void ResetRound(GameBeginningRound gameBeginningRound)
        {
            foreach (var tuple in gameBeginningRound.SelectedRegions)
            {
                var region = tuple.Item2;

                Recolor(region, regionNotVisibleColor);
            }
        }

        public void ResetRound(Round round)
        {
            throw new NotImplementedException();
        }

        public void ResetRoundPhase(Attacking deploying)
        {
            throw new NotImplementedException();
        }

        void Redraw(SingleplayerGame game)
        {
            var humanPlayer = (from player in game.Players
                               where player.GetType() == typeof(HumanPlayer)
                               select player).First() as HumanPlayer;
            // get owned regions
            var ownedRegions = humanPlayer.ControlledRegions;

            // recolor them to players color
            foreach (var ownedRegion in ownedRegions)
            {
                Recolor(ownedRegion, Color.FromKnownColor(humanPlayer.Color));

                DrawArmyNumber(ownedRegion, ownedRegion.Army);
            }

            // recolor neighbour regions local player does not own
            var neighbourNotOwnedRegions = (from ownedRegion in ownedRegions
                                            from neighbour in ownedRegion.NeighbourRegions
                                            where neighbour.Owner != humanPlayer
                                            select neighbour).Distinct();

            foreach (var region in neighbourNotOwnedRegions)
            {
                var owner = region.Owner;
                if (owner == null)
                {
                    Recolor(region, regionVisibleUnoccupiedColor);
                }
                else
                {
                    var ownerColor = Color.FromKnownColor(owner.Color);

                    Recolor(region, ownerColor);
                }
                DrawArmyNumber(region, region.Army);

            }
        }

        Color highlightColor = Color.Gold;
        /// <summary>
        /// Highlights given region in the map by drawing stripes onto it.
        /// </summary>
        /// <param name="region">Region in the map.</param>
        /// <param name="army">Army of the highlighted region</param>
        public void HighlightRegion(Region region, int army)
        {
            Color color;
            {
                var colorOrNull = templateProcessor.GetColor(region);
                if (colorOrNull == null) return;

                color = colorOrNull.Value;
            }

            var regionHighlightedImage = templateProcessor.RegionHighlightedImage;
            var mapImage = MapImage;

            // lock the bits and change format to rgb 
            BitmapData regionHighlightedImageData =
                regionHighlightedImage.LockBits(new Rectangle(0, 0, regionHighlightedImage.Width, regionHighlightedImage.Height), ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
            try
            {
                BitmapData imageData =
                    mapImage.LockBits(new Rectangle(0, 0, mapImage.Width, mapImage.Height), ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* regionHighlightedMapPtr = (byte*)regionHighlightedImageData.Scan0;
                    byte* mapPtr = (byte*)imageData.Scan0;

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

            DrawArmyNumber(region, army);


        }

        public void UnhighlightRegion(Region region, Player playerOnTurn, int army)
        {
            if (region == null) return;
            if (region.Owner == null)
            {
                // is it neighbour of some of players region?
                bool isNeighbour = (from controlledRegion in playerOnTurn.ControlledRegions
                                    where controlledRegion.IsNeighbourOf(region)
                                    select controlledRegion).Any();
                if (isNeighbour)
                {
                    Recolor(region, regionVisibleUnoccupiedColor);
                    DrawArmyNumber(region, army);
                }
                else
                {
                    Recolor(region, regionNotVisibleColor);
                }
            }
            else
            {
                Recolor(region, Color.FromKnownColor(region.Owner.Color));
                DrawArmyNumber(region, army);
            }
        }

        /// <summary>
        /// Draws army number into the map. If its been drawed previously, it overdraws it
        /// resetting color to regions owner color. If not, it draws it.
        /// </summary>
        /// <param name="region">Region to draw it into.</param>
        /// <param name="army">Army number to draw.</param>
        public void OverDrawArmyNumber(Region region, int army)
        {
            // get color that match the region
            var colorOrNull = templateProcessor.GetColor(region);
            if (colorOrNull == null) return;
            // source color
            var sourceColor = colorOrNull.Value;

            // recolor back to the previous color
            if (region.Owner != null)
                Recolor(sourceColor, Color.FromKnownColor(region.Owner.Color));
            else
            {
                bool isNeighbour = (from item in region.NeighbourRegions
                                    where item == region
                                    select item).Any();
                if (!isNeighbour) Recolor(sourceColor, regionNotVisibleColor);
                else Recolor(sourceColor, regionVisibleUnoccupiedColor);
            }

            DrawArmyNumber(region, army);

        }
        /// <summary>
        /// Draws number of army by selected region onto MapImage.
        /// </summary>
        /// <param name="region">Regions army to be drawed.</param>
        /// <param name="army">Army number to draw.</param>
        void DrawArmyNumber(Region region, int army)
        {
            // get color that match the region
            var colorOrNull = templateProcessor.GetColor(region);
            if (colorOrNull == null) return;
            // source color
            var sourceColor = colorOrNull.Value;

            Rectangle rect = new Rectangle(0, 0, TemplateImage.Width, TemplateImage.Height);
            BitmapData bmpData =
                TemplateImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            PointF point = default(PointF);
            try
            {
                IntPtr ptr = bmpData.Scan0;

                int stride = bmpData.Stride;
                byte[] rgbValues = new byte[bmpData.Stride * bmpData.Height];

                Marshal.Copy(ptr, rgbValues, 0, rgbValues.Length);

                PointF GetMatchingPoint()
                {
                    for (int column = 0; column < bmpData.Height; column++)
                    {
                        Color previousColor = default(Color);
                        for (int row = 0; row < bmpData.Width; row++)
                        {
                            byte red = rgbValues[(column * stride) + (row * 3) + 2];
                            byte green = rgbValues[(column * stride) + (row * 3) + 1];
                            byte blue = rgbValues[(column * stride) + (row * 3)];
                            Color color = Color.FromArgb(red, green, blue);
                            // if it is point to draw and its in the correct region, get point coordinates
                            if (color == textPlacementColor && previousColor == sourceColor)
                                return new PointF(row, column);
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
                TemplateImage.UnlockBits(bmpData);
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
        /// Initializes an instance of MapImageProcessor.
        /// </summary>
        /// <param name="map">Map of the future game.</param>
        /// <param name="regionHighlightedImagePath">Path of image whose role is to map region to certain color to recognize what image has been clicked on by the user.</param>
        /// <param name="regionColorMappingPath">Path of file mapping color to certain existing map region.</param>
        /// <param name="mapImagePath">Map of image that will be used as map and displayed to the user.</param>
        /// <returns>Initialized instance.</returns>
        public static MapImageProcessor Create(Map map, string regionHighlightedImagePath, string regionColorMappingPath, string mapImagePath)
        {
            var regionHighlightedImage = new Bitmap(regionHighlightedImagePath);
            var image = new Bitmap(mapImagePath);

            // images are not equally sized
            if (image.Size != regionHighlightedImage.Size) throw new ArgumentException();

            // read the file
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ValidationType = ValidationType.Schema
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, args) =>
            {
                switch (args.Severity)
                {
                    case XmlSeverityType.Error:
                        throw new XmlSchemaValidationException();
                    case XmlSeverityType.Warning:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            var dictionary = new Dictionary<Color, Region>();
            using (XmlReader reader = XmlReader.Create(regionColorMappingPath, settings))
            {
                Color color = default(Color);
                Region region = default(Region);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Entry":
                                    break;
                                case nameof(Color):
                                    // TODO : maybe better way is possible
                                    byte red = Convert.ToByte(reader.GetAttribute("Red"));
                                    byte green = Convert.ToByte(reader.GetAttribute("Green"));
                                    byte blue = Convert.ToByte(reader.GetAttribute("Blue"));
                                    byte alpha = Convert.ToByte(reader.GetAttribute("Alpha"));
                                    color = Color.FromArgb(alpha, red, green, blue);
                                    break;
                                case nameof(Region):
                                    region = (from item in map.Regions
                                              where item.Name == reader.GetAttribute("Name")
                                              select item).First();
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Entry")
                            {
                                dictionary.Add(color, region);
                            }
                            break;
                    }
                }
            }

            MapImageTemplateProcessor mapImageTemplateProcessor = new MapImageTemplateProcessor(map, regionHighlightedImage, dictionary);


            return new MapImageProcessor(mapImageTemplateProcessor, image);
        }

    }
}
