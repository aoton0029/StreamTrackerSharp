using StreamLib.Twitch;

namespace StreamTrackerWinforms
{
    public partial class Form1 : Form
    {
        string client_id = "0238mibvr44ru779463nq7wu55rqy1";
        string secret_token = "zj9noonilb8mtun9upfsfkdm7xqmch";
        string user_id = "aoton0029";

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            try 
            {
                //var httpClient = new HttpClient();
                //var oauthService = new TwitchOAuthService(client_id, secret_token);
                //var twitchApi = new TwitchApiService(httpClient, oauthService, client_id);

                //// アクセストークン取得（最初のログイン）
                //var accessToken = await oauthService.GetAccessTokenAsync();

                //var followedIds = await twitchApi.GetFollowedUserIdsAsync(user_id);

                //var liveStreams = await twitchApi.GetLiveStreamsByUserIdsAsync(followedIds);
                //foreach (var stream in liveStreams)
                //{
                //    Console.WriteLine($"{stream.GetProperty("user_name")}: {stream.GetProperty("title")}");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
