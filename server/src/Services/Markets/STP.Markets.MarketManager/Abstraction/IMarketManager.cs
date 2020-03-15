using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Markets.MarketManagerService {
    public interface IMarketManager {
        Task<Market> GetByIdAsync(long id);
        Task<Market[]> GetMarketsAsync(Expression<Func<Market, bool>> expression, int skip, int take);
        Task<Market[]> GetMarketsAsync(Expression<Func<Market, bool>> expression);
        Task<Stream> GetIconAsync(long id);
    }
}
