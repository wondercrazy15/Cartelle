using System;
using Xamarin.Forms;
using Stance.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using Foundation;

[assembly: ExportRenderer(typeof(ScrollView), typeof(Stance.iOS.ExtendedScrollViewRenderer))]
namespace Stance.iOS
{
    public class ExtendedScrollViewRenderer : ScrollViewRenderer
    {
        //protected override void OnElementChanged(VisualElementChangedEventArgs e)
        //{
        //    base.OnElementChanged(e);
        //    UIScrollView v = (UIScrollView)NativeView;
        //    v.PagingEnabled = false;
        //    v.ScrollsToTop = false;
        //    //v.ScrollEnabled = _IsScrollEnabled;
        //}
        CGPoint offset;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            UIScrollView sv = NativeView as UIScrollView;

            sv.Scrolled += (sender, evt) => {
                // Checking if sv.ContentOffset is not 0,0
                // because the ScrollView resets the ContentOffset to 0,0 when rotation starts
                // even if the ScrollView had been scrolled (I believe this is likely the cause for the bug).
                // so you only want to set offset variable if the ScrollView is scrolled away from 0,0
                // and I do not want to reset offset to 0,0 when the rotation starts, as it would overwrite my saved offset.
                if (sv.ContentOffset.X != 0 || sv.ContentOffset.Y != 0)
                    offset = sv.ContentOffset;

            };
            // Subscribe to the oreintation changed event.
            //NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("handleRotation"), new NSString("UIDeviceOrientationDidChangeNotification"), null);
        }

        public override void LayoutSubviews()
        {
            if (offset.X != 0 || offset.Y != 0)
            {
                UIScrollView sv = NativeView as UIScrollView;
                // Reset the ScrollView offset from the last saved offset.
                sv.ContentOffset = offset;
            }
            base.LayoutSubviews();
        }




    }
}