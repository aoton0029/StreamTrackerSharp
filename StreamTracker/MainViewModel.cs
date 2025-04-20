using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StreamLib.Twitch;
using StreamTracker.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StreamTracker
{
    public class MainViewModel : ObservableObject
    {
        string client_id = "0238mibvr44ru779463nq7wu55rqy1";
        string secret_token = "zj9noonilb8mtun9upfsfkdm7xqmch";
        string user_id = "aoton0029";

        private readonly TwitchOAuthService _oAuthService;
        private TwitchApiService? _apiService;
        private StreamFetchBackgroundService? _backgroundService;

        public ObservableCollection<StreamCardViewModel> LiveStreams { get; } = new();

        public ICommand LoginCommand { get; }

        public MainViewModel()
        {
            _oAuthService = new TwitchOAuthService(
                clientId: client_id,
                clientSecret: secret_token,
                redirectUri: "http://localhost",
                scopes: "user:read:follows");

            LoginCommand = new AsyncRelayCommand(LoginAndStartAsync);
        }

        private async Task LoginAndStartAsync()
        {
            try
            {
                var authUrl = _oAuthService.GenerateAuthorizationUrl();
                Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
                var code = await _oAuthService.ListenForAuthorizationCodeAsync();
                var token = await _oAuthService.ExchangeCodeForTokenAsync(code!);

                // APIサービス初期化
                _apiService = new TwitchApiService(token!.AccessToken, user_id);
                var user = await _apiService.GetCurrentUserAsync();

                // バックグラウンドサービス開始
                _backgroundService = new StreamFetchBackgroundService(
                    _apiService,
                    user!.Id,
                    TimeSpan.FromSeconds(60),
                    OnStreamsUpdated);

                _backgroundService.Start();
            }
            catch (Exception ex)
            {

            }
        }

        private void OnStreamsUpdated(List<TwitchStream> streams)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                LiveStreams.Clear();
                foreach (var stream in streams)
                {
                    LiveStreams.Add(new StreamCardViewModel(stream));
                }
            });
        }
    }


}
