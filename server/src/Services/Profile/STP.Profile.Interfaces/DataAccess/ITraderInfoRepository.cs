using STP.Interfaces;
using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using System.Threading.Tasks;

namespace STP.Profile.Interfaces.DataAccess
{
    public interface ITraderInfoRepository : IRepository<TraderInfoEntity, string>
    {
        Task<TraderInfoEntity[]> GetAllAsync(BaseFilterModel filter);
        Task<TraderInfoEntity[]> GetByFilterAsync(TraderInfoFilterModel filter);
        Task<TraderInfoEntity[]> GetByCopyCountAsync(BaseFilterModel filter);
    }
}
