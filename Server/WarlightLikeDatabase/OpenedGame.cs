namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;
    using System.IO;
    using System.Threading.Tasks;
    using GameObjectsLib.Game;
    using GameObjectsLib.NetworkCommObjects;

    public class OpenedGame : GameEntity
    {
        [Required]
        public int OpenedSlotsNumber { get; set; }
        
        [Required]
        [StringLength(50)]
        public string GameCreatedDateTime { get; set; }

        public Game GetGame()
        {
            using (var ms = new MemoryStream(SerializedGame))
            {
                return SerializationObjectWrapper.Deserialize(ms).Value as Game;
            }
        }

        public async Task<Game> GetGameAsync()
        {
            using (var ms = new MemoryStream(SerializedGame))
            {
                return (await SerializationObjectWrapper.DeserializeAsync(ms)).Value as Game;
            }
        }

        public async Task SetGameAsync(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);

                ms.Position = 0;

                SerializedGame = ms.GetBuffer();
            }
        }

        public void SetGame(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                ms.Position = 0;

                SerializedGame = ms.GetBuffer();
            }
        }

        public async Task SetGameAsync(Game game)
        {
            SerializedGame = await game.GetBytesAsync();
        }

        [Required]
        [MaxLength(20480)]
        protected virtual byte[] SerializedGame { get; set; }
    }
}
