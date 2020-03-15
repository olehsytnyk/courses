using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Markets.WatchlistManagerService {
    public interface IWatchlistManager {
        Task<Watchlist> GetByIdAsync(long id);
        Task<Watchlist[]> GetWatchlistsAsync(Expression<Func<Watchlist, bool>> expression, int skip, int take);
        Task<Watchlist> CreateAsync(Watchlist item);
        Task<Watchlist> UpdateAsync(Watchlist item);
        Task<bool> DeleteAsync(Watchlist item);
        Task<bool> DeleteAsync(long id);
        Task RemoveMarketsAsync(IEnumerable<MarketWatchlist> items);
    }
}
