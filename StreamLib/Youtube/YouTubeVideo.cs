using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib.Youtube
{
    public class YouTubeVideo
    {
        public string VideoId { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime PublishedAt { get; }
        public string ThumbnailUrl { get; }

        public YouTubeVideo(string videoId, string title, string description, DateTime publishedAt, string thumbnailUrl)
        {
            VideoId = videoId;
            Title = title;
            Description = description;
            PublishedAt = publishedAt;
            ThumbnailUrl = thumbnailUrl;
        }

        public string GetPublishedTime()
        {
            var span = DateTime.UtcNow - PublishedAt;
            return span.TotalHours < 24
                ? $"{(int)span.TotalHours} hours ago"
                : $"{(int)span.TotalDays} days ago";
        }
    }
}
