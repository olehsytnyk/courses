using STP.Markets.Domain.Entities;
using STP.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace STP.Markets.Infrastructure.Abstraction {
    public interface IWatchlistRepository : IRepository<Watchlist, long>{
        Task RemoveMarketsAsync(IEnumerable<MarketWatchlist> items);
        Task<Watchlist[]> GetAsync(Expression<Func<Watchlist, bool>> expression, int skip, int take);
    }
}
