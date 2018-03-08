namespace GameObjectsLib.GameRecording
{
    using GameMap;
    using Players;

    /// <summary>
    /// Represents one action in the game.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Player who invoked this action.
        /// </summary>
        Player ActionInvoker { get; }

        /// <summary>
        /// Reports whether this action concerns specified <see cref="Region"/>
        /// in any way.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        bool DoesConcernRegion(Region region);
    }
}