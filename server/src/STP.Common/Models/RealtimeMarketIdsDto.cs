using System.Collections.Generic;

namespace STP.Common.Models
{
    public sealed class RealtimeMarketIdsDto
    {
        public string sessionId { get; set; }

        public List<long> MarketIds { get; set; }
    }
}
