namespace GameAi.Data
{
    /// <summary>
    /// Represents owner state given region has. It's always measured from perspective of a certain player.
    /// </summary>
    public enum OwnerPerspective : byte
    {
        Unoccupied = 0,
        Enemy = 1,
        Mine = 2
    }
}