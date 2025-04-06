using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    interface ITwitchOAuthService
    {

    }

    public class TwitchOAuthService : ITwitchOAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public TwitchOAuthService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<AccessToken> ExchangeCodeAsync(string code)
        {
            var clientId = _config["Twitch:ClientId"];
            var clientSecret = _config["Twitch:ClientSecret"];
            var redirectUri = _config["Twitch:RedirectUri"];

            var response = await _httpClient.PostAsync("https://id.twitch.tv/oauth2/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUri
            }));

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AccessToken>(json);
        }
    }

}
