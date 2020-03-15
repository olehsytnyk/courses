using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace STP.Infrastructure
{
    public class ProfileHttpService : HttpService
    {
        public ProfileHttpService(HttpClient httpClient, TokenProvider token)
           : base(httpClient, token)
        {
        }
        public async Task<bool> SubscribeUnrealisedProfitLossAsync(string positionId)
        {
            //RealtimeMarketIdsDto dtoList = new RealtimeMarketIdsDto();
            //dtoList.MarketIds = new List<long>();
            //dtoList.MarketIds.Add(marketId);
            //dtoList.sessionId = sessionId;
            //List<MarketUpdateDto> result = await PostAsync<RealtimeMarketIdsDto, List<MarketUpdateDto>>("api/markets/all", dtoList);
            //if (result == null || result.Count == 0)
            //{
            //    return false;
            //}
            //return true;
            return false;
        }
        public async Task<bool> UnsubscribeUnrealisedProfitLossAsync(string positionId)
        {
            //RealtimeMarketIdsDto dtoList = new RealtimeMarketIdsDto();
            //dtoList.MarketIds = new List<long>();
            //dtoList.MarketIds.Add(marketId);
            //dtoList.sessionId = sessionId;
            //List<MarketUpdateDto> result = await PostAsync<RealtimeMarketIdsDto, List<MarketUpdateDto>>("api/markets/all", dtoList);
            //if (result == null || result.Count == 0)
            //{
            //    return false;
            //}
            //return true;
            return false;
        }
    }
}
