using Android.App;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using Com.Citrix.Mvpn.Api;
using Android.Util;
using System.Threading.Tasks;
using System.Net.Http;

namespace MvpnTestAndroidApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private readonly static string TAG = "MVPN-MainActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.WebView);

            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;
            webView.SetWebViewClient(new HybridWebViewClient(this));

            var model = new UrlModel() { Text = "https://citrix.com" };
            var template = new RazorView() { Model = model };
            var page = template.GenerateString();

            webView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);
        }

        private void OnStartTunnel()
        {
            var progressBar = FindViewById<View>(Resource.Id.progressBar);
            progressBar.Visibility = ViewStates.Visible;
            MicroVPNSDK.StartTunnel(this, new Messenger(new XamarinTunnelHandler((ProgressBar)progressBar)));
        }

        private void OnWebView(string urlString)
        {
            var intent = new Intent(this, typeof(WebViewActivity));
            intent.PutExtra("URL", urlString);
            StartActivity(intent);
        }

        private async Task<string> OnHttpClient(string urlString)
        {
            Log.Info(TAG, "Within HttpClient()");

            HttpClient httpClient = new HttpClient(new AndroidMvpnClientHandler());
            string str = await httpClient.GetStringAsync(urlString);

            Log.Info(TAG, "Done Fetching Data!!! Bytes Received: " + str.Length);
            Toast.MakeText(Application.Context, str, ToastLength.Long).Show();

            return str;
        }

        private class HybridWebViewClient : WebViewClient
        {
            MainActivity activity;

            public HybridWebViewClient(MainActivity activity)
            {
                this.activity = activity;
            }

            [System.Obsolete]
            public override bool ShouldOverrideUrlLoading(WebView webView, string url)
            {

                // If the URL is not our own custom scheme, just let the webView load the URL as usual
                var scheme = "hybrid:";

                if (!url.StartsWith(scheme))
                    return false;

                // This handler will treat everything between the protocol and "?"
                // as the method name.  The querystring has all of the parameters.
                var resources = url.Substring(scheme.Length).Split('?');
                var method = resources[0];
                var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);

                if (method == "MvpnTest")
                {
                    var button = parameters["Button"];
                    var urlString = parameters["textbox"];

                    if (button == "Start Tunnel")
                    {
                        activity.OnStartTunnel();
                    }
                    else if (button == "WebView")
                    {
                        activity.OnWebView(urlString);
                    }
                    else if (button == "HttpClient")
                    {
                        _ = activity.OnHttpClient(urlString);
                    }
                }

                return true;
            }
        }
    }
}
