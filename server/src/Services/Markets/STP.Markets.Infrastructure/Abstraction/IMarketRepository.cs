using STP.Interfaces;
using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Markets.Infrastructure.Abstraction {
    public interface IMarketRepository : IRepository<Market, long> {
        Task<Market[]> GetAsync(Expression<Func<Market, bool>> expression, int skip, int take);
    }
}
