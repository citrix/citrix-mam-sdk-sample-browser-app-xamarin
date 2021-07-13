using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Com.Citrix.MamCore.Api;
using System.Threading.Tasks;

namespace MvpnTestFormsApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override async void OnStart()
        {
            // Handle when your app starts

            var coreService = DependencyService.Get<IMamCoreService>();

            if (coreService == null || Device.RuntimePlatform == Device.Android)
            {
                Console.Error.WriteLine("Could not get MamCoreService dependency service for this platform - {0}", Device.RuntimePlatform);
            }
            else
            {
                try
                {
                    coreService.CtxMamLog_Initialize();
                    coreService.CtxMamLog_InfoFrom("XAMARIN Forms", FileName(), MemberName(), LineNumber(), "Initializing MAM SDKs");

                    await coreService.InitializeSdks().ConfigureAwait(false);

                    coreService.CtxMamLog_InfoFrom("XAMARIN Forms", FileName(), MemberName(), LineNumber(), "MAM SDK initialization succeeded");
                }
                catch (CtxMamErrorException ex)
                {
                    // If MAM SDK initialization fails, a CtxMamErrorException will be thrown.
                    coreService.CtxMamLog_ErrorFrom("XAMARIN Forms", FileName(), MemberName(), LineNumber(), "MAM SDK initialization failed - {0}", ex.Description());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex);
                }
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        static string FileName([System.Runtime.CompilerServices.CallerFilePath] string fileName = "")
        {
            return fileName;
        }

        static string MemberName([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return memberName;
        }

        static int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
    }
}
