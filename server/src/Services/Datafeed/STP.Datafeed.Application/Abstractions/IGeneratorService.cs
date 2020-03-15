using STP.Datafeed.Application.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace STP.Datafeed.Application.Abstractions
{
    public interface IGeneratorService
    {
        Dictionary<long, Market> Markets { get; }

        ConcurrentDictionary<long, MarketUpdate> LastMarketUpdates { get; }

        bool Subscribe(long marketId, string sessionId);

        bool Unsubcribe(long marketId, string sessionId);

        Task StartGeneratorAsync();
    }
}

