
using STP.Common.Models;
using STP.Profile.Domain.DTO.Position;
using STP.Profile.Domain.Entities;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace STP.Profile.Interfaces
{
    public interface IPositionCache
    {
        void AddPosition(PositionEntity position);
        void RemovePosition(long positionId, long marketId);
        List<UserUPL> UpdatePositionsUPL(MarketUpdateDto marketUpdate);
        ConcurrentDictionary<long, MarketUpdateDto> MarketPrice { get; }
        PositionEntity TryGetOpenedPosition(long marketId, string UserId);
    }
}
