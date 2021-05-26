using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Stance.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(DisabledButtonRenderer))]

namespace Stance.iOS
{
    public class DisabledButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetTitleColor(UIColor.White, UIControlState.Disabled);
            }
        }

    }
}