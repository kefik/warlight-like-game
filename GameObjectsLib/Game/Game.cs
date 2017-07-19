using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using ProtoBuf;

namespace GameObjectsLib.Game
{
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
    [ProtoInclude(600, typeof(SingleplayerGame))]
    public abstract class Game// : ISerializable
    {
        protected Game() { }
        /// <summary>
        /// Represents boolean containing information whether the game has started.
        /// </summary>
        public bool HasStarted { get; protected set; }
        
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
        public abstract void Start();

        public static Game Create(GameType gameType, Map map, ICollection<Player> players)
        {
            switch (gameType)
            {
                case GameType.SinglePlayer:
                {
                        return new SingleplayerGame(map, players);
                }
                case GameType.MultiplayerHotseat:
                    throw new NotImplementedException();
                case GameType.MultiplayerNetwork:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException();
            }
        }
    }

}
