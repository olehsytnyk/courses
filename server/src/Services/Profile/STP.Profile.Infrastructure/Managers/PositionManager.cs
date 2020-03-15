using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Interfaces.DataAccess;
using STP.Profile.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using STP.Common.Extensions;
using STP.Common.Exceptions;
using STP.Interfaces.Enums;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using STP.Interfaces.Messages;
using STP.Profile.UpdateService.Abstract;
using STP.Profile.Interfaces;

namespace STP.Profile.Infrastructure.Managers
{
    public class PositionManager : IPositionManager
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IMessageBus _messageBus;
        private readonly IPositionCache _positionCache;
        private readonly IUpdatesManager _updatesManager;
        private readonly ITraderInfoManager _traderInfoManager;
        public PositionManager(IPositionRepository positionRepository, 
            IMessageBus messageBus, 
            IPositionCache positionCache, 
            IUpdatesManager updatesManager,
            ITraderInfoManager traderInfoManager)
        {
            _positionRepository = positionRepository;
            _messageBus = messageBus;
            _positionCache = positionCache;
            _updatesManager = updatesManager;
            _traderInfoManager = traderInfoManager;
        }

        public async Task<PositionEntity> FindAsync(long key)
        {
            var entity = await _positionRepository.FindAsync(key);
            if (entity == null)
            {
                throw new NotFoundException(ErrorCode.PositionNotFound);
            }
            return entity;
        }

        public async Task<PositionEntity[]> GetOpenedAsync(PositionFilterModel filter)
        {
            var entities = await _positionRepository.GetAsync(o =>
                (!filter.MarketId.HasValue || o.MarketId == filter.MarketId) &&
                (filter.UserId == null || o.UserId == filter.UserId) &&
                (!filter.Quantity.HasValue || o.Quantity == filter.Quantity) &&
                (!filter.AveragePrice.HasValue || o.AveragePrice == filter.AveragePrice) &&
                (!filter.ProfitLoss.HasValue || o.ProfitLoss == filter.ProfitLoss) &&
                (!filter.Timestamp.HasValue || o.Timestamp.Ticks == filter.Timestamp) &&
                    o.Kind.IsOpened(),
                    filter.Skip, filter.Take
                );

            if (entities.Length == 0)
            {
                throw new NotFoundException(ErrorCode.PositionNotFound);
            }

            return entities;
        }


        public async Task<PositionEntity[]> GetHistoryAsync(PositionFilterModel filter)
        {
            var entities = await _positionRepository.GetAsync(o =>
                (!filter.MarketId.HasValue || o.MarketId == filter.MarketId) &&
                (filter.UserId == null || o.UserId == filter.UserId) &&
                (!filter.Quantity.HasValue || o.Quantity == filter.Quantity) &&
                (!filter.AveragePrice.HasValue || o.AveragePrice == filter.AveragePrice) &&
                (!filter.ProfitLoss.HasValue || o.ProfitLoss == filter.ProfitLoss) &&
                (!filter.Timestamp.HasValue || o.Timestamp.Ticks == filter.Timestamp) &&
                  !o.Kind.IsOpened(),
                    filter.Skip, filter.Take
                );

            if (entities.Length == 0)
            {
                throw new NotFoundException(ErrorCode.PositionNotFound);
            }

            return entities;
        }

        public async Task<bool> Remove(long key)
        {
            var result = _positionRepository.Remove(key);
            if (!result)
            {
                throw new NotFoundException(ErrorCode.PositionNotFound);
            }
            await _positionRepository.UnitOfWork.SaveChangesAsync();
            return result;
        }
        public async Task PlaceOrderAsync(OrderEntity order)
        {
            PositionEntity position =  _positionCache.TryGetOpenedPosition(order.MarketId, order.UserId);
            if (position == null)
            {
                await CreatePositionAsync(order);
            }
            else
            {
                long remainder = 0;
                if (order.Quantity > position.Quantity)
                {
                    remainder = order.Quantity - position.Quantity;
                    order.Quantity -= remainder;
                }

                if (position.Kind == PositionKind.Long)
                {
                    if (order.Action == OrderAction.Buy)
                    {
                        await IncreaseAsync(position, order);
                    }
                    else // Sell
                    {
                        await DecreaseAsync(position, order);
                    }
                }
                else // Short Position
                {
                    if (order.Action == OrderAction.Buy)
                    {
                        await DecreaseAsync(position, order);
                    }
                    else // sell
                    {
                        await IncreaseAsync(position, order);
                    }
                }
                _positionRepository.Update(position);
                await _positionRepository.UnitOfWork.SaveChangesAsync();
                _messageBus.Publish(position, "exc.Position", RabbitExchangeType.DirectExchange, $"Update:{position.UserId}");
                TraderInfoEntity trader = await _traderInfoManager.GetByIdAsync(position.UserId);
                trader.ProfitLoss += position.ProfitLoss;
                await _traderInfoManager.UpdateAsync(trader);
                if (remainder > 0)
                {
                    await CreatePositionAsync(new OrderEntity()
                    {
                        Action = order.Action,
                        MarketId = order.MarketId,
                        Price = order.Price,
                        Quantity = remainder,
                        Timestamp = DateTime.UtcNow,
                        UserId = order.UserId
                    });
                }
            }
        }
        private async Task CreatePositionAsync(OrderEntity order)
        {
            PositionEntity newPosition = new PositionEntity()
            {
                MarketId = order.MarketId,
                UserId = order.UserId,
                Quantity = order.Quantity,
                Kind = order.Action == OrderAction.Buy ? PositionKind.Long : PositionKind.Short,
                AveragePrice = order.Price,
                Timestamp = DateTime.UtcNow,
                EntryOrderId = order.Id
            };
            await _positionRepository.InsertAsync(newPosition);
            await _positionRepository.UnitOfWork.SaveChangesAsync();
            _positionCache.AddPosition(newPosition);
            await _updatesManager.SubscribeAsync(newPosition.MarketId);
            _messageBus.Publish(newPosition,"exc.Position",RabbitExchangeType.DirectExchange,$"Update:{newPosition.UserId}");
        }
        private async Task IncreaseAsync(PositionEntity position, OrderEntity order)
        {
            position.Quantity += order.Quantity;
            position.AveragePrice = (position.AveragePrice + order.Price) / 2;
        }
        public async Task DecreaseAsync(PositionEntity position, OrderEntity order)
        {
            position.Quantity -= order.Quantity;
            position.AveragePrice = (position.AveragePrice + order.Price) / 2;
            position.ProfitLoss += GetProfitLoss(position, order);
            
            if (position.Quantity == 0)
            {
                position.Kind = PositionKind.Flat;
                _positionCache.RemovePosition(position.Id,position.MarketId);
                await _updatesManager.UnSubscribeAsync(position.MarketId);
            }
        }
        public double GetProfitLoss(PositionEntity position, OrderEntity order)
        {
            if (position.Kind == PositionKind.Long)
            {
                return (order.Price - position.AveragePrice) * 1 * order.Quantity;
            }
            else
            {
                return (order.Price - position.AveragePrice) * -1 * order.Quantity;
            }
        }
        public async Task PutOpenPositionInCache()
        {
            PositionEntity[] opened =  await _positionRepository.GetAsync(p=>p.Kind.IsOpened());
            foreach (var position in opened)
            {
                   _positionCache.AddPosition(position);
                   await  _updatesManager.SubscribeAsync(position.MarketId);
            }
        }
    }
}