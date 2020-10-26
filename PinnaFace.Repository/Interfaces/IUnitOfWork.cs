using System;
using System.Threading.Tasks;
using PinnaFace.Core;

namespace PinnaFace.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        Task<int> CommitAync();
        //void Dispose();        
        void Dispose(bool disposing);
        IRepository<TEntity> Repository<TEntity>() where TEntity : EntityBase;
        IUserRepository<TEntity> UserRepository<TEntity>() where TEntity : UserEntityBase;
    }
}
