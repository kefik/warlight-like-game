namespace GameHandlersLib.MapHandlers
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
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using Region = GameObjectsLib.GameMap.Region;

    /// <summary>
    ///     Represents visual representation of the game map and functionality linked with it.
    /// </summary>
    public class MapImageProcessor
    {
        private readonly bool isFogOfWar;

        public Bitmap MapImage { get; }

        public Bitmap TemplateImage
        {
            get { return templateProcessor.RegionHighlightedImage; }
        }

        private readonly MapImageTemplateProcessor templateProcessor;
        private readonly SelectRegionHandler selectRegionHandler;
        private readonly ColoringHandler coloringHandler;
        private readonly TextDrawingHandler textDrawingHandler;

        /// <summary>
        /// Is invoked when image is changed (redrawn, etc...)
        /// </summary>
        public Action OnImageChanged;

        /// <summary>
        /// Gets selected region by the player.
        /// </summary>
        public IReadOnlyList<Region> SelectedRegions
        {
            get { return selectRegionHandler.SelectedRegions; }
        }

        private MapImageProcessor(MapImageTemplateProcessor mapImageTemplateProcessor, Bitmap gameMapMapImage, TextDrawingHandler textDrawingHandler,
            ColoringHandler coloringHandler, SelectRegionHandler selectRegionHandler, bool isFogOfWar)
        {
            MapImage = gameMapMapImage;
            templateProcessor = mapImageTemplateProcessor;
            this.isFogOfWar = isFogOfWar;
            this.textDrawingHandler = textDrawingHandler;
            this.coloringHandler = coloringHandler;
            this.selectRegionHandler = selectRegionHandler;
        }

        internal Region GetRegion(int x, int y)
        {
            return templateProcessor.GetRegion(x, y);
        }

        /// <summary>
        /// Seizes specified region for player on turn.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="playerPerspective"></param>
        public void Seize(Region region, Player playerPerspective)
        {
            coloringHandler.Recolor(region, playerPerspective.Color);
            OnImageChanged?.Invoke();
        }

        /// <summary>
        /// Selects region specified on (x,y) coordinates with army.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playerPerspective"></param>
        /// <param name="army"></param>
        /// <returns></returns>
        public int Select(int x, int y, Player playerPerspective, int army)
        {
            int regionsSelectedCount = selectRegionHandler.SelectRegion(x, y, playerPerspective, army);
            OnImageChanged?.Invoke();
            return regionsSelectedCount;
        }

        /// <summary>
        /// Resets selection. Returns resetted regions selected count.
        /// </summary>
        /// <returns></returns>
        public int ResetSelection()
        {
            int resettedRegionsCount = selectRegionHandler.ResetSelection();
            OnImageChanged?.Invoke();
            return resettedRegionsCount;
        }

        /// <summary>
        /// Deploys army graphically.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="newArmy"></param>
        public void Deploy(Region region, int newArmy)
        {
            textDrawingHandler.OverDrawArmyNumber(region, newArmy);

            OnImageChanged?.Invoke();
        }

        /// <summary>
        /// Attacks graphically.
        /// </summary>
        /// <param name="newRegionArmy"></param>
        public void Attack(int newRegionArmy)
        {
            if (SelectedRegions.Count != 2)
            {
                throw new ArgumentException("2 regions must be selected in order to properly attack.");
            }

            var attacker = SelectedRegions[0];

            selectRegionHandler.ResetSelection();
            textDrawingHandler.OverDrawArmyNumber(attacker, newRegionArmy);

            OnImageChanged?.Invoke();
        }

        /// <summary>
        /// Resets round passed as parameter
        /// </summary>
        public void ResetRound(Round round)
        {
            if (round.GetType() == typeof(GameRound))
            {
                ResetRound((GameRound) round);
            }
            else if (round.GetType() == typeof(GameBeginningRound))
            {
                ResetRound((GameBeginningRound) round);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            OnImageChanged?.Invoke();
        }

        /// <summary>
        ///     Resets round, recoloring region selected by player to the default color.
        /// </summary>
        /// <param name="gameBeginningRound">What happened in the game round.</param>
        internal void ResetRound(GameBeginningRound gameBeginningRound)
        {
            foreach (var tuple in gameBeginningRound.SelectedRegions)
            {
                Region region = tuple.Region;

                coloringHandler.Recolor(region, Global.RegionNotVisibleColor);
            }
        }

        /// <summary>
        ///     Resets round, recoloring everything to previous color, writing
        ///     original numbers of armies.
        /// </summary>
        /// <param name="gameRound">Round to reset</param>
        internal void ResetRound(GameRound gameRound)
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
            var attacks = attackingPhase.Attacks;
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

                IEnumerable<int> regionDeployedArmy = from tuple in deployingPhase.ArmiesDeployed
                                                      where tuple.Region == attacker
                                                      select tuple.Army;
                if (!regionDeployedArmy.Any())
                {
                    textDrawingHandler.OverDrawArmyNumber(attacker, attacker.Army);
                }
                else
                {
                    int army = regionDeployedArmy.First();
                    textDrawingHandler.OverDrawArmyNumber(attacker, army);
                }

                OnImageChanged?.Invoke();
            }
        }

        /// <summary>
        ///     Resets deploying phase, recoloring everything to the previous color,
        ///     redrawing everything to the original state.
        /// </summary>
        /// <param name="deployingPhase"></param>
        public void ResetDeployingPhase(Deploying deployingPhase)
        {
            foreach (var tuple in deployingPhase.ArmiesDeployed)
            {
                Region region = tuple.Region;
                if (region.Owner == null)
                {
                    throw new ArgumentException();
                }
                coloringHandler.Recolor(region, Color.FromKnownColor(region.Owner.Color));
                textDrawingHandler.DrawArmyNumber(region, region.Army);
            }

            OnImageChanged?.Invoke();
        }

        /// <summary>
        ///     Refreshes the bitmaps, redrawing all the content according to the
        ///     information player possesses.
        /// </summary>
        /// <param name="game">Game from which it has source.</param>
        /// <param name="playerPerspective">Player, from whose perspective should be image redrawed.</param>
        public void RedrawMap(Game game, Player playerPerspective)
        {
            if (isFogOfWar)
            {
                RedrawWithFogOfWar(game, playerPerspective);
            }
            else
            {
                RedrawWithoutFogOfWar(game);
            }

            OnImageChanged?.Invoke();
        }

        /// <summary>
        /// Redraws game with fog of war considered.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playerPerspective"></param>
        private void RedrawWithFogOfWar(Game game, Player playerPerspective)
        {
            // get owned regions
            IList<Region> ownedRegions = playerPerspective.ControlledRegions;

            // recolor them to players color
            foreach (Region ownedRegion in ownedRegions)
            {
                coloringHandler.Recolor(ownedRegion, playerPerspective.Color);

                textDrawingHandler.DrawArmyNumber(ownedRegion, ownedRegion.Army);
            }

            // recolor neighbour regions local player does not own
            IList<Region> neighbourNotOwnedRegions = (from ownedRegion in ownedRegions
                                                            from neighbour in ownedRegion.NeighbourRegions
                                                            where neighbour.Owner != playerPerspective
                                                            select neighbour).Distinct().ToList();

            foreach (Region region in neighbourNotOwnedRegions)
            {
                Player owner = region.Owner;
                if (owner == null)
                {
                    coloringHandler.Recolor(region, Global.RegionVisibleUnoccupiedColor);
                }
                else
                {
                    Color ownerColor = Color.FromKnownColor(owner.Color);

                    coloringHandler.Recolor(region, ownerColor);
                }
                textDrawingHandler.DrawArmyNumber(region, region.Army);
            }

            var allOtherRegions = game.Map.Regions.Except(neighbourNotOwnedRegions).Except(ownedRegions);
            foreach (var region in allOtherRegions)
            {
                coloringHandler.Recolor(region, Global.RegionNotVisibleColor);
            }
        }

        /// <summary>
        /// Redraws game without fog of war.
        /// </summary>
        /// <param name="game"></param>
        private void RedrawWithoutFogOfWar(Game game)
        {
            var regions = game.Map.Regions;
            foreach (var region in regions)
            {
                if (region.Owner == null)
                {
                    coloringHandler.Recolor(region, Global.RegionVisibleUnoccupiedColor);
                }
                else
                {
                    coloringHandler.Recolor(region, region.Owner.Color);
                }
            }
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

            ColoringHandler coloringHandler = new ColoringHandler(image, mapImageTemplateProcessor);

            TextDrawingHandler textDrawingHandler = new TextDrawingHandler(image, mapImageTemplateProcessor, coloringHandler);

            HighlightHandler highlightHandler = new HighlightHandler(image, mapImageTemplateProcessor, textDrawingHandler, coloringHandler);

            SelectRegionHandler selectRegionHandler = new SelectRegionHandler(mapImageTemplateProcessor, highlightHandler, isFogOfWar);

            return new MapImageProcessor(mapImageTemplateProcessor, image, textDrawingHandler, coloringHandler, selectRegionHandler, isFogOfWar);
        }
    }
}