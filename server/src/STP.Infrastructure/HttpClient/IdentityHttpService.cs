using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace STP.Infrastructure
{
    public class IdentityHttpService : HttpService
    {
        public IdentityHttpService(HttpClient httpClient, TokenProvider token)
            : base(httpClient, token)
        { }

        public async Task<bool> IsExistUserAsync(string id)
        {
            try
            {
                string result = await GetAsync<string>($"api/users/{id}/exist");
                return result == "OK";
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
