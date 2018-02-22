namespace GameAi.GameRecording
{
    public abstract class BotTurn
    {
        public int PlayerId { get; }

        protected BotTurn(int playerId)
        {
            PlayerId = playerId;
        }
    }
}