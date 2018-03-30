namespace GameAi.Data.GameRecording
{
    using System;
    using System.Collections.Generic;

    public abstract class BotTurn : IEquatable<BotTurn>
    {
        public int PlayerId { get; }

        protected BotTurn(int playerId)
        {
            PlayerId = playerId;
        }

        public virtual bool Equals(BotTurn other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            return PlayerId == other.PlayerId;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }
            if (obj.GetType() == typeof(BotTurn))
            {
                return Equals((BotTurn) obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return 956575109 + PlayerId.GetHashCode();
        }
    }
}