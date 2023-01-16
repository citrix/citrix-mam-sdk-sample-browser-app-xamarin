/*
 * Copyright © 2022. Cloud Software Group, Inc. All Rights Reserved. Confidential & Proprietary.
 *
 * Use and support of this software is governed by the terms
 * and conditions of the software license agreement and support
 * policy of Cloud Software Group, Inc. and/or its subsidiaries.
 *
 */

using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using com.citrix.ios.CTXMAMCore;
using com.citrix.ios.CTXMAMCompliance;
using com.citrix.ios.CTXMAMContainment;
using com.citrix.ios.CTXMAMLocalAuth;
using Com.Citrix.Mvpn.Api.iOS;
using Com.Citrix.MamCore.Api.iOS;

namespace MvpnTestFormsApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.

    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // Compliance delegate is a weak property, so an instance variable is needed to retain a handle to it to avoid garbage collection.
        TestIosAppMamComplianceSdkDelegate mamComplianceDelegate;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            TestIosAppMamCoreSdkDelegate mamCoreDelegate = new TestIosAppMamCoreSdkDelegate();
            CTXMAMCore.SetDelegate(mamCoreDelegate);

            CTXMAMNotificationCenter.MainNotificationCenter?.RegisterForNotificationsFromSource(
                com.citrix.ios.CTXMAMNetwork.Constants.CTXMAMNotificationSource_Network,
                delegate (CTXMAMNotification notification) {
                    Console.WriteLine("Received notification from CTXMAMNotificationSource_Network");
                });

            double appCoreVersion = com.citrix.ios.CTXMAMAppCore.Constants.CTXMAMAppCoreVersionNumber;

            mamComplianceDelegate = new TestIosAppMamComplianceSdkDelegate();
            CTXMAMCompliance.SharedInstance().Delegate = mamComplianceDelegate;

            TestIosAppMamContainmentSdkDelegate mamContainmentDelegate = new TestIosAppMamContainmentSdkDelegate();
            CTXMAMContainment.SetDelegate(mamContainmentDelegate);

            TestIosAppMamLocalAuthSdkDelegate mamLocalAuthDelegate = new TestIosAppMamLocalAuthSdkDelegate();
            CTXMAMLocalAuth.SetDelegate(mamLocalAuthDelegate);

            DependencyService.Register<MamCoreService>();
            DependencyService.Register<MamCoreConfigService>();
            DependencyService.Register<MicroVPNService>();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }

    #region CTXMAMCore delegates
    // Protocol to be implemented when you want your app to receive callbacks from the CTXMAMCore SDK.
    public class TestIosAppMamCoreSdkDelegate : CTXMAMCoreSdkDelegate
    {
        // This delegate method is used to let the app know that the user has configured a proxy server to redirect the network requests. This is not allowed and so the app will
        // shut down. The app developer needs to let the user know that they need to remove the proxy server setting and quit the app. The app needs to be exited upon receiving this
        // delegate callback.
        // Return - true if app handles this; otherwise false.
        public override bool ProxyServerSettingDetectedWithDefaultHandlerOption()
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Proxy Server Detected", "Please remove proxy server setting", "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }
    }
    #endregion

    #region CTXMAMCompliance delegates
    // Protocol to be implemented when you want your app to receive callbacks from the CTXMAMCompliance SDK.
    // App should implement any callback methods that it wants to have custom behavior for, and the implementations should adhere to the documented guidelines.
    public class TestIosAppMamComplianceSdkDelegate : CTXMAMComplianceDelegate
    {

        // To handle App Lock security action imposed by organization admin.
        // Handle it by requiring the user to logon (call PerformLogonWithErrorContext(NSError errorContext, Action<bool> completionBlock)) in order to continue using the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleAdminLockAppSecurityActionForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Admin Lock - Please logon", error.ToString(), "Logon");
            //     CTXMAMCompliance.SharedInstance().PerformLogonWithErrorContext(error, completionBlock: (bool success) =>
            //     {
            //         Console.WriteLine(success ? "Logon Successful" : "Logon Failed");
            //     });
            // });
            // return true;
        }

        // To handle App Wipe security action imposed by organization admin.
        // Handle it by requiring the user to logon (call PerformLogonWithErrorContext(NSError errorContext, Action<bool> completionBlock)) in order to continue using the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleAdminWipeAppSecurityActionForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Admin Wipe", error.ToString(), "Logon");
            //     CTXMAMCompliance.SharedInstance().PerformLogonWithErrorContext(error, completionBlock: (bool success) =>
            //     {
            //         Console.WriteLine(success ? "Logon Successful" : "Logon Failed");
            //     });
            // });
            // return true;
        }

        // To handle admin imposed App Disable security action.
        // Handle it by requiring the user to quit the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleAppDisabledSecurityActionForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("App Disabled by Admin", error.ToString(), "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }

        // To handle app container self-destruct security action imposed by organization admin.
        // Handle it by requiring the user to logon (call PerformLogonWithErrorContext(NSError errorContext, Action<bool> completionBlock)) in order to continue using the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleContainerSelfDestructSecurityActionForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Container Self Destruct by Admin", error.ToString(), "Logon");
            //     CTXMAMCompliance.SharedInstance().PerformLogonWithErrorContext(error, completionBlock: (bool success) =>
            //     {
            //         Console.WriteLine(success ? "Logon Successful" : "Logon Failed");
            //     });
            // });
            // return true;
        }

        // To handle significant date and time change security action.
        // Handle it by requiring the user to logon (call PerformLogonWithErrorContext(NSError errorContext, Action<bool> completionBlock)) in order to continue using the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleDateAndTimeChangeSecurityActionForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Time Change Violation", error.ToString(), "Logon");
            //     CTXMAMCompliance.SharedInstance().PerformLogonWithErrorContext(error, completionBlock: (bool success) =>
            //     {
            //         Console.WriteLine(success ? "Logon Successful" : "Logon Failed");
            //     });
            // });
            // return true;
        }

        // To handle compliance violation of not setting the device passcode.
        // Handle it by requiring the user to quit the app and asking the user to set the app passcode before re-using the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleDevicePasscodeComplianceViolationForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Passcode Violation", error.ToString(), "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }

        // To handle compliance violation when the device falls below the minimum compliance requirements of encryption.
        // If error code is
        //    - CTXMAMCompliance_Violation_EDP_BlockApp: Handle it by requiring the user to quit the app and suggesting to re-launch the app after fixing the
        //      compliance violation based on the suggestion mentioned in the user info message.
        //    - CTXMAMCompliance_Violation_EDP_WarnUser: Handle it by warning user of the compliance violation and risks of leaking app container data when
        //      device is lost. For example, by prompting an obtrusive alert view.
        //    - CTXMAMCompliance_Violation_EDP_InformUser: Handle it by unobtrusively informing user of the compliance violation and risks of leaking app container
        //      data when device is lost.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleEDPComplianceViolationForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("EDP Violation", error.ToString(), "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }

        // To handle compliance violation of using a jailbroken device.
        // Handle it by requiring the user to quit the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleJailbreakComplianceViolationForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Jailbreak Violation", error.ToString(), "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }

        // To handle XenMobile/SecureHub user account change security action.
        // Handle it by requiring the user to quit the app and suggesting the user to re-install the app.
        // Param - error. CTXMAMComplianceError error code and localized message in NSLocalizedDescriptionKey of user info.
        // Return - true if app handles this; otherwise false.
        public override bool HandleUserChangeSecurityActionForError(NSError error)
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("User Has Changed", error.ToString(), "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }
    }
    #endregion

    #region CTXMAMContainment delegates
    // Protocol to be implemented when you want your app to receive callbacks from the CTXMAMContainment SDK.
    // App should implement any callback methods that it wants to have custom behavior for, and the implementations should adhere to the documented guidelines.
    public class TestIosAppMamContainmentSdkDelegate : CTXMAMContainmentSdkDelegate
    {
        // Method called when app geolocation is out of the bounds defined by the admin.
        // If the method is not implemented, the app is blocked, and an alert is shown stating that the app needs to be closed until the device meets the geofencing criteria.
        // Otherwise, the method implemented by the app will be called when the current location of the device doesn't meet the boundary requirements.
        // Return - true if the app handles this; otherwise false.
        public override bool AppIsOutsideGeofencingBoundaryWithDefaultHandlerOption()
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Outside Organization's Designated Area", "Please return to designated area and relaunch the app", "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }

        // Method called when location services are disabled.
        // If the method is not implemented, the app is blocked, and an alert is shown stating that the app needs to be closed until location services are enabled.
        // Otherwise, the method implemented by the app will be called when location services are disabled.
        // Return - true if the app handles this; otherwise false.
        public override bool AppNeedsLocationServicesEnabledWithDefaultHandlerOption()
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Location Services Disabled", "Please enable Location Services in Settings", "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }
    }
    #endregion

    #region CTXMAMLocalAuth delegates
    // Protocol to be implemented when you want your app to receive callbacks from the CTXMAMLocalAuth SDK.
    // App should implement any callback methods that it wants to have custom behavior for, and the implementations should adhere to the documented guidelines.
    public class TestIosAppMamLocalAuthSdkDelegate : CTXMAMLocalAuthSdkDelegate
    {
        // This delegate method is used to let the app know that the device passcode is necessary since the App Passcode policy is enabled and without the device
        // passcode this policy cannot function as expected. The user may also enable the TouchID/FaceID which also means that device passcode is present.
        // When TouchID/FaceID is enabled, then it will be used first and if its locked out then device passcode prompt will be presented.
        // The app developer needs to let the user know that they need to enable Device Passcode and optionally enable TouchID/FaceID.
        // The app needs to be exited upon receiving this delegate callback.
        // Return - true if the app handles this; otherwise false.
        public override bool DevicePasscodeRequired()
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Device Passcode Required", "Please enable device passcode in Settings", "Quit");
            //     System.Diagnostics.Process.GetCurrentProcess().Kill();
            // });
            // return true;
        }

        // This delegate method is used to let the app know that the Max Offline Period has expired. To continue using the app the user must go online and login to SecureHub
        // The app developer needs to let the user know that they need to do a successful login by going online. Until then the app will not be allowed to be used.
        // The API to perform login will be available in the Compliance SDK.
        // Return - true if the app handles this; otherwise false.
        public override bool MaxOfflinePeriodExceeded()
        {
            return false;

            // Example code - only if you want custom behavior:

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Offline lease has expired", "Please sign in to SecureHub", "Logon");
            //     CTXMAMCompliance.SharedInstance().PerformLogonWithErrorContext(error, completionBlock: (bool success) =>
            //     {
            //         Console.WriteLine(success ? "Logon Successful" : "Logon Failed");
            //     });
            // });
            // return true;
        }

        // This delegate method is used to let the app know that the Max Offline Period expiration is approaching. It will be sent 30 minutes, 15 minutes and 5 minutes
        // before the expiration. The app developer needs to let the user know that the Max Offline Period is going to expire soon and this is a warning.
        // Also let the user know that it is advisable to login soon to continue using the app uninterrrupted.
        // The app can be continued to be used by the user and no need to quit the app.
        // Param - secondsToExpire. The number of seconds remaining to expire the Max Offline Period.
        // Return - true if the app handles this; otherwise false.
        public override bool MaxOfflinePeriodWillExceedWarning(double secondsToExpire)
        {
            return false;

            // Example code - only if you want custom behavior:

            // var minutes = (long)(secondsToExpire / 60);
            // var expiryTime = minutes.ToString() + " minutes";
            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Sign in to SecureHub. Offline lease is expiring in:", expiryTime, "OK");
            // });
            // return true;
        }
    }
    #endregion
}
