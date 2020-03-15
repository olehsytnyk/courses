using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Application {
    public class MarketInnerDto {
        public long Id { get; set; }
        public double TickSize { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
    }
}
