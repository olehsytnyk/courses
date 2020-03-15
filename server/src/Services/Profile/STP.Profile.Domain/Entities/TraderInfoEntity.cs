using STP.Interfaces;
using System;

namespace STP.Profile.Domain.Entities
{
    public class TraderInfoEntity : IEntity<string>
    {
        public string Id { get; set; } //TraderId
        public double ProfitLoss { get; set; } = 0;
        public DateTime LastChanged { get; set; }
        public int CopyCount { get; set; } = 0;
    }
}
