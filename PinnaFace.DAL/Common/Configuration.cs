using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL
{
    public class Configuration : DbMigrationsConfiguration<PinnaFaceDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PinnaFaceDbContext context)
        {
            if (Singleton.SeedDefaults)
            {
                var setting = context.Set<UserDTO>().Find(1);
                if (setting == null)
                {
                    context = (PinnaFaceDbContext)DbContextUtil.Seed(context);
                }
            }

            base.Seed(context);
        }
    }

    
}
