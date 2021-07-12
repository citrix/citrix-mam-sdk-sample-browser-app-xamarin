using Xamarin.Forms;

namespace MvpnTestFormsApp
{
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage(string address)
        {
            InitializeComponent();
            webView.Source = address;
        }
    }
}
