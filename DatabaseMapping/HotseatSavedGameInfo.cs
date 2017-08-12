﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinformsUI
{
    [Table(nameof(HotseatSavedGameInfo) + "s")]
    public class HotseatSavedGameInfo
    {
        [Key]
        public int Id { get; set; }
        public int AINumber { get; set; }
        public int HumanNumber { get; set; }
        public string MapName { get; set; }
        public string SavedGameDate { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return string.Format($"Human: {HumanNumber}, AI: {AINumber}; Map: {MapName}, Saved: {SavedGameDate}");
        }
    }
}
