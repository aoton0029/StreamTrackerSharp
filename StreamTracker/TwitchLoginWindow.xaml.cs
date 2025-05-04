using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StreamTracker
{
    /// <summary>
    /// TwitchLoginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TwitchLoginWindow : Window
    {
        private readonly string _authUrl;
        private readonly string _redirectUri;
        private TaskCompletionSource<string> _authCodeTaskSource;

        public TwitchLoginWindow(string authUrl, string redirectUri)
        {
            InitializeComponent();
            _authUrl = authUrl;
            _redirectUri = redirectUri;
            _authCodeTaskSource = new TaskCompletionSource<string>();

            Loaded += TwitchLoginWindow_Loaded;
        }

        private async void TwitchLoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebView();
        }

        private async Task InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async();

            // リダイレクトURLを検出するためのナビゲーションイベントを登録
            webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;

            // 認証URLに移動
            webView.CoreWebView2.Navigate(_authUrl);
        }

        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith(_redirectUri))
            {
                // リダイレクトURLからコードを抽出
                Uri uri = new Uri(e.Uri);
                string code = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("code");

                if (!string.IsNullOrEmpty(code))
                {
                    // ナビゲーションをキャンセルして自前で完了ページを表示
                    e.Cancel = true;
                    webView.CoreWebView2.NavigateToString("<html><body><h1>認証が完了しました</h1><p>このウィンドウは閉じて構いません。</p></body></html>");

                    // 認証コードを取得したので TaskCompletionSource を完了させる
                    _authCodeTaskSource.SetResult(code);

                    // 少し待ってからウィンドウを閉じる
                    Task.Delay(2000).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            this.DialogResult = true;
                            this.Close();
                        });
                    });
                }
            }
        }

        /// <summary>
        /// 認証コードを取得するタスクを返します
        /// </summary>
        public Task<string> GetAuthorizationCodeAsync()
        {
            return _authCodeTaskSource.Task;
        }
    }
}
