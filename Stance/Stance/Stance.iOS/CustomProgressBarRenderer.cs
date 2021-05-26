using CoreGraphics;
using Stance;
using Stance.iOS;
using Stance.Pages.Sub;
using Stance.Utils;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomProgressBar), typeof(CustomProgressBarRenderer))]
namespace Stance.iOS
{
    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            try
            {
                base.OnElementChanged(e);
                if(Control != null)
                {
                    Control.ProgressTintColor = Color.FromHex("#00BBCB").ToUIColor();//Color.FromHex("#76E982").ToUIColor();//Color.FromRgb(182, 231, 233).ToUIColor();//
                    Control.TrackTintColor = Color.Transparent.ToUIColor();
                }
             
            } catch(Exception ex)
            {
                var st = ex.ToString();
            }           
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var X = 1.0f;
            var Y = 10.0f;

            CGAffineTransform transform = CGAffineTransform.MakeScale(X, Y);
            this.Transform = transform;

            this.ClipsToBounds = true;
            this.Layer.MasksToBounds = true;
            this.Layer.CornerRadius = 0; // This is for rounded corners.
        }



    }
}