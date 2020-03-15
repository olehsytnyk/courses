using STP.Interfaces.Enums;

namespace STP.Profile.Domain.DTO.Order
{
    public class BaseOrderDTO
    {
        public long MarketId { get; set; }
        public long Quantity { get; set; }
        public OrderAction Action { get; set; }
    }
}