/*
 * Copyright © 2022. Cloud Software Group, Inc. All Rights Reserved. Confidential & Proprietary.
 *
 * Use and support of this software is governed by the terms
 * and conditions of the software license agreement and support
 * policy of Cloud Software Group, Inc. and/or its subsidiaries.
 *
 */

using Foundation;
using UIKit;
using com.citrix.ios.CTXMAMCore;
using com.citrix.ios.CTXMAMAppCore;
using com.citrix.ios.CTXMAMCompliance;
using com.citrix.ios.CTXMAMContainment;
using com.citrix.ios.CTXMAMLocalAuth;
using com.citrix.ios.CTXMAMNetwork;
using System;
using ObjCRuntime;

namespace MvpnTestIOSApp
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate
    {

        [Export("window")]
        public UIWindow Window { get; set; }

        [Export("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            MvpnTestIOSAppMAMCoreSDKDelegate mamCoreDelegate = new MvpnTestIOSAppMAMCoreSDKDelegate();
            CTXMAMCore.SetDelegate(mamCoreDelegate);

            NSString source = com.citrix.ios.CTXMAMNetwork.Constants.CTXMAMNotificationSource_Network;
            CTXMAMNotificationCenter.MainNotificationCenter?.RegisterForNotificationsFromSource(
                source,
                delegate (CTXMAMNotification notification) {
                    Console.WriteLine("Received notification from CTXMAMNotificationSource_Network: " + notification.Message);
                });

            MvpnTestIOSAppCTXMAMLocalAuthSdkDelegate localAuthDelegate = new MvpnTestIOSAppCTXMAMLocalAuthSdkDelegate();
            CTXMAMLocalAuth.SetDelegate(localAuthDelegate);

            MvpnTestIOSAppCTXMAMContainmentSdkDelegate containmentDelegate = new MvpnTestIOSAppCTXMAMContainmentSdkDelegate();
            CTXMAMContainment.SetDelegate(containmentDelegate);

            source = com.citrix.ios.CTXMAMContainment.Constants.CTXMAMNotificationSource_Containment;
            CTXMAMNotificationCenter.MainNotificationCenter?.RegisterForNotificationsFromSource(
                source,
                delegate (CTXMAMNotification notification) {
                    Console.WriteLine("Received notification from CTXMAMNotificationSource_Containment: " + notification.Message);

                    var okAlertController = UIAlertController.Create("Alert", notification.Message, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(okAlertController, true, null);                     
                });

            MvpnTestIOSAppMAMComplianceDelegate mamComplianceDelegate = new MvpnTestIOSAppMAMComplianceDelegate();
            CTXMAMCompliance.SharedInstance().Delegate = mamComplianceDelegate;

            double appCoreVersion = com.citrix.ios.CTXMAMAppCore.Constants.CTXMAMAppCoreVersionNumber;


            // Initializing the MAM SDKs must come after setting the delegates.
            CTXMAMCore.InitializeSDKsWithCompletionBlock(initResultHandler: (NSError errObj) =>
            {
                if (errObj == null)
                {
                    // If MAM SDK initialization succeeds, this code will be executed.
                    Console.WriteLine("MAM SDK initialization succeeded");
                }
                else
                {
                    // If MAM SDK initialization fails, this code will be executed.
                    Console.WriteLine("MAM SDK initialization failed - {0}", errObj);
                }
            });

            return true;
        }

        // UISceneSession Lifecycle

        [Export("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create("Default Configuration", connectingSceneSession.Role);
        }

        [Export("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions(UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
    }


    public class MvpnTestIOSAppCTXMAMLocalAuthSdkDelegate : CTXMAMLocalAuthSdkDelegate
    {
        public override bool DevicePasscodeRequired()
        {
            Console.WriteLine("Device passcode required.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool MaxOfflinePeriodExceeded()
        {
            Console.WriteLine("Max offline period exceeded.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool MaxOfflinePeriodWillExceedWarning(double secondsToExpire)
        {
            Console.WriteLine("Max offline period about to expire.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }
    }

    public class MvpnTestIOSAppCTXMAMContainmentSdkDelegate : CTXMAMContainmentSdkDelegate
    {
        public override bool AppIsOutsideGeofencingBoundaryWithDefaultHandlerOption()
        {
            Console.WriteLine("App is outside geofencing boundary.");
            return false;
        }

        public override bool AppNeedsLocationServicesEnabledWithDefaultHandlerOption()
        {
            Console.WriteLine("App needs location services enabled.");
            return false;
        }
    }

    public class MvpnTestIOSAppMAMCoreSDKDelegate : CTXMAMCoreSdkDelegate
    {
        public override bool ProxyServerSettingDetectedWithDefaultHandlerOption()
        {
            Console.WriteLine("Proxy server setting detected.");
            return false;
        }
    }

    public class MvpnTestIOSAppMAMComplianceDelegate : CTXMAMComplianceDelegate
    {
        public override bool HandleAdminLockAppSecurityActionForError(NSError error)
        {
            Console.WriteLine("HandleAdminLockAppSecurityActionForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleAdminWipeAppSecurityActionForError(NSError error)
        {
            Console.WriteLine("HandleAdminWipeAppSecurityActionForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleAppDisabledSecurityActionForError(NSError error)
        {
            Console.WriteLine("HandleAppDisabledSecurityActionForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleContainerSelfDestructSecurityActionForError(NSError error)
        {
            Console.WriteLine("HandleContainerSelfDestructSecurityActionForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleDateAndTimeChangeSecurityActionForError(NSError error)
        {
            Console.WriteLine("HandleDateAndTimeChangeSecurityActionForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleDevicePasscodeComplianceViolationForError(NSError error)
        {
            Console.WriteLine("HandleDevicePasscodeComplianceViolationForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleEDPComplianceViolationForError(NSError error)
        {
            Console.WriteLine("HandleEDPComplianceViolationForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleJailbreakComplianceViolationForError(NSError error)
        {
            Console.WriteLine("HandleJailbreakComplianceViolationForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }

        public override bool HandleUserChangeSecurityActionForError(NSError error)
        {
            Console.WriteLine("HandleUserChangeSecurityActionForError.");
            return false;   // By returning false, we're not handling this callback and letting the MAMSDK to handle it.
        }
    }
}

