namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using GameAi.Data.Restrictions;
    using GameMap;
    using GameRestrictions;
    using Players;

    /// <summary>
    /// Factory for creating game.
    /// </summary>
    public class GameFactory
    {
        /// <summary>
        ///     Creates an instance of new <see cref="Game" />, validates it and returns it.
        /// </summary>
        /// <param name="id">Id of the game corresponding to Id that will be stored in the database.</param>
        /// <param name="gameType">Type of the game.</param>
        /// <param name="map">Map of the game.</param>
        /// <param name="players">Players that will be playing the game.</param>
        /// <param name="fogOfWar">True, if the game will be fog of war.</param>
        /// <param name="restrictions">Restrictions of the game.</param>
        /// <returns>Created instance of the game.</returns>
        public Game CreateGame(int id, GameType gameType, Map map,
            IList<Player> players, bool fogOfWar, GameRestrictions restrictions)
        {
            switch (gameType)
            {
                case GameType.SinglePlayer:
                    return new SingleplayerGame(id, map, players, fogOfWar, restrictions);
                case GameType.MultiplayerHotseat:
                    return new HotseatGame(id, map, players, fogOfWar, restrictions);
                case GameType.MultiplayerNetwork:
                    return new NetworkGame(id, map, players, fogOfWar, restrictions);
                case GameType.Simulator:
                    return new SimulatorGame(id, map, players, fogOfWar, restrictions);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }
    }
}