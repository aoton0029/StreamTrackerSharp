using StreamLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    public class TwitchApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _clientId;
        private readonly Logger _logger;
        private const string LogCategory = "TwitchAPI";
        private readonly TwitchOAuthService _oAuthService;

        public TwitchApiService(string accessToken, string clientId, TwitchOAuthService oAuthService = null)
        {
            _accessToken = accessToken;
            _clientId = clientId;
            _oAuthService = oAuthService;
            _logger = Logger.Instance;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Add("Client-Id", _clientId);

            _logger.Info($"TwitchApiService initialized with ClientID: {_clientId}", LogCategory);
        }

        public async Task<TwitchUser?> GetCurrentUserAsync()
        {
            _logger.Debug("Getting current user information", LogCategory);
            try
            {
                var response = await _httpClient.GetAsync("https://api.twitch.tv/helix/users");

                // レスポンスをログに記録
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.Debug($"API Response: {(int)response.StatusCode} {response.ReasonPhrase}", LogCategory);
                _logger.Debug($"Response Body: {responseBody}", LogCategory);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error($"Failed to get user info: {response.StatusCode} - {responseBody}", LogCategory);

                    // トークン更新の試行 (OAuthServiceが利用可能な場合)
                    if (response.StatusCode == HttpStatusCode.Unauthorized && _oAuthService != null)
                    {
                        _logger.Info("Token might be expired, attempting to refresh...", LogCategory);
                        return await RefreshTokenAndRetryAsync();
                    }

                    return null;
                }

                var wrapper = JsonSerializer.Deserialize<TwitchUserResponse>(responseBody);
                var user = wrapper?.Data.FirstOrDefault();

                if (user != null)
                {
                    _logger.Info($"Successfully retrieved user: {user.DisplayName} (ID: {user.Id})", LogCategory);
                }
                else
                {
                    _logger.Warning("User information was empty in the response", LogCategory);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, "Error while getting current user", LogCategory);
                return null;
            }
        }

        private async Task<TwitchUser?> RefreshTokenAndRetryAsync()
        {
            // このメソッドは、OAuthServiceがnullでない場合にのみ呼び出される前提
            try
            {
                // 実装されていない場合は、必要に応じて実装してください
                // 例：トークンの更新、新しいトークンでのリクエスト再試行
                return null;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, "Failed to refresh token and retry", LogCategory);
                return null;
            }
        }

        public async Task<List<TwitchFollowedChannel>> GetFollowedChannelsAsync(string userId)
        {
            _logger.Debug($"Getting followed channels for user ID: {userId}", LogCategory);
            var result = new List<TwitchFollowedChannel>();
            string? pagination = null;

            try
            {
                do
                {
                    var url = $"https://api.twitch.tv/helix/channels/followed?user_id={userId}";
                    if (!string.IsNullOrEmpty(pagination))
                        url += $"&after={pagination}";

                    _logger.Debug($"Requesting: {url}", LogCategory);
                    var response = await _httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Error($"Failed to get followed channels: {response.StatusCode}", LogCategory);
                        break;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var wrapper = JsonSerializer.Deserialize<TwitchFollowedResponse>(json);
                    if (wrapper?.Data is not null)
                    {
                        result.AddRange(wrapper.Data);
                        _logger.Debug($"Retrieved {wrapper.Data.Count} followed channels", LogCategory);
                    }

                    pagination = wrapper?.Pagination?.Cursor;

                } while (!string.IsNullOrEmpty(pagination));

                _logger.Info($"Successfully retrieved {result.Count} total followed channels", LogCategory);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, "Error while getting followed channels", LogCategory);
                return result;
            }
        }

        public async Task<List<TwitchStream>> GetLiveStreamsAsync(string userId)
        {
            _logger.Info($"Getting live streams for user ID: {userId}", LogCategory);

            try
            {
                // 1. フォロー中のチャンネルを取得
                var followedChannels = await GetFollowedChannelsAsync(userId);
                var broadcasterIds = followedChannels.Select(f => f.BroadcasterId).ToList();

                if (broadcasterIds.Count == 0)
                {
                    _logger.Info("No followed channels found", LogCategory);
                    return new List<TwitchStream>();
                }

                var liveStreams = new List<TwitchStream>();

                // Twitch API は一度に最大100ユーザーまでしかクエリできないため分割
                const int batchSize = 100;
                for (int i = 0; i < broadcasterIds.Count; i += batchSize)
                {
                    var batch = broadcasterIds.Skip(i).Take(batchSize);
                    var query = string.Join("&user_id=", batch);
                    var url = $"https://api.twitch.tv/helix/streams?user_id={query}";

                    _logger.Debug($"Requesting batch {i / batchSize + 1} of {(broadcasterIds.Count - 1) / batchSize + 1}: {url}", LogCategory);
                    var response = await _httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Error($"Failed to get live streams for batch {i / batchSize + 1}: {response.StatusCode}", LogCategory);
                        continue;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<TwitchStreamResponse>(json);
                    if (result?.Data != null)
                    {
                        liveStreams.AddRange(result.Data);
                        _logger.Debug($"Retrieved {result.Data.Count} live streams in batch {i / batchSize + 1}", LogCategory);
                    }
                }

                _logger.Info($"Successfully retrieved {liveStreams.Count} total live streams", LogCategory);
                return liveStreams;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, "Error while getting live streams", LogCategory);
                return new List<TwitchStream>();
            }
        }
    }

}
