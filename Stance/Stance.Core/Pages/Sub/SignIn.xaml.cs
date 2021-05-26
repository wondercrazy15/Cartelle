using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using StanceWeb.Models.App.Optimized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stance.Pages.Sub
{
    public partial class SignIn : BaseContentPage
    {
        private const string _PageName = "Sign In";
        public string _programId = String.Empty;
        public Guid _programGuid = Guid.Empty;
        private bool _IsSigninIn = false;

        public SignIn()
        {
            try
            {
                InitializeComponent();
                Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");
                On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

                Spinner.IsVisible = false;
                //Task.Factory.StartNew(Database.ClearAsync).Wait();
                //Task.Factory.StartNew(Database.CreateAsync).Wait();

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    SignInBox.Padding = new Thickness(40, 60, 40, 60);
                    //Welcome.FontSize = 21;
                    cartelleLogo.WidthRequest = 180;
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    SignInBox.Padding = new Thickness(200, 160, 200, 60);
                    // Welcome.FontSize = 27;
                    cartelleLogo.WidthRequest = 240;
                }
                else
                {
                    SignInBox.Padding = new Thickness(40, 60, 40, 60);
                    // Welcome.FontSize = 21;
                    cartelleLogo.WidthRequest = 180;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private async void Entry_Changed(object sender, EventArgs e)
        {
            try
            {
                if (Email.Text.Length >= 7 && Password.Text.Length >= 5)
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

        private async void SignInSubmitted(object sender, EventArgs e)
        {
            try
            {
                if (_IsSigninIn)
                    return;

                _IsSigninIn = true;
                Spinner.IsVisible = true;
                FormValidationSpecialMessage.Text = "";
                Email.IsEnabled = false;
                Password.IsEnabled = false;

                if (Email.Text == null || Email.Text == "")
                {
                    Spinner.IsVisible = false;
                    FormValidationSpecialMessage.Text = "Enter your email.";
                    EmailError.BackgroundColor = Color.Red;
                    Email.IsEnabled = true;
                    Password.IsEnabled = true;
                    _IsSigninIn = false;
                    return;
                }
                else
                {
                    EmailError.BackgroundColor = Color.Transparent;
                }

                if (Password.Text == null || Password.Text == "")
                {
                    Spinner.IsVisible = false;
                    FormValidationSpecialMessage.Text = "Enter your password.";
                    PasswordError.BackgroundColor = Color.Red;
                    Email.IsEnabled = true;
                    Password.IsEnabled = true;
                    _IsSigninIn = false;
                    return;
                }
                else
                {
                    PasswordError.BackgroundColor = Color.Transparent;
                }

                if (!IsInternetConnected())
                {
                    JoinNowBtn.Text = "LOGIN";
                    Spinner.IsVisible = false;
                    FormValidationSpecialMessage.Text = "";
                    Email.IsEnabled = true;
                    Password.IsEnabled = true;
                    await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                    _IsSigninIn = false;
                    return;
                }

                try
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sign In Attempt" } });

                    JoinNowBtn.Text = "LOGGING IN...";
                    var response = await WebAPIService.InitialLoad(_client, Email.Text.ToLower(), Password.Text);
                    //var result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JoinNowBtn.Text = "LOADING...";
                        HttpContent content = response.Content;
                        var json = await content.ReadAsStringAsync();
                        var signInResponse = JsonConvert.DeserializeObject<SignInResponseV5>(json);
                        JoinNowBtn.Text = "SETTING YOU UP...";

                        //if (signInResponse.Subscriptions.Count() == 0)
                        //{
                        //    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sign In No Subscription" } });
                        //    Spinner.IsVisible = false;
                        //    SignInBtn.Text = "Login";
                        //    FormValidationSpecialMessage.Text = "You don't have a subscription";
                        //    Email_SignIn.IsEnabled = true;
                        //    Password_SignIn.IsEnabled = true;
                        //    _IsSigninIn = false;
                        //    return;
                        //}

                        // If good
                        Spinner.IsVisible = false;
                        //FormValidationSpecialMessage.Text = "Loading...";
                        //FormValidationSpecialMessage.TextColor = Color.White;
                        //FormValidationSpecialMessage.FontSize = 14;
                        await WebAPIService.SaveInitialLoadData(signInResponse);
                        Task tSub = Task.Run(() => WebAPIService.UpdateLocalIAPs());
                        //FormValidationSpecialMessage.Text = "Success";
                        Auth.SaveCredentials(Email.Text.ToLower(), Password.Text);
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sign In Success" } });

                        //DependencyService.Get<IFacebookEvent>().LoggedIn();
                        //if (signInResponse.Profile.RP != 0)
                        //{
                        //    DependencyService.Get<IFacebookEvent>().SendRevenuePool(signInResponse.Profile.RP);
                        //}
                        await tSub;

                        foreach (var ContactProgram in signInResponse.ContactPrograms)
                        {
                            if (ContactProgram.StateCodeValue == 0 && ContactProgram.Program.Heading.Contains("FAB IN FIVE"))
                            {
                                await Navigation.PushModalAsync(new FabInFivePage("https://thecartelle.azureedge.net/cartelle/Ash%20FabIn5%20Part01%20SUBTITLES.mp4"));
                                return;
                            }
                        }
                        App.Current.MainPage = new MainStartingPage("login");
                        return;
                    }
                    else
                    {
                        //If Bad
                        Spinner.IsVisible = false;
                        JoinNowBtn.Text = "LOGIN";
                        FormValidationSpecialMessage.Text = "Email or password is invalid.";
                        Email.IsEnabled = true;
                        Password.IsEnabled = true;
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sign In Failure" } });
                        _IsSigninIn = false;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                    JoinNowBtn.Text = "LOGIN";
                    Spinner.IsVisible = false;
                    FormValidationSpecialMessage.Text = "Something went wrong, try again later.";
                    Email.IsEnabled = true;
                    Password.IsEnabled = true;
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SignInSubmitted()" } });
                    _IsSigninIn = false;
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }


        private async void ResetPassword_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ResetPassword());
        }

        protected override void OnAppearing()
        {
            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
                MessagingCenter.Send(this, "OnSignIn");
                base.OnAppearing();
            }
            catch (Exception ex)
            {

            }
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}
