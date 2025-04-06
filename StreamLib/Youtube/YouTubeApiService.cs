using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamLib.Youtube
{
    public class YouTubeApiService : IYouTubeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public YouTubeApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["YouTube:ApiKey"];
        }

        public async Task<IEnumerable<YouTubeVideo>> GetVideosAsync(string channelId)
        {
            var url = $"https://www.googleapis.com/youtube/v3/search?channelId={channelId}&key={_apiKey}&part=snippet&type=video";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<YouTubeVideo[]>(json);
        }
    }

}
