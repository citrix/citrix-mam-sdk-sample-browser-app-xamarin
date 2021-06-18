/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 *
 * Use and support of this software is governed by the terms
 * and conditions of the software license agreement and support
 * policy of Citrix Systems, Inc. and/or its subsidiaries.
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
        public MainPage()
        {
            try
            {
                var mvpnService = DependencyService.Get<IMicroVPNService>();
                mvpnService?.Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            InitializeComponent();
            urlEntry.Text = "https://citrix.com";
        }

        void OnStartTunnel(object sender, EventArgs args)
        {
            try
            {
                activityIndicator.IsRunning = true;
                var mvpnService = DependencyService.Get<IMicroVPNService>();
                mvpnService?.StartTunnel(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void OnSuccess()
        {
            activityIndicator.IsRunning = false;
            DisplayAlert("Success", "Tunnel Started Successfully!!!", "OK");
        }

        public void OnError(StartTunnelError error)
        {
            activityIndicator.IsRunning = false;
            DisplayAlert("Error", error.ToString(), "OK");
        }

        void OnStopTunnel(object sender, EventArgs args)
        {
            try
            {
                var mvpnService = DependencyService.Get<IMicroVPNService>();
                mvpnService?.StopTunnel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void OnCheckTunnel(object sender, EventArgs args)
        {
            try
            {
                var mvpnService = DependencyService.Get<IMicroVPNService>();
                if (mvpnService != null)
                {
                    bool isRunning = mvpnService.IsNetworkTunnelRunning();
                    DisplayAlert("Info", isRunning ? "Tunnel is started" : "Tunnel is stopped", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
                var mvpnService = DependencyService.Get<IMicroVPNService>();
                if (mvpnService != null)
                {
                    HttpClient httpClient = mvpnService.CreateHttpClient();
                    var result = await httpClient.GetStringAsync(urlEntry.Text);
                    await DisplayAlert("HttpClient", result, "OK");
                }
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