namespace GameObjectsLib.GameMap
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Schema;
    using Game;

    /// <summary>
    ///     Represents visual representation of the game map and functionality linked with it.
    /// </summary>
    public class MapImageProcessor : IRefreshable<Game>
    {
        private readonly Color regionNotVisibleColor = Color.FromArgb(155, 150, 122);
        private readonly Color regionVisibleUnoccupiedColor = Color.White;
        private readonly Color textPlacementColor = Color.FromArgb(78, 24, 86);
        private readonly bool isFogOfWar;

        public Bitmap TemplateImage
        {
            get { return templateProcessor.RegionHighlightedImage; }
        }

        public Bitmap MapImage { get; }
        private readonly MapImageTemplateProcessor templateProcessor;

        private MapImageProcessor(MapImageTemplateProcessor mapImageTemplateProcessor, Bitmap gameMapMapImage, bool isFogOfWar)
        {
            MapImage = gameMapMapImage;
            templateProcessor = mapImageTemplateProcessor;
            this.isFogOfWar = isFogOfWar;
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
                    var regionHighlightedMapPtr = (byte*) regionHighlightedImageData.Scan0;
                    var mapPtr = (byte*) imageData.Scan0;

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

        /// <summary>
        ///     Recolors given region to target color.
        /// </summary>
        /// <param name="region">Given region.</param>
        /// <param name="targetColor">Color that given region will be recolored to.</param>
        public void Recolor(Region region, Color targetColor)
        {
            if (region == null)
            {
                return;
            }

            Color? colorOrNull = templateProcessor.GetColor(region);
            if (colorOrNull == null)
            {
                return;
            }

            Recolor(colorOrNull.Value, targetColor);
        }

        public void Recolor(Region region, KnownColor targetColor)
        {
            Recolor(region, Color.FromKnownColor(targetColor));
        }


        public Region GetRegion(int x, int y)
        {
            return templateProcessor.GetRegion(x, y);
        }

        /// <summary>
        ///     Refreshes the bitmaps, redrawing all the content according to the
        ///     information player possesses.
        /// </summary>
        /// <param name="game">Game from which it has source.</param>
        public void Refresh(Game game)
        {
            if (isFogOfWar)
            {
                switch (game.GameType)
                {
                    case GameType.SinglePlayer:
                        Redraw((SingleplayerGame) game);
                        break;
                    case GameType.MultiplayerHotseat:
                        Redraw((HotseatGame) game);
                        break;
                    case GameType.MultiplayerNetwork:
                        break;
                }
            }
            else
            {
                RedrawWithoutFogOfWar(game);
            }
        }

        /// <summary>
        ///     Resets round, recoloring region selected by player to the default color.
        /// </summary>
        /// <param name="gameBeginningRound">What happened in the game round.</param>
        public void ResetRound(GameBeginningRound gameBeginningRound)
        {
            foreach (Tuple<Player, Region> tuple in gameBeginningRound.SelectedRegions)
            {
                Region region = tuple.Item2;

                Recolor(region, regionNotVisibleColor);
            }
        }

        /// <summary>
        ///     Resets round, recoloring everything to previous color, writing
        ///     original numbers of armies.
        /// </summary>
        /// <param name="gameRound">Round to reset</param>
        public void ResetRound(GameRound gameRound)
        {
            Attacking attackingPhase = gameRound.Attacking;
            ResetAttackingPhase(attackingPhase, gameRound.Deploying);

            Deploying deployingPhase = gameRound.Deploying;
            ResetDeployingPhase(deployingPhase);
        }

        /// <summary>
        ///     Resets attacking phase recoloring everything back.
        ///     Notice: deploying phase passed in parameter must be corect in order
        ///     to make this method work properly.
        /// </summary>
        /// <param name="attackingPhase">Attacking phase</param>
        /// <param name="deployingPhase">Deploying phase</param>
        public void ResetAttackingPhase(Attacking attackingPhase, Deploying deployingPhase)
        {
            // TODO: check + should not clear
            List<Attack> attacks = attackingPhase.Attacks;
            //for (int i = attacks.Count - 1; i >= 0; i--)
            //{
            //    var attacker = attacks[i].Attacker;
            //    attacks.Remove(attacks[i]);
            //    // + 1 bcuz 1 army unit must always stay in the region
            //    int armyBeforeAttack
            //        = attackingPhase.GetUnitsLeftToAttack(attacker, deployingPhase) + 1;
            //    OverDrawArmyNumber(attacker, armyBeforeAttack);
            //}
            IEnumerable<IGrouping<Region, Attack>> attackerRegionsGroups = from attack in attacks
                                                                           group attack by attack.Attacker;
            var regionAttackingArmyPairEnumerable = from gr in attackerRegionsGroups
                                                    select new
                                                    {
                                                        Attacker = gr.Key,
                                                        AttackingArmy = gr.Sum(x => x.AttackingArmy)
                                                    };

            foreach (var item in regionAttackingArmyPairEnumerable)
            {
                Region attacker = item.Attacker;
                int attackingArmy = item.AttackingArmy;

                IEnumerable<int> regionDeployedArmy = from tuple in deployingPhase.ArmiesDeployed
                                                      where tuple.Item1 == attacker
                                                      select tuple.Item2;
                if (!regionDeployedArmy.Any())
                {
                    OverDrawArmyNumber(attacker, attacker.Army);
                }
                else
                {
                    int army = regionDeployedArmy.First();
                    OverDrawArmyNumber(attacker, army);
                }
            }
        }

        /// <summary>
        ///     Resets deploying phase, recoloring everything to the previous color,
        ///     redrawing everything to the original state.
        /// </summary>
        /// <param name="deployingPhase"></param>
        public void ResetDeployingPhase(Deploying deployingPhase)
        {
            // TODO: check + should not clear
            foreach (Tuple<Region, int> tuple in deployingPhase.ArmiesDeployed)
            {
                Region region = tuple.Item1;
                if (region.Owner != null)
                {
                    Recolor(region, Color.FromKnownColor(region.Owner.Color));
                }
                else
                {
                    throw new ArgumentException();
                }
                DrawArmyNumber(region, region.Army);
            }
        }

        private void Redraw(SingleplayerGame game)
        {
            HumanPlayer humanPlayer = (from player in game.Players
                                       where player.GetType() == typeof(HumanPlayer)
                                       select player).First() as HumanPlayer;
            // get owned regions
            IList<Region> ownedRegions = humanPlayer.ControlledRegions;

            // recolor them to players color
            foreach (Region ownedRegion in ownedRegions)
            {
                Recolor(ownedRegion, humanPlayer.Color);

                DrawArmyNumber(ownedRegion, ownedRegion.Army);
            }

            // recolor neighbour regions local player does not own
            IEnumerable<Region> neighbourNotOwnedRegions = (from ownedRegion in ownedRegions
                                                            from neighbour in ownedRegion.NeighbourRegions
                                                            where neighbour.Owner != humanPlayer
                                                            select neighbour).Distinct();

            foreach (Region region in neighbourNotOwnedRegions)
            {
                Player owner = region.Owner;
                if (owner == null)
                {
                    Recolor(region, regionVisibleUnoccupiedColor);
                }
                else
                {
                    Color ownerColor = Color.FromKnownColor(owner.Color);

                    Recolor(region, ownerColor);
                }
                DrawArmyNumber(region, region.Army);
            }
        }

        private void RedrawWithoutFogOfWar(Game game)
        {
            var regions = game.Map.Regions;
            foreach (var region in regions)
            {
                if (region.Owner == null)
                {
                    Recolor(region, regionVisibleUnoccupiedColor);
                }
                else
                {
                    Recolor(region, region.Owner.Color);
                }
            }
        }

        private void Redraw(HotseatGame game)
        {
            IEnumerable<HumanPlayer> humanPlayers = from player in game.Players
                                                    where player.GetType() == typeof(HumanPlayer)
                                                    select (HumanPlayer) player;

            IEnumerable<Region> controlledRegions = from humanPlayer in humanPlayers
                                                    from controlledRegion in humanPlayer.ControlledRegions
                                                    select controlledRegion;

            foreach (Region ownedRegion in controlledRegions)
            {
                Recolor(ownedRegion, ownedRegion.Owner.Color);

                DrawArmyNumber(ownedRegion, ownedRegion.Army);
            }


            IEnumerable<Region> neighbourNotOwnedRegions = (from humanPlayer in humanPlayers
                                                            from ownedRegion in humanPlayer.ControlledRegions
                                                            from neighbour in ownedRegion.NeighbourRegions
                                                            where neighbour.Owner != humanPlayer
                                                            select neighbour).Distinct();
            foreach (Region region in neighbourNotOwnedRegions)
            {
                if (region.Owner == null)
                {
                    Recolor(region, regionVisibleUnoccupiedColor);
                }
                else
                {
                    Recolor(region, region.Owner.Color);
                }

                DrawArmyNumber(region, region.Army);
            }
        }

        private Color highlightColor = Color.Gold;

        /// <summary>
        ///     Highlights given region in the map by drawing stripes onto it.
        /// </summary>
        /// <param name="region">Region in the map.</param>
        /// <param name="army">Army of the highlighted region</param>
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
                    var regionHighlightedMapPtr = (byte*) regionHighlightedImageData.Scan0;
                    var mapPtr = (byte*) imageData.Scan0;

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
            if (region == null)
            {
                return;
            }
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
                Recolor(sourceColor, Color.FromKnownColor(region.Owner.Color));
            }
            else
            {
                bool isNeighbour = (from item in region.NeighbourRegions
                                    where item == region
                                    select item).Any();
                if (!isNeighbour)
                {
                    Recolor(sourceColor, regionNotVisibleColor);
                }
                else
                {
                    Recolor(sourceColor, regionVisibleUnoccupiedColor);
                }
            }

            DrawArmyNumber(region, army);
        }

        /// <summary>
        ///     Draws number of army by selected region onto MapImage.
        /// </summary>
        /// <param name="region">Regions army to be drawed.</param>
        /// <param name="army">Army number to draw.</param>
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

            Rectangle rect = new Rectangle(0, 0, TemplateImage.Width, TemplateImage.Height);
            BitmapData bmpData =
                TemplateImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

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
                            if (color == textPlacementColor && previousColor == sourceColor)
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
        ///     Initializes an instance of MapImageProcessor.
        /// </summary>
        /// <param name="map">Map of the future game.</param>
        /// <param name="regionHighlightedImagePath">
        ///     Path of image whose role is to map region to certain color to recognize what
        ///     image has been clicked on by the user.
        /// </param>
        /// <param name="regionColorMappingPath">Path of file mapping color to certain existing map region.</param>
        /// <param name="mapImagePath">Map of image that will be used as map and displayed to the user.</param>
        /// <param name="isFogOfWar">True, if the given map should be processed as fog of war type.</param>
        /// <returns>Initialized instance.</returns>
        public static MapImageProcessor Create(Map map, string regionHighlightedImagePath,
            string regionColorMappingPath, string mapImagePath, bool isFogOfWar)
        {
            Bitmap regionHighlightedImage = new Bitmap(regionHighlightedImagePath);
            Bitmap image = new Bitmap(mapImagePath);

            // images are not equally sized
            if (image.Size != regionHighlightedImage.Size)
            {
                throw new ArgumentException();
            }

            // read the file
            XmlReaderSettings settings = new XmlReaderSettings
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

            MapImageTemplateProcessor mapImageTemplateProcessor =
                new MapImageTemplateProcessor(map, regionHighlightedImage, dictionary);


            return new MapImageProcessor(mapImageTemplateProcessor, image, isFogOfWar);
        }
    }
}