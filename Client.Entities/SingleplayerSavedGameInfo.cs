﻿namespace Client.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(SingleplayerSavedGameInfo) + "s")]
    public class SingleplayerSavedGameInfo
    {
        [Key]
        public int Id { get; set; }
        public int AiNumber { get; set; }
        public string MapName { get; set; }
        public string SavedGameDate { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return string.Format($"Ai: {AiNumber}; Map: {MapName}, Saved: {SavedGameDate}");
        }
    }
}
