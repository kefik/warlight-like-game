using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using ProtoBuf;

namespace GameObjectsLib.Game
{
    interface ISaveable
    {
        void Save(ICanSaveGame canSave);
    }
    /// <summary>
    /// Enum containing types of game any game can have.
    /// </summary>
    public enum GameType
    {
        None,
        SinglePlayer,
        MultiplayerHotseat,
        MultiplayerNetwork
    }

    /// <summary>
    /// Represents one game.
    /// </summary>
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(10, typeof(SingleplayerGame))]
    [ProtoInclude(11, typeof(HotseatGame))]
    [ProtoInclude(12, typeof(NetworkGame))]
    public abstract class Game : ISaveable
    {
        protected Game() { }
        /// <summary>
        /// Represents map being played in this game.
        /// </summary>
        public Map Map { get; }

        /// <summary>
        /// Represents list of players playing this game.
        /// </summary>
        public ICollection<Player> Players { get; }

        /// <summary>
        /// Return game type this game has.
        /// </summary>
        public abstract GameType GameType { get; }

        protected Game(Map map, ICollection<Player> players)
        {
            this.Map = map;
            this.Players = players;
        }


        /// <summary>
        /// Starts the game if theres no error.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        /// Creates an instance of new <see cref="Game"/>, validates it and returns it.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        /// <param name="map">Map of the game.</param>
        /// <param name="players">Players that will be playing the game.</param>
        /// <returns>Created instance of the game.</returns>
        public static Game Create(GameType gameType, Map map, ICollection<Player> players)
        {
            switch (gameType)
            {
                case GameType.SinglePlayer:
                    var sp = new SingleplayerGame(map, players);
                    sp.Validate();
                    return sp;
                case GameType.MultiplayerHotseat:
                    throw new NotImplementedException();
                case GameType.MultiplayerNetwork:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException();
            }
        }
        /// <summary>
        /// Saves the game to the object based on parameter.
        /// </summary>
        /// <param name="canSave">Object which can save the current instance of the game.</param>
        public void Save(ICanSaveGame canSave)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                stream.Position = 0; // reset position to be able to read from it again
                canSave.SaveGame(this, stream);
            }
        }
        /// <summary>
        /// Loads the game based on parameters.
        /// </summary>
        /// <typeparam name="TLoadSource">Source type.</typeparam>
        /// <param name="canLoad">Object which can load the game.</param>
        /// <param name="source">Source from which we can load the game.</param>
        /// <returns>Loaded game.</returns>
        public static Game Load<TLoadSource>(ICanLoadGame<TLoadSource> canLoad, TLoadSource source)
        {
            using (var stream = canLoad.LoadGame(source))
            {
                Game game = Serializer.Deserialize<Game>(stream);
                return game;
            }
        }
    }

}
