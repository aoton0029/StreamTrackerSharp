using System;
using System.Collections.Generic;
using System.Linq;
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

        public TwitchApiService(string accessToken, string clientId)
        {
            _accessToken = accessToken;
            _clientId = clientId;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Add("Client-Id", _clientId);
        }

        public async Task<TwitchUser?> GetCurrentUserAsync()
        {
            var response = await _httpClient.GetAsync("https://api.twitch.tv/helix/users");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var wrapper = JsonSerializer.Deserialize<TwitchUserResponse>(json);
            return wrapper?.Data.FirstOrDefault();
        }

        public async Task<List<TwitchFollowedChannel>> GetFollowedChannelsAsync(string userId)
        {
            var result = new List<TwitchFollowedChannel>();
            string? pagination = null;

            do
            {
                var url = $"https://api.twitch.tv/helix/channels/followed?user_id={userId}";
                if (!string.IsNullOrEmpty(pagination))
                    url += $"&after={pagination}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    break;

                var json = await response.Content.ReadAsStringAsync();
                var wrapper = JsonSerializer.Deserialize<TwitchFollowedResponse>(json);
                if (wrapper?.Data is not null)
                    result.AddRange(wrapper.Data);

                pagination = wrapper?.Pagination?.Cursor;

            } while (!string.IsNullOrEmpty(pagination));

            return result;
        }

        public async Task<List<TwitchStream>> GetLiveStreamsAsync(string userId)
        {
            // 1. フォロー中のチャンネルを取得
            var followedChannels = await GetFollowedChannelsAsync(userId);
            var broadcasterIds = followedChannels.Select(f => f.BroadcasterId).ToList();

            if (broadcasterIds.Count == 0)
                return new List<TwitchStream>();

            var liveStreams = new List<TwitchStream>();

            // Twitch API は一度に最大100ユーザーまでしかクエリできないため分割
            const int batchSize = 100;
            for (int i = 0; i < broadcasterIds.Count; i += batchSize)
            {
                var batch = broadcasterIds.Skip(i).Take(batchSize);
                var query = string.Join("&user_id=", batch);
                var url = $"https://api.twitch.tv/helix/streams?user_id={query}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    continue;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TwitchStreamResponse>(json);
                if (result?.Data != null)
                    liveStreams.AddRange(result.Data);
            }

            return liveStreams;
        }

    }

}
