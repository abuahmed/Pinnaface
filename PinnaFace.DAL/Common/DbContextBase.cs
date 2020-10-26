using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading;
using System.Threading.Tasks;
using PinnaFace.Core;
using PinnaFace.Core.Models.Interfaces;
using PinnaFace.DAL.Interfaces;

namespace PinnaFace.DAL
{
    public class DbContextBase : DbContext, IDbContext
    {
        private readonly Guid _instanceId;

        public DbContextBase(string nameOrConnectionString) :
            base(nameOrConnectionString)
        {
            _instanceId = Guid.NewGuid();
            Configuration.LazyLoadingEnabled = false;
        }

        public DbContextBase(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            _instanceId = Guid.NewGuid();
            Configuration.LazyLoadingEnabled = false;
        }

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public override int SaveChanges()
        {
            string userName, agencyName;
            try
            {
                userName = Singleton.User.UserName;
                agencyName = Singleton.Agency.AgencyName;
            }
            catch
            {
                userName = "Default User";
                agencyName = "Default Agency";
            }

            try
            {
                // Your code...

                var changes = base.SaveChanges();
                return changes;
            }
            catch (DbEntityValidationException e)
            {
                var ex = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    ex = "Entity of type \"" + eve.Entry.Entity.GetType().Name +
                         "\" in state \"" + eve.Entry.State + "\" has the following validation errors:";
                    
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        //eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ex = ex + Environment.NewLine+"- Property: \""+ve.PropertyName+"\", Error: \""+ve.ErrorMessage+"\"";
                        
                        //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",ve.PropertyName, ve.ErrorMessage);
                    }

                }
                LogUtil.LogError(ErrorSeverity.Critical, "SaveChanges", ex , userName, agencyName);
                //throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "SaveChanges",
                    ex.Message + Environment.NewLine + ex.InnerException, userName, agencyName);
                //throw;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "SaveChanges",
                    ex.Message + Environment.NewLine + ex.InnerException, userName, agencyName);
                //Console.WriteLine(ex.InnerException + ex.Message);
                //throw;
            }
            return -1;
        }

        //public override int SaveChanges()
        //{
        //    SyncObjectsStatePreCommit();
        //    var changes = base.SaveChanges();
        //    SyncObjectsStatePostCommit();
        //    return changes;
        //}

        public override Task<int> SaveChangesAsync()
        {
            //SyncObjectsStatePreCommit();
            var changesAsync = base.SaveChangesAsync();
            //SyncObjectsStatePostCommit();
            return changesAsync;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            //SyncObjectsStatePreCommit();
            var changesAsync = base.SaveChangesAsync(cancellationToken);
            //SyncObjectsStatePostCommit();
            return changesAsync;
        }

        public void SyncObjectState(object entity)
        {
            Entry(entity).State = StateHelper.ConvertState(((IObjectState)entity).ObjectState);
        }
        private void SyncObjectsStatePreCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
        }

        private void SyncObjectsStatePostCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
                ((IObjectState)dbEntityEntry.Entity).ObjectState = StateHelper.ConvertState(dbEntityEntry.State);
        }
    }
}
