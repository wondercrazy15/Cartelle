
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

namespace Stance.Droid
{
    [Activity(Label = "Cartelle", HardwareAccelerated = true, MainLauncher = true, Theme = "@style/Splash",
        NoHistory = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}
