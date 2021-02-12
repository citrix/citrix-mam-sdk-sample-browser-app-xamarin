using Android.Content;
using Android.Webkit;
using Com.Citrix.Mvpn.Api;
using MvpnTestFormsApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(XamarinWebViewRenderer))]
namespace MvpnTestFormsApp.Droid
{
    public class XamarinWebViewRenderer : MvpnWebViewRenderer
    {
        public XamarinWebViewRenderer(Context context) : base(context)
        {
        }

        public override WebViewClient CreateWebViewClient()
        {
            return new FormsWebViewClient(this);
        }
    }
}
