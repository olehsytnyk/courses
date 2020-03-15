using Microsoft.EntityFrameworkCore;
using STP.Infrastructure.DataAccess;
using STP.Interfaces;
using STP.Profile.Domain.Entities;
using STP.Profile.Interfaces.DataAccess;
using STP.Profile.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STP.Profile.Infrastructure.DataAccess
{
    public class OrderRepository : BaseRepository<OrderEntity, long>, IOrderRepository
    {
        public OrderRepository(ProfileDbContext context) : base(context)
        {
            UnitOfWork = context;
        }

        public override IUnitOfWork UnitOfWork
        {
            get;
            protected set;
        }

        public Task<OrderEntity[]> GetAsync(Expression<Func<OrderEntity, bool>> expression, int skip, int take)
        {
            return _entities
                .Where(expression)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToArrayAsync();
        }
    }
}
