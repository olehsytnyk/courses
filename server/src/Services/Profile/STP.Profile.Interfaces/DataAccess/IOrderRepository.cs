using STP.Interfaces;
using STP.Profile.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace STP.Profile.Interfaces.DataAccess
{
    public interface IOrderRepository : IRepository<OrderEntity, long>
    {
        Task<OrderEntity[]> GetAsync(Expression<Func<OrderEntity, bool>> expression, int skip, int take);
    }
}
