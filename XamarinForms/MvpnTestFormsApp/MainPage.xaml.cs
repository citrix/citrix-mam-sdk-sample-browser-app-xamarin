using System;
using System.ComponentModel;
using System.Net.Http;
using Com.Citrix.Mvpn.Api;
using Xamarin.Forms;

namespace MvpnTestFormsApp
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage, IStartTunnelCallback
    {

        public MainPage()
        {
            InitializeComponent();
            urlEntry.Text = "http://testweb.cemmobile.ctx";
            IMicroVPNService service = DependencyService.Get<IMicroVPNService>();
            service.Init();
        }

        void OnStartTunnel(object sender, EventArgs args)
        {
            activityIndicator.IsRunning = true;
            IMicroVPNService service = DependencyService.Get<IMicroVPNService>();
            service.StartTunnel(this);
        }

        public void OnSuccess()
        {
            activityIndicator.IsRunning = false;
        }

        public void OnError(StartTunnelError error)
        {
            activityIndicator.IsRunning = false;
            DisplayAlert("Error", error.ToString(), "OK");
        }

        void OnStopTunnel(object sender, EventArgs args)
        {
            IMicroVPNService service = DependencyService.Get<IMicroVPNService>();
            service.StopTunnel();
        }

        void OnCheckTunnel(object sender, EventArgs args)
        {
            IMicroVPNService service = DependencyService.Get<IMicroVPNService>();
            bool isRunning = service.IsNetworkTunnelRunning();
            DisplayAlert("Info", isRunning ? "Tunnel is started" : "Tunnel is stopped", "OK");
        }

        async void OnWebView(object sender, EventArgs args)
        {
            activityIndicator.IsRunning = false;
            await Navigation.PushAsync(new WebViewPage(urlEntry.Text));
        }

        async void OnHttpClient(object sender, EventArgs args)
        {
            activityIndicator.IsRunning = true;

            try
            {
                IMicroVPNService service = DependencyService.Get<IMicroVPNService>();
                HttpClient httpClient = service.CreateHttpClient();
                var result = await httpClient.GetStringAsync(urlEntry.Text);
                await DisplayAlert("HttpClient", result, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                activityIndicator.IsRunning = false;

            }
        }
    }
}