namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.Player;
    using Region = GameObjectsLib.GameMap.Region;

    /// <summary>
    /// Handler for all region selections.
    /// </summary>
    internal class SelectRegionHandler
    {
        private readonly MapImageTemplateProcessor templateProcessor;
        private readonly HighlightHandler highlightRegionHandler;
        private readonly bool isFogOfWar;
        
        /// <summary>
        /// List of selected regions.
        /// </summary>
        private readonly List<Region> selectedRegions = new List<Region>();

        /// <summary>
        /// Read-only list of selected regions.
        /// </summary>
        public IReadOnlyList<Region> SelectedRegions
        {
            get { return selectedRegions.AsReadOnly(); }
        }

        /// <summary>
        /// Is region selected?
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool IsSelected(Region region)
        {
            return selectedRegions.Any(x => x == region);
        }

        public SelectRegionHandler(MapImageTemplateProcessor templateProcessor, HighlightHandler highlightRegionHandler, bool isFogOfWar)
        {
            if (templateProcessor == null || highlightRegionHandler == null)
            {
                throw new ArgumentException();
            }
            
            this.highlightRegionHandler = highlightRegionHandler;
            this.isFogOfWar = isFogOfWar;
            this.templateProcessor = templateProcessor;
        }

        /// <summary>
        /// Selects region, highlights it. If incorrect region is selected, all previously selected regions are resetted.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="playerPerspective"></param>
        /// <param name="army">Army size of the (x,y) specified region.</param>
        /// <param name="x"></param>
        /// <returns>How many regions are selected at the moment.</returns>
        public int SelectRegion(int x, int y, Player playerPerspective, int army)
        {
            var region = templateProcessor.GetRegion(x, y);

            // is it fog of war game?
            if (isFogOfWar)
            {
                // owner is player and its my or neighbour region
                if (region.Owner != null && (region.Owner == playerPerspective || region.IsNeighbourOf(playerPerspective)))
                {
                    highlightRegionHandler.HighlightRegion(region, Color.FromKnownColor(region.Owner.Color), army);
                }
                // owner is nobody and its an neighbour
                else if (region.Owner == null && region.IsNeighbourOf(playerPerspective))
                {
                    highlightRegionHandler.HighlightRegion(region, Global.RegionVisibleUnoccupiedColor, army);
                }
                // its not an neighbour
                else
                {
                    highlightRegionHandler.HighlightRegion(region, Global.RegionNotVisibleColor, army);
                }
            }
            else
            {
                if (region.Owner != null)
                {
                    highlightRegionHandler.HighlightRegion(region, Color.FromKnownColor(region.Owner.Color), army);
                }
                else
                {
                    highlightRegionHandler.HighlightRegion(region, Global.RegionVisibleUnoccupiedColor, army);
                }
            }

            selectedRegions.Add(region);

            return highlightRegionHandler.HighlightedRegionsCount;
        }

        /// <summary>
        /// Resets selection and returns number regions hit by this method.
        /// </summary>
        /// <returns></returns>
        public int ResetSelection()
        {
            int resettedSelection = highlightRegionHandler.HighlightedRegionsCount;

            highlightRegionHandler.ResetHighlight();

            selectedRegions.Clear();

            return resettedSelection;
        }
    }
}