using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using System.Threading.Tasks;

namespace STP.Profile.Interfaces.Managers
{
    public interface IPositionManager
    {
        Task<PositionEntity> FindAsync(long key);
        Task<PositionEntity[]> GetOpenedAsync(PositionFilterModel filter);
        Task<PositionEntity[]> GetHistoryAsync(PositionFilterModel filter);
        Task<bool> Remove(long key);
        Task PlaceOrderAsync(OrderEntity order);
    }
}