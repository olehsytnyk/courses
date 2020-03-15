using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Application {
    public class WatchlistWithMarketsDto : WatchlistBaseDto{
        public IEnumerable<long> MarketsId { get; set; }
    }
}
