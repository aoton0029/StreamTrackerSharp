using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    public class TwitchApiService : ITwitchApiService
    {
        private readonly HttpClient _httpClient;

        private string client_id = "0238mibvr44ru779463nq7wu55rqy1";
        private string secret_key = "zj9noonilb8mtun9upfsfkdm7xqmch";

        public TwitchApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TwitchStream>> GetLiveStreamsAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitch.tv/helix/streams?user_id={userId}");
            request.Headers.Add("Client-Id", "your-client-id");
            request.Headers.Add("Authorization", "Bearer your-access-token");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TwitchStream[]>(json);
        }

        public async Task<string> RefreshAccessTokenAsync()
        {
            // 実装はトークンエンドポイントに従う
            return "new-access-token";
        }
    }

}
