using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using STP.Infrastructure.DataAccess;
using STP.Interfaces;
using STP.Markets.Domain.Entities;
using STP.Markets.Infrastructure.Abstraction;
using STP.Markets.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Markets.Infrastructure.Repository {
    public class WatchlistRepository : BaseRepository<Watchlist, long>, IWatchlistRepository {

        public WatchlistRepository(MarketsDbContext context) : base(context) {
            UnitOfWork = context;
        }

        public override IUnitOfWork UnitOfWork { get; protected set; }

        public override Task<Watchlist[]> GetAsync(Expression<Func<Watchlist, bool>> expression) {
            var items = _entities.AsNoTracking()
                .Where(expression)
                .Include(w => w.MarketWatchlists).ToArrayAsync();
            return items;
        }

        public override Task<Watchlist> FindAsync(long key) {
            return _entities.AsNoTracking().Include(w => w.MarketWatchlists).FirstOrDefaultAsync(w => w.Id == key);
        }

        public override Task<Watchlist[]> GetAllAsync() {
            return _entities.AsNoTracking().Include(w => w.MarketWatchlists).ToArrayAsync();
        }

        public async Task RemoveMarketsAsync(IEnumerable<MarketWatchlist> items) {
            _context.Set<MarketWatchlist>().RemoveRange(items);
        }

        public Task<Watchlist[]> GetAsync(Expression<Func<Watchlist, bool>> expression, int skip, int take) {
            var items = _entities.AsNoTracking()
                .Where(expression)
                .OrderBy(w => w.Id)
                .Skip(skip)
                .Take(take)
                .Include(w => w.MarketWatchlists).ToArrayAsync();
            return items;
        }
    }
}
