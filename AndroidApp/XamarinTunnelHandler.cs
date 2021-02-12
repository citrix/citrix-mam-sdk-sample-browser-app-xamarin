using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Citrix.Mvpn.Api;

namespace MvpnTestAndroidApp
{
    public class XamarinTunnelHandler : MvpnDefaultHandler
    {
        private readonly static string TAG = "MVPN-TunnelHandler";

        private ProgressBar progressBar;

        public XamarinTunnelHandler(ProgressBar progressBar)
        {
            this.progressBar = progressBar;
        }

        public override void HandleMessage(Message msg)
        {
            progressBar.Visibility = ViewStates.Gone;

            ResponseStatusCode responseStatusCode = ResponseStatusCode.FromId(msg.What);
            if (responseStatusCode == ResponseStatusCode.StartTunnelSuccess)
            {
                Log.Info(TAG, "Tunnel started successfully!!!");
                Toast.MakeText(Application.Context, Resource.String.MvpnTunnelStarted, ToastLength.Short).Show();
            }
            else if (responseStatusCode == ResponseStatusCode.TunnelAlreadyRunning)
            {
                Log.Warn(TAG, "Tunnel is already running.");
                Toast.MakeText(Application.Context, Resource.String.MvpnTunnelAlreadyRunning, ToastLength.Short).Show();
            }
            else if (responseStatusCode == ResponseStatusCode.StartTunnelFailed)
            {
                Log.Error(TAG, "Failed to start tunnel!!!");
                Toast.MakeText(Application.Context, Resource.String.MvpnTunnelFailed, ToastLength.Long).Show();
            }
            else if (responseStatusCode == ResponseStatusCode.SessionExpired)
            {
                Log.Info(TAG, "Session Expired!!!");
                Toast.MakeText(Application.Context, Resource.String.MvpnSessionExpired, ToastLength.Short).Show();
            }
            else if (responseStatusCode == ResponseStatusCode.FoundLegacyMode)
            {
                Log.Error(TAG, "Cannot start tunnel for Legacy ManagementMode!!!");
            }
            else if (responseStatusCode == ResponseStatusCode.FoundNonManagedApp)
            {
                Log.Error(TAG, "Could not retrieve policies!!! \n This could be because of the following reasons: \n\t 1. SecureHub is not installed.\n\t 2. SecureHub enrollment is not completed.\n\t 3. App is not managed through CEM.");
                Toast.MakeText(Application.Context, Resource.String.MvpnNonManagedApp, ToastLength.Long).Show();
            }
            else if (responseStatusCode == ResponseStatusCode.FoundNonWebssoMode)
            {
                Log.Error(TAG, "Cannot start tunnel for NetworkAccess mode other than Tunneled - Web SSO!!!");
                Toast.MakeText(Application.Context, Resource.String.MvpnNonWebSsoMode, ToastLength.Long).Show();
            }
            else if (responseStatusCode == ResponseStatusCode.NoNetworkConnection)
            {
                Log.Error(TAG, "Failed to start tunnel. No Network!!!");
                Toast.MakeText(Application.Context, Resource.String.MvpnNoNetworkConnection, ToastLength.Long).Show();
            }
        }
    }
}
