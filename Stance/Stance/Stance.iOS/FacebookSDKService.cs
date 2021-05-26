using Facebook.CoreKit;
using Foundation;
using Stance.iOS;
using Stance.Utils;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(FacebookSDKService))]
namespace Stance.iOS
{
    public class FacebookSDKService : IFacebookEvent
    {
        public FacebookSDKService() { }

        //public void Activated()
        //{
        //    Facebook.CoreKit.AppEvents.ActivateApp();
        //}

        public void AppInstall()
        {
            Facebook.CoreKit.AppEvents.LogEvent("Install_Initial");
        }

        //public void AppLaunch()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("App_Launch");
        //}

        //public void LoggedIn()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Logged In");
        //}

        //public void StartedRegistration()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Started Registration");
        //}

        //public void ClickedGetStarted()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Clicked Get Started");
        //}

        //public void CartelleAcquisition()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Cartelle Acquisition");
        //}

        //public void AthleteAcquisition()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Athlete Acquisition");
        //}

        //public void CompletedOnBoardingProcess()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Completed OnBoarding Process");
        //}

        //public void CompletedRegistration()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Completed Registration");
        //}

        //public void ViewedCheckout()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Viewed Checkout");
        //}

        //public void InitCheckout()
        //{
        //    Facebook.CoreKit.AppEvents.LogEvent("Initialized Checkout");
        //}

        //public void SendRevenuePool(int revenuePool)
        //{
        //    string RevenuePool = "Unknown Revenue";
        //    if (revenuePool == 866660000)
        //    {
        //        RevenuePool = "Athlete Revenue";
        //    }
        //    else if (revenuePool == 866660001)
        //    {
        //        RevenuePool = "Cartelle Revenue";
        //    }
        //    Facebook.CoreKit.AppEvents.LogEvent(RevenuePool);
        //}

        public void IAP(double price, string currency, string productId, int revenuePool, string username, string transactionId, string store)
        {
            string RevenuePool = "Unknown";
            if (revenuePool == 866660000)
            {
                RevenuePool = "Athlete";
            }
            else if (revenuePool == 866660001)
            {
                RevenuePool = "Cartelle";
            }
            Facebook.CoreKit.AppEvents.LogPurchase(price, currency, new NSDictionary("ProductId", productId, "RevenuePool", RevenuePool, "Username", username, "TransactionId", transactionId, "Store", store));
        }

   
    }
}