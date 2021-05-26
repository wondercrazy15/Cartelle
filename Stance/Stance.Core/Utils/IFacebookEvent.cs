using System;
using System.Collections.Generic;
using System.Text;

namespace Stance.Utils
{
    public interface IFacebookEvent
    {

        //void Activated();

        void AppInstall();

        //void AppLaunch();

        //void LoggedIn();

        //void StartedRegistration();
        //void ClickedGetStarted();

        //void CartelleAcquisition();

        //void AthleteAcquisition();

        //void CompletedOnBoardingProcess();

        //void CompletedRegistration();

        //void SendRevenuePool(int revenuePool);

        //void ViewedCheckout();

        //void InitCheckout();

        void IAP(double price, string currency, string productId, int revenuePool, string username, string transactionId, string store);

    }
}
