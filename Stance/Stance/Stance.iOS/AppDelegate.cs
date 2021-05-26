using Foundation;
using UIKit;
//using ImageCircle.Forms.Plugin.iOS;
using Octane.Xamarin.Forms.VideoPlayer.iOS;
using AudioToolbox;
using Microsoft.AppCenter.Push;
using SegmentedControl.FormsPlugin.iOS;
using Facebook.CoreKit;
using StoreKit;
using System.Linq;
using BranchXamarinSDK;
using Stance.Utils;


namespace Stance.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.

    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        string appId = "259417778087969";//Prod: "259417778087969"; Test: "1956097581361493"
        string appName = "Cartelle Fitness App";//Prod: "Cartelle Fitness App"; Test: "Cartelle Fitness App Test"
        //StoreObserver iapObserver = new StoreObserver();

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();

            var obj = new Stance.iOS.SQLiteDb(); // otherwise throws missing constructor error
                                                 //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Profile.EnableUpdatesOnAccessTokenChange(true);
            Settings.AppId = appId;
            Settings.DisplayName = appName;

            UIApplication.SharedApplication.IdleTimerDisabled = true;
            //FormsVideoPlayer.Init("BC4E2C14506C1904C05CCE3B98D7A3BCDED90B60");


            FormsVideoPlayer.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Global.VideoPlayer.iOS.VideoPlayer.Init();
            //ImageCircleRenderer.Init();
            AudioSession.Initialize();
            AudioSession.Category = AudioSessionCategory.MediaPlayback;
            AudioSession.Mode = AudioSessionMode.Default;
            AudioSession.AudioShouldDuck = true;
            AudioSession.OverrideCategoryMixWithOthers = true; 
            AudioSession.SetActive(true);
            SegmentedControlRenderer.Init();

  
           // Plugin.InAppBilling.InAppBillingImplementation.OnShouldAddStorePayment = OnShouldAddStorePayment;
            //var current = Plugin.InAppBilling.CrossInAppBilling.Current;

            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                TextColor = UIColor.White, //.FromHex("#00bac6"),
                Font = UIFont.FromName("AvenirNextCondensed-Medium", 16),
            });

            // Debug mode - set to 'false' before releasing to production
            //BranchIOS.Debug = false;
            App appBUO = new App();
            BranchIOS.Init("key_live_jjQpL5Qu1AcVswe2Go1QPphlssohNEvW", options, appBUO);//key_test_ikVoMYJt1zpKysc1Sp0GGlodBrmpUyHj
            LoadApplication(appBUO);

            return base.FinishedLaunching(app, options);
        }

        // Called when the app is opened via URI scheme
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return BranchIOS.getInstance().OpenUrl(url);
        }

        // Called when the app is opened from a Universal Link 
        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            return BranchIOS.getInstance().ContinueUserActivity(userActivity);
        }

        // Called when the app receives a push notification
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            BranchIOS.getInstance().HandlePushNotification(userInfo);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, System.Action<UIBackgroundFetchResult> completionHandler)
        {
            var result = Push.DidReceiveRemoteNotification(userInfo);
            if (result)
            {
                completionHandler?.Invoke(UIBackgroundFetchResult.NewData);
            }
            else
            {
                completionHandler?.Invoke(UIBackgroundFetchResult.NoData);
            }
        }

        //public bool OnShouldAddStorePayment(SKPaymentQueue queue, SKPayment payment, SKProduct product)
        //{
        //    // true in app purchase is initiated, false cancels it.
        //    // you can check if it was already purchased.
        //    return true;
        //}

        //public override void WillTerminate(UIApplication uiApplication)
        //{
        //    SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(iapObserver);
        //    base.WillTerminate(uiApplication);
        //}

        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    throw new Exception("Sender: " + sender.ToString() + "@ Error: " + e.ToString());
        //}
    }
}
