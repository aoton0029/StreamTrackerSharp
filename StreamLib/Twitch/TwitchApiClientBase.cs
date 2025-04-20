using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    //public abstract class TwitchApiClientBase
    //{
    //    private readonly HttpClient _httpClient;

    //    protected TwitchApiClientBase(HttpClient httpClient)
    //    {
    //        _httpClient = httpClient;
    //        _httpClient.BaseAddress = new Uri("https://api.twitch.tv/helix/");
    //    }

    //    protected async Task<T?> GetAsync<T>(string endpoint, string accessToken, Dictionary<string, string>? queryParams = null)
    //    {
    //        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
    //        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    //        request.Headers.Add("Client-Id", "あなたのクライアントID");

    //        if (queryParams != null)
    //        {
    //            var query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
    //            request.RequestUri = new Uri($"{_httpClient.BaseAddress}{endpoint}?{query}");
    //        }

    //        var response = await _httpClient.SendAsync(request);
    //        response.EnsureSuccessStatusCode();

    //        var json = await response.Content.ReadAsStringAsync();
    //        return JsonSerializer.Deserialize<T>(json);
    //    }
    //}

    //public class TwitchApiClient
    //{
    //    private readonly HttpClient _httpClient;
    //    private readonly TwitchAuthService _authService;

    //    public TwitchApiClient(HttpClient httpClient, TwitchAuthService authService)
    //    {
    //        _httpClient = httpClient;
    //        _authService = authService;
    //    }

    //    public async Task<IReadOnlyList<string>> GetFollowedUserIdsAsync(string userId)
    //    {
    //        var token = await _authService.GetAccessTokenAsync();
    //        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitch.tv/helix/users/follows?from_id={userId}");
    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //        request.Headers.Add("Client-Id", _authService.ClientId);

    //        var response = await _httpClient.SendAsync(request);
    //        response.EnsureSuccessStatusCode();
    //        var json = await response.Content.ReadAsStringAsync();
    //        // JSONからfollowed user IDリスト抽出
    //        return ParseFollowedUsers(json);
    //    }

    //    public async Task<IReadOnlyList<StreamInfo>> GetLiveStreamsAsync(IEnumerable<string> userIds)
    //    {
    //        var token = await _authService.GetAccessTokenAsync();
    //        var userQuery = string.Join("&user_id=", userIds);
    //        var url = $"https://api.twitch.tv/helix/streams?user_id={userQuery}";

    //        var request = new HttpRequestMessage(HttpMethod.Get, url);
    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //        request.Headers.Add("Client-Id", _authService.ClientId);

    //        var response = await _httpClient.SendAsync(request);
    //        response.EnsureSuccessStatusCode();
    //        var json = await response.Content.ReadAsStringAsync();
    //        return ParseStreamInfos(json);
    //    }

    //    private static IReadOnlyList<string> ParseFollowedUsers(string json)
    //    {
    //        using var doc = JsonDocument.Parse(json);
    //        var root = doc.RootElement;
    //        var userIds = new List<string>();

    //        if (root.TryGetProperty("data", out JsonElement dataElement))
    //        {
    //            foreach (var item in dataElement.EnumerateArray())
    //            {
    //                if (item.TryGetProperty("to_id", out var toId))
    //                {
    //                    userIds.Add(toId.GetString()!);
    //                }
    //            }
    //        }

    //        return userIds;
    //    }

    //    private static IReadOnlyList<StreamInfo> ParseStreamInfos(string json)
    //    {
    //        using var doc = JsonDocument.Parse(json);
    //        var root = doc.RootElement;
    //        var streamList = new List<StreamInfo>();

    //        if (root.TryGetProperty("data", out JsonElement dataElement))
    //        {
    //            foreach (var item in dataElement.EnumerateArray())
    //            {
    //                var userId = item.GetProperty("user_id").GetString() ?? "";
    //                var userName = item.GetProperty("user_name").GetString() ?? "";
    //                var title = item.GetProperty("title").GetString() ?? "";
    //                var thumbnailUrl = item.GetProperty("thumbnail_url").GetString() ?? "";
    //                var viewerCount = item.GetProperty("viewer_count").GetInt32();

    //                streamList.Add(new StreamInfo(userId, userName, title, thumbnailUrl, viewerCount));
    //            }
    //        }

    //        return streamList;
    //    }
    //}

}
