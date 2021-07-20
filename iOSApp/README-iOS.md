
# Introduction

The Citrix MAM SDK provides a range of features allowing developers to work in familiar environments to build native apps. Once apps are built, developers can assume the role of an IT admin to quickly validate the management and distribution of their apps through the Citrix Endpoint Management.

The Citrix MAM SDK includes support for the following services:

- Core & AppCore
- Compliance
- Local Auth
- Containment
- Micro VPN

For more information about the suported app policies, see the [Third-party app policies](https://docs.citrix.com/en-us/mdx-toolkit/mam-sdk-overview.html#third-party-app-policies-for-ios-and-android).

## Pre-requisities

### Software Requirements

| Type | Requirements |
|---|---|
| Supported OS | iOS. |
| Visual Studio | Visual Studio 2019 for Mac 8.9.4 (Build 25) or later. |
| Xcode | 12.4 or later. |
| .NET Standard | 2.0 or later. |
| Xamarin.iOS | 14.14.2.5 or later. |
| Mono Framework | 6.12.0.125 or later |
| Test devices | iOS Device or iOS Virtual Device. |
| Apps | Latest version of Secure Hub from the Apple App Store. |

### System requirements

Before you set up the MAM SDK, ensure that the following prerequisites are met:

-  A [Citrix developer account](https://www.citrix.com/welcome/create-account/).
-  An [Apple developer account](https://developer.apple.com/support/).
-  Experience with the following:
    -  iOS mobile app development
    -  Xamarin application development
    -  Access to test .mdx and upload .ipa to Citrix Endpoint Management.
-  For customers using XenMobile Server:
    -  Access to test micro VPN tunneling with your app through Citrix NetScaler.

## Build and deploy your app

Perform the following steps to build and deploy your app:

1.  Download and review the contents of the [Xamarin iOS MAM SDK](https://www.citrix.com/downloads/citrix-endpoint-management/product-software/mdx-toolkit.html). Please reach out to your Citrix point of contact for further information.
1.  Integrate Xamarin MAM SDK **Core** and **AppCore** Libraries to your project.
    - See [Integrate Xamarin MAM SDK](#integrate-xamarin-mam-sdk)
1.  Integrate the optional libraries:
    -  Compliance
    -  Containment
    -  Local Authentication
    -  Micro VPN
1.  [Generate the MDX file] (#generate-and-update-an-mdx-file).
1.  Test and debug your app.
1.  Distribute your app. For more information, see [Distribute MAM SDK enabled apps](https://docs.citrix.com/en-us/citrix-endpoint-management/device-management/apple/distribute-apple-apps.html)).
1.  Review the configuration and deploy policies. For more information, see [Configure Policies](https://docs.citrix.com/en-us/mdx-toolkit/mam-sdk-policies-ios.html)

## Sample App

Sample apps called **MvpnTestiOSApp** and **MvpnTestFormsApp** are included in the downloads folder of the MAM SDKs. All the MAM SDKs are included and used in this sample app. Developers should be able to directly launch and verify the functionality of the SDKs and take similar steps for handling the delegate callbacks as shown in the app.

The samples are stored in Xamarin/SampleCode/.


# Integrate Xamarin MAM SDK

Perform the following steps to integrate the MAM SDK into your project.

## Add MAM SDK to your project

1. After the package has been [downloaded] (./download-xamarin-mam-sdk)
    - Unzip Xamarin.MAMSDK-\<MAM SDK Version Number>.zip. This will have the directory structure pictured below.
    - The NugetPackage folder will contain all the necessary nuget libraries required to build Xamarin iOS and Xamarin Forms App using Citrix MAM SDK.


1. Add Nuget source
    - Add nuget sources to your project to point to MAM SDK nuget libraries. Example:

        ```bash
        nuget sources Add -Name Xamarin.MAMSDK -Source <Enter Xamarin MAM SDK Location Here>/NugetPackage
        ```

1. Add Nuget Packages
    - Add Nuget Packages in your Xamarin project.

    > **For Xamarin iOS and Xamarin Forms apps:**
    - In the Xamarin .iOS project for the app, e.g. MvpnTestFormsApp.iOS, Right-Click the project -> Add -> NuGet Packages and add:
        - Citrix.Xamarin.iOS.MAMSDK.Core
        - Citrix.Xamarin.iOS.MAMSDK.AppCore
        - Citrix.Xamarin.iOS.MAMSDK.Compliance
        - Citrix.Xamarin.iOS.MAMSDK.Containment
        - Citrix.Xamarin.iOS.MAMSDK.LocalAuth
        - Citrix.Xamarin.iOS.MAMSDK.Network

    > **For Xamarin Forms apps only:**
    - In the Xamarin Forms shared project, e.g. MvpnTestFormsApp, Right-Click the project -> Add -> NuGet Packages and add:
        - Citrix.Xamarin.Forms.MAMSDK

## Entitlements

1. Entitlements.plist
    - Ensure you have an Entitlements.plist in your Xamarin project with the following Keychain groups added:
       - com.\<Your app's bundle ID>
       - com.citrix.mdx
    - A sample Entitlements.plist is included in the sample code.


## Initializing the MAM SDK

1. Initializing the Xamarin MAM SDK
    - The **InitializeSDKs()** method from MAM Core must be called when starting the app, either via the Core Xamarin iOS library (com.citrix.ios.CTXMAMCore) **OR** via the Xamarin Forms MamCoreService Dependency Service (Com.Citrix.MamCore.Api). You must set the MAM SDK delegates before calling InitializeSDKs().
        - Xamarin iOS
            >**AppDelegate.cs**:

            ```csharp
            using com.citrix.ios.CTXMAMCore;
            ```

            ```csharp
            _ = com.citrix.ios.CTXMAMAppCore.Constants.CTXMAMAppCoreVersionNumber;
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
            ```

        - Xamarin Forms
            >**AppDelegate.cs**:

            ```csharp
            using Com.Citrix.MamCore.Api.iOS;
            ```

            ```csharp
            _ = com.citrix.ios.CTXMAMAppCore.Constants.CTXMAMAppCoreVersionNumber;
            DependencyService.Register<MamCoreService>();
            ```

            >**App.xaml.cs**:

            ```csharp
            using Com.Citrix.MamCore.Api;
            ```

            ```csharp
            var service = DependencyService.Get<IMamCoreService>();
            if (service != null)
            {
                try
                {
                    await service.InitializeSdks().ConfigureAwait(false);
                }
                catch (CtxMamErrorException ex)
                {
                    // If MAM SDK initialization fails, a CtxMamErrorException will be thrown.
                }
            }
            ```

    - Examples of this are included in the sample code.

# MDX policy file

## Create an MDX File

MDX file contains policies and meta data about the application. For information about the app policies that are available for MAM SDK policies for third-party iOS apps, see [MAM SDK policies for third-party apps for iOS](https://docs.citrix.com/en-us/mdx-toolkit/mam-sdk-policies-ios.html).

MDX files are generated automatically as part of the build process, but you will need to follow this one time setup.

1. Ensure you have the  **CreateMDXFile.sh**  script in your Xamarin Forms iOS project. This can be found in the provided sample code.

1.  Edit the script by providing:

    **STOREURL**: During development, use any URL (For example "http://yourstore.yourdomain.com" ). Note: The -storeURL argument value may need to be changed later depending on the distribution method selected.

    **APPTYPE**: Must be set to "sdkapp". Note: "sdkapp" is the only valid option when utilizing the iOS MAM SDK.  Since the iOS MAM SDK is built into the app, XCode can prepare the app like any other Apple application, consequently less post-build preparation is needed.

    **PACKAGEID**: Generate a UUID using the uuidgen tool in the macOS terminal. The UUID is referred to as a Package ID. Specify URL Scheme such as com.citrix.sso.\<package ID>. Add the new unique URL Scheme with the prefix com.citrix.sso, in the PACKAGEID field of this script.

    **APPIDPREFIX** (aka Team ID): Ensure that you add your app ID prefix in the APPIDPREFIX field. The app ID can be found in the Apple Developer portal, under the AppIDs section using your Apple Developer Account.

    **TOOLKIT_DIR**: Ensure this points to "iOSTools" provided in the MAM SDK Xamarin package.

1. Edit the script and provide specific code signing cert i.e. "Apple Development" or "Distribution" cert.
    - Mac: CMD + Space → Keychain Access.app to check.

    ```
    /usr/bin/codesign --force --sign "Apple Development John Doe (AAAAAAAAAA)" ...
    ```

1. Change the project settings to run the CreateMDXFile.sh script after the build:
    - Check the Configuration and Platform are correct for your build.
    - Ensure  "After Build" is selected in the dropdown.

1.  After building the project, the "Developer" .mdx policy file has been created and can be found in the same location where Visual Studio for Mac stores its build artifacts. For example: ..../bin/iPhone/Debug/device-builds/iphone12.8-14.6.

1. You can verify you have completed the preceding steps correctly by ensuring the .app in the build artifact directory contains a Citrix.plist.

1.  Depending on the [distribution method](./distribute-apps), perform the following steps:

    - **App Store (Including Public, Volume purchase, and custom apps)**: If you are using the App Store to distribute your app, the [IPA will be signed](#generate-the-ipa-file) with an [Apple Developer Program distribution provisioning profile](https://help.apple.com/xcode/mac/current/#/dev3a05256b8). Once the app is uploaded to the App Store, you must update the MDX generation script with the following:

        - STOREURL: Use your app Apple App Store URL
        - Run the script to generate the new MDX. Once you have the new MDX file, you can [upload](https://docs.citrix.com/en-us/citrix-endpoint-management/apps.html#add-an-mdx-app) it to Citrix Endpoint Management. 

    - **MDX (Enterprise/B2B)**: If you are distributing your app to users in your organization, perform the following steps:
    
        - [Sign your app (.IPA)](#generate-the-ipa-file) with an Apple Developer Enterprise Program distribution provisioning profile.
        - After signing the IPA, [embed the IPA into the MDX](#embed-the-ipa-file-into-the-mdx-file).

> **Note**
>
> Do not clean the project before the following steps. If the project is cleaned at any point during these steps, they will have to be undone and the .app and .mdx files will need to be regenerated again.

## Generate the IPA file

1. After the project has been built and a .mdx file has been generated, right-click on your app in Visual Studio and click **Options** and select **iOS IPA Options**.
1. Select **Build iTunes Package Archive**.
1. Re-build the project without cleaning it.
1. The .ipa file will be found in the same folder as the .app and .mdx files.
1. You can verify you have completed the preceding steps correctly by making a copy of the .ipa, renaming it to a .zip, unpackaging it, and ensuring the .app contains a Citrix.plist.

## Embed the IPA file into the MDX file

If you are deploying the app internally in your organization and not using App store, the .ipa file must be embedded into the MDX policy file. This step combines .ipa file with .mdx policy file, other Citrix components and your keystore or signing certificate to produce an MDX policy file with the app embedded.

Once the IPA file is [created] (#generate-the-ipa-file) and signed using Apple Developer Enterprise Program distribution provisioning profile, perform the following steps to embed an IPA into the MDX file:

1.  Ensure you have the  **EmbedIPA.sh**  script in the same folder as your Xamarin iOS project. This script can be found in the provided sample code.

1. Run the script from the Mac terminal, ensuring the .ipa is present within your Xamarin project folder structure, and not located elsewhere.

1. The embedded.mdx file can be found in the same location where the .ipa was saved.

- **Alternatively**, you can copy the following content to a script and run the script, after filling out the relevant information:

    ```bash

    export TOOLKIT_DIR="Provide iOSTools path"
    export IPA_FILE_PATH="Provide IPA file path"
    export MDX_PATH="Provide .mdx file path"
    export EMBEDDED_MDX_PATH="Provide desired file path for your embedded .mdx file. This must be different from MDX_PATH"
    "$TOOLKIT_DIR/CGAppCLPrepTool" SetInfo -in "$MDX_PATH" -out "$EMBEDDED_MDX_PATH" -embedBundle "${IPA_FILE_PATH}"
    ```

## Upload the MDX file

Once the MDX is generated you [upload the .mdx file to the Endpoint Management console](https://docs.citrix.com/en-us/citrix-endpoint-management/apps.html#add-an-mdx-app) where you and [configure](https://docs.citrix.com/en-us/mdx-toolkit/mam-sdk-policies-ios.html) specific app details and policy settings that the Endpoint Management Store enforces.


