using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using STP.Infrastructure.DataAccess;
using STP.Interfaces;
using STP.Markets.Infrastructure.Abstraction;
using STP.Markets.Persistance.Context;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace STP.Markets.Infrastructure.Repository {
    public sealed class MarketRepository : BaseRepository<Market, long>, IMarketRepository {
        public MarketRepository (MarketsDbContext context) : base(context) {
            UnitOfWork = context;
        }

        public override IUnitOfWork UnitOfWork {
            get;
            protected set;
        }

        public Task<Market[]> GetAsync(Expression<Func<Market, bool>> expression, int skip, int take) {
            var items = _entities.AsNoTracking()
                .Where(expression)
                .OrderBy(w => w.Id)
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();
            return items;
        }
    }
}
