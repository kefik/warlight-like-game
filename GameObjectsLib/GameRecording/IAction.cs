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
        /// True, if the action was run successfully (
        /// meaning it really happened, e.g. attack can be canceled
        /// by circumstances).
        /// </summary>
        bool RanSuccessfully { get; }

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

        /// <summary>
        /// Reports whether this action is happening
        /// close to the player (neighbour or his own).
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        bool IsCloseOrRelatedTo(Player player);
    }
}