using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using STP.Common.Models;

namespace STP.Profile.UpdateService.Abstract
{
    public interface IUpdatesManager
    {
        Task SubscribeAsync(long marketId);
        Task UnSubscribeAsync(long marketId);
        Task<MarketUpdateDto> GetLastUpdateAsync(long marketId);
        
    }
}
