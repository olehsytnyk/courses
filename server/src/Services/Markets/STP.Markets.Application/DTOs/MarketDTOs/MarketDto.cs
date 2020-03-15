using STP.Markets.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Application {
    public class MarketDto : MarketBaseDto {
        public long Id { get; set; }
        public double TickSize { get; set; }
    }
}
