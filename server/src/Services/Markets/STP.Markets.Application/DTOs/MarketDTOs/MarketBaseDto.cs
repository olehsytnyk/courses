using STP.Markets.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Application {
    public class MarketBaseDto {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public MarketKind Kind { get; set; }
        public Currency Currency { get; set; }
    }
}
