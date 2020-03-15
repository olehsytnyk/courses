using STP.Interfaces;
using STP.Profile.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace STP.Profile.Interfaces.DataAccess
{
    public interface IPositionRepository : IRepository<PositionEntity, long>
    {
        Task<PositionEntity[]> GetAsync(Expression<Func<PositionEntity, bool>> expression, int skip, int take);
    }
}
