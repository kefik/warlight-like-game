using System.IO;

namespace GameObjectsLib.Game
{
    /// <summary>
    /// Represents a component that is able to save the game, serializing it
    /// into given stream.
    /// </summary>
    public interface IGameSaver
    {
        /// <summary>
        /// Saves the game.
        /// </summary>
        /// <param name="game">Game to be saved (needed for header).</param>
        /// <param name="stream">Stream where the game is represented in serialized form.</param>
        void SaveGame(Game game, Stream stream);
    }

}
