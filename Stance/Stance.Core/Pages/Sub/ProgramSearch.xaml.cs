using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Stance.Models.LocalDB;
using FFImageLoading.Forms;
using Plugin.Connectivity.Abstractions;
using Stance.ViewModels;

namespace Stance.Pages.Sub
{
    public partial class ProgramSearch : BaseContentPage
    {
        private const string _PageName = "Program Search";
        private bool _isFromSignUp = false;
        private List<ProgramItem> _programs = new List<ProgramItem>();
        private string[] searchTags = new string[3];
        public bool _changedFilter = false;
        public bool _filterTransition = false;
        public bool _sendingProgramData = false;
        public bool _isRefreshing = false;

        public string _WorkoutSetting = "home";
        public string _WorkoutGoal = "tone up";
        public string _WorkoutLevel = "beginner";

        ContactSignUpV2 _contactSignUp = null;

        public class ProgramItem
        {
            public StackLayout SL { get; set; }
            public string Tags { get; set; }
            public string ProgramName { get; set; }
        }

        public ProgramSearch(ContactSignUpV2 contactSignUp = null)
        {
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            _contactSignUp = contactSignUp;

            searchTags[0] = ",";
            searchTags[1] = ",";
            searchTags[2] = ",";

            NotLoadedYet.IsVisible = true;

            if (_contactSignUp != null)
            {
                TitleSL.IsVisible = true;
                _isFromSignUp = true;
                //CheckIfHasContactProgram();
                SetupPrograms();
            }
            else
            {
                //var toolItem1 = new ToolbarItem
                //{
                //    Icon = "Spin_24px_@2.png",
                //    Order = ToolbarItemOrder.Primary,
                //    Priority = 1
                //};
               
                //toolItem1.Clicked += (s, e) =>
                //{
                //    RefreshProgramList(s, e);
                //};
                //ToolbarItems.Add(toolItem1);

                var toolItem2 = new ToolbarItem
                {
                    Icon = "Filter_24px_2.png",
                    Order = ToolbarItemOrder.Primary,
                    Priority = 0
                };

                toolItem2.Clicked += (s, e) =>
                {
                    Filter_Clicked(s, e);
                };

                ToolbarItems.Add(toolItem2);

                RefreshProgramList(this, null);
            }

            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            if (IsInternetConnected())
            {
                NoNetwork.IsVisible = false;
            }
            else
            {
                NoNetwork.IsVisible = true;
            }
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }

        //private async void CheckIfHasContactProgram()
        //{
        //    var cps = await _connection.Table<LocalDBContactProgram>().ToListAsync();
        //    if (cps.Count() != 0)
        //    {
        //        await Navigation.PushModalAsync(new ConfirmEmail(true), false);
        //        return;
        //    }
        //    //sedn request to check again
        //}

        private async void SetupPrograms(bool isRefresh = false)
        {
            try
            {
                NotLoadedYet.IsVisible = true;
                var allPrograms = await _connection.Table<LocalDBProgram>().ToListAsync();

                if (_isFromSignUp)
                {
                    var Account = await _connection.Table<LocalDBAccount>().Where(x => x.GuidCRM == _contactSignUp.AccountGuid).FirstOrDefaultAsync();
                    if (Account != null)
                    {
                        allPrograms = allPrograms.Where(x => x.AccountId == Account.Id).ToList();
                    }
                }
                else
                {
                    if (allPrograms.Count() == 0)
                    {
                        if (IsInternetConnected())
                        {
                            //RefreshBtn.Text = "Refreshing";
                            await WebAPIService.GetAllPrograms(_client);
                            allPrograms = await _connection.Table<LocalDBProgram>().ToListAsync();
                        }
                        else
                        {
                            await DisplayAlert("No Internet", "Connect to the internet, and refresh to see programs.", "Ok");
                            return;
                        }
                    }

                    try
                    {
                        foreach (var prog in allPrograms)
                        {
                            var pMatch = allPrograms.Where(x => x.GuidCRM == prog.GuidCRM && prog.Id != x.Id).FirstOrDefault();
                            if (pMatch != null)
                            {
                                await _connection.DeleteAsync(pMatch);
                                allPrograms.Remove(pMatch);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SetupPrograms()" } });

                    }

                    var ContactProfile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();
                    if (ContactProfile != null)
                    {
                        if (ContactProfile.IsAdmin)
                        {
                            allPrograms = allPrograms.Where(x => x.StatusCodeValue == 585860000 || x.StatusCodeValue == 585860001).OrderBy(x => x.Heading).ToList();//Publsihed and App Testing
                        }
                        else
                        {
                            allPrograms = allPrograms.Where(x => x.StatusCodeValue == 585860000 && (x.Type == 866660000 || x.Type == 866660002)).OrderBy(x => x.Heading).ToList();//Publsihed, program and program/challenge types
                        }
                    }
                    else
                    {
                        allPrograms = allPrograms.Where(x => x.StatusCodeValue == 585860000 && (x.Type == 866660000 || x.Type == 866660002)).OrderBy(x => x.Heading).ToList();//Publsihed, program and program/challenge types
                    }

                    if (allPrograms.Count() == 0)
                    {
                        DisplayNoPrograms();
                        return;
                    }

                    if (_programs.Count() != 0 && !isRefresh)
                    {
                        UpdateProgramsList();
                        return;
                    }

                    _programs.Clear();
                }

                foreach (var program in allPrograms)
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
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = "AvenirNextCondensed-Bold",
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        //Margin = new Thickness(30, 0, 0, 0)
                    };

                    var headingSL = new StackLayout
                    {
                        //Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Spacing = 0,
                    };
                    headingSL.Children.Add(heading);

                    var arrowImg = new Image
                    {
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                        Source = (FileImageSource)ImageSource.FromFile("Arrow_26.png"),
                    };

                    if (!_isFromSignUp)
                    {
                        headingSL.Orientation = StackOrientation.Horizontal;
                        heading.Margin = new Thickness(30, 0, 0, 0);
                        headingSL.Children.Add(arrowImg);
                        relLayout.Children.Add(headingSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));
                    }
                    else
                    {

                        var heading2 = new Label
                        {
                            Text = program.AthleteName,
                            FontSize = 15,
                            FontAttributes = FontAttributes.Bold,
                            FontFamily = "AvenirNextCondensed-Bold",
                            TextColor = Color.White,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            //Margin = new Thickness(30, 0, 0, 0)
                        };
                        headingSL.Children.Add(heading2);
                        headingSL.Margin = new Thickness(0, 30, 0, 0);

                        var masterHeadingSL = new StackLayout
                        {
                            //HorizontalOptions = LayoutOptions.FillAndExpand,
                            //VerticalOptions = LayoutOptions.End,
                            Spacing = 0,
                            //HorizontalOptions = LayoutOptions.EndAndExpand,                        
                        };
                        masterHeadingSL.Children.Add(headingSL);

                        var secondaryHeadingSL = new StackLayout
                        {
                            Spacing = 0,
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.End,
                            BackgroundColor = Color.FromHex("#555757"),
                            Margin = new Thickness(0, 0, 0, 15),
                            Opacity = 0.85,
                            Padding = new Thickness(10, 5, 5, 5),
                        };

                        var chooseProgram = new Label
                        {
                            Text = "START PROGRAM",
                            FontSize = 16,
                            FontAttributes = FontAttributes.Bold,
                            FontFamily = "AvenirNextCondensed-Bold",
                            TextColor = Color.FromHex("#00BBCB"),
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            Margin = new Thickness(10, 0, 5, 0)
                        };

                        arrowImg.HeightRequest = 18;
                        secondaryHeadingSL.Children.Add(chooseProgram);
                        secondaryHeadingSL.Children.Add(arrowImg);
                        masterHeadingSL.Children.Add(secondaryHeadingSL);
                        relLayout.Children.Add(masterHeadingSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));
                    }

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

                    //mainSL.Children.Add(bottonSL);

                    var lineSL = new StackLayout()
                    {
                        HeightRequest = 1,
                        BackgroundColor = Color.FromHex("#939598"),
                    };
                    mainSL.Children.Add(lineSL);

                    var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
                    if (isOfXFamily)
                    {
                        var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                        var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                        var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                        if (isDeviceIphoneXorXS)
                        {
                            mainSL.HeightRequest = 235; //290
                            imgSL.HeightRequest = 235;
                        }
                        else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                        {
                            mainSL.HeightRequest = 260;//320;
                            imgSL.HeightRequest = 260;
                        }
                        else
                        {
                            mainSL.HeightRequest = 235;//290;
                            imgSL.HeightRequest = 235;
                        }
                    }
                    else if (Device.Idiom == TargetIdiom.Phone)
                    {
                        var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                        if (isDeviceIphonePlus)
                        {
                            mainSL.HeightRequest = 220;//275
                            imgSL.HeightRequest = 220;
                        }
                        else
                        {
                            mainSL.HeightRequest = 200;//255
                            imgSL.HeightRequest = 200;
                        }
                    }
                    else if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                        if (isDeviceLargeIPad)
                        {
                            mainSL.HeightRequest = 530;//570
                            imgSL.HeightRequest = 530;
                        }
                        else
                        {
                            mainSL.HeightRequest = 440;//480
                            imgSL.HeightRequest = 440;
                        }
                    }
                    else
                    {
                        mainSL.HeightRequest = 180;//235
                        imgSL.HeightRequest = 180;
                    }

