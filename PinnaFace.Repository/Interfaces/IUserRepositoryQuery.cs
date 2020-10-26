using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PinnaFace.Core;

namespace PinnaFace.Repository.Interfaces
{
    public interface IUserRepositoryQuery<TEntity> where TEntity : UserEntityBase
    {
        UserRepositoryQuery<TEntity> Filter(Expression<Func<TEntity, bool>> filter);
        UserRepositoryQuery<TEntity> FilterList(Expression<Func<TEntity, bool>> filter);
        UserRepositoryQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        UserRepositoryQuery<TEntity> Include(params Expression<Func<TEntity, object>>[] expression);

        IEnumerable<TEntity> GetPage(int page, int pageSize, out int totalCount, int? showDeleted = null);
        IQueryable<TEntity> Get(int? showDeleted = null);
        IQueryable<TEntity> GetList(int? showDeleted = null);

        Task<IEnumerable<TEntity>> GetAsync();
        IQueryable<TEntity> SqlQuery(string query, params object[] parameters);
    }
}