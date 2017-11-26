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
        SettingsBot,
        SettingsStartingArmies,
        SettingsStartingRegions,
        SettingsStartingPickAmount,
        UpdateMap,
        OpponentMoves,
        PickStartingRegion,
        PlaceArmiesRequest,
        PlaceArmiesResponse,
        AttackRequest,
        AttackReponse
    }
}