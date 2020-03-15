using STP.Interfaces;
using STP.Interfaces.Enums;
using STP.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Profile.Domain.Entities
{
    public class OrderEntity : IEntity<long>, IMessage
    {
        public long Id { get; set; }
        public long MarketId { get; set; }
        public string UserId { get; set; }
        public long Quantity { get; set; }
        public double Price { get; set; }
        public OrderAction Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
