using FFImageLoading;
using FFImageLoading.Cache;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class BillingTerms : BaseContentPage
    {
        private const string _PageName = "Billing Terms";
        private bool _isRefreshing = false;

        public BillingTerms(bool isFromIAP)
        {
            InitializeComponent();

            if (isFromIAP)
            {
                ExitBtn.IsVisible = true;
            }
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void TermsOfUse_Clicked(object sender, EventArgs e)
        {
            if (IsInternetConnected())
            {
                Device.OpenUri(new Uri("https://thecartelle.com/terms.html"));
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }

        private async void PrivacyPolicies_Clicked(object sender, EventArgs e)
        {
            if (IsInternetConnected())
            {
                Device.OpenUri(new Uri("https://thecartelle.com/privacy.html"));
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }

        private async void RefreshIAPs_Clicked(object sender, EventArgs e)
        {
            if (_isRefreshing)
                return;

            _isRefreshing = true;
            RefreshBtn.Text = "Refreshing...";

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
                RefreshBtn.Text = "Refresh In App Purchase(s)";
                _isRefreshing = false;
                return;
            }

            try
            {
                bool syncIAPsWorked = await Task.Run(() => WebAPIService.UpdateLocalIAPs()); //Task.Run(() => SyncIAPs());
                if (!syncIAPsWorked)
                {
                    await DisplayAlert("Sync Error", "We had an error syncing your in app purchase(s). Assure you are connected to the internet. Check that you're logged into iTunes and have a valid payment method", "Ok");
                }
                else
                {
                    await DisplayAlert("Refreshed", "Your In App Purchases have been refreshed.", "OK");

                    App.Current.MainPage = new MainStartingPage();
                    return;
                }
            } catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "RefreshIAPs_Clicked" } });
                await DisplayAlert("Error", "We had an error, try again later.", "OK");
            }

            RefreshBtn.Text = "Refresh In App Purchase(s)";
            _isRefreshing = false;
        }

        //private async Task<bool> SyncIAPs()
        //{
        //    try
        //    {
        //        var _connected = await CrossInAppBilling.Current.ConnectAsync();
        //        if (!_connected)
        //            return true;

        //        var allPurchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);

        //        if (allPurchases.Count() == 0)
        //            return true;

        //        var purchases = allPurchases.Where(x => x.ProductId == App._productId_Monthly || x.ProductId == App._productId_Quarterly || x.ProductId == App._productId_Yearly).ToList();

        //        var purchasesToSync = new List<IAPToSyncV2>();
        //        foreach (var item in purchases)
        //        {
        //            if (purchasesToSync.Where(x => x.PruchaseId == item.Id).ToList().Count() == 0)
        //            {
        //                IAPToSyncV2 purchase = new IAPToSyncV2 { PruchaseId = item.Id, ProgramGuid = Guid.Empty };
        //                var newIAP = await _connection.Table<LocalDBIAP>().Where(x => x.PurchaseId == item.Id).FirstOrDefaultAsync();
        //                if (newIAP != null)
        //                {
        //                    if (newIAP.Confirmed)
        //                        continue;

        //                    purchase.Token = newIAP.Token;
        //                    purchase.ProgramGuid = newIAP.ProgramGuid;
        //                }
        //                purchase.Status = (int)item.State;
        //                purchase.ProductId = item.ProductId;
        //                purchase.TransactionDate = item.TransactionDateUtc;
        //                purchasesToSync.Add(purchase);
        //            }
        //        }

        //        if (purchasesToSync.Count() == 0)
        //            return true;

        //        SyncIAPsV2 syncIAPs = new SyncIAPsV2
        //        {
        //            Purchases = purchasesToSync,
        //            DeviceModel = DeviceInfo.Model,
        //            PaymentProvider = 866660001, //Apple
        //            AppVersion = App.AppVersion,
        //            PlatformSource = Device.Idiom == TargetIdiom.Phone ? 866660001 : 866660003,
        //            DeviceOS = 866660001, //apple iOS
        //        };

        //        return await WebAPIService.SyncIAPs(_client, syncIAPs);
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SyncIAPs()" } });
        //        return false;
        //    }
        //}


        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
