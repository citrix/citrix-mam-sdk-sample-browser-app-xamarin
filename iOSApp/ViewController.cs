/*
 * Copyright © 2022. Cloud Software Group, Inc. All Rights Reserved. Confidential & Proprietary.
 *
 * Use and support of this software is governed by the terms
 * and conditions of the software license agreement and support
 * policy of Cloud Software Group, Inc. and/or its subsidiaries.
 *
 */

using Foundation;
using System;
using UIKit;
using WebKit;
using MobileCoreServices;
using MessageUI;
using com.citrix.ios.CTXMAMCore;
using AVFoundation;
using System.Net.Http;
using System.Linq;
using Speech;

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

                try
                {
                    NSUrl url = new NSUrl(urlStr);
                    NSUrlRequest urlRequest = new NSUrlRequest(url);

                    this.webView.LoadRequest(urlRequest);
                }
                catch (Exception ex)
                {
                    ShowMessage("Error", ex.ToString());
                }
                textField.ResignFirstResponder();
                return true;
            };
            this.GoButtonAction(null);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void FetchButtonAction(Foundation.NSObject sender)
        {
            string urlStr = this.httpUrlTextField.Text;
            if (urlStr.Length == 0)
            {
                urlStr = "https://www.google.com";
            }
            var httpClient = new HttpClient();
            try
            {
                var result = httpClient.GetStringAsync(urlStr).Result;
                ShowMessage("HttpClient", result);
            }
            catch (Exception ex)
            {
                ShowMessage("Error", ex.ToString());
            }
        }

        partial void GoButtonAction(Foundation.NSObject sender)
        {
            CTXMAMLogger.CTXMAMLog_CriticalErrorFrom("MvpnTestApp", "myFile", "myMethod", 0, "testing this string", (IntPtr)null);

            string urlStr = this.httpUrlTextField.Text;
            if (urlStr.Length == 0)
            {
                urlStr = "https://www.google.com";
            }
            try
            {
                NSUrl url = new NSUrl(urlStr);
                NSUrlRequest urlRequest = new NSUrlRequest(url);

                this.webView.LoadRequest(urlRequest);
            }
            catch (Exception ex)
            {
                ShowMessage("Error", ex.ToString());
            }
        }

        UIImagePickerController imagePicker;

        partial void OpenCameraAction(Foundation.NSObject sender)
        {
            if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
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
            else
            {
                ShowMessage("Alert", "\"Camera\" is blocked for this app");
            }
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

        partial void OpenPhotoLibraryAction(Foundation.NSObject sender)
        {
            if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.PhotoLibrary))
            {
                imagePicker = new UIImagePickerController();
                imagePicker.PrefersStatusBarHidden();

                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                //present 
                PresentViewController(imagePicker, true, () => { });
            }
            else
            {
                ShowMessage("Alert", "\"Photo Library\" is blocked for this app");
            }
        }

        partial void OpenMicRecordAction(Foundation.NSObject sender)
        {
            switch (AVAudioSession.SharedInstance().RecordPermission)
            {
                case AVAudioSessionRecordPermission.Granted:
                    ShowMessage("Microphone access", "Permission granted");
                    break;
                case AVAudioSessionRecordPermission.Denied:
                    ShowMessage("Microphone access", "Permission denied");
                    break;
                case AVAudioSessionRecordPermission.Undetermined:
                    ShowMessage("Microphone access", "Permission undeterminated");
                    break;
                default:
                    break;
            }
        }

        partial void OpenDictationAction(Foundation.NSObject sender)
        {
            var audioEngine = new AVAudioEngine();
            var request = new SFSpeechAudioBufferRecognitionRequest();
            var node = audioEngine.InputNode;
            var recordinFormat = node.GetBusOutputFormat(0);
            node.InstallTapOnBus(0, 1024, recordinFormat, new AVAudioNodeTapBlock((buffer, _) => { request.Append(buffer); }));
            audioEngine.Prepare();
            audioEngine.StartAndReturnError(out var error);
            Console.WriteLine(error);
            this.testTextField.ResignFirstResponder();
            this.testTextField.Enabled = true;
            this.testTextField.BecomeFirstResponder();
        }

        void ShowMessage(string title, string message)
        {
            var okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(okAlertController, true, null);
        }

        partial void OpenSmsComposeAction(Foundation.NSObject sender)
        {
            if (MFMessageComposeViewController.CanSendText)
            {
                var controller = new MFMessageComposeViewController();
                controller.Body = "Message Body";
                controller.Recipients = new[] { "12345567" };
                controller.Finished += (s, e) => DismissViewController(true, null);
                PresentViewController(controller, true, null);
            }
            else
            {
                ShowMessage("Alert", "\"SMS Compose\" is blocked for this app");
            }
        }

        partial void OpeniCloudAction(Foundation.NSObject sender)
        {
            var iCloudDocumentsURL = NSFileManager.DefaultManager.GetUrlForUbiquityContainer(null)?.AppendPathExtension("myCloudTest");
            ShowMessage("Success", "iCloud can be accessed");
        }

        partial void OpenFileBackupAction(Foundation.NSObject sender)
        {
            var documentUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).LastOrDefault();
            var libraryUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User).LastOrDefault();
            try
            {
                var documentSucess = documentUrl.GetResourceValues(new[] { NSUrl.IsExcludedFromBackupKey }, out var documentError);
                var librarySucess = libraryUrl.GetResourceValues(new[] { NSUrl.IsExcludedFromBackupKey }, out var libraryError);
                if (documentSucess[NSUrl.IsExcludedFromBackupKey].ToString() == "1" && librarySucess[NSUrl.IsExcludedFromBackupKey].ToString() == "1")
                {
                    ShowMessage("Alert", "Backup Services are blocked");
                }
                else
                {
                    ShowMessage("Success", "Backup can be perfomed");
                }
            }
            catch
            {
                Console.WriteLine("Fail - Backup can be perfomed. Check policy value");
            }
        }

        partial void OpenAirPrintAction(Foundation.NSObject sender)
        {
            var printController = UIPrintInteractionController.SharedPrintController;
            var printInfo = UIPrintInfo.PrintInfo;
            printInfo.OutputType = UIPrintInfoOutputType.General;
            printInfo.JobName = "Print Job";
            printController.PrintInfo = printInfo;
            var formatter = new UIMarkupTextPrintFormatter("Text test");
            formatter.PerPageContentInsets = new UIEdgeInsets(top: 72, left: 72, bottom: 72, right: 72);
            printController.PrintFormatter = formatter;
            printController.Present(true, null);
        }

        partial void OpenAirDropAction(Foundation.NSObject sender)
        {
            var item = UIActivity.FromObject("test");
            var activityItems = new NSObject[] { item };
            UIActivity[] applicationActivities = null;
            try
            {
                var activityController = new UIActivityViewController(activityItems, applicationActivities);
                PresentViewController(activityController, true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        partial void SendEmailAction(Foundation.NSObject sender)
        {
            if (MFMailComposeViewController.CanSendMail)
            {
                var controller = new MFMailComposeViewController();
                controller.SetMessageBody("Message Body", false);
                controller.SetToRecipients(new[] { "test@citri.com" });
                PresentViewController(controller, true, null);
            }
        }
    }
}