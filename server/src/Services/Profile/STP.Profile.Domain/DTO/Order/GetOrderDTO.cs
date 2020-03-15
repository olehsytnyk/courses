namespace STP.Profile.Domain.DTO.Order
{
    public class GetOrderDTO : BaseOrderDTO
    {
        public string UserId { get; set; }
        public long Id { get; set; }
        public double Price { get; set; }
        public long Timestamp { get; set; }
    }
}
