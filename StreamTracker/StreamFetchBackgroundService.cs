using StreamLib.Twitch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StreamTracker
{
    public class StreamFetchBackgroundService : IDisposable
    {
        private readonly TwitchApiService _apiService;
        private readonly string _userId;
        private readonly TimeSpan _interval;
        private readonly Action<List<TwitchStream>> _onStreamsUpdated;

        private Timer? _timer;

        public StreamFetchBackgroundService(
            TwitchApiService apiService,
            string userId,
            TimeSpan interval,
            Action<List<TwitchStream>> onStreamsUpdated)
        {
            _apiService = apiService;
            _userId = userId;
            _interval = interval;
            _onStreamsUpdated = onStreamsUpdated;
        }

        public void Start()
        {
            _timer = new Timer(async _ => await FetchStreamsAsync(), null, TimeSpan.Zero, _interval);
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
        }

        private async Task FetchStreamsAsync()
        {
            var streams = await _apiService.GetLiveStreamsAsync(_userId);
            _onStreamsUpdated(streams);
        }

        public void Dispose() => Stop();
    }

}
