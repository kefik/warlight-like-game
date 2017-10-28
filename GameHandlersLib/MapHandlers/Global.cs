namespace GameHandlersLib.MapHandlers
{
    using System.Drawing;

    /// <summary>
    /// Represents global information available for this library.
    /// </summary>
    internal static class Global
    {
        internal static readonly Color TextPlacementColor = Color.FromArgb(78, 24, 86);
        internal static readonly Color HighlightColor = Color.Gold;
        internal static readonly Color RegionNotVisibleColor = Color.FromArgb(155, 150, 122);
        internal static readonly Color RegionVisibleUnoccupiedColor = Color.White;
        internal static readonly Color TextColor = Color.Black;

        internal const double AttackingUnitWillKillProbability = 0.6;
        internal const double DefendingUnitWillKillProbability = 0.7;
    }
}