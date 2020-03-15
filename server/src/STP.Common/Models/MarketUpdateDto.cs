using STP.Interfaces.Messages;

namespace STP.Common.Models
{
    public class MarketUpdateDto : IMessage
    { 
        public long MarketId { get; set; }

        public double AskPrice { get; set; }

        public double BidPrice { get; set; }

        public double Change { get; set; }

        public double PercentChange { get; set; }

        public long Timestamp { get; set; }
    }
}
