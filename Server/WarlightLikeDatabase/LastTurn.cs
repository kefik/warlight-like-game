namespace Server.WarlightLikeDatabase
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using GameObjectsLib;
    using GameObjectsLib.NetworkCommObjects;

    public class LastTurn : Entity
    {
        [MaxLength(1024)]
        [Required]
        protected byte[] SerializedTurn { get; set; }

        public object GetLastTurn()
        {
            using (var ms = new MemoryStream())
            {
                return SerializationObjectWrapper.Deserialize(ms).Value;
            }
        }

        public int UserId { get; set; }
        public User User { get; set; }

        public int LastRoundId { get; set; }
        public LastRound LastRound { get; set; }
    }
}
