using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using Microsoft.AppCenter.Push;
using Octane.Xamarin.Forms.VideoPlayer.Android;
using SegmentedControl.FormsPlugin.Android;
using FFImageLoading.Forms.Platform;
using BranchXamarinSDK;
using MediaManager;
using Acr.UserDialogs;
using Stance.Pages.Main;
using Plugin.InAppBilling;
using Android.Content;
using Plugin.CurrentActivity;

namespace Stance.Droid
{

    [Activity(Label = "Cartelle", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, HardwareAccelerated = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    [assembly: Dependency(typeof(MainActivity))]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                //MainStartingPage.EmulateBackPressed = OnBackPressed;
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;
                BranchAndroid.Debug = true;
                base.OnCreate(bundle);
                
                global::Xamarin.Forms.Forms.Init(this, bundle);
                //FormsVideoPlayer.Init();
                FormsVideoPlayer.Init("FAB8932451217CD6D68F4D27E0E2B0CF1D274C63");
                CachedImageRenderer.Init(enableFastRenderer: true);
                UserDialogs.Init(this);
                CrossCurrentActivity.Current.Init(this, bundle);
                ImageCircleRenderer.Init();
                SegmentedControlRenderer.Init();
                Xamarin.Essentials.Platform.Init(this, bundle);
                Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
                var pixels = Resources.DisplayMetrics.WidthPixels;
                var scale = Resources.DisplayMetrics.Density;

                var dps = (double)((pixels - 0.5f) / scale);

                var ScreenWidth = (int)dps;

                App.screenWidth = ScreenWidth;

                //RequestedOrientation = ScreenOrientation.Portrait;

                pixels = Resources.DisplayMetrics.HeightPixels;
                dps = (double)((pixels - 0.5f) / scale);

                var ScreenHeight = (int)dps;
                App.screenHeight = ScreenHeight;

               
                SegmentedControlRenderer.Init();

                App appBUO = new App();

                // BranchIOS.Init("key_live_jjQpL5Qu1AcVswe2Go1QPphlssohNEvW", options, appBUO);//key_test_ikVoMYJt1zpKysc1Sp0GGlodBrmpUyHj
                BranchAndroid.Init(this, "key_test_ikVoMYJt1zpKysc1Sp0GGlodBrmpUyHj", appBUO);

                LoadApplication(new App());
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);
            Push.CheckLaunchedFromNotification(this, intent);
        }


    }
}

