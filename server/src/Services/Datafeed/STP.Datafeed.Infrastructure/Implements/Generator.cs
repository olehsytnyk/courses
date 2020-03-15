
using STP.Common;
using STP.Common.Exceptions;
using STP.Datafeed.Application.Models;

using STP.Datafeed.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace STP.Datafeed.Infrastructure.Implements
{
    public class Generator
    {
        public Market Market { get;  }
        private Random _random;
        public MarketUpdate _marketUpdate;
        private readonly int digits; 
        public event EventHandler<EventArgs<MarketUpdate>> NotifyUpdateEvent;
        public const double PriceEpsilon = 1E-10;

        public Generator(Market market)
        {
            Market = market;
            _random = new Random();
            _marketUpdate = new MarketUpdate();
            digits = CalculateDigitsOnTickSize(Market.TickSize);
            _marketUpdate.MarketId = Market.MarketId;
        }
        /// <summary>
        /// Generation new market update
        /// </summary>
        public void Generate(object obj)
        {
            GenerateAskAndBidPrice();
            UpdateLastPriceAndLastDailyPrice();
            UpdateChange();
            UpdatePercentChange();
            UpdateTime();
            NotifyUpdate();

        }
        /// <summary>
        /// Generation Ask and Bid price 
        /// </summary>
        private void GenerateAskAndBidPrice()
        {
                _marketUpdate.AskPrice = Math.Round(_random.NextDouble(Market.MinPrice, Market.MaxPrice), digits);
                _marketUpdate.BidPrice = Math.Round(_random.NextDouble(Market.MinPrice, _marketUpdate.AskPrice), digits);
        }

        /// <summary>
        /// Generation Last and Last Daily price 
        /// </summary>
        private void UpdateLastPriceAndLastDailyPrice()
        {
            _marketUpdate.LastPrice = Math.Round((_marketUpdate.AskPrice + _marketUpdate.BidPrice) / 2, digits);


            if (_marketUpdate.Timestamp.Day != DateTime.UtcNow.Day)
            {
                _marketUpdate.LastDailyPrice = _marketUpdate.LastPrice;
            }
            else if (_marketUpdate.LastDailyPrice == 0)
            {
                _marketUpdate.LastDailyPrice = _marketUpdate.LastPrice;
            }

        }

        /// <summary>
        /// Calculation change
        /// </summary>
        private void UpdateChange()
        {
            _marketUpdate.Change = Math.Round(_marketUpdate.LastPrice - _marketUpdate.LastDailyPrice, digits);
        }
        /// <summary>
        /// Calculation change in percent
        /// </summary>
        private void UpdatePercentChange()
        {
            _marketUpdate.PercentChange = Math.Round(_marketUpdate.Change / _marketUpdate.LastDailyPrice * 100, 2);
        }
        /// <summary>
        /// Update time generated market update
        /// </summary>
        private void UpdateTime()
        {
            _marketUpdate.Timestamp = DateTime.UtcNow;
        }

        /* private double Round(double value)
         {
             decimal valueDec = (decimal)value;
             decimal tickSize = (decimal)Market.TickSize;

             return (double)(valueDec - valueDec % tickSize + ((valueDec % tickSize < tickSize / 2) ? 0M : tickSize));
         }*/

        private static int CalculateDigitsOnTickSize(double tickSize)
        {
            if (tickSize >= (1.0 - Double.Epsilon))
                return 0;

            var digits = 0;
            do
            {
                tickSize *= 10d;
                ++digits;
            } while (Math.Abs(TruncateEx(tickSize) - tickSize) > PriceEpsilon);

            digits = Math.Min(digits, 15);

            return digits;
        }

        private static double TruncateEx(double value)
        {
            var lowerInt = Math.Truncate(value);
            var upperInt = Math.Round(lowerInt + 1.0, MidpointRounding.AwayFromZero);

            return Math.Abs(value - upperInt) < PriceEpsilon ? upperInt : lowerInt;
        }

        private void NotifyUpdate()
        {
            NotifyUpdateEvent?.Invoke(this, new EventArgs<MarketUpdate>(_marketUpdate));
        }

    }
}
