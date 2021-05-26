using FFImageLoading.Forms;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Stance.Models.LocalDB;
using Stance.Utils;
using Stance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stance.Pages.Sub
{
    public partial class RecommendedBy : BaseContentPage
    {
        private const string _PageName = "Recommended By";
        private bool _SendBtnActive = false;
        ContactSignUpV2 _contactSignUp = null;

        public RecommendedBy()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            _contactSignUp = App._contactSignUp;
            SetupAthletes();
        }

        private void Yes_Clicked(object sender, EventArgs e)
        {
            ClearContactSignUpInfo();
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Yes Recommended" } });
            modalPopup.IsVisible = false;
            ScrollSL.IsEnabled = true;
            LogoSL.IsEnabled = true;
            ScrollSL.Opacity = 1;
            LogoSL.Opacity = 1;
        }

        private async void No_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Not Recommended" } });
            //DependencyService.Get<IFacebookEvent>().CartelleAcquisition();
            ClearContactSignUpInfo();
            _contactSignUp.RevenuePool = 866660001;//cartelle            
           await Navigation.PushModalAsync(new WorkoutSetting(_contactSignUp), false);
        }

        private void ClearContactSignUpInfo()
        {
            _contactSignUp.AccountGuid = Guid.Empty;
            _contactSignUp.AccountCode = "";
            _contactSignUp.ProgramGuid = Guid.Empty;
            _contactSignUp.ProgramCode = "";
            _contactSignUp.WorkoutSetting = "";
            _contactSignUp.WorkoutLevel = "";
            _contactSignUp.WorkoutGoal = "";
        }

        private async void Athlete_Clicked(Guid accountId)
        {
            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Selected Recommender" } });
                //DependencyService.Get<IFacebookEvent>().AthleteAcquisition();
                ClearContactSignUpInfo();

                _contactSignUp.RevenuePool = 866660001;//cartelle            
                _contactSignUp.AccountGuid = accountId;

                //if athlete has 1 program then go to create account with program Guid
                var Account = await _connection.Table<LocalDBAccount>().Where(x => x.GuidCRM == accountId).FirstOrDefaultAsync();
                if (Account != null)
                {
                    _contactSignUp.AccountCode = Account.Code;
                    var Programs = await _connection.Table<LocalDBProgram>().Where(x => x.AccountId == Account.Id).ToListAsync();
                    if (Programs.Count() == 1)
                    {
                        _contactSignUp.ProgramGuid = Programs[0].GuidCRM;
                        _contactSignUp.ProgramCode = Programs[0].Code;
                        await Navigation.PushModalAsync(new CreateAccount(_contactSignUp), false);
                        return;
                    }
                    else if (Programs.Count() > 1)
                    {
                        await Navigation.PushModalAsync(new ProgramSearch(_contactSignUp), false);
                        return;
                    }
                }
                await Navigation.PushModalAsync(new WorkoutSetting(_contactSignUp), false);
            }
            catch (Exception ex)
            {

            }
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

        private async void SetupAthletes()
        {
            var Athletes = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000 || x.StatusCodeValue == 866660002).OrderBy(x => x.Heading).ToListAsync();
            if (Athletes.Count() <= 1)
            {
                if (!IsInternetConnected())
                {
                    await DisplayAlert("No Internet", "Connect to the internet and try again", "Ok");
                    //BtnSL.IsVisible = true;
                    //Spinner2.IsVisible = false;
                    return;
                }

                try
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "GetAthletes" } });
                    await WebAPIService.GetSignUpData(_client);
                    Athletes = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000 || x.StatusCodeValue == 866660002).OrderBy(x => x.Heading).ToListAsync();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "GetAthletes()" } });
                    await DisplayAlert("Error", "We had an error, try again", "Ok");
                    //BtnSL.IsVisible = true;
                    //Spinner2.IsVisible = false;
                    return;
                }
            }

            int count = 0;
            var SL = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 7,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };

            foreach (var item in Athletes)
            {
                count++;

                var SL1 = new StackLayout
                {
                    Spacing = 3,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                };

                var athImg = new CachedImage
                {
                    HeightRequest = 150,
                    WidthRequest = 150,
                    Aspect = Aspect.AspectFill,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#909090"),
                    CacheDuration = TimeSpan.FromDays(30),
                    RetryCount = 5,
                    RetryDelay = 250,
                    BitmapOptimizations = true,
                };

                var containerSL = new StackLayout
                {
                    // BackgroundColor = Color.Red,
                    Spacing = 0,
                    //Padding = 0,
                    //Margin = 0,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                containerSL.Children.Add(athImg);

                var frameForRoundedCorners = new Frame
                {
                    Padding = 0,
                    Margin = 0,
                    CornerRadius = 15,
                    HeightRequest = 150,
                    WidthRequest = 150,
                    IsClippedToBounds = true,
                    //BackgroundColor = Color.Gray,
                };
                frameForRoundedCorners.Content = containerSL;

                if (item.IGProfileUrl != "" && item.IGProfileUrl != null)
                {
                    athImg.Source = UriImageSource.FromUri(new Uri(item.IGProfileUrl));
                }
                else
                {
                    athImg.Source = UriImageSource.FromFile("Athletes_96 pix.png");
                }
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, ce) =>
                {
                // handle the tap
                Athlete_Clicked(item.GuidCRM);
                };
                frameForRoundedCorners.GestureRecognizers.Add(tapGestureRecognizer);

                SL1.Children.Add(frameForRoundedCorners);
                SL.Children.Add(SL1);

                if (count % 2 == 0)
                {
                    listOfAthletes.Children.Add(SL);
                    SL = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 7,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                    };
                }
                else if (count == Athletes.Count())
                {
                    listOfAthletes.Children.Add(SL);
                }
            }
            Spinner2.IsVisible = false;
        }


    }
}
