using STP.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Domain.Entities {
    public class Watchlist : IEntity<long> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public ICollection<MarketWatchlist> MarketWatchlists { get; set; }
    }
}
