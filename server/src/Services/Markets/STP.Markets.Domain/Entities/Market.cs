using STP.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Domain.Entities {
    public class Market : IEntity<long> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public double TickSize { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public MarketKind Kind { get; set; }
        public Currency Currency { get; set; }
        public ICollection<MarketWatchlist> MarketWatchlists { get; set; }
    }
}