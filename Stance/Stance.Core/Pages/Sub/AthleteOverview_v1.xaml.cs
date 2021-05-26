using FFImageLoading.Forms;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
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
    public partial class AthleteOverview_v1 : BaseContentPage
    {
        private const string _PageName = "Athlete Overview";
        private string _id = String.Empty;
        private string _videoUrl = String.Empty;
        private Guid _accountGuid;
        private int _accountId;
        private LocalDBAccount _account = new LocalDBAccount();
        private bool _isFromSignUp = false;

        public AthleteOverview_v1(LocalDBAccount account, string isFromSignUp = null)
        {
            InitializeComponent();

            if (isFromSignUp != null)
            {
                //change text to say 'choose a program'
                //go to create an account page
                On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
                _isFromSignUp = true;
                WatchVideoBtn.IsVisible = false;
                ProgramSectionTitle.Text = "CHOOSE YOUR PROGRAM";
                ProgramSectionTitle.FontSize = 20;
                ProgramTitleBar.HeightRequest = 40;
                SignUpHeader.IsVisible = true;
                athleteName.Text = account.Heading;
            }
            else
            {
                Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");
                Title.Text = account.Heading;
            }

            _account = account;
            _accountId = account.Id;
            _accountGuid = account.GuidCRM;
            _videoUrl = account.VideoUrl;
            //athleteName.Text = account.Heading;

            //var imgSource = new UriImageSource() { Uri = new Uri(account.SecondaryPhotoUrl) };
            //imgSource.CachingEnabled = true;
            //imgSource.CacheValidity = TimeSpan.FromDays(7);
            //accountImage.Source = imgSource;

            accountImage.CacheDuration = TimeSpan.FromDays(30);
            accountImage.RetryCount = 5;
            accountImage.RetryDelay = 250;
            accountImage.BitmapOptimizations = true;

            try
            {
                accountImage.Source = new UriImageSource() { Uri = new Uri(account.SecondaryPhotoUrl) };
            }
            catch (Exception ex)
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

            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily)
            {
                var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                if (isDeviceIphoneXorXS)
                {
                    AthleteRow.HeightRequest = 460;
                }
                else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                {
                    AthleteRow.HeightRequest = 505;
                }
                else
                {
                    AthleteRow.HeightRequest = 460;
                }
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                if (isDeviceIphonePlus)
                {
                    AthleteRow.HeightRequest = 490;
                }
                else
                {
                    AthleteRow.HeightRequest = 460;
                }
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                AthleteRow.HeightRequest = 1200;
                DetailsSL.Margin = new Thickness(0, 400, 70, 20);
            }
            else
            {
                AthleteRow.HeightRequest = 375;
            }

            BindPrograms();

        }

        private void SetupPrograms(List<LocalDBProgram> accountPrograms)
        {
            NotLoadedYet.IsVisible = true;

            listOfPrograms.Children.Clear();

            var ContactProfile = _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync().Result;
            if (ContactProfile != null)
            {
                if (ContactProfile.IsAdmin)
                {
                    accountPrograms = accountPrograms.Where(x => x.StatusCodeValue == 585860000 || x.StatusCodeValue == 585860001).OrderBy(x => x.SequenceNumber).ToList();//Publsihed and App Testing
                }
                else
                {
                    accountPrograms = accountPrograms.Where(x => x.StatusCodeValue == 585860000 && (x.Type == 866660000 || x.Type == 866660002)).OrderBy(x => x.SequenceNumber).ToList();//Publsihed, program and program/challenge types
                }
            }
            else
            {
                accountPrograms = accountPrograms.Where(x => x.StatusCodeValue == 585860000 && (x.Type == 866660000 || x.Type == 866660002)).OrderBy(x => x.SequenceNumber).ToList();//Publsihed, program and program/challenge types
            }

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
                    FontSize = 16,
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
                    Padding = new Thickness(20, 5, 20, 5),
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.White,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };

                var descSL = new StackLayout
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    //Padding = new Thickness(0, 0, 0, 0),
                };

                var descLabel = new Label
                {
                    Text = program.SubHeading,
                    TextColor = Color.FromHex("#909090"),
                    FontSize = 12,
                    FontFamily = "HelveticalNeue-Bold"
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

                var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
                if (isOfXFamily)
                {
                    var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                    var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                    var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                    if (isDeviceIphoneXorXS)
                    {
                        mainSL.HeightRequest = 290; //280
                        imgSL.HeightRequest = 235;
                    }
                    else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                    {
                        mainSL.HeightRequest = 320;
                        imgSL.HeightRequest = 260;
                    }
                    else
                    {
                        mainSL.HeightRequest = 290;
                        imgSL.HeightRequest = 235;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Phone)
                {
                    var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                    if (isDeviceIphonePlus)
                    {
                        mainSL.HeightRequest = 275;//265
                        imgSL.HeightRequest = 220;
                    }
                    else
                    {
                        mainSL.HeightRequest = 255;//245
                        imgSL.HeightRequest = 200;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                    if (isDeviceLargeIPad)
                    {
                        mainSL.HeightRequest = 570;//560
                        imgSL.HeightRequest = 530;
                    }
                    else
                    {
                        mainSL.HeightRequest = 480;//470
                        imgSL.HeightRequest = 440;
                    }
                }
                else
                {
                    mainSL.HeightRequest = 235;//225
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
                var Programs = await _connection.Table<LocalDBProgram>().Where(x => x.AccountId == Account.Id).ToListAsync();
                if (Programs.Count() > 0)
                {
                    SetupPrograms(Programs);
                }

                if (IsInternetConnected())
                {
                    try
                    {
                        await WebAPIService.GetAccountPrograms(_client, _accountGuid, Account.Id);

                        var Programs1 = await _connection.Table<LocalDBProgram>().Where(x => x.AccountId == Account.Id).ToListAsync();
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
            if (_isFromSignUp)
            {
                ContactSignUpV2 _contactSignUp = new ContactSignUpV2
                {
                    ReferralPool = App._contactSignUp.ReferralPool,
                    ReferrerCode = App._contactSignUp.ReferrerCode,
                    LeadSource = App._contactSignUp.LeadSource,
                    RevenuePool = 866660000, //athlete b/c only athlete refferals follow this process  
                    AccountCode = _account.Code,
                    AccountGuid = _account.GuidCRM,
                    ProgramCode = program.Code,
                    ProgramGuid = program.GuidCRM,
                };
                await Navigation.PushModalAsync(new CreateAccount(_contactSignUp), false);
                return;
            }
            await Navigation.PushAsync(new ProgramOverview_v1(program, null));
        }

        private void Handle_IDClicked(object sender, EventArgs e)
        {
            DisplayAlert("ID is:", _id, "OK");

        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
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
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Account", _account.Heading }, { "Action", "Watch Promo Video" } });
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
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Account", _account.Heading }, { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
