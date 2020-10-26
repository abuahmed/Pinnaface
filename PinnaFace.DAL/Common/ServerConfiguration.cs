using System.Data.Entity.Migrations;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL
{
    public class ServerConfiguration : DbMigrationsConfiguration<PinnaFaceServerDBContext>
    {
        public ServerConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PinnaFaceServerDBContext context)
        {
            //if (Singleton.SeedDefaults)
            //{
            //    var setting = context.Set<UserDTO>().Find(1);
            //    if (setting == null)
            //    {
            //        context = (PinnaFaceServerDBContext)DbContextUtil.Seed(context);
            //    }
            //}
            base.Seed(context);
        }
    }
}