using CommunityToolkit.Mvvm.ComponentModel;
using StreamLib.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace StreamTracker.Items
{
    public class StreamCardViewModel : ObservableObject
    {
        public string Title { get; set; }
        public string UserName { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime StartTime { get; set; }
        public string ServiceType { get; set; } // "Twitch", "YouTube" など

        public string ServiceIconPath =>
            ServiceType switch
            {
                "Twitch" => "/Resources/twitch_icon.png",
                "YouTube" => "/Resources/youtube_icon.png",
                _ => "/Resources/default_icon.png"
            };

        public string ElapsedTime => $"{(DateTime.Now - StartTime):hh\\:mm\\:ss}";
        public string StreamUrl => @$"https://www.twitch.tv/{UserName}";

        public StreamCardViewModel(TwitchStream stream)
        {
            Title = stream.Title;
            UserName = stream.UserName;
            ThumbnailUrl = stream.ThumbnailUrl.Replace("{width}", "320").Replace("{height}", "180");
            StartTime = stream.StartedAt;
            ServiceType = "Twitch";
        }
    }
}