                    if (!_isFromSignUp)
                    {
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += (s, e) =>
                        {
                            // handle the tap
                            ProgCell_Clicked(program);
                        };
                        mainSL.GestureRecognizers.Add(tapGestureRecognizer);
                    }
                    else
                    {
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += (s, e) =>
                        {
                            // handle the tap
                            StartProgramFromSignUp_Clicked(program);
                        };
                        mainSL.GestureRecognizers.Add(tapGestureRecognizer);
                    }

                    var pr = new ProgramItem
                    {
                        SL = mainSL,
                        Tags = program.Tags,
                        ProgramName = program.Heading,
                    };
                    _programs.Add(pr);
                    
                }

                if (!_isFromSignUp)
                {
                    UpdateProgramsList();
                }
                else
                {
                    foreach (var item in _programs)
                    {
                        listOfPrograms.Children.Add(item.SL);
                    }
                    NotLoadedYet.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                NotLoadedYet.IsVisible = false;
            }
        }


        //private void RefreshBtn_Clicked(object sender, EventArgs e)
        //{
        //    if(RefreshBtn.Text != "Refreshing")
        //    {
        //        RefreshBtn.Text = "Refreshing";

        //        if (IsInternetConnected())
        //        {
        //            try
        //            {
        //                RefreshProgramList();
        //            }
        //            catch (Exception ex)
        //            {
        //                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "BindPrograms()" } });
        //                // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
        //                RefreshBtn.Text = "Error";
        //            }
        //        } else
        //        {
        //            RefreshBtn.Text = "No Internet";
        //        }
        //    }
        //}

        private async void RefreshProgramList(object sender, EventArgs e)
        {
            if (_isRefreshing)
                return;

            if (!IsInternetConnected() && sender != this)
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "Ok");
                return;
            }

            Scrolllist(NotLoadedYet);
            _isRefreshing = true;
            NotLoadedYet.IsVisible = true;
            await WebAPIService.GetAllPrograms(_client);//Get all programs: published and app testing
            SetupPrograms(true);
        }

        private async void ProgCell_Clicked(LocalDBProgram program)
        {
            await Navigation.PushAsync(new ProgramOverview_v1(program, null),true);
        }

        private async void StartProgramFromSignUp_Clicked(LocalDBProgram program)
        {
            if (_sendingProgramData)
                return;

            _sendingProgramData = true;

            _contactSignUp.ProgramGuid = program.GuidCRM;
            _contactSignUp.ProgramCode = program.Code;
            await Navigation.PushModalAsync(new CreateAccount(_contactSignUp), false);

            _sendingProgramData = false;
            return;

        }

        private void UpdateProgramsList()
        {
            try
            {
                listOfPrograms.Children.Clear();
                StackLayout slToScrollTo = null;
                int i = 0;
                var pg = _programs.Where(x => x.Tags.Contains(searchTags[0]) && x.Tags.Contains(searchTags[1]) && x.Tags.Contains(searchTags[2])).OrderBy(x => x.ProgramName).ToList();

                foreach (var item in pg)
                {
                    listOfPrograms.Children.Add(item.SL);
                    i++;
                    if (slToScrollTo == null)
                    {
                        slToScrollTo = item.SL;
                    }
                }

                if (i == 0)
                {
                    NoResultsStackLayout.IsVisible = true;
                }
                else
                {
                    NoResultsStackLayout.IsVisible = false;
                }
                NotLoadedYet.IsVisible = false;
                //RefreshBtn.Text = "Refresh";
                if (slToScrollTo != null)
                {
                    Scrolllist(slToScrollTo);
                }
                _isRefreshing = false;
            } catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "UpdateProgramsList()" } });
                NotLoadedYet.IsVisible = false;
                _isRefreshing = false;
            }

        }

        private async void Scrolllist(StackLayout sl)
        {
            try
            {
                await ScrollViewPrograms.ScrollToAsync(sl, ScrollToPosition.Center, true);
            }
            catch (Exception ex)
            {
            }
        }

        void Handle_ValueChanged_WorkoutSetting(object sender, SegmentedControl.FormsPlugin.Abstractions.ValueChangedEventArgs e)
        {
            if (!_changedFilter)
                return;

            if (_sendingProgramData)
                return;

            switch (e.NewValue)
            {
                case 0:
                    searchTags[0] = "at home";
                    break;
                case 1:
                    searchTags[0] = "gym";
                    break;
            }
            _WorkoutSetting = searchTags[0];
            UpdateProgramsList();
        }

        void Handle_ValueChanged_GoalSetting(object sender, SegmentedControl.FormsPlugin.Abstractions.ValueChangedEventArgs e)
        {
            if (!_changedFilter)
                return;

            if (_sendingProgramData)
                return;

            switch (e.NewValue)
            {
                case 0:
                    searchTags[1] = "tone up";
                    break;
                case 1:
                    searchTags[1] = "fat loss";
                    break;
                case 2:
                    searchTags[1] = "strengthen";
                    break;
            }
            _WorkoutGoal = searchTags[1];
            UpdateProgramsList();
        }

        void Handle_ValueChanged_StartingLevelSetting(object sender, SegmentedControl.FormsPlugin.Abstractions.ValueChangedEventArgs e)
        {
            if (!_changedFilter)
                return;

            if (_sendingProgramData)
                return;

            switch (e.NewValue)
            {
                case 0:
                    searchTags[2] = "beginner";
                    break;
                case 1:
                    searchTags[2] = "intermediate";
                    break;
                case 2:
                    searchTags[2] = "advanced";
                    break;
            }
            _WorkoutLevel = searchTags[2];
            UpdateProgramsList();
        }

        private void Filter_Clicked(object sender, EventArgs e)
        {
            if (_filterTransition)
                return;

            if (_sendingProgramData)
                return;

            _filterTransition = true;

            if (FilterSL.IsVisible)
            {
                FilterSL.IsVisible = false;
                searchTags[0] = ",";
                searchTags[1] = ",";
                searchTags[2] = ",";
                _changedFilter = false;
            }
            else
            {
                FilterSL.IsVisible = true;
                searchTags[0] = _WorkoutSetting;
                searchTags[1] = _WorkoutGoal;
                searchTags[2] = _WorkoutLevel;
                _changedFilter = true;
            }
            UpdateProgramsList();
            _filterTransition = false;
        }

        private void DisplayNoPrograms()
        {
            NotLoadedYet.IsVisible = false;
            NoResultsStackLayout.IsVisible = true;
            //RefreshBtn.Text = "Refresh";
            _isRefreshing = false;
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
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


    }
}