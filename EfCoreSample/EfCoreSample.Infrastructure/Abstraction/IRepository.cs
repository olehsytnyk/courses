using EfCoreSample.Doman.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EfCoreSample.Infrastructure.Abstraction
{
    public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        Task<TEntity> FindAsync(TKey key);
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> InsertAsync(TEntity item);
        Task<bool> IsExistAsync(TKey key);
        void UpdateRange(IEnumerable<TEntity> items);
        TEntity Update(TEntity item);
        bool Remove(TEntity item);
        bool Remove(TKey key);
    }
}
