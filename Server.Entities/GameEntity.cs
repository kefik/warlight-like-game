namespace Server.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Threading.Tasks;
    using GameObjectsLib.Game;
    using GameObjectsLib.NetworkCommObjects;

    public abstract class GameEntity : Entity
    {
        [Required]
        [StringLength(50)]
        public string MapName { get; set; }

        [Required]
        public int AiPlayersCount { get; set; }

        [Required]
        public int HumanPlayersCount { get; set; }

        [Required]
        [MaxLength(20480)]
        public virtual byte[] SerializedGame { get; set; }

        public virtual Game GetGame()
        {
            using (MemoryStream ms = new MemoryStream(SerializedGame))
            {
                return SerializationObjectWrapper.Deserialize(ms).Value as Game;
            }
        }

        public virtual async Task<Game> GetGameAsync()
        {
            using (MemoryStream ms = new MemoryStream(SerializedGame))
            {
                return (await SerializationObjectWrapper.DeserializeAsync(ms)).Value as Game;
            }
        }

        public virtual async Task SetGameAsync(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);

                ms.Position = 0;

                SerializedGame = ms.GetBuffer();
            }
        }

        public virtual void SetGame(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                ms.Position = 0;

                SerializedGame = ms.GetBuffer();
            }
        }

        public virtual void SetGame(Game game)
        {
            SerializedGame = game.GetBytes();
        }
    }
}
