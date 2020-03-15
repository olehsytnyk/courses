using STP.Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace STP.Infrastructure
{
    public class DatafeedHttpService : HttpService
    {
        public DatafeedHttpService(HttpClient httpClient, TokenProvider token)
           : base(httpClient, token)
        {
        }

        public async Task<bool> SubscribeMarketAsync(DatafeedAction datafeedAction)
        {
            try
            {
                string result = await PostAsync<DatafeedAction, string>("api/market-updates/subscribe", datafeedAction);
                return result == "OK";
            }
            catch(Exception e)
            {
                return false;
            };
        }

        public async Task<bool> UnsubscribeMarketAsync(DatafeedAction datafeedAction)
        {
            try
            {
                string result = await PostAsync<DatafeedAction, string>("api/market-updates/unsubscribe", datafeedAction);
                return result == "OK";
            }
            catch (Exception e)
            {
                return false;
            };
        }
        public async Task<MarketUpdateDto> GetUpdateMarketAsync(long marketId)
        {
            return await GetAsync<MarketUpdateDto>("api/market-updates/" + marketId.ToString());

        }

    }
}
