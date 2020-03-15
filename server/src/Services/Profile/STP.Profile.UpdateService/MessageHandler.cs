using System;
using System.Collections.Generic;
using STP.Interfaces.Messages;
using System.Threading.Tasks;
using STP.Profile.UpdateService.Dto;
using STP.Profile.Domain.DTO.Position;
using STP.Profile.Interfaces;
using STP.Interfaces.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace STP.Profile.UpdateService
{

    public class MessageHandler : IMessageHandler
    {
        private readonly IPositionCache _cache;
        private readonly IMessageBus _messageBus;
        private readonly ILogger<MessageHandler> _logger;
        public int Count = 1;
        public long MarketId;

        internal MessageHandler(long marketId, IPositionCache cache, IMessageBus messageBus, ILogger<MessageHandler> logger)
        {
            _cache = cache;
            _logger = logger;
            _messageBus = messageBus;
            MarketId = marketId;
        }
        public async Task HandleAsync(byte[] message)
        {
            _logger.LogWarning("Received Message via byte[] interface(THIS IS BAD!)!!!");
            try
            {
                string convertedMessage = JsonConvert.SerializeObject(Encoding.UTF8.GetString(message));
                _logger.LogWarning("RabbitMQ message received: {0}" + convertedMessage.Substring(0, 100));
            }
            catch { };
        }
        public async Task HandleAsync(IMessage message)
        {
            _logger.LogInformation("Received rabbitMessage");
            MarketUpdateDtoI marketUpdate;
            try
            {
                marketUpdate = message as MarketUpdateDtoI;
                _cache.MarketPrice.AddOrUpdate(marketUpdate.MarketId, marketUpdate, (key, value) => marketUpdate);
                List<UserUPL> uplList = _cache.UpdatePositionsUPL(marketUpdate);
                if (uplList != null)
                {
                    _logger.LogInformation("_cache.UpdatePositionsUPL(marketUpdate.marketid={0}) returns {1} values", marketUpdate.MarketId, uplList.Count);
                    foreach (UserUPL userUPL in uplList)
                    {
                        Task.Run(() => Publish(userUPL));
                    }
                }
                else
                {
                    _logger.LogWarning("_cache.UpdatePositionsUPL(marketUpdate.marketid ={ 0}) returns NULL", marketUpdate.MarketId);
                }

            }
            catch
            {
                _logger.LogWarning("HandleAsync Error, serialize fail ?");
            }
        }
        private  void Publish(UserUPL userUpl)
        {
            _messageBus.Publish(userUpl.PositionUPL, "exc.Position", RabbitExchangeType.DirectExchange, "UPL:" + userUpl.UserId);
        }
    }
}
