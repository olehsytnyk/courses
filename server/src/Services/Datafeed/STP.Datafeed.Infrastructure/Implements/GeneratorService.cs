using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using STP.Common;
using STP.Common.Exceptions;
using STP.Common.Models;
using STP.Datafeed.Application.Abstractions;
using STP.Datafeed.Application.Models;
using STP.Infrastructure;
using STP.Interfaces.Enums;
using STP.Interfaces.Messages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Datafeed.Infrastructure.Implements
{
    public class GeneratorService : IGeneratorService
    {
        private const long defaultFrequency = 500;

        private readonly ILogger<GeneratorService> _logger;
        private readonly Dictionary<long,Generator> _generators = new Dictionary<long, Generator>();
        public Dictionary<long, Market> Markets { get; private set; } 
        public ConcurrentDictionary<long, MarketUpdate> LastMarketUpdates { get; }
        private readonly ConcurrentDictionary<long, HashSet<string>> _subscribers = new ConcurrentDictionary<long, HashSet<string>>();
        private readonly Dictionary<long, Timer> timers = new Dictionary<long, Timer>();
        private readonly MarketHttpService _marketHttpService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;

        private readonly IConfiguration _configuration;

        public GeneratorService(IConfiguration configuration,
            ILogger<GeneratorService> logger, 
            MarketHttpService marketHttpService, 
            IMapper mapper,
            IMessageBus messageBus)
        {
            _configuration = configuration;
            _logger = logger;
            LastMarketUpdates = new ConcurrentDictionary<long, MarketUpdate>();
            Markets = new Dictionary<long, Market>();
            _marketHttpService = marketHttpService;
            _mapper = mapper;
            _messageBus = messageBus;
          
        }
        /// <summary>
        /// Subscribe sessionId to Market Update
        /// </summary>
        public bool Subscribe(long marketId, string sessionId)
        {
            Generator generator;
            List<MarketUpdate> marketUpdates = new List<MarketUpdate>();
            MarketUpdate marketUpdate;

            if (_generators.TryGetValue(marketId, out generator))
            {
                if (_subscribers[marketId].Add(sessionId))
                {
                    if (LastMarketUpdates.TryGetValue(marketId, out marketUpdate))
                    {
                        marketUpdates.Add(marketUpdate);
                    }
                    else return false;
                }
                else return false;
            }
            else return false;


            return true;
        }
        /// <summary>
        /// Unsubscribe sessionId to Market Update
        /// </summary>
        public bool Unsubcribe(long marketId, string sessionId)
        {

            if (_subscribers[marketId].Remove(sessionId))
            {
                _logger.LogInformation($"Session: {sessionId} unsubscribe from: {marketId}");
                return true;
            }
            else return false;


        }
        /// <summary>
        /// Initialize generators
        /// </summary>
        public async Task StartGeneratorAsync()
        {
            long conf = _configuration.GetValue<long>("Generator:Frequency");
            conf = conf <= 0 ? defaultFrequency : conf;
            var temp = await _marketHttpService.GetAllMarketsAsync();
            foreach (var item in temp)
            {
                if (item.MaxPrice < item.MinPrice || item.MinPrice < 0)
                {
                    _logger.LogWarning("MaxPrice must be bigger than MinPrice and MinPrice must be bigger then zero");
                    continue;
                }
                else if (item.TickSize <= 0.0)
                {
                    _logger.LogWarning("TickSize must be bigger then zero.");
                    continue;
                }
                Markets.TryAdd(item.Id, _mapper.Map<Market>(item));
            }

            foreach (var item in Markets.Values)
            {

                Generator generator = new Generator(item);
                generator.NotifyUpdateEvent += NotifyUpdate;
                _generators.Add(item.MarketId, generator);

                TimerCallback tm = new TimerCallback(generator.Generate);
                Timer timer = new Timer(tm, 0, 0, conf);
                timers.Add(item.MarketId, timer);

                _subscribers.TryAdd(item.MarketId, new HashSet<string>());

            }
        }
        /// <summary>
        /// Notify update 
        /// </summary>
        public void NotifyUpdate(object sender, EventArgs<MarketUpdate> marketUpdate)
        {
            LastMarketUpdates.AddOrUpdate(marketUpdate.Value.MarketId, marketUpdate.Value, (key, oldValue) => oldValue = marketUpdate.Value);

            HashSet<string> item;
            if (_subscribers.TryGetValue(marketUpdate.Value.MarketId, out item))
            {
                if (item.Any())
                {
                    
                    MarketUpdateDto marketUpdateDto = _mapper.Map<MarketUpdateDto>(marketUpdate.Value);
                   _messageBus.Publish(marketUpdateDto, "exc.Datafeed", Interfaces.Enums.RabbitExchangeType.DirectExchange, $"Update:{marketUpdate.Value.MarketId}");

                    //_messageBus.Publish(marketUpdateDto, "exc.Datafeed", Interfaces.Enums.RabbitExchangeType.DirectExchange, $"PL:{marketUpdate.Value.MarketId}");
                    Debug.WriteLine($"asdfg:\n " + marketUpdate.Value.AskPrice + " / " + marketUpdate.Value.BidPrice + "\n" +
                                marketUpdate.Value.LastPrice + " / " + marketUpdate.Value.LastDailyPrice + "\n" +
                                marketUpdate.Value.Change + " / " + marketUpdate.Value.PercentChange + "\n" + marketUpdate.Value.MarketId + " / " + marketUpdate.Value.Timestamp + "\n");
                }
            }
        }
    }
}
