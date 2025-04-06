using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    public class AccessToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }


    public class TwitchChannel
    {
        public string Id { get; }
        public string DisplayName { get; }

        public TwitchChannel(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }

    public class TwitchLiveStream
    {
        public string UserId { get; }
        public string UserName { get; }
        public string Title { get; }
        public int ViewerCount { get; }

        public TwitchLiveStream(string userId, string userName, string title, int viewerCount)
        {
            UserId = userId;
            UserName = userName;
            Title = title;
            ViewerCount = viewerCount;
        }
    }

}
