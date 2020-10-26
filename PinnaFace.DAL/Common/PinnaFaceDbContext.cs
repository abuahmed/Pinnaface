using System.Data.Common;
using System.Data.Entity;

namespace PinnaFace.DAL
{
    public class PinnaFaceDbContext : DbContextBase
    {
        public PinnaFaceDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PinnaFaceDbContext, Configuration>());
            Configuration.ProxyCreationEnabled = false;
        }

        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DbContextUtil.OnModelCreating(modelBuilder);
        }
    }
}