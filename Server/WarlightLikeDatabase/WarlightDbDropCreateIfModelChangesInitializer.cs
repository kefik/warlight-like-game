using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.WarlightLikeDatabase
{
    using System.Data.Entity;

    class WarlightDbDropCreateIfModelChangesInitializer : DropCreateDatabaseAlways<WarlightDbContext>
    {
        protected override void Seed(WarlightDbContext context)
        {
            context.Maps.Add(new MapInfo()
            {
                MapInfoId = 1,
                ColorRegionsTemplatePath = "Maps/WorldColorRegionMapping.xml",
                ImageColoredRegionsPath = "Maps/WorldTemplate.png",
                ImagePath = "Maps/World.png",
                TemplatePath = "Maps/World.xml",
                PlayersLimit = 10,
                Name = "World"
            });

            string passwordHash;
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes("1234");
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                passwordHash = System.Text.Encoding.ASCII.GetString(data);
            }
            context.Users.Add(new User()
            {
                Email = "bimbinbiribong@seznam.cz",
                UserId = 1,
                PasswordHash = passwordHash, // TODO
                Login = "Hez"
            });

            context.SaveChanges();
        }
    }
}
