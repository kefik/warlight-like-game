namespace GameHandlersLib.GameHandlers
{
    public enum GameState : byte
    {
        GameBeginning,
        RoundBeginning,
        Deploying,
        Attacking,
        Committing,
        Committed,
        GameEnd
    }
}