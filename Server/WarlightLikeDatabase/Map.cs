using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.WarlightLikeDatabase
{
    public class Map
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
