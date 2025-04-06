using StreamLib.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamLib.Youtube
{
    interface IYouTubeOAuthService
    {

    }

    public class YouTubeOAuthService : IYouTubeOAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public YouTubeOAuthService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<AccessToken> ExchangeCodeAsync(string code)
        {
            var clientId = _config["YouTube:ClientId"];
            var clientSecret = _config["YouTube:ClientSecret"];
            var redirectUri = _config["YouTube:RedirectUri"];

            var body = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            };

            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(body));
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AccessToken>(json);
        }
    }

}
