using System;
using Microsoft.Extensions.Logging;
using STP.Interfaces.Messages;
using STP.Infrastructure;
using STP.Profile.UpdateService.Abstract;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using STP.Common.Models;
using STP.Profile.UpdateService.Dto;
using STP.Profile.Interfaces;
using STP.Interfaces.Enums;



namespace STP.Profile.UpdateService
{
    public class UpdatesManager : IUpdatesManager
    {
        private const string _exchangeName = "exc.Datafeed";

        private readonly IMessageBus _messageBus;
        private readonly ILogger<UpdatesManager> _logger;
        private readonly ILogger<MessageHandler> _loggerHandler;
        private readonly IPositionCache _cache;

        private readonly DatafeedHttpService _datafeedHttpService;
        private readonly ConcurrentDictionary<long, MessageHandler> _marketList = new ConcurrentDictionary<long, MessageHandler>();
        private readonly string _sessionId;
        public UpdatesManager(IMessageBus messageBus, ILogger<UpdatesManager> logger, ILogger<MessageHandler> loggerHandler,
            DatafeedHttpService datafeedHttpService, IPositionCache cache)
        {
            _messageBus = messageBus;
            _logger = logger;
            _loggerHandler = loggerHandler;
            _datafeedHttpService = datafeedHttpService;
            _cache = cache;
            _sessionId = Guid.NewGuid().ToString();
            _logger.LogInformation("sessionId {0} , assigned to UpdatesManager", _sessionId);
        }

        public async Task SubscribeAsync(long marketId)
        {
            if (_marketList.TryGetValue(marketId, out MessageHandler messageHandler))
            {
                Interlocked.Increment(ref messageHandler.MarketId);

                //trying add again, to avoid removing instance of another thread;
                if (_marketList.TryAdd(marketId, messageHandler))
                {
                    //was removed by another thread..., resubscribing
                    await SubscribeDatafeedAsync(messageHandler);
                    _logger.LogInformation("Resubscribe to marketId={0} success(other thread removed this market)", marketId);
                }
                else
                {
                    _logger.LogInformation("Subscribe to marketId={0} ignored. Already subscribed(count was>0)", marketId);
                }
            }
            else
            {
                messageHandler = new MessageHandler(marketId, _cache, _messageBus, _loggerHandler);
                if (!_marketList.TryAdd(marketId, messageHandler))
                {
                    //if can't update value, that's means another thread created instance with same market's ID. Let's call function again
                    _logger.LogInformation("Creating new record for to marketId={0} fail. Another thread already created it. Trying resubscribe", marketId);
                    await SubscribeAsync(marketId);
                }
                else
                {//success, 1 user exists now;
                    await SubscribeDatafeedAsync(messageHandler);
                    _logger.LogInformation("Created new record for marketId={0} and subscribed", marketId);
                }
            }
        }
        public async Task UnSubscribeAsync(long marketId)
        {
            if (_marketList.TryGetValue(marketId, out MessageHandler messageHandler))
            {
                Interlocked.Decrement(ref messageHandler.Count);
                if (messageHandler.Count <= 0)
                {
                    if (_marketList.TryRemove(marketId, out MessageHandler count))
                    {
                        await UnsubscribeDatafeed(messageHandler);
                        _logger.LogInformation("Unsubscribe from marketId={0} success. Count=0", marketId);
                    }
                }
                else
                {
                    _logger.LogInformation("Unsubscribe from marketId={0} success. Count>0", marketId);
                }
            }
        }

        public async Task<MarketUpdateDto> GetLastUpdateAsync(long marketId)
        {
            _logger.LogInformation("GetLastUpdate received, marketId={0}", marketId);
            if (_cache.MarketPrice.TryGetValue(marketId, out MarketUpdateDto marketUpdate) == false)
            {
                _logger.LogInformation("GetLastUpdate no information in cache by {0}, trying get by internalHTTP", marketId);
                try
                {
                    marketUpdate = await _datafeedHttpService.GetUpdateMarketAsync(marketId);
                }
                catch
                {
                    _logger.LogInformation("Exception while connecting to datafeedService marketId={0}", marketId);
                }
                if (marketUpdate == null)
                {
                    _logger.LogInformation("MarketUpdate for marketId={0} is NULL", marketId);
                }
            }
            return marketUpdate;
        }
        private async Task SubscribeDatafeedAsync(MessageHandler messageHandler)
        {
            DatafeedAction datafeedAction = new DatafeedAction(messageHandler.MarketId, _sessionId);
            try
            {
                await _datafeedHttpService.SubscribeMarketAsync(datafeedAction);
               // {
                    //_logger.LogInformation("SubscribeDatafeedAsync marketId={0} result=true", messageHandler.MarketId);
              //  }
                //else
              //  {
                   // _logger.LogInformation("SubscribeDatafeedAsync marketId={0} result=false", messageHandler.MarketId);
               // }
            }
            catch(Exception e)
            { }

            _messageBus.Subscribe<MarketUpdateDtoI, MessageHandler>(messageHandler, _exchangeName, RabbitExchangeType.DirectExchange, $"Update:{messageHandler.MarketId}", false);
        }
        private async Task UnsubscribeDatafeed(MessageHandler messageHandler)
        {
            DatafeedAction datafeedAction = new DatafeedAction(messageHandler.MarketId, _sessionId);
            try
            {
                if (await _datafeedHttpService.UnsubscribeMarketAsync(datafeedAction))
                {
                    _logger.LogInformation("UnsubscribeDatafeedAsync marketId={0} result=true", messageHandler.MarketId);
                }
                else
                {
                    _logger.LogInformation("UnsubscribeDatafeedAsync marketId={0} result=false", messageHandler.MarketId);
                }

            }
            catch { }
            _messageBus.Unsubscribe<MarketUpdateDtoI, MessageHandler>(_exchangeName, "Update:" + messageHandler.MarketId.ToString());
        }

    }
}
