using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    public class TwitchOAuthService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string _scopes;

        public TwitchOAuthService(string clientId, string clientSecret, string redirectUri, string scopes)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
            _scopes = scopes;
        }

        public string GenerateAuthorizationUrl()
        {
            return $"https://id.twitch.tv/oauth2/authorize?client_id={_clientId}&redirect_uri={_redirectUri}&response_type=code&scope={_scopes}";
        }

        public async Task<string?> ListenForAuthorizationCodeAsync()
        {
            Debug.Print($"ListenForAuthorizationCodeAsync");
            using var listener = new HttpListener();
            listener.Prefixes.Add(_redirectUri + "/");
            listener.Start();

            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];
            Debug.Print($"CODE : {code}");

            using var writer = new StreamWriter(context.Response.OutputStream);
            writer.WriteLine("認証が完了しました。アプリに戻ってください。");
            writer.Flush();

            return code;
        }

        public async Task<TokenResponse?> ExchangeCodeForTokenAsync(string code)
        {
            Debug.Print($"ExchangeCodeForTokenAsync");
            using var client = new HttpClient();

            var values = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", _redirectUri }
            };

            var response = await client.PostAsync("https://id.twitch.tv/oauth2/token", new FormUrlEncodedContent(values));
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(json);
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            using var client = new HttpClient();

            var values = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var response = await client.PostAsync("https://id.twitch.tv/oauth2/token", new FormUrlEncodedContent(values));
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(json);
        }

    }



}
