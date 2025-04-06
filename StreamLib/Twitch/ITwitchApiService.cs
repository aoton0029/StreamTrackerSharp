using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib.Twitch
{
    interface ITwitchApiService
    {
        Task<IEnumerable<TwitchStream>> GetLiveStreamsAsync(string userId);
        Task<string> RefreshAccessTokenAsync();
        Task<IEnumerable<TwitchChannel>> GetFollowedChannelsAsync(string userId, string accessToken);
        Task<IEnumerable<TwitchLiveStream>> GetLiveStreamsAsync(IEnumerable<string> userIds, string accessToken);
    }
}
