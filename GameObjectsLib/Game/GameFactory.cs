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
        /// <param name="gameType">Type of the game.</param>
        /// <param name="map">Map of the game.</param>
        /// <param name="players">Players that will be playing the game.</param>
        /// <param name="fogOfWar">True, if the game will be fog of war.</param>
        /// <param name="objectsRestrictions">Restrictions of the game.</param>
        /// <returns>Created instance of the game.</returns>
        public Game CreateGame(GameType gameType, Map map,
            IList<Player> players, bool fogOfWar, GameObjectsRestrictions objectsRestrictions)
        {
            switch (gameType)
            {
                case GameType.SinglePlayer:
                    return new SingleplayerGame(map, players, fogOfWar, objectsRestrictions);
                case GameType.MultiplayerHotseat:
                    return new HotseatGame(map, players, fogOfWar, objectsRestrictions);
                case GameType.MultiplayerNetwork:
                    return new NetworkGame(map, players, fogOfWar, objectsRestrictions);
                case GameType.Simulator:
                    return new SimulatorGame(map, players, fogOfWar, objectsRestrictions);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }
    }
}