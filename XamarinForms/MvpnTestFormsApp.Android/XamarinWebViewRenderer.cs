using System;
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
        bool disposed;
        FormsWebViewClient client;

        public XamarinWebViewRenderer(Context context) : base(context)
        {
        }

        public override WebViewClient CreateWebViewClient()
        {
            client = new FormsWebViewClient(this);
            return client;
        }

        protected override void Dispose(bool disposing)
        {

            if (disposed)
            {
                Console.WriteLine("Already Disposed.");
                return;
            }

            try
            {
                Console.WriteLine("Disposing objects.");
                base.Dispose(disposing);
                client.Dispose();
                disposed = true;
                Console.WriteLine("Dispose Done.");
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("Dispose Failed:"+ e);
            }
        }
    }
}
