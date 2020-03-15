using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Datafeed.Application.Models
{
    public class Market
    {
        public long MarketId { get; set; }

        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public double TickSize { get; set; }

    }
}



