using BranchXamarinSDK;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using Stance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stance.Pages.Sub
{
    public partial class IAP_FIF : BaseContentPage
    {
        private const string _PageName = "IAP_FIF";
        private bool _sendingIAP = false;
        private bool _connected = false;
        private SyncIAP _newToSyncIAP = new SyncIAP();
        private string _PurchaseId = "";
        private Guid _ProgramGuid = Guid.Empty;
        private LocalDBContactProgram _ContactProgram = new LocalDBContactProgram();
        private IEnumerable<InAppBillingProduct> _prodInfo;
        private bool _popIAPModal = false;
        //#2ecc71 past greeen color for most popular

        public IAP_FIF(Guid ProgramGuid, LocalDBContactProgram ContactProgram)
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            _ProgramGuid = ProgramGuid;
            _ContactProgram = ContactProgram;
            SetupIAPOptions();
            SetupCountDown();
        }

        private void SetupCountDown()
        {
            if (_ContactProgram.StartDate.HasValue)
            {
                if ((DateTime)_ContactProgram.StartDate?.AddDays(7) > DateTime.UtcNow)
                {
                    var diff = (DateTime)_ContactProgram.StartDate?.AddDays(7) - DateTime.UtcNow;
                    Timer.Text = ToReadableString(diff);
                    Timer.IsVisible = true;
                    Device.StartTimer(TimeSpan.FromSeconds(1), TimerElapsed);
                }
            }
        }

        private bool TimerElapsed()
        {
            if (_ContactProgram.StartDate.HasValue)
            {
                if ((DateTime)_ContactProgram.StartDate?.AddDays(7) > DateTime.UtcNow)
                {
                    var diff = (DateTime)_ContactProgram.StartDate?.AddDays(7) - DateTime.UtcNow;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Timer.Text = ToReadableString(diff);
                    });
                    return true;
                }
            }
            return false;
        }

        public static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0}day{1} ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0}hr{1} ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0}min{1} ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0}s{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }

        private async void SetupIAPOptions()
        {
            try
            {
                _connected = await CrossInAppBilling.Current.ConnectAsync(ItemType.Subscription);
                if (!_connected)
                {
                    //Couldn't connect to billing, could be offline, alert user
                    return;
                }

                _prodInfo = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.Subscription, App._productId_Monthly_NoTrial, App._productId_Yearly_NoTrial, App._productId_Monthly);
                var monthlySub = _prodInfo.Where(x => x.ProductId == App._productId_Monthly_NoTrial).FirstOrDefault();
                var yearlySub = _prodInfo.Where(x => x.ProductId == App._productId_Yearly_NoTrial).FirstOrDefault();
                var monthlySubWithTrial = _prodInfo.Where(x => x.ProductId == App._productId_Monthly).FirstOrDefault();

                var currentSymbol = Regex.Replace(monthlySub.LocalizedPrice, @"[\d-]", string.Empty).Replace(".", "");

                monthlyMonthlyCost.Text = monthlySub.LocalizedPrice;

                decimal ymc = Decimal.Round((decimal)yearlySub.MicrosPrice / 12000000M, 2, MidpointRounding.AwayFromZero);
                yearlyMonthlyCost.Text = yearlySub.LocalizedPrice;

                monthlyCost.Text = monthlySub.LocalizedPrice + "/month";
                yearlyCost.Text = currentSymbol + ymc + "/month";

                var dealAmount = Decimal.Round((1M - (decimal)yearlySub.MicrosPrice / ((decimal)monthlySubWithTrial.MicrosPrice * 12M)) * 100M, 0, MidpointRounding.AwayFromZero);
                DealAmount.Text = dealAmount + "% OFF";

                var dealAmountMonthly = Decimal.Round((1M - ((decimal)monthlySub.MicrosPrice) / ((decimal)monthlySubWithTrial.MicrosPrice)) * 100M, 0, MidpointRounding.AwayFromZero);
                DealAmountMonthly.Text = dealAmountMonthly + "% OFF";

                var tapGestureRecognizer = new TapGestureRecognizer
                {
                    Command = new Command<string>(IAP_Buy),
                    CommandParameter = App._productId_Monthly_NoTrial,
                    NumberOfTapsRequired = 1
                };
                MonthlySubSL.GestureRecognizers.Add(tapGestureRecognizer);

                var tapGestureRecognizer2 = new TapGestureRecognizer
                {
                    Command = new Command<string>(IAP_Buy),
                    CommandParameter = App._productId_Yearly_NoTrial,
                    NumberOfTapsRequired = 1
                };
                YearlySubSL.GestureRecognizers.Add(tapGestureRecognizer2);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Couldn't load in-app purchases. Try to exit and end this screen again to retry.", "Ok");
                return;
            }

            IAPSL.IsVisible = true;
            NotLoadedYet.IsVisible = false;
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            //if (!_popIAPModal)
            //{
            //    _popIAPModal = true;
            //    await Navigation.PushModalAsync(new AppStoreVideo(), true);
            //}
            //else
            //{
            //    await Navigation.PopModalAsync();
            //}

            await Navigation.PopModalAsync();

        }

        private async void BillingTerms_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new BillingTerms(true), true);
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

        private void StartFreeTrial_Clicked(object sender, EventArgs e)
        {
            IAP_Buy(App._productId_Yearly_NoTrial);
        }

        private async void IAP_Buy(string productId)
        {
            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                return;
            }

            if (_sendingIAP)
                return;

            _sendingIAP = true;
            IAPSL.IsVisible = false;
            NotLoadedYet.IsVisible = true;

            //DependencyService.Get<IFacebookEvent>().InitCheckout();
            Branch branch = Branch.GetInstance();
            branch.SendEvent(new BranchEvent(BranchEventType.INITIATE_PURCHASE));
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "IAP", productId }, { "Action", "Selected IAP" } });

            try
            {
                _connected = await CrossInAppBilling.Current.ConnectAsync(ItemType.Subscription);
                if (!_connected)
                {
                    //Couldn't connect to billing, could be offline, alert user
                    await DisplayAlert("Offline", "Couldn't connect to the App Store. Check your internet and try again.", "Ok");
                    return;
                }

                //try to purchase item
                var purchase = await CrossInAppBilling.Current.PurchaseAsync(productId, ItemType.Subscription, Auth.Username);
                if (purchase == null)
                {
                    //Not purchased, alert the user
                }
                else
                {
                    //Purchased, save this information
                    try
                    {
                        var purchaseId = purchase.Id;
                        _PurchaseId = purchaseId;
                        var token = purchase.PurchaseToken;
                        var state = purchase.State;
                        var purchaseDate = purchase.TransactionDateUtc;

                        //Save this to the localDB then sync to server
                        var newIAPDB = new LocalDBIAP
                        {
                            PurchaseId = purchaseId,
                            Token = token,
                            State = (int)state,
                            PurchaseDate = purchaseDate,
                            IsSynced = false,
                            Confirmed = false,
                            ProgramGuid = _ProgramGuid,
                            //IsInTestingMode = isTestingMode,
                        };
                        await _connection.InsertAsync(newIAPDB);

                        _newToSyncIAP = new SyncIAP
                        {
                            Token = token,
                            DeviceModel = DeviceInfo.Model,
                            PaymentProvider = 866660001, //Apple
                            AppVersion = App.AppVersion,
                            PlatformSource = Device.Idiom == TargetIdiom.Phone ? 866660001 : 866660003,//if iphone else is ipad
                            DeviceOS = 866660001, //apple iOS
                            ProgramGuid = _ProgramGuid,
                            //IsInTestingMode = isTestingMode
                        };

                        bool didSync = await WebAPIService.SyncNewIAP(_client, _newToSyncIAP);

                        int revenuePool = 0;
                        var Profile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();
                        if (Profile != null)
                        {
                            revenuePool = Profile.RP;
                        }
                        SendBranchEvent(branch, revenuePool, productId);
                        var SubPurchased = _prodInfo.Where(x => x.ProductId == productId).FirstOrDefault();
                        DependencyService.Get<IFacebookEvent>().IAP(((double)SubPurchased.MicrosPrice / 1000000D), SubPurchased.CurrencyCode, productId, revenuePool, "", _PurchaseId, "Apple App Store");
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "IAP", productId }, { "Action", "Purchased" } });

                        if (didSync)
                        {
                            var newIAP = await _connection.Table<LocalDBIAP>().Where(x => x.PurchaseId == purchaseId).FirstOrDefaultAsync();
                            newIAP.IsSynced = true;
                            newIAP.Confirmed = true;
                            await _connection.UpdateAsync(newIAP);
                        }
                        else
                        {
                            didSync = await WebAPIService.SyncNewIAP(_client, _newToSyncIAP);
                            if (didSync)
                            {
                                var newIAP = await _connection.Table<LocalDBIAP>().Where(x => x.PurchaseId == purchaseId).FirstOrDefaultAsync();
                                newIAP.IsSynced = true;
                                newIAP.Confirmed = true;
                                await _connection.UpdateAsync(newIAP);
                            }
                        }

                        if (didSync)
                        {
                            await Navigation.PushModalAsync(new PurchaseComplete(), false);
                            return;
                        }
                        else
                        {
                            //deal with error of not syncing
                            //show button to try to sync again until it's synced
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Username", Auth.Username + "_" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss") }, { "Action", "Sync Issue" } });
                            SyncSL.IsVisible = true;
                            mainScrollView.IsVisible = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "IAP_Buy" }, { "Error", "IAP Error: " + Auth.Username } });
                        await DisplayAlert("Error", "We had an error. Please contact us.", "Ok");
                    }
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                var message = string.Empty;
                switch (purchaseEx.PurchaseError)
                {
                    case PurchaseError.AppStoreUnavailable:
                        message = "Currently the App Store seems to be unavailble. Try again later.";
                        break;
                    case PurchaseError.BillingUnavailable:
                        message = "Billing seems to be unavailable, please try again later.";
                        break;
                    case PurchaseError.PaymentInvalid:
                        message = "Payment seems to be invalid, please try again.";
                        break;
                    case PurchaseError.PaymentNotAllowed:
                        message = "Payment does not seem to be enabled/allowed, please try again.";
                        break;
                }

                //Decide if it is an error we care about
                if (string.IsNullOrWhiteSpace(message))
                    message = "Cannot connect to iTunes Store. If you already have a Membership, try to refresh your Purchase by going to the More Tab > Billing Terms";

                //Display message to user
                Crashes.TrackError(purchaseEx, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "IAP_Buy" }, { "Error", "AppStore Purchase Error" } });
                await DisplayAlert("Purchase Error", message, "Ok");

            }
            catch (Exception ex)
            {
                //Something bad has occurred, alert user
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "IAP_Buy" }, { "Error", "Unable to Purchase" } });
                await DisplayAlert("Unable to Purchase", "We could not process your purchase at this time. Check that your Apple ID is properly connected to the App Store and that you have accepted Apple's latest agreement (Settings > Click Your Name > iTunes & App Store > Apple ID).", "Ok");
            }
            finally
            {
                _sendingIAP = false;
                IAPSL.IsVisible = true;
                NotLoadedYet.IsVisible = false;
            }
        }

        private void SendBranchEvent(Branch branch, int revenuePool, string productId)
        {
            string affiliation = "Unknown";
            if (revenuePool == 866660001)
            {
                affiliation = "Cartelle";
            }
            else if (revenuePool == 866660000)
            {
                affiliation = "Athlete";
            }

            var prodInfo = _prodInfo.Where(x => x.ProductId == productId).FirstOrDefault();

            BranchEvent purchaseEvent = new BranchEvent(BranchEventType.PURCHASE);
            purchaseEvent.AddCustomData("Store", "Apple");
            purchaseEvent.SetAffiliation(affiliation);
            purchaseEvent.SetDescription(productId);
            purchaseEvent.AddCustomData("Price", prodInfo.LocalizedPrice);

            var result = GetBranchCurrencyCode(prodInfo);
            if (result.Item1 == true)
            {
                purchaseEvent.SetCurrency(result.Item2);
                decimal price = Decimal.Round((decimal)prodInfo.MicrosPrice / 1000000M, 2, MidpointRounding.AwayFromZero);
                purchaseEvent.SetRevenue((float)price);
            }
            else
            {
                purchaseEvent.SetCurrency(BranchCurrencyType.USD);
                if (productId == App._productId_Monthly_NoTrial)
                {
                    purchaseEvent.SetRevenue(9.99F);
                }
                else if (productId == App._productId_Yearly_NoTrial)
                {
                    purchaseEvent.SetRevenue(34.99F);
                }
            }
            branch.SendEvent(purchaseEvent);
        }

        private Tuple<bool, BranchCurrencyType> GetBranchCurrencyCode(InAppBillingProduct prodInfo)
        {
            if ("USD" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.USD);
            }
            else if ("GBP" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.GBP);
            }
            else if ("CAD" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.CAD);
            }
            else if ("MXN" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.MXN);
            }
            else if ("AUD" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.AUD);
            }
            else if ("ZAR" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.ZAR);
            }
            else if ("EUR" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.EUR);
            }
            else if ("PHP" == prodInfo.CurrencyCode)
            {
                return new Tuple<bool, BranchCurrencyType>(true, BranchCurrencyType.PHP);
            }

            return new Tuple<bool, BranchCurrencyType>(false, BranchCurrencyType.USD);

        }

        private async void SyncBtn_Clicked(object sender, EventArgs e)
        {
            if (_sendingIAP)
                return;

            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Clicked Sync Btn" } });

            if (_PurchaseId == "")
            {
                await DisplayAlert("Error", "Contact us.", "Ok");
                return;
            }

            _sendingIAP = true;
            SyncBtn.Text = "SYNCING";

            bool didSync = await WebAPIService.SyncNewIAP(_client, _newToSyncIAP);
            if (didSync)
            {
                var newIAP = await _connection.Table<LocalDBIAP>().Where(x => x.PurchaseId == _PurchaseId).FirstOrDefaultAsync();
                newIAP.IsSynced = true;
                await _connection.UpdateAsync(newIAP);
                await Navigation.PushModalAsync(new PurchaseComplete(), false);
                return;
            }

            await DisplayAlert("Sync Error", "Connect to the internet and try to sync again.", "Ok");
            _sendingIAP = false;
            SyncBtn.Text = "SYNC";
        }

        protected async override void OnDisappearing()
        {
            try
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
            catch (Exception ex)
            {

            }
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            //DependencyService.Get<IFacebookEvent>().ViewedCheckout();
            Branch branch = Branch.GetInstance();
            branch.SendEvent(new BranchEvent("Viewed_Checkout_FIF"));

            base.OnAppearing();
        }

    }
}
