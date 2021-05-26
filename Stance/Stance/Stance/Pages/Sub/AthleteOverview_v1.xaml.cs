using FFImageLoading.Forms;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Stance.Models.LocalDB;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class AthleteOverview_v1 : BaseContentPage
    {
        private const string _PageName = "Athlete Overview";
        private string _id = String.Empty;
        private string _videoUrl = String.Empty;
        private Guid _accountGuid;
        private int _accountId;
        private LocalDBAccount _account = new LocalDBAccount();

        public AthleteOverview_v1(LocalDBAccount account)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            _account = account;
            _accountId = account.Id;
            _accountGuid = account.GuidCRM;
            _videoUrl = account.VideoUrl;
            athleteName.Text = account.Heading;

            //var imgSource = new UriImageSource() { Uri = new Uri(account.SecondaryPhotoUrl) };
            //imgSource.CachingEnabled = true;
            //imgSource.CacheValidity = TimeSpan.FromDays(7);
            //programImage.Source = imgSource;

            programImage.CacheDuration = TimeSpan.FromDays(30);
            programImage.RetryCount = 5;
            programImage.RetryDelay = 250;
            programImage.BitmapOptimizations = true;

            try
            {
                programImage.Source = new UriImageSource() { Uri = new Uri(account.SecondaryPhotoUrl) };
            } catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SecondaryPhotoUrl Error" } });
            }

            SubHeading.Text = account.SubHeading;

            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            if (IsInternetConnected())
            {
                NoNetwork.IsVisible = false;
            }
            else
            {
                NoNetwork.IsVisible = true;
            }

            if (Device.Idiom == TargetIdiom.Phone)
            {
                AthleteRow.HeightRequest = 400;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                AthleteRow.HeightRequest = 950;
            }
            else
            {
                AthleteRow.HeightRequest = 375;
            }

            BindPrograms();

        }

        private void SetupPrograms(List<LocalDBProgram> accountPrograms)
        {
            listOfPrograms.Children.Clear();

            foreach (var program in accountPrograms)
            {
                var mainSL = new StackLayout
                {
                    Spacing = 0,
                };

                var imgBucket = new StackLayout
                {
                    Spacing = 0,
                    //Padding = new Thickness(2,2,2, 0),
                    BackgroundColor = Color.White,
                };

                var imgSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#909090"),
                };

                imgBucket.Children.Add(imgSL);
                mainSL.Children.Add(imgBucket);

                var relLayout = new RelativeLayout();

                imgSL.Children.Add(relLayout);

                //var imgSource = new UriImageSource() { Uri = new Uri(program.PhotoUrl) };
                //imgSource.CachingEnabled = true;
                //imgSource.CacheValidity = TimeSpan.FromDays(7);

                //var img = new Image
                //{
                //    Source = imgSource,
                //    HeightRequest = 170,
                //    Aspect = Aspect.AspectFill,
                //};

                var cachedImage = new CachedImage()
                {
                    CacheDuration = TimeSpan.FromDays(30),
                    RetryCount = 5,
                    RetryDelay = 250,
                    BitmapOptimizations = true,
                    HeightRequest = 170,
                    Aspect = Aspect.AspectFill,
                };

                try
                {
                    cachedImage.Source = new UriImageSource() { Uri = new Uri(program.PhotoUrl) };
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "PhotoUrl Error" } });
                }

                relLayout.Children.Add(cachedImage, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var heading = new Label
                {
                    Text = program.Heading,
                    FontSize = 26,
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "AvenirNextCondensed-Bold",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(30, 0, 0, 0)
                };

                var headingSL = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Spacing = 0,
                };

                var arrowImg = new Image
                {
                    HorizontalOptions = LayoutOptions.End,
                    Source = (FileImageSource)ImageSource.FromFile("Arrow_26.png"),
                };

                headingSL.Children.Add(heading);
                headingSL.Children.Add(arrowImg);

                relLayout.Children.Add(headingSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var bottonSL = new StackLayout
                {
                    Spacing = 0,
                    Padding = new Thickness(20, 15, 10, 15),
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.White,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };

                var descSL = new StackLayout
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Padding = new Thickness(0, 0, 20, 0),
                };

                var descLabel = new Label
                {
                    Text = program.SubHeading,
                    TextColor = Color.Gray,
                    FontSize = 13,
                    FontFamily = "PingFangTC-Regular"
                };

                var iconImg = new Image
                {
                    Source = "arrow_26_green.png",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.End,
                };

                descSL.Children.Add(descLabel);

                bottonSL.Children.Add(descSL);
                //bottonSL.Children.Add(iconImg);

                mainSL.Children.Add(bottonSL);

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    mainSL.HeightRequest = 245;
                    imgSL.HeightRequest = 180;
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    mainSL.HeightRequest = 500;
                    imgSL.HeightRequest = 450;
                }
                else
                {
                    mainSL.HeightRequest = 245;
                    imgSL.HeightRequest = 180;
                }


                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) =>
                {
                    // handle the tap
                    ProgCell_Clicked(program);
                };

                mainSL.GestureRecognizers.Add(tapGestureRecognizer);

                listOfPrograms.Children.Add(mainSL);

            }
            NotLoadedYet.IsVisible = false;

        }

        private async void BindPrograms()
        {
            NotLoadedYet.IsVisible = true;

            var Account = await _connection.Table<LocalDBAccount>().Where(x => x.Id == _accountId).FirstOrDefaultAsync();
            if (Account != null)
            {
                var Programs = await _connection.Table<LocalDBProgram>().Where(x => x.AccountId == Account.Id && x.StatusCodeValue == 585860000).OrderBy(x => x.SequenceNumber).ToListAsync();

                if (Programs.Count() > 0)
                {
                    SetupPrograms(Programs);
                }

                if (IsInternetConnected())
                {
                    try
                    {
                        await WebAPIService.GetAccountPrograms(_client, _accountGuid, Account.Id);

                        var Programs1 = await _connection.Table<LocalDBProgram>().Where(x => x.AccountId == Account.Id && x.StatusCodeValue == 585860000).OrderBy(x => x.SequenceNumber).ToListAsync();//Published
                        if (Programs1.Count() > 0)
                        {
                            SetupPrograms(Programs1);
                        }
                        else
                        {
                            DisplayNoPrograms();
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "BindPrograms()" } });
                        // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                    }

                }
            }

        }

        private void DisplayNoPrograms()
        {
            listOfPrograms.Children.Clear();
            NotLoadedYet.IsVisible = false;

        }

        private async void ProgCell_Clicked(LocalDBProgram program)
        {
            await Navigation.PushAsync(new ProgramOverview_v1(program, null));
        }

        private void Handle_IDClicked(object sender, EventArgs e)
        {
            DisplayAlert("ID is:", _id, "OK");

        }

        private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            //var program = e.SelectedItem as Program;
            //var id = program.Id;
            //MainContentStackLayout.IsEnabled = false;
            //await Navigation.PushAsync(new ProgramOverview_v1(""));
            //MainContentStackLayout.IsEnabled = true;
        }

        private async void WatchVideoBtn_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Account", _account.Heading }, { "Action", "Watch Promo Video" }  });
            await Navigation.PushModalAsync(new WatchVideoPage(_videoUrl));
        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                NoNetwork.IsVisible = false;
            }
            else if (!e.IsConnected)
            {
                NoNetwork.IsVisible = true;
            }
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Account" , _account.Heading }, { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
