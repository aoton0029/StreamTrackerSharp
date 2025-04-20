using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public List<string> Scope { get; set; } = new();

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "bearer";
    }

    public class TwitchStreamResponse
    {
        public List<TwitchStream> Data { get; set; } = new();
    }

    public class TwitchStream
    {
        public string Id { get; set; } = "";
        public string UserId { get; set; } = "";
        public string UserLogin { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Title { get; set; } = "";
        public int ViewerCount { get; set; }
        public string ThumbnailUrl { get; set; } = "";
        public DateTime StartedAt { get; set; }
        public string Language { get; set; } = "";
        public string GameName { get; set; } = "";
    }

    public class TwitchUserResponse
    {
        public List<TwitchUser> Data { get; set; } = new();
    }

    public class TwitchUser
    {
        public string Id { get; set; } = "";
        public string Login { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string ProfileImageUrl { get; set; } = "";
    }

    public class TwitchFollowedResponse
    {
        public List<TwitchFollowedChannel> Data { get; set; } = new();
        public TwitchPagination? Pagination { get; set; }
    }

    public class TwitchFollowedChannel
    {
        public string BroadcasterId { get; set; } = "";
        public string BroadcasterLogin { get; set; } = "";
        public string BroadcasterName { get; set; } = "";
        public DateTime FollowedAt { get; set; }
    }

    public class TwitchPagination
    {
        public string? Cursor { get; set; }
    }

}
