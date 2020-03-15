using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Datafeed.Application.Models
{
    public class MarketUpdate
    {
        public long MarketId { get; set; }

        public double AskPrice { get; set; }

        public double BidPrice { get; set; }

        public double LastPrice { get; set; }

        public double Change { get; set; }

        public double PercentChange { get; set; }

        public double LastDailyPrice { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
