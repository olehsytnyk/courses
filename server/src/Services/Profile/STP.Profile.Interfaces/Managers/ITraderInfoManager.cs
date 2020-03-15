using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using System.Threading.Tasks;

namespace STP.Profile.Interfaces.Managers
{
    public interface ITraderInfoManager
    {
        Task<TraderInfoEntity[]> GetAllAsync(BaseFilterModel filter);
        Task<TraderInfoEntity[]> GetByFilterAsync(TraderInfoFilterModel filter);
        Task<TraderInfoEntity[]> GetByCopyCountAsync(BaseFilterModel filter);
        Task<TraderInfoEntity> UpdateAsync(TraderInfoEntity entity);
        Task<TraderInfoEntity> CreateAsync(TraderInfoEntity entity);
        Task<TraderInfoEntity> GetByIdAsync(string id);
        Task<bool> DeleteAsync(string id);
    }
}
