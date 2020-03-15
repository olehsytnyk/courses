using STP.Common.Models;
using STP.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace STP.Infrastructure
{
    public class MarketHttpService : HttpService
    {
        public MarketHttpService(HttpClient httpClient, TokenProvider token)
            : base(httpClient, token)
        { }

        public Task<IEnumerable<MarketInnerDto>> GetAllMarketsAsync()
        {
            return GetAsync<IEnumerable<MarketInnerDto>>("api/markets/all");
        }
    }
}