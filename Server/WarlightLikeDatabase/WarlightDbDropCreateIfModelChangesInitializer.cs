namespace Server.WarlightLikeDatabase
{
    using System.Data.Entity;
    using System.Security.Cryptography;
    using System.Text;

    internal class WarlightDbDropCreateIfModelChangesInitializer : DropCreateDatabaseAlways<WarlightDbContext>
    {
        protected override void Seed(WarlightDbContext context)
        {
            context.Maps.Add(new MapInfo
            {
                Id = 1,
                ColorRegionsTemplatePath = "Maps/WorldColorRegionMapping.xml",
                ImageColoredRegionsPath = "Maps/WorldTemplate.png",
                ImagePath = "Maps/World.png",
                TemplatePath = "Maps/World.xml",
                PlayersLimit = 10,
                Name = "World"
            });

            string passwordHash;
            {
                byte[] data = Encoding.ASCII.GetBytes("1234");
                data = new SHA256Managed().ComputeHash(data);
                passwordHash = Encoding.ASCII.GetString(data);
            }
            context.Users.Add(new User
            {
                Email = "bimbinbiribong@seznam.cz",
                PasswordHash = passwordHash, // TODO
                Name = "Hez"
            });
            context.Users.Add(new User
            {
                Email = "bim@seznam.cz",
                PasswordHash = passwordHash,
                Name = "Bim"
            });

            context.SaveChanges();
        }
    }
}
