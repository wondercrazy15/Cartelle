using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Stance.Utils;
using UIKit;
using Stance.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarImplementation))]
namespace Stance.iOS
{
public class StatusBarImplementation : IStatusBar
    {
        public StatusBarImplementation()
        {
        }

        #region IStatusBar implementation

        public void HideStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = true;
        }

        public void ShowStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
        }

        #endregion
    }
}