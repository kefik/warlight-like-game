using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseMappingLib.UtilsDb
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
        public string ImageTemplatePath { get; set; }
    }
}
