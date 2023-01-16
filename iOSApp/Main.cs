/*
 * Copyright © 2022. Cloud Software Group, Inc. All Rights Reserved. Confidential & Proprietary.
 *
 * Use and support of this software is governed by the terms
 * and conditions of the software license agreement and support
 * policy of Cloud Software Group, Inc. and/or its subsidiaries.
 *
 */

using UIKit;

namespace MvpnTestIOSApp
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
