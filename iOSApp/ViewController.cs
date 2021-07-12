using Foundation;
using System;
using UIKit;
using WebKit;
using MobileCoreServices;
using MessageUI;
using com.citrix.ios.CTXMAMCore;

namespace MvpnTestIOSApp
{
    public partial class ViewController : UIViewController, IUINavigationControllerDelegate
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"SdksInitializedAndReady", (Action) =>
            {
                Console.WriteLine("SdksInitializedAndReady received, initializing WebView");
                InvokeOnMainThread(() => { webView.Init(); });
            });

            httpUrlTextField.Text = "https://citrix.com";

            this.httpUrlTextField.ShouldReturn += (textField) => {
                string urlStr = this.httpUrlTextField.Text;

                NSUrl url = new NSUrl(urlStr);
                NSUrlRequest urlRequest = new NSUrlRequest(url);

                this.webView.LoadRequest(urlRequest);
                textField.ResignFirstResponder();
                return true;
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void GoButtonAction(Foundation.NSObject sender)
        {
            CTXMAMLogger.CTXMAMLog_CriticalErrorFrom("MvpnTestApp", "myFile", "myMethod", 0, "testing this string", (IntPtr)null);

            string urlStr = this.httpUrlTextField.Text;
            if(urlStr.Length == 0)
            {
                urlStr = "https://www.google.com";
            }
            NSUrl url = new NSUrl(urlStr);
            NSUrlRequest urlRequest = new NSUrlRequest(url);

            this.webView.LoadRequest(urlRequest);
        }


        UIImagePickerController imagePicker;

        partial void OpenCameraAction(Foundation.NSObject sender)
        {
            imagePicker = new UIImagePickerController();
            imagePicker.PrefersStatusBarHidden();

            imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;

            //Add event handlers when user finished Capturing image or Cancel
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            //present 
            PresentViewController(imagePicker, true, () => { });
        }


        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            // determine what was selected, video or image
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            // get common info (shared between images and video)
            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.ToString());

            // if it was an image, get the other image info
            if (isImage)
            {
                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the original image");
                    //imageView.Image = originalImage; // display
                }
            }
            else
            {
                // if it's a video
                // get video url
                NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
                if (mediaURL != null)
                {
                    Console.WriteLine(mediaURL.ToString());
                }
            }
            // dismiss the picker
            imagePicker.DismissModalViewController(true);
        }

        void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }
    }

}