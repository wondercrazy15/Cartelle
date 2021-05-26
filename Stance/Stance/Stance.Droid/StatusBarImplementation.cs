using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Stance.Utils;
using Stance.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarImplementation))]
namespace Stance.Droid
{
public class StatusBarImplementation : IStatusBar
    {
        public StatusBarImplementation()
        {
        }

        WindowManagerFlags _originalFlags;

        #region IStatusBar implementation

        public void HideStatusBar()
        {
            var activity = (Activity)Forms.Context;
            var attrs = activity.Window.Attributes;
            _originalFlags = attrs.Flags;
            attrs.Flags |= Android.Views.WindowManagerFlags.Fullscreen;
            activity.Window.Attributes = attrs;
        }

        public void ShowStatusBar()
        {
            var activity = (Activity)Forms.Context;
            var attrs = activity.Window.Attributes;
            attrs.Flags = _originalFlags;
            activity.Window.Attributes = attrs;
        }

        #endregion
    }
}