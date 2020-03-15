using STP.Interfaces;
using STP.Interfaces.Enums;
using STP.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Profile.Domain.Entities
{
    public class PositionEntity : IEntity<long>, IMessage
    {
        public long Id { get; set; }
        public long MarketId { get; set; }
        public string UserId { get; set; }
        public PositionKind Kind { get; set; }
        public long Quantity { get; set; }
        public double AveragePrice { get; set; }
        public double ProfitLoss { get; set; }
        public double UnrealizedProfitLoss { get; set; }
        public DateTime Timestamp { get; set; }
        public long EntryOrderId { get; set; }
        public long ExitOrderId { get; set; }
    }
}
