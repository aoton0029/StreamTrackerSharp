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
                // 認証URLを生成
                var authUrl = _oAuthService.GenerateAuthorizationUrl();

                // WebView2ウィンドウを表示して認証
                var loginWindow = new TwitchLoginWindow(authUrl, "http://localhost");
                var windowResult = loginWindow.ShowDialog();

                string code = null;
                if (windowResult == true)
                {
                    code = await loginWindow.GetAuthorizationCodeAsync();
                }

                if (string.IsNullOrEmpty(code))
                {
                    MessageBox.Show("認証が取り消されました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var token = await _oAuthService.ExchangeCodeForTokenAsync(code);
                if (token == null)
                {
                    MessageBox.Show("トークンの取得に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // APIサービス初期化
                _apiService = new TwitchApiService(token.AccessToken, user_id, _oAuthService);
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
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
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
