using System;
using System.Collections;
using System.Data.Entity;
using System.Threading.Tasks;
using PinnaFace.Core;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.Repository
{
    public class UnitOfWorkCommon : IUnitOfWork
    {
        protected IDbContext Context;
        private Hashtable _repositories;
        protected Guid _instanceId;

        //public UnitOfWork(IDbContext dbContext)
        //{
        //    if (dbContext == null)
        //        throw new ArgumentNullException("context");

        //    Context = dbContext;
        //    _instanceId = Guid.NewGuid();
        //}

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        internal DbSet<T> GetDbSet<T>() where T : class
        {
            return Context.Set<T>();
        }

        public int Commit()
        {
            var changes=Context.SaveChanges();
            return changes;
        }
        public async Task<int> CommitAync()
        {
            return await Context.SaveChangesAsync();
        }

        #region Dispose
        private bool _disposed;
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    Context.Dispose();

            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public IRepository<T> Repository<T>() where T : EntityBase
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                        .MakeGenericType(typeof(T)), Context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }
        public IUserRepository<T> UserRepository<T>() where T : UserEntityBase
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(UserRepository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                        .MakeGenericType(typeof(T)), Context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IUserRepository<T>)_repositories[type];
        }
    }
}