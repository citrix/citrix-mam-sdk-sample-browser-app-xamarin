/*
 * Copyright © 2022. Cloud Software Group, Inc. All Rights Reserved. Confidential & Proprietary.
 *
 * Use and support of this software is governed by the terms
 * and conditions of the software license agreement and support
 * policy of Cloud Software Group, Inc. and/or its subsidiaries.
 *
 */

using Xamarin.Forms;

namespace MvpnTestFormsApp
{
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage(string address)
        {
            InitializeComponent();
            webView.Source = address;
        }
    }
}
