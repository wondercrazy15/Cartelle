using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using System;
using System.Collections.Generic;
using System.Net;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stance.Pages.Sub
{
    public partial class PurchaseComplete : BaseContentPage
    {
        private const string _PageName = "Purchase Complete";
        private bool _LoadedData = false;
        private bool _IsGettingData = true;

        public PurchaseComplete()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Purchased Synced" } });

            ConfirmBtn.Text = "CONFIRMING...";
            FetchInitialLoad();
        }

        private async void FetchInitialLoad()
        {
            _IsGettingData = true;
            ConfirmBtn.Text = "CONFIRMING...";

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Connect to the internet.", "Ok");
                ConfirmBtn.Text = "CONNECT";
                _IsGettingData = false;
                return;
            }

            var response = await WebAPIService.RefreshApp(_client);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _IsGettingData = false;
                _LoadedData = true;
                ConfirmBtn.Text = "LET'S GO";
                return;
            } else
            {
                ConfirmBtn.Text = "TRY AGAIN";
                _IsGettingData = false;
                return;
            }
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            if (_IsGettingData)
                return;

            if (_LoadedData)
            {
                App.Current.MainPage = new MainStartingPage();
                return;
            }

            FetchInitialLoad();
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() {  { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
