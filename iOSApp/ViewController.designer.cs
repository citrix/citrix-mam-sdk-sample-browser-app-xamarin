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
		WebKit.WKWebView webView { get; set; }

		[Action ("GoButtonAction:")]
		partial void GoButtonAction (Foundation.NSObject sender);

		[Action ("OpenCameraAction:")]
		partial void OpenCameraAction (Foundation.NSObject sender);
		
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

			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
