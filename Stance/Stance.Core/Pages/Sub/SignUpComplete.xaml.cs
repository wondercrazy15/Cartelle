using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class SignUpComplete : BaseContentPage
    {
        private const string _PageName = "Completed SignUp";
        private bool _LoadedData = false;
        private bool _IsGettingData = true;

        public SignUpComplete()
        {
            InitializeComponent();

            ConfirmBtn.Text = "Connecting...";
            FetchInitialLoad();
        }

        private async void FetchInitialLoad()
        {
            _IsGettingData = true;
            ConfirmBtn.Text = "Connecting...";

            //var ContactPrograms = await _connection.Table<LocalDBContactProgram>().ToListAsync();
            //if(ContactPrograms.Count() != 0)
            //{
            //    _IsGettingData = false;
            //    _LoadedData = true;
            //    ConfirmBtn.Text = "Let's Go";
            //    return;
            //}

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Connect to the internet.", "Ok");
                ConfirmBtn.Text = "Connect";
                _IsGettingData = false;
                return;
            }

            var response = await WebAPIService.RefreshApp(_client);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _IsGettingData = false;
                _LoadedData = true;
                ConfirmBtn.Text = "Let's Go";
                return;
            } else
            {
                ConfirmBtn.Text = "Try Again";
                _IsGettingData = false;
                return;
            }
        }

        private async void Submit_Clicked(object sender, EventArgs e)
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
