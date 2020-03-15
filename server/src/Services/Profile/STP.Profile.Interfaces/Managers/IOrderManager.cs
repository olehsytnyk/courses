using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using System.Threading.Tasks;

namespace STP.Profile.Interfaces.Managers
{
    public interface IOrderManager
    {
        Task<OrderEntity> FindAsync(long key);
        Task<OrderEntity[]> GetAsync(OrderFilterModel filter);
        Task<OrderEntity> InsertAsync(OrderEntity item);
    }
}