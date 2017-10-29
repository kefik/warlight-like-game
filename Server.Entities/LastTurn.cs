﻿namespace Server.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using GameObjectsLib.NetworkCommObjects;

    public class LastTurn : Entity
    {
        [MaxLength(1024)]
        [Required]
        public byte[] SerializedTurn { get; set; }

        public object GetLastTurn()
        {
            using (MemoryStream ms = new MemoryStream())
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
