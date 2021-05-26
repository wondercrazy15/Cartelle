﻿using System;
using Stance.Droid.CustomControll;
using Stance.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomProgressBar), typeof(CustomProgressBarRenderer))]
namespace Stance.Droid.CustomControll
{
    [Obsolete]
    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);
            //Control.ProgressDrawable.SetColorFilter(Color.FromRgb(232, 232, 232).ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn);

            // Control.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromRgb(232, 232, 232).ToAndroid()); //Change the color
             Control.ScaleY = 3; //Changes the height

        }
    }
}
