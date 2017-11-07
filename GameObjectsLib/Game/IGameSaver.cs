namespace GameObjectsLib.Game
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    ///     Represents a component that is able to save the game, serializing it
    ///     into given stream.
    /// </summary>
    public interface IGameSaver<in T>
    {
        /// <summary>
        ///     Saves the game.
        /// </summary>
        /// B
        /// <param name="gameMetaInfo">Game to be saved (needed for header).</param>
        void SaveGame(T gameMetaInfo);
    }

    public interface IGameSaverAsync<in T>
    {
        Task SaveGameAsync(T gameMetaInfo);
    }
}
