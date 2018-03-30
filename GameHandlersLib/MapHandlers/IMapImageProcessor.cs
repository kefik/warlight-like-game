namespace GameHandlersLib.MapHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;

    public interface IMapImageProcessor
    {
        Bitmap MapImage { get; }
        IReadOnlyList<GameObjectsLib.GameMap.Region> SelectedRegions { get; }
        Bitmap TemplateImage { get; }
        event Action OnImageChanged;

        GameObjectsLib.GameMap.Region GetRegion(int x, int y);

        void Attack(int newRegionArmy);
        void Deploy(GameObjectsLib.GameMap.Region region, int newArmy);
        void RedrawMap(Game game, Player playerPerspective);
        void ResetAttackingPhase(Attacking attackingPhase, Deploying deployingPhase);
        void ResetDeployingPhase(Deploying deployingPhase);
        void ResetRound(Turn round);
        int ResetSelection();
        void HighlightUnavailableRegions(
                    IEnumerable<GameObjectsLib.GameMap.Region> unavailableRegions);

        void ResetUnavailableRegionsHighlight();
        void Seize(GameObjectsLib.GameMap.Region region, Player playerPerspective);
        int Select(int x, int y, Player playerPerspective, int army);
    }
}