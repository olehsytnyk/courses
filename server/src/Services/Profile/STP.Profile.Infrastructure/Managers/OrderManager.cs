using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Interfaces.DataAccess;
using STP.Profile.Interfaces.Managers;
using System.Threading.Tasks;
using STP.Common.Exceptions;
using STP.Interfaces.Enums;
using System;
using STP.Common.Models;
using STP.Interfaces.Messages;
using STP.Profile.UpdateService.Abstract;

namespace STP.Profile.Infrastructure.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderRepository _order;
        private readonly IPositionManager _positionManager;
        private readonly IUpdatesManager _updatesManager;
        private readonly ITraderInfoManager _trader;
        private readonly IMessageBus _messageBus;
        public OrderManager(IOrderRepository order, 
            IPositionManager positionManager, 
            IUpdatesManager updatesManager, 
            ITraderInfoManager trader,
            IMessageBus messageBus)
        {
            _order = order;
            _positionManager = positionManager;
            _updatesManager = updatesManager;
            _trader = trader;
            _messageBus = messageBus;
        }

        public async Task<OrderEntity> FindAsync(long key)
        {
            var entity = await _order.FindAsync(key);
            if (entity == null)
            {
                throw new NotFoundException(ErrorCode.OrderNotFound);
            }
            return entity;
        }

        public async Task<OrderEntity[]> GetAsync(OrderFilterModel filter)
        {
            var entities = await _order.GetAsync(oe =>
                (!filter.MarketId.HasValue || oe.MarketId == filter.MarketId) &&
                (filter.UserId == null || oe.UserId == filter.UserId) &&
                (!filter.Quantity.HasValue || oe.Quantity == filter.Quantity) &&
                (!filter.Price.HasValue || oe.Price == filter.Price) &&
                (!filter.Action.HasValue || oe.Action == filter.Action) &&
                (!filter.Timestamp.HasValue || oe.Timestamp.Ticks == filter.Timestamp),
                filter.Skip, filter.Take
            );

            if (entities.Length == 0)
            {
                throw new NotFoundException(ErrorCode.OrderNotFound);
            }

            return entities;
        }

        public async Task<OrderEntity> InsertAsync(OrderEntity item)
        {
            try
            {
                await _trader.GetByIdAsync(item.UserId);
            }
            catch (NotFoundException ex)
            {
                await _trader.CreateAsync(new TraderInfoEntity() { Id = item.UserId });
            }
            MarketUpdateDto marketPrice =  await _updatesManager.GetLastUpdateAsync(item.MarketId);
            if(marketPrice == null)
            {
                throw new Exception($"Cant get price of Market with id: {item.MarketId}");
            }
            item.Price = item.Action == OrderAction.Buy ? marketPrice.AskPrice : marketPrice.BidPrice;
            item.Timestamp = DateTime.UtcNow;
            var entity = await _order.InsertAsync(item);
            if (entity == null)
            {
                throw new InvalidDataException(ErrorCode.CannotCreateOrder);
            }
            await _order.UnitOfWork.SaveChangesAsync();
            _messageBus.Publish(entity, "exc.Order", RabbitExchangeType.DirectExchange, $"Add:{entity.UserId}");
            await _positionManager.PlaceOrderAsync(entity);
            return entity;
        }
    }
}
