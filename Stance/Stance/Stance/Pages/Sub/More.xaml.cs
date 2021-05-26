using FFImageLoading;
using FFImageLoading.Cache;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PCLStorage;
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

namespace Stance.Pages.Sub
{
    public partial class More : BaseContentPage
    {
        private const string _PageName = "More Tab";
        private bool _IsRefreshing = false;
        private bool _IsLoggingOut = false;


        public More()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");

            var tapGestureRecognizer2 = new TapGestureRecognizer
            {
                Command = new Command(AuthBtn_Clicked),
            };
            SignOutSL.GestureRecognizers.Add(tapGestureRecognizer2);

            var tapGestureRecognizer3 = new TapGestureRecognizer
            {
                Command = new Command(PolicyBtn_Clicked),
            };
            PolicySL.GestureRecognizers.Add(tapGestureRecognizer3);

            var tapGestureRecognizer4 = new TapGestureRecognizer
            {
                Command = new Command(TermsBtn_Clicked),
            };
            TermsSL.GestureRecognizers.Add(tapGestureRecognizer4);

            var tapGestureRecognizer5 = new TapGestureRecognizer
            {
                Command = new Command(RefreshBtn_Clicked),
            };
            RefreshSL.GestureRecognizers.Add(tapGestureRecognizer5);

            var tapGestureRecognizer6 = new TapGestureRecognizer
            {
                Command = new Command(AuthBtn_Clicked),
            };
            SignOutSL.GestureRecognizers.Add(tapGestureRecognizer6);

            var tapGestureRecognizer7 = new TapGestureRecognizer
            {
                Command = new Command(ContactUs_Clicked),
            };
            ContactUsSL.GestureRecognizers.Add(tapGestureRecognizer7);

            var tapGestureRecognizer8 = new TapGestureRecognizer
            {
                Command = new Command(Feedback_Clicked),
            };
            FeedbackSL.GestureRecognizers.Add(tapGestureRecognizer8);
        }

        private async void TermsBtn_Clicked()
        {
            if (IsInternetConnected())
            {
                await Navigation.PushAsync(new Terms());
            }
            else
            {
                await DisplayAlert("NO INTERNET", "Connect to the internet and try again", "OK");
            }
        }

        private async void PolicyBtn_Clicked()
        {
            if (IsInternetConnected())
            {
                await Navigation.PushAsync(new PrivacyPolicy());
            }
            else
            {
                await DisplayAlert("NO INTERNET", "Connect to the internet and try again", "OK");
            }
        }

        private async void ContactUs_Clicked()
        {
            await Navigation.PushAsync(new ContactUs("", "contactus"));
        }

        private async void Feedback_Clicked()
        {
            await Navigation.PushAsync(new ContactUs("", "feedback"));
        }

        private async void CheckInternet()
        {
            if (!IsInternetConnected())
            {
                await DisplayAlert("NO INTERNET", "Connect to the internet and try again", "OK");
            }
        }

        private async void RefreshBtn_Clicked()
        {
            if (_IsRefreshing)
                return;

            var result = await DisplayAlert("REFRESH APP", "Refreshing your App will bring in your latest information.", "REFRESH", "NO");
            if (result)
            {
                try
                {
                    _IsRefreshing = true;
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Refreshing App" } });

                    var response = await WebAPIService.RefreshApp(_client);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        try
                        {
                            await ImageService.Instance.InvalidateCacheAsync(CacheType.All);
                        }
                        catch (Exception ex)
                        {
                            var err = ex.ToString();
                        }

                        App.Current.MainPage = new MainStartingPage();
                    }
                    else
                    {
                        await DisplayAlert("Error", "There was an error. Try again later.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "RefreshBtn_Clicked()" } });
                }
            }
            _IsRefreshing = false;

        }

        private async void AuthBtn_Clicked()
        {
            if (_IsLoggingOut)
                return;

            _IsLoggingOut = true;

            var result = await DisplayAlert("LOG OUT", "Your downloaded workouts will be deleted and unavailable offline upon log out!", "LOG OUT", "NO");
            if (result)
            {
                //Sign Out
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Logging Out" } });
                Task tw = Task.Run(() => Database.ClearAsync());
                tw.Wait();
                Auth.DeleteCredentials();
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Logged Out" } });
                App.Current.MainPage = new SignIn();
            }
            _IsLoggingOut = false;
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
