namespace STP.Markets.Application
{
    public class MarketFilterDto : MarketBaseDto {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
}