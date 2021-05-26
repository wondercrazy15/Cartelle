using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class ConfirmEmail : BaseContentPage
    {
        private const string _PageName = "Confirm Email";
        private bool _SendBtnActive = false;
        private bool _LoadedData = false;

        public ConfirmEmail(bool FromSignUpOrLogin = false)
        {
            InitializeComponent();

            var profile = _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync().Result;
            if (profile != null)
            {
                EmailAddress.Text = profile.Email;
            }

        }

        private async void CheckEmailConfirmation(bool ComingFromBtn)
        {
            var profile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();
            if (profile != null)
            {
                if (profile.ConfirmedEmail)
                {
                    await Navigation.PushModalAsync(new SignUpComplete(), false);
                }
            }

            try
            {
                LockForm("CHECKING...");

                var response = await WebAPIService.CheckEmailConfirmation(_client);
                if (response)
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Email Confirmed" } });
                    DependencyService.Get<IFacebookEvent>().CompletedRegistration();
                    await Navigation.PushModalAsync(new SignUpComplete(), false);
                }
                else
                {
                    if (ComingFromBtn)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Email Not Confirmed" } });
                        //await DisplayAlert("Not Confirmed", response, "Ok");
                        await DisplayAlert("Not Confirmed", "Confirm your email by clicking the confirm button in the email we sent you", "Ok");
                        UnlockForm();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ComingFromBtn)
                {
                    await DisplayAlert("Error", "Something went wrong", "OK");
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Submit_Clicked" } });
                    UnlockForm();
                    return;
                }
            }

            UnlockForm();
        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
            if (_SendBtnActive)
                return;

            LockForm("CHECKING...");

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                UnlockForm();
                return;
            }

            CheckEmailConfirmation(true);
        }

        private async void Resend_Clicked(object sender, EventArgs e)
        {
            //resend confirmation email
            if (_SendBtnActive)
                return;

            LockForm("RESENDING...");

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                UnlockForm();
                return;
            }

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Resending Email Confirmation" } });

                var response = await WebAPIService.ResendEmailConfirmation(_client);
                if (response)
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Email Confirmed Resent" } });
                    UnlockForm();
                    return;
                }
                else
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Resend Email Confirm Error" } });
                    await DisplayAlert("Error", "Something went wrong, try again later.", "OK");
                    UnlockForm();
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Something went wrong.", "OK");
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Resend_Clicked" } });
                UnlockForm();
                return;
            }
        }

        private async void DiffEmail_Clicked(object sender, EventArgs e)
        {
            if (_SendBtnActive)
                return;

            LockForm("CONFIRM EMAIL");
            //logout and send to home page
            var result = await DisplayAlert("Different Email", "Are you sure that you want to use a different email.", "Yes", "No");
            if (result)
            {
                LockForm("LOGGING OUT...");
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Use Different Email" } });
                Task tw = Task.Run(() => Database.ClearAsync());
                tw.Wait();
                Auth.DeleteCredentials();
                App.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.FromHex("#17191A"), BarTextColor = Color.White };
                return;
            }
            else
            {
                UnlockForm();
            }
        }

        private void UnlockForm()
        {
            ConfirmBtn.Text = "CONFIRM EMAIL";
            _SendBtnActive = false;
        }

        private void LockForm(string text)
        {
            _SendBtnActive = true;
            ConfirmBtn.Text = text;
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            CheckEmailConfirmation(false);
            base.OnAppearing();
        }

    }
}
