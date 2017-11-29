namespace Communication.Shared
{
    /// <summary>
    /// Enum for all command token types.
    /// </summary>
    public enum CommandTokenType
    {
        SetupSuperRegions,
        SetupRegions,
        SetupNeighbours,
        SetupWastelands,
        /// <summary>
        /// TimeBank that the player has for the game token type.
        /// </summary>
        SettingsTimeBank,
        /// <summary>
        /// Time that will be added to players time bank token type.
        /// </summary>
        SettingsTimePerMove,
        /// <summary>
        /// Round limit that the games has token type.
        /// </summary>
        SettingsMaxRounds,
        /// <summary>
        /// Settings for bot player-id mapping.
        /// </summary>
        SettingsBot,
        SettingsStartingArmies,
        SettingsStartingRegions,
        SettingsStartingPickAmount,
        UpdateMap,
        OpponentMoves,
        PickStartingRegionsRequest,
        PickStartingRegionsResponse,
        PlaceArmiesRequest,
        PlaceArmiesResponse,
        AttackRequest,
        AttackResponse
    }
}