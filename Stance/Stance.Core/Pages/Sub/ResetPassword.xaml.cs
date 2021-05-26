using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Stance.Utils;
using Stance.Utils.Auth;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stance.Pages.Sub
{
    public partial class ResetPassword : BaseContentPage
    {
        private const string _PageName = "Reset Password";
        public bool _SendBtnActive = false;

        public ResetPassword()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            Spinner.IsVisible = false;


            if (Device.Idiom == TargetIdiom.Phone)
            {
                SignInBox.Padding = new Thickness(40, 100, 40, 60);
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                SignInBox.Padding = new Thickness(200, 200, 200, 60);
            }
            else
            {
                SignInBox.Padding = new Thickness(40, 100, 40, 60);
            }

        }

        private async void Entry_Changed(object sender, EventArgs e)
        {
            try
            {
                if (Email.Text.Length >= 7)
                {
                    JoinNowBtn.BackgroundColor = Color.FromHex("#00BBCB");
                    JoinNowBtn.Opacity = 1;
                    JoinNowBtn.IsEnabled = true;
                }
                else
                {
                    JoinNowBtn.BackgroundColor = Color.FromHex("#4F6572");
                    JoinNowBtn.Opacity = 0.5;
                    JoinNowBtn.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                var err = ex.ToString();
            }

        }

        private async void ResetPasswordBtn_Clicked(object sender, EventArgs e)
        {
            if (_SendBtnActive)
                return;

            _SendBtnActive = true;
            Spinner.IsVisible = true;
            FormValidationSpecialMessage.Text = "";
            Email.IsEnabled = false;
            EmailError.BackgroundColor = Color.Transparent;

            Regex emailReg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);

            if (Email.Text == null || Email.Text == "")
            {
                FormValidationSpecialMessage.Text = "Enter your email.";
                EmailError.BackgroundColor = Color.Red;
                UnlockForm();
            }
            else if (!emailReg.Match(Email.Text).Success)
            {
                EmailError.BackgroundColor = Color.Red;
                FormValidationSpecialMessage.Text = "Email doesn't look real.";
                UnlockForm();
            }
            else
            {
                if (IsInternetConnected())
                {
                    try
                    {
                       JoinNowBtn.Text = "PROCESSING...";
                        //HttpClient _client = new HttpClient(new NativeMessageHandler());
                        //string json = JsonConvert.SerializeObject(Email.Text);
                        //var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                        var newUri = new Uri(App._absoluteUri, App._resetpasswordUri + Email.Text);
                        _client.DefaultRequestHeaders.Clear();
                        _client.DefaultRequestHeaders.Add("Authorization", GeneralAuth.Token);
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Resetting Password" } });
                        HttpResponseMessage response = await _client.GetAsync(newUri);
                        //var res = response.Content.ReadAsStringAsync().Result;

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sent Reset Password Email" } });
                            FormValidationSpecialMessage.Text = "We sent you an Email to reset your password.";
                            FormValidationSpecialMessage.TextColor = Color.White;
                            FormValidationSpecialMessage.FontSize = 22;
                            JoinNowBtn.IsVisible = false;
                            EmailError.IsVisible = false;
                            Spinner.IsVisible = false;
                        }
                        else
                        {
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Error Resetting Password" } });
                            FormValidationSpecialMessage.Text = "Check that your Email is spelled correctly.";
                            EmailError.BackgroundColor = Color.Red;
                            UnlockForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ResetPasswordBtn_Clicked" } });
                        FormValidationSpecialMessage.Text = "Something went wrong.";
                        EmailError.BackgroundColor = Color.Red;
                        UnlockForm();
                    }
                }
                else
                {
                    await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                    UnlockForm();
                }
            }
            _SendBtnActive = false;

        }

        private void UnlockForm()
        {
           JoinNowBtn.Text = "RESET PASSWORD";
            Email.IsEnabled = true;
            Spinner.IsVisible = false;
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}