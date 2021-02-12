using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Android.Util;

namespace MvpnTestAndroidApp
{
    [Activity(Label = "WebViewActivity")]
    public class WebViewActivity : Activity
    {
        private static string TAG = "MVPN-WebViewActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.WebView);

            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;
            WebViewClient webViewClient = new WebViewClient();
            webView.SetWebViewClient(webViewClient);
            Com.Citrix.Mvpn.Api.MicroVPNSDK.EnableWebViewObjectForNetworkTunnel(this, webView, webViewClient);
            string url = Intent.GetStringExtra("URL");
            Log.Info(TAG, url);
            webView.LoadUrl(url);
        }
    }
}
