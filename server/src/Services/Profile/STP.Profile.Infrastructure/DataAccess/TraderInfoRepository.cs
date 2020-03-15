using Microsoft.EntityFrameworkCore;
using STP.Infrastructure.DataAccess;
using STP.Interfaces;
using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Interfaces.DataAccess;
using STP.Profile.Persistence.Context;
using System.Linq;
using System.Threading.Tasks;

namespace STP.Profile.Infrastructure.DataAccess
{
    public class TraderInfoRepository : BaseRepository<TraderInfoEntity, string>, ITraderInfoRepository
    {
        public TraderInfoRepository(ProfileDbContext context) : base(context)
        {
            UnitOfWork = context;
        }

        public override IUnitOfWork UnitOfWork
        {
            get;
            protected set;
        }

        public Task<TraderInfoEntity[]> GetAllAsync(BaseFilterModel filter)
        {
            return _entities
                .AsNoTracking()
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToArrayAsync();
        }

        public Task<TraderInfoEntity[]> GetByFilterAsync(TraderInfoFilterModel filter)
        {
            return _entities
                .AsNoTracking()
                .Where( ti =>
                    ( ! filter.ProfitLoss.HasValue  || ti.ProfitLoss == filter.ProfitLoss   ) &&
                    ( ! filter.LastChanged.HasValue || ti.LastChanged == filter.LastChanged ) &&
                    ( ! filter.CopyCount.HasValue   || ti.CopyCount == filter.CopyCount     )
                )
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToArrayAsync();
        }

        public Task<TraderInfoEntity[]> GetByCopyCountAsync(BaseFilterModel filter)
        {
            return _entities
                .AsNoTracking()
                .OrderByDescending(ti => ti.CopyCount)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToArrayAsync();
        }
    }
}
