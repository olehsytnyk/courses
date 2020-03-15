using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Domain.Entities {
    public class MarketWatchlist {
        public long MarketId { get; set; }
        public long WatchlistId { get; set; }
        public Market Market { get; set; }
        public Watchlist Watchlist { get; set; }
    }
}
