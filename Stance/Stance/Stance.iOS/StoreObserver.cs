using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Plugin.InAppBilling;
using StoreKit;
using UIKit;

namespace Stance.iOS
{
    class StoreObserver : InAppBillingImplementation
    {
       // public static Func<SKPaymentQueue, SKPayment, SKProduct, bool> OnShouldAddStorePayment { get; set; } = null;

        //PaymentObserver paymentObserver;

        //public StoreObserver()
        //{
        //    paymentObserver = new PaymentObserver(OnPurchaseComplete, OnShouldAddStorePayment);
        //    SKPaymentQueue.DefaultQueue.AddTransactionObserver(paymentObserver);
        //}

    }
}