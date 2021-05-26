using BranchXamarinSDK;
using FFImageLoading;
using FFImageLoading.Cache;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class More : BaseContentPage, IBranchIdentityInterface
    {
        private const string _PageName = "More Tab";
        private bool _IsRefreshing = false;
        private bool _IsLoggingOut = false;


        public More()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");
            NavigationPage.SetHasNavigationBar(this, false);
            Padding = new Thickness(0, 20, 0, 0);
            BackgroundColor = Color.White;

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

            var tapGestureRecognizer9 = new TapGestureRecognizer
            {
                Command = new Command(MyAccountBtn_Clicked),
            };
            MyAccountSL.GestureRecognizers.Add(tapGestureRecognizer9);

            var tapGestureRecognizer10 = new TapGestureRecognizer
            {
                Command = new Command(MyProfileBtn_Clicked),
            };
            MyProfileSL.GestureRecognizers.Add(tapGestureRecognizer10);

            var tapGestureRecognizer11 = new TapGestureRecognizer
            {
                Command = new Command(BillingBtn_Clicked),
            };
            BillingSL.GestureRecognizers.Add(tapGestureRecognizer11);

            var tapGestureRecognizer12 = new TapGestureRecognizer
            {
                Command = new Command(FacebookGroup_Clicked),
            };
            FacebookGroupSL.GestureRecognizers.Add(tapGestureRecognizer12);

           // MessagingCenter.Send(this, "LoadFixScrollIssue");
        }

        private async void FacebookGroup_Clicked()
        {
            if (IsInternetConnected())
            {
                Device.OpenUri(new Uri("https://www.facebook.com/groups/307902269825399/"));
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }

        private async void MyProfileBtn_Clicked()
        {
            await Navigation.PushAsync(new PersonalProfile(), true);
        }

        private async void MyAccountBtn_Clicked()
        {
            if (IsInternetConnected())
            {
                Device.OpenUri(new Uri("https://thecartelle.com/subscription"));
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }

        private async void TermsBtn_Clicked()
        {
            if (IsInternetConnected())
            {
                await Navigation.PushAsync(new Terms());
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
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
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }

        private async void BillingBtn_Clicked()
        {
            if (IsInternetConnected())
            {
                await Navigation.PushAsync(new BillingTerms(false));
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
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
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }

        private async void RefreshBtn_Clicked()
        {
            if (_IsRefreshing)
                return;

            var result = await DisplayAlert("Refresh App", "Refreshing your App will bring in your latest information.", "Refresh", "Not now");
            if (result)
            {
                try
                {
                    RefreshLabel.Text = "...Refreshing your App";
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
                        RefreshLabel.Text = "Refresh App (reload/resolve)";
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "RefreshBtn_Clicked()" } });
                    RefreshLabel.Text = "Refresh App (reload/resolve)";
                }
            }
            _IsRefreshing = false;

        }

        private async void AuthBtn_Clicked()
        {
            if (_IsLoggingOut)
                return;

            _IsLoggingOut = true;

            var result = await DisplayAlert("Log Out", "Your downloaded workouts will be deleted and unavailable offline upon log out!", "Log Out", "Not now");
            if (result)
            {
               

                LogoutLabel.Text = "...Logging you Out";
                //Sign Out
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Logging Out" } });
                Task tw = Task.Run(() => Database.ClearAsync());
                tw.Wait();
                Auth.DeleteCredentials();
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Logged Out" } });
                try
                {
                    Branch branch = Branch.GetInstance();
                    branch.Logout(this);
                }
                catch (Exception ex)
                {
                    var err = ex.ToString();
                }
                App.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.FromHex("#17191A"), BarTextColor = Color.White };

                //App.Current.MainPage = new MainStartingPage();
            }
            _IsLoggingOut = false;
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

        public void IdentitySet(Dictionary<string, object> data)
        {
            //throw new NotImplementedException();
        }

        public void LogoutComplete()
        {
            //throw new NotImplementedException();
        }

        public void IdentityRequestError(BranchError error)
        {
            //throw new NotImplementedException();
        }
    }
}
