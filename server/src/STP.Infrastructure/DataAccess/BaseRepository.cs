using Microsoft.EntityFrameworkCore;
using STP.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace STP.Infrastructure.DataAccess
{
    public abstract class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        public abstract IUnitOfWork UnitOfWork { get; protected set; }

        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public BaseRepository(DbContext context) {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public virtual Task<TEntity> FindAsync(TKey key)
        {
            return _entities.AsNoTracking().FirstOrDefaultAsync(item => item.Id.Equals(key));
        }

        public virtual Task<TEntity[]> GetAllAsync()
        {
            return _entities.AsNoTracking().ToArrayAsync();
        }

        public virtual Task<TEntity[]> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _entities.Where(expression).AsNoTracking().ToArrayAsync();
        }

        public virtual async Task<TEntity> InsertAsync(TEntity item)
        {
            var result = await _entities.AddAsync(item);
            return result.Entity;
        }

        public virtual async Task<bool> IsExistAsync(TKey key)
        {
            var result = await _entities.FindAsync(key);
            return result != null;
        }

        public virtual bool Remove(TEntity item)
        {
            var result = _entities.Remove(item);
            return result.State == EntityState.Deleted;
        }

        public virtual bool Remove(TKey key)
        {
            var item = _entities.Find(key);
            if (item == null)
                return false;

            var result = _entities.Remove(item);
            return result.State == EntityState.Deleted;
        }

        public virtual TEntity Update(TEntity item)
        {
            var result = _entities.Update(item);
            return result.Entity;
        }
    }
}
