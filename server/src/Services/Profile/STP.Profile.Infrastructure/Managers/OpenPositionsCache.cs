
using STP.Common.Models;
using STP.Messages;
using STP.Profile.Domain.DTO.Position;
using STP.Profile.Domain.Entities;
using STP.Profile.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace STP.Profile.Infrastructure.Managers
{
    public class OpenPositionsCache : IPositionCache
    {
        private readonly ConcurrentDictionary<long, List<PositionEntity>> _marketsPositions;
        public ConcurrentDictionary<long, MarketUpdateDto> MarketPrice { get; private set; }

        private object list_root = new object();
        public OpenPositionsCache()
        {
            _marketsPositions = new ConcurrentDictionary<long, List<PositionEntity>>();
            MarketPrice = new ConcurrentDictionary<long, MarketUpdateDto>();
        }
        
        public void AddPosition(PositionEntity position)
        {
            _marketsPositions.AddOrUpdate(position.MarketId, new List<PositionEntity> { position }, (k, existingList) =>
            {
                lock (existingList)
                {
                    existingList.Add(position);
                }
                return existingList;
            });
        }

       
        public void RemovePosition(long positionId, long marketId)
        {
            if (_marketsPositions.TryGetValue(marketId, out List<PositionEntity> positions))
            {
                lock (positions)
                {
                    positions.RemoveAll(p => p.Id.Equals(positionId));
                }
            }      
        }

        public List<UserUPL> UpdatePositionsUPL(MarketUpdateDto marketUpdate)
        {
            UpdateMarketPrice(marketUpdate);
            var usersUPLs = new List<UserUPL>();
            if (_marketsPositions.TryGetValue(marketUpdate.MarketId, out List<PositionEntity> marketPositions))
            {
                RecalculateMultiplePositionsUPL(marketPositions, marketUpdate);
                CollectUpdatedUPL(marketPositions, usersUPLs);
            }
            return usersUPLs;
        }


        private void UpdateMarketPrice(MarketUpdateDto marketUpdate)
        {
            MarketPrice.AddOrUpdate(marketUpdate.MarketId, marketUpdate, (k, v) => v = marketUpdate);
        }
        

        private void CollectUpdatedUPL(List<PositionEntity> positions, List<UserUPL> usersUPLs)
        {
            lock (list_root)
            {
                foreach (var position in positions)
                {
                    usersUPLs.Add(new UserUPL
                    {
                        UserId = position.UserId,
                        PositionUPL = new UPLMessage
                        {
                            PositionId = position.Id,
                            UnrealizedProfitLoss = position.UnrealizedProfitLoss
                        }
                    });
                }
            }
        }
        

        private void RecalculateMultiplePositionsUPL(List<PositionEntity> positions, MarketUpdateDto marketUpdate)
        {
            lock (list_root)
            {
                foreach (var position in positions)
                {
                    RecalculateUPL(position, marketUpdate);
                }
            } 
        }

        private void RecalculateUPL(PositionEntity position, MarketUpdateDto marketUpdate)
        {
            if (position.Kind == STP.Interfaces.Enums.PositionKind.Short)
            {
                position.UnrealizedProfitLoss = (position.AveragePrice - marketUpdate.AskPrice) * position.Quantity;
            }
            if (position.Kind == STP.Interfaces.Enums.PositionKind.Long)
            {
                position.UnrealizedProfitLoss = (marketUpdate.BidPrice - position.AveragePrice) * position.Quantity;
            }
        }
        public PositionEntity TryGetOpenedPosition(long marketId, string UserId)
        {
            if (_marketsPositions.TryGetValue(marketId, out List<PositionEntity> marketPositions))
            {
                lock(list_root)
                {
                    return marketPositions.FirstOrDefault(p=>p.UserId == UserId);
                }
            }
            return null;
        }
    }
    
}
