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
        private readonly HighlightHandler highlightRegionHandler;
        
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

        public SelectRegionHandler(Bitmap mapImage, MapImageTemplateProcessor templateProcessor, ColoringHandler coloringHandler,
            TextDrawingHandler textDrawingHandler, HighlightHandler highlightRegionHandler)
        {
            if (templateProcessor == null || coloringHandler == null || textDrawingHandler == null || mapImage == null)
            {
                throw new ArgumentException();
            }

            this.textDrawingHandler = textDrawingHandler;
            this.highlightRegionHandler = highlightRegionHandler;
            this.coloringHandler = coloringHandler;
            this.templateProcessor = templateProcessor;
            this.mapImage = mapImage;
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
            var region = templateProcessor.GetRegion(x, y);

            highlightRegionHandler.HighlightRegion(region, army);

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