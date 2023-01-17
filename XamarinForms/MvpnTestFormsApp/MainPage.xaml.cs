/*
 * Copyright © 2022. Cloud Software Group, Inc. All Rights Reserved. Confidential & Proprietary.
 *
 * Use and support of this software is governed by the terms
 * and conditions of the software license agreement and support
 * policy of Cloud Software Group, Inc. and/or its subsidiaries.
 *
 */

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
        public static Label statLabel;
        
        public MainPage()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                MessagingCenter.Subscribe<Xamarin.Forms.Application>(Xamarin.Forms.Application.Current, "SdksInitializedAndReady", (sender) =>
                {
                    // Only perform tunneled network operations after receiving this callback.
                });
            }
            else
            {
                var mvpnService = DependencyService.Get<IMicroVPNService>();
                mvpnService?.Init();
            }

            InitializeComponent();
            urlEntry.Text = "https://testweb.cemmobile.ctx";

            if (stat == null)
            {
                throw new NullReferenceException("Status label is null");
            }
            //statLabel is created so webview renderer can edit label
            statLabel = stat;
        }

        void OnStartTunnel(object sender, EventArgs args)
        {
            //resets status label
            stat.Text = "Status";
            if (Device.RuntimePlatform == Device.iOS)
            {
                activityIndicator.IsRunning = false;
                return;
            }

            activityIndicator.IsRunning = true;
            var mvpnService = DependencyService.Get<IMicroVPNService>();
            mvpnService?.StartTunnel(this);
        }

        public void OnSuccess()
        {
            activityIndicator.IsRunning = false;
            stat.Text = "Tunnel started";
        }

        public void OnError(StartTunnelError error)
        {
            activityIndicator.IsRunning = false;
            stat.Text = "Error: " + error.ToString();
        }

        void OnStopTunnel(object sender, EventArgs args)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                return;
            }
            if (IsNetworkTunnelOff())
            {
                stat.Text = "Tunnel is not running";
                return;
            }
            //resets status label
            stat.Text = "Status";
            var mvpnService = DependencyService.Get<IMicroVPNService>();
            mvpnService?.StopTunnel();
            stat.Text = "Tunnel stopped";
        }

        void OnCheckTunnel(object sender, EventArgs args)
        {
            //resets status label
            stat.Text = "Status";
            if (Device.RuntimePlatform == Device.iOS)
            {
                return;
            }
            var mvpnService = DependencyService.Get<IMicroVPNService>();
            if (mvpnService != null)
            {
                bool isRunning = mvpnService.IsNetworkTunnelRunning();
                stat.Text = isRunning ? "Tunnel is running" : "Tunnel is not running";
            }
        }

        //returns true when the tunnel is not running or there is some error
        bool IsNetworkTunnelOff()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                return true;
            }
            var mvpnService = DependencyService.Get<IMicroVPNService>();
            if (mvpnService != null)
            {
                return !mvpnService.IsNetworkTunnelRunning();
            }
            return true;
        }

        async void OnWebView(object sender, EventArgs args)
        {
            if (Device.RuntimePlatform == Device.Android && IsNetworkTunnelOff())
            {
                stat.Text = "Tunnel is not running";
                return;
            }
            //resets status label
            stat.Text = "Status";
            activityIndicator.IsRunning = false;
            WebViewPage page = new WebViewPage(urlEntry.Text);
            await Navigation.PushAsync(page);
        }

        async void OnHttpClient(object sender, EventArgs args)
        {
            if (Device.RuntimePlatform == Device.Android && IsNetworkTunnelOff())
            {
                stat.Text = "Tunnel is not running";
                return;
            }
            //resets status label
            stat.Text = "Status";
            activityIndicator.IsRunning = true;
            var mvpnService = DependencyService.Get<IMicroVPNService>();
            if (mvpnService != null)
            {
                HttpClient httpClient = mvpnService.CreateHttpClient();
                try
                {
                    var result = await httpClient.GetStringAsync(urlEntry.Text);
                    await DisplayAlert("HttpClient", result, "OK");
                    if (result.Equals("") || result == null)
                    {
                        stat.Text = "Fetch failure";
                    }
                    else
                    {
                        stat.Text = "Fetch successful";
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.ToString(), "OK");
                }
            }
            activityIndicator.IsRunning = false;
        }
    }
}