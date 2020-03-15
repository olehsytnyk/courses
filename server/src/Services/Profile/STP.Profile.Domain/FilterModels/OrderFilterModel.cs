using STP.Interfaces.Enums;

namespace STP.Profile.Domain.FilterModels
{
    public class OrderFilterModel : BaseFilterModel
    {
        public long? MarketId { get; set; }
        public string UserId { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public OrderAction? Action { get; set; }
        public long? Timestamp { get; set; }
    }
}
