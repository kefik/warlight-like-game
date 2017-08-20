﻿namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Threading.Tasks;
    using GameObjectsLib;
    using GameObjectsLib.NetworkCommObjects;

    public class LastRound : Entity
    {
        [MaxLength(6144)]
        [Required]
        public virtual byte[] SerializedAiTurns { get; set; }

        public async Task<IEnumerable<GameRound>> GetAiTurnsAsync()
        {
            using (var ms = new MemoryStream(SerializedAiTurns))
            {
                return (await SerializationObjectWrapper.DeserializeAsync(ms)).Value as IEnumerable<GameRound>;
            }
        }

        public IEnumerable<GameRound> GetAiTurns()
        {
            using (var ms = new MemoryStream(SerializedAiTurns))
            {
                return SerializationObjectWrapper.Deserialize(ms).Value as IEnumerable<GameRound>;
            }
        }

        public async Task SetAiTurns(IList<GameRound> rounds)
        {
            SerializationObjectWrapper wrapper = new SerializationObjectWrapper<IList<GameRound>>()
            {
                TypedValue = rounds
            };
            using (var ms = new MemoryStream())
            {
                await wrapper.SerializeAsync(ms);

                ms.Position = 0;

                SerializedAiTurns = ms.GetBuffer();
            }
        }

        [Required]
        [StringLength(50)]
        public string RoundStartedDateTime { get; set; }

        public virtual ICollection<LastTurn> LastTurns { get; set; }
    }
}
