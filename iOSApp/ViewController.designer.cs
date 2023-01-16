// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MvpnTestIOSApp
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIView goButton { get; set; }

		[Outlet]
		UIKit.UITextField httpUrlTextField { get; set; }

		[Outlet]
		UIKit.UITextField testTextField { get; set; }

		[Outlet]
		WebKit.WKWebView webView { get; set; }

		[Action ("FetchButtonAction:")]
		partial void FetchButtonAction (Foundation.NSObject sender);

		[Action ("GoButtonAction:")]
		partial void GoButtonAction (Foundation.NSObject sender);

		[Action ("OpenAirDropAction:")]
		partial void OpenAirDropAction (Foundation.NSObject sender);

		[Action ("OpenAirPrintAction:")]
		partial void OpenAirPrintAction (Foundation.NSObject sender);

		[Action ("OpenCameraAction:")]
		partial void OpenCameraAction (Foundation.NSObject sender);

		[Action ("OpenDictationAction:")]
		partial void OpenDictationAction (Foundation.NSObject sender);

		[Action ("OpenFileBackupAction:")]
		partial void OpenFileBackupAction (Foundation.NSObject sender);

		[Action ("OpeniCloudAction:")]
		partial void OpeniCloudAction (Foundation.NSObject sender);

		[Action ("OpenMicRecordAction:")]
		partial void OpenMicRecordAction (Foundation.NSObject sender);

		[Action ("OpenPhotoLibraryAction:")]
		partial void OpenPhotoLibraryAction (Foundation.NSObject sender);

		[Action ("OpenSmsComposeAction:")]
		partial void OpenSmsComposeAction (Foundation.NSObject sender);

		[Action ("SendEmailAction:")]
		partial void SendEmailAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (goButton != null) {
				goButton.Dispose ();
				goButton = null;
			}

			if (httpUrlTextField != null) {
				httpUrlTextField.Dispose ();
				httpUrlTextField = null;
			}

			if (testTextField != null) {
				testTextField.Dispose ();
				testTextField = null;
			}

			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
