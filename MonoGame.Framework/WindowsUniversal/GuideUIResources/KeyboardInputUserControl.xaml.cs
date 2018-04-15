﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Microsoft.Xna.Framework.WindowsUniversal.GuideUIResources
{
    public sealed partial class KeyboardInputUserControl : UserControl
    {
        private TaskCompletionSource<string> tcs;
        private Action closeMe;
        public KeyboardInputUserControl(TaskCompletionSource<string> _tcs)
        {
           
            tcs = _tcs;
            
            this.InitializeComponent();
        }

        public void SetValues(string title, string description, string defaultText = "", Action _closeMe = null)
        {
            closeMe = _closeMe;
            this.Title.Text = title;
            this.Description.Text = description;
            if (!string.IsNullOrWhiteSpace(defaultText))
            {
                TextEntry.Text = defaultText;
            }

        }

        private void OkayTapped(object sender, TappedRoutedEventArgs e)
        {
            tcs.TrySetResult(TextEntry.Text);
            closeMe?.Invoke();
        }

        private void CancelTapped(object sender, TappedRoutedEventArgs e)
        {
            tcs.TrySetCanceled();
            closeMe?.Invoke();
        }
    }
}
