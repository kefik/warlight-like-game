using System.IO;

namespace GameObjectsLib.Game
{
    public interface ICanSaveGame
    {
        void SaveGame(Game game, Stream stream);
    }

}
