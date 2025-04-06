using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    public class TwitchStream
    {
        public string Id { get; }
        public string UserId { get; }
        public string UserName { get; }
        public string Title { get; }
        public string GameName { get; }
        public int ViewerCount { get; }
        public DateTime StartedAt { get; }
        public string ThumbnailUrl { get; }

        public TwitchStream(string id, string userId, string userName, string title, string gameName, int viewerCount, DateTime startedAt, string thumbnailUrl)
        {
            Id = id;
            UserId = userId;
            UserName = userName;
            Title = title;
            GameName = gameName;
            ViewerCount = viewerCount;
            StartedAt = startedAt;
            ThumbnailUrl = thumbnailUrl;
        }

        public string GetElapsedTime()
        {
            var span = DateTime.UtcNow - StartedAt;
            return $"{(int)span.TotalMinutes} mins ago";
        }
    }
}
