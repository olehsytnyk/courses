namespace STP.Profile.Domain.DTO.TraderInfo
{
    public class GetTraderInfoDTO : BaseTraderInfoDTO
    {
        public string TraderId { get; set; }
        public long LastChanged { get; set; }
    }
}
