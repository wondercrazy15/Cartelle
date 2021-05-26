using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Stance.Utils;
using Stance.Utils.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stance.Pages.Sub
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResetPassword : BaseContentPage
    {
        private const string _PageName = "Reset Password";
        public bool _SendBtnActive = false;

        public ResetPassword()
        {
            InitializeComponent();
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

        private async void ResetPasswordBtn_Clicked(object sender, EventArgs e)
        {
            if (!_SendBtnActive)
            {
                _SendBtnActive = true;
                Spinner.IsVisible = true;
                FormMessage.Text = "";                
                Email.IsEnabled = false;
                Regex emailReg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);

                if (Email.Text == null || Email.Text == "")
                {
                    FormMessage.Text = "Enter your Email";
                    UnlockForm();
                }
                else if (!emailReg.Match(Email.Text).Success)
                {
                    Email.BackgroundColor = Color.FromHex("#ffb2b2");
                    FormMessage.Text = "Email doesn't look real";
                    UnlockForm();
                }
                else
                {
                    if (IsInternetConnected())
                    {
                        try
                        {
                            ResetPasswordBtn.Text = "PROCESSING...";
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
                                FormMessage.Text = "We sent you an Email to reset your password";
                                FormMessage.TextColor = Color.White;
                                FormMessage.FontSize = 22;
                                ResetPasswordBtn.IsVisible = false;
                                Email.IsVisible = false;
                                Spinner.IsVisible = false;
                            }
                            else
                            {
                                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Error Resetting Password" } });
                                await DisplayAlert("ERROR", "Something went wrong. Check that your email is spelled correctly.", "OK");
                                UnlockForm();
                            }
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("ERROR", "Something went wrong.", "OK");
                            Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ResetPasswordBtn_Clicked" } });
                            UnlockForm();
                        }
                    }
                    else
                    {
                        await DisplayAlert("NO INTERNET", "Please connect to the internet and try again.", "OK");
                        UnlockForm();
                    }
                }

                _SendBtnActive = false;
            }

        }

        private void UnlockForm()
        {
            ResetPasswordBtn.Text = "RESET PASSWORD";
            Email.IsEnabled = true;
            Spinner.IsVisible = false;
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
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