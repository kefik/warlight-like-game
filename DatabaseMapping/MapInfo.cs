using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinformsUI
{
    [Table("MapInfos")]
    public class MapInfo
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int PlayersLimit { get; set; }
        public string TemplatePath { get; set; }
        public string ImagePath { get; set; }
        public string ImageColoredRegionsPath { get; set; }
        public string ColorRegionsTemplatePath { get; set; }
    }
}
