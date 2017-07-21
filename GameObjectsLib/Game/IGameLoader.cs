using System.IO;

namespace GameObjectsLib.Game
{
    /// <summary>
    /// Represents object that can load the game.
    /// </summary>
    /// <typeparam name="T">Type that this object is able to load.</typeparam>
    public interface IGameLoader<in T>
    {
        /// <summary>
        /// Represents object that can load game based on parameter.
        /// </summary>
        /// <param name="source">Source from which can the game be loaded.</param>
        /// <returns>The serialized game.</returns>
        Stream LoadGame(T source);
    }

}
