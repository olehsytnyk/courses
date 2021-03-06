﻿using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Datafeed.Domain.DTOs
{
    public class MarketUpdateDto
    {
        public long MarketId { get; set; }

        public double AskPrice { get; set; }

        public double BidPrice { get; set; }

        public double Change { get; set; }

        public double PercentChange { get; set; }

        public long Timestamp { get; set; }
    }
}
