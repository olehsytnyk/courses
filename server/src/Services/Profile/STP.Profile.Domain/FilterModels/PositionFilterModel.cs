namespace STP.Profile.Domain.FilterModels
{
    public class PositionFilterModel : BaseFilterModel
    {
        public long? MarketId { get; set; }
        public string UserId { get; set; }
        public long? Quantity { get; set; }
        public double? AveragePrice { get; set; }
        public double? ProfitLoss { get; set; }
        public double? UnrealizedProfitLoss { get; set; }
        public long? Timestamp { get; set; }
        public long? EntryOrderId { get; set; }
        public long? ExitOrderId { get; set; }
    }
}
