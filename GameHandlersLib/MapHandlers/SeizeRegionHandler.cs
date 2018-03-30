namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Collections.Generic;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Component that serves for seizing regions.
    /// </summary>
    internal class SeizeRegionHandler
    {
        private readonly HighlightHandler highlightHandler;
        private readonly bool isFogOfWar;

        public SeizeRegionHandler(HighlightHandler highlightHandler, bool isFogOfWar)
        {
            this.highlightHandler = highlightHandler;
            this.isFogOfWar = isFogOfWar;
        }

        public void HighlightUnavailableOptions(IEnumerable<Region> regionsToChooseOptions)
        {
            foreach (var regionsToChooseOption in regionsToChooseOptions
            )
            {
                highlightHandler.HighlightRegion(
                    regionsToChooseOption,
                    isFogOfWar ? Global.RegionNotVisibleColor : Global.RegionVisibleUnoccupiedColor,
                    isFogOfWar ? (int?)null : regionsToChooseOption.Army);
            }
        }

        public void UnhighlightRegion(Region region)
        {
            highlightHandler.UnhighlightRegion(region);
        }

        public void RefreshHighlight()
        {
            highlightHandler.RefreshHighlighting();
        }

        public void ResetHighlight()
        {
            highlightHandler.ResetHighlight();
        }
    }
}