using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class ProgramOverview_v1 : BaseContentPage
    {
        private const string _PageName = "Program Overview";
        private LocalDBContactProgram _ContactProgram = null;
        private string _videoUrl = String.Empty;
        private string _programId = String.Empty;
        private Guid _ProgramGuid;
        private LocalDBProgram _Program = new LocalDBProgram();
        private string _IsInWorkoutNav = null;
        private string _ContactProgramState = "LEARN MORE";

        public ProgramOverview_v1(LocalDBProgram program, LocalDBContactProgram contactProgram, string IsInWorkoutNav = null)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");
            Title.Text = program.Heading;
            //programName.Text = program.Heading;

            if (contactProgram != null)
            {
                _ContactProgram = contactProgram;
            }

            _Program = program;
            _IsInWorkoutNav = IsInWorkoutNav;

            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            if (IsInternetConnected())
            {
                NoNetwork.IsVisible = false;
            }
            else
            {
                NoNetwork.IsVisible = true;
            }

            //var apiHelper = new MockApiHelper();
            //var program = apiHelper.GetAthleteProgram("1");


            //var imgSource = new UriImageSource() { Uri = new Uri(program.SecondaryPhotoUrl) };
            //imgSource.CachingEnabled = true;
            //imgSource.CacheValidity = TimeSpan.FromDays(7);
            //programImage.Source = imgSource;

            programImage.CacheDuration = TimeSpan.FromDays(30);
            programImage.RetryCount = 5;
            programImage.RetryDelay = 250;
            programImage.BitmapOptimizations = true;

            try
            {
                programImage.Source = new UriImageSource() { Uri = new Uri(program.SecondaryPhotoUrl) };
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SecondaryPhotoUrl Error" } });
            }

            numberOfWeeks.Text = program.TotalWeeks.ToString();
            programGoal.Text = program.Goal;
            programLevel.Text = program.Level;
            SubHeading.Text = program.SubHeading;
            _videoUrl = program.VideoUrl;
            //_cost = program.Cost;
            _programId = program.GuidCRM.ToString();
            _ProgramGuid = program.GuidCRM;

            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily)
            {
                var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                if (isDeviceIphoneXorXS)
                {
                    MainImage.HeightRequest = 525;
                    LearnMoreBtnIPad.Margin = new Thickness(0, 300, 0, 0);
                }
                else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                {
                    MainImage.HeightRequest = 580;
                    LearnMoreBtnIPad.Margin = new Thickness(0, 350, 0, 0);
                }
                else
                {
                    MainImage.HeightRequest = 525;
                    LearnMoreBtnIPad.Margin = new Thickness(0, 300, 0, 0);
                }
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                MainImage.HeightRequest = 475;
                LearnMoreBtnIPad.Margin = new Thickness(0, 260, 0, 0);
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                if (isDeviceLargeIPad)
                {
                    MainImage.HeightRequest = 1150;
                    LearnMoreBtnIPad.Margin = new Thickness(0, 650, 0, 10);
                }
                else
                {
                    MainImage.HeightRequest = 1050;
                    LearnMoreBtnIPad.Margin = new Thickness(0, 600, 0, 10);
                }
            }
            else
            {
                MainImage.HeightRequest = 475;
            }
            PurchaseBtn.IsVisible = false;

            var cp = _connection.Table<LocalDBContactProgram>().Where(x => x.ProgramId == _Program.Id).FirstOrDefaultAsync().Result;
            if (cp == null)
            {
                AddToMyPlansSL.IsVisible = true;
            }

            GetProgramDays();
        }

        private async void AddToMyPlans_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Click Add to Plans" } });
            AddToMyPlansBtn.IsEnabled = false;

            bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("addtomyplans",_Program.Id)).Result;
            if (!allowDownload)
            {
                await Navigation.PushModalAsync(new IAP(_Program.GuidCRM), false);
                AddToMyPlansBtn.IsEnabled = true;
                return;
            }

            var cp = await _connection.Table<LocalDBContactProgram>().Where(x => x.ProgramId == _Program.Id).FirstOrDefaultAsync();
            if (cp != null)
            {
                await DisplayAlert("Already Added", "You will find this program in your plans.", "OK");
                return;
            }

            if (!IsInternetConnected())
            {
                AddToMyPlansBtn.Text = "NO INTERNET";
                AddToMyPlansBtn.IsEnabled = true;
                return;
            }
            AddToMyPlansBtn.Text = "... ONE MOMENT";
            //Make an API call to add this contact program, make it active if it's the only one
            var result = await WebAPIService.AddToMyPlans(_client, _Program.GuidCRM);
            if (result == HttpStatusCode.OK)
            {
                App.Current.MainPage = new MainStartingPage();
                return;
            }
            else
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Error with Adding Plan" } });
                await DisplayAlert("Error", "Sorry, there was an error. Try again later.", "OK");
                AddToMyPlansBtn.IsEnabled = true;
                AddToMyPlansBtn.Text = "ADD TO MY PLANS";
            }
        }

        private async void WatchVideoBtn_Clicked(object sender, EventArgs e)
        {
            if (_ContactProgramState != "LEARN MORE")
            {
                bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("workout",_Program.Id)).Result;
                if (!allowDownload)
                {
                    await Navigation.PushModalAsync(new IAP(_Program.GuidCRM), false);
                    return;
                }

                if (_ContactProgramState == "WORKOUT")//Active
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Click Workout Button" } });
                    //Must add in code to go to workout tab using messaging centre
                    MessagingCenter.Send(this, "GoToWorkoutTab");

                    if (_IsInWorkoutNav != null)
                    {
                        await Navigation.PopAsync();
                    }
                    return;
                }
                else if (_ContactProgramState == "ACTIVATE") //Inactive and not completed
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Click Activate Button" } });
                    var result = await DisplayAlert("Activate Program", "Are you sure that you want to activate this program?", "Activate", "Not now");
                    if (result)
                    {
                        if (!IsInternetConnected())
                        {
                            await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                            return;
                        }

                        try
                        {
                            PurchaseBtn.IsEnabled = false;
                            PurchaseBtn.Text = "... ONE MOMENT";
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Activating Program" } });

                            //Request to activate this program, witch returns the programs contact program days and the contact program with new status.
                            //var request = new HttpRequestMessage();
                            //request.RequestUri = new Uri(App._absoluteUri, App._contactProgramsActivateUri + ContactProgram.GuidCRM);
                            //request.Method = HttpMethod.Get;
                            //request.Headers.Add("Accept", "application/json");
                            ////request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", Auth.Username, Auth.Password))));
                            //request.Headers.Add("Authorization", Auth.Token);
                            //var response = await _client.SendAsync(request);
                            //var error = await response.Content.ReadAsStringAsync();

                            HttpResponseMessage response = await WebAPIService.ActivateProgram(_client, _ContactProgram.GuidCRM);

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                //Reset all of the app
                                App.Current.MainPage = new MainStartingPage();
                                return;
                            }
                            else
                            {
                                await DisplayAlert("Error", "We had a small error. Please try again later.", "OK");
                                PurchaseBtn.IsEnabled = false;
                                PurchaseBtn.Text = "ACTIVATE";
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            PurchaseBtn.IsEnabled = true;
                            PurchaseBtn.Text = "ACTIVATE";
                            Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "WatchVideoBtn_Clicked()" } });
                        }
                    }
                    return;
                }
                else if (_ContactProgramState == "COMPLETED") //Inactive Complete
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Click Completed Button" } });
                    //Already compelte
                    return;
                }
                else if (_ContactProgramState == "ENDED") //Inactive Complete
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Click ENDED Button" } });
                    return;
                }
            }
            else
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Watch Program Promo Video" } });
                await Navigation.PushModalAsync(new WatchVideoPage(_videoUrl));
            }

        }

        private void AddWeekSeprator(int num)
        {
            var weekSL = new StackLayout
            {
                Spacing = 0,
                HeightRequest = 30,
                BackgroundColor = Color.FromHex("#909090"),//("#555757"),//#007077, #00bac6
                Orientation = StackOrientation.Horizontal,
            };

            var weekLabel = new Label
            {
                Text = "Week " + num.ToString(),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Start,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(15, 0, 0, 0)
            };
            weekSL.Children.Add(weekLabel);
           
            listOfProgramDays.Children.Add(weekSL);
        }

        private void AddWorkoutDay(LocalDBProgramDay programDay, int dayNumber)
        {
            var mainSL = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Spacing = 0,
                HeightRequest = 40,
                Padding = new Thickness(10, 5, 5, 5)
            };

            var daySL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 40,
                //BackgroundColor = Color.Gray,
                Padding = new Thickness(0, 0, 0, 0),
            };
            mainSL.Children.Add(daySL);

            var dayLabel = new Label
            {
                Text = "Day",
                FontSize = 9,
                FontFamily = "PingFangTC-Regular",
                TextColor = Color.FromHex("#909090"),
                HorizontalTextAlignment = TextAlignment.Center
            };

            var dayNum = new Label
            {
                Text = dayNumber.ToString(),
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "HelveticalNeue-Bold",
                TextColor = Color.FromHex("#909090"),
                HorizontalTextAlignment = TextAlignment.Center
            };

            daySL.Children.Add(dayLabel);
            daySL.Children.Add(dayNum);

            var barSL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 30,
                WidthRequest = 1,
                BackgroundColor = Color.FromHex("#909090"),
                Margin = new Thickness(10, 0, 10, 0),
            };

            mainSL.Children.Add(barSL);

            var SecondarySL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(5, 0, 0, 0)
                //BackgroundColor = Color.Blue
            };

            mainSL.Children.Add(SecondarySL);

            var arrowSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
                WidthRequest = 40,
                //BackgroundColor = Color.Yellow
            };

            var arrowImg = new Image
            {
                Source = "Cartelle_Black_Arrow_20px.png",
                HeightRequest = 15,
                WidthRequest = 15,
                HorizontalOptions = LayoutOptions.End,
                Opacity = 0.5
            };

            arrowSL.Children.Add(arrowImg);
            mainSL.Children.Add(arrowSL);

            var heading = new Label
            {
                Text = programDay.Heading,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromHex("#17191A"),
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
            };

            var headingSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
            };

            headingSL.Children.Add(heading);
            SecondarySL.Children.Add(headingSL);

            var bottomSL = new StackLayout
            {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
            };

            SecondarySL.Children.Add(bottomSL);

            var timeSL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                //WidthRequest = 60,
                Margin = new Thickness(0, 3, 0, 0)
            };

            var timeNumLabel = new Label
            {
                Text = programDay.TimeMinutes.ToString() + " mins",
                FontSize = 10,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromHex("#909090")
            };

            timeSL.Children.Add(timeNumLabel);
            bottomSL.Children.Add(timeSL);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                // handle the tap
                ProgramDayCell_Clicked(programDay);
            };

            mainSL.GestureRecognizers.Add(tapGestureRecognizer);
            listOfProgramDays.Children.Add(mainSL);
        }

        private void AddRestDay(LocalDBProgramDay programDay, int dayNumber)
        {
            var mainSL = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Spacing = 0,
                HeightRequest = 35,
                Padding = new Thickness(10, 5, 5, 5),
            };

            var daySL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 40,
                //BackgroundColor = Color.Gray,
                Padding = new Thickness(0, 0, 0, 0),
            };

            mainSL.Children.Add(daySL);

            var dayLabel = new Label
            {
                Text = "Day",
                FontSize = 10,
                FontFamily = "PingFangTC-Regular",
                TextColor = Color.FromHex("#909090"),
                HorizontalTextAlignment = TextAlignment.Center
            };

            var dayNum = new Label
            {
                Text = dayNumber.ToString(),
                FontSize = 13,
                TextColor = Color.FromHex("#909090"),
                FontAttributes = FontAttributes.Bold,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center
            };

            daySL.Children.Add(dayLabel);
            daySL.Children.Add(dayNum);

            var barSL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 20,
                WidthRequest = 1,
                BackgroundColor = Color.FromHex("#909090"),
                Margin = new Thickness(10, 0, 10, 0),
            };

            mainSL.Children.Add(barSL);

            var SecondarySL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Fill,
                //BackgroundColor = Color.Lime
                Margin = new Thickness(5, 0, 0, 0)
            };

            mainSL.Children.Add(SecondarySL);

            var heading = new Label
            {
                Text = programDay.Heading,
                FontSize = 12,
                TextColor = Color.FromHex("#17191A"),
                FontFamily = "HelveticalNeue-Bold",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
            };

            var headingSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Spacing = 0,
                //BackgroundColor = Color.Blue
            };

            headingSL.Children.Add(heading);
            SecondarySL.Children.Add(headingSL);

            listOfProgramDays.Children.Add(mainSL);
        }

        private void SetupProgramDays(List<LocalDBProgramDay> programDays)
        {
            listOfProgramDays.Children.Clear();

            int dayNumber = 1;
            bool AddWeekSL = true;
            int WeekNum = 1;

            foreach (var programDay in programDays)
            {
                if (AddWeekSL)
                {
                    AddWeekSeprator(WeekNum);
                    AddWeekSL = false;
                }

                if (programDay.DayTypeValue == 585860000)
                {
                    //WORKOUT
                    AddWorkoutDay(programDay, dayNumber);

                }
                else if (programDay.DayTypeValue == 585860001)
                {
                    //REST
                    AddRestDay(programDay, dayNumber);
                }

                dayNumber++;
                if (dayNumber > 7)
                {
                    WeekNum++;
                    dayNumber = 1;
                    AddWeekSL = true;
                }
            }
            NotLoadedYet.IsVisible = false;
        }

        private async void ProgramDayCell_Clicked(LocalDBProgramDay programDay)
        {
            if (programDay.DayTypeValue == 585860000)
            {
                //Workout Day, go to the ProgramDay Overview Page                
                await Navigation.PushAsync(new ProgramDayOverview(programDay));
            }

        }

        private async void DetermineBtnText(LocalDBContactProgram ContactProgram)
        {
            if (ContactProgram != null)
            {
                _ContactProgram = ContactProgram;
                var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == _ContactProgram.Id).ToListAsync();

                if (ContactProgramDays.Count() == ContactProgramDays.Where(x => x.StatusCodeValue == 585860004).ToList().Count() && ContactProgramDays.Count() != 0)
                {
                    _ContactProgramState = "COMPLETED";
                }
                else if (ContactProgram.StateCodeValue == 1 && ContactProgram.StatusCodeValue == 585860004) //Inactive and completed
                {
                    _ContactProgramState = "COMPLETED";
                }
                else if (ContactProgram.StatusCodeValue == 585860003)
                {
                    _ContactProgramState = "ENDED";
                }
                else if (ContactProgram.StateCodeValue == 1 && ContactProgram.StatusCodeValue != 585860004) //Inactive and Not completed
                {
                    _ContactProgramState = "ACTIVATE";
                }
                else if (ContactProgram.IsScheduleBuilt)
                {
                    var lastDate = ContactProgramDays.Where(x => x.ScheduledStartDate != null).OrderByDescending(x => x.ScheduledStartDate).Select(x => x.ScheduledStartDate).FirstOrDefault();

                    if (lastDate != null)
                    {
                        if (lastDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                        {
                            _ContactProgramState = "COMPLETED";
                        }
                    }
                }

                if (ContactProgram.StateCodeValue == 0 && _ContactProgramState == "LEARN MORE") //Active
                {
                    _ContactProgramState = "WORKOUT";
                }

            }
            PurchaseBtn.Text = _ContactProgramState;
            PurchaseBtn.IsVisible = true;
        }

        private async void GetProgramDays()
        {
            NotLoadedYet.IsVisible = true;
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == _ProgramGuid).FirstOrDefaultAsync();

            if (_ContactProgram == null)
            {
                var ContactPrograms = await _connection.Table<LocalDBContactProgram>().Where(x => x.ProgramId == Program.Id).ToListAsync();
                if (ContactPrograms.Count() != 0)
                {
                    var ContactProgram_Active = ContactPrograms.Where(x => x.StateCodeValue == 0).FirstOrDefault();
                    if (ContactProgram_Active != null)
                    {
                        DetermineBtnText(ContactProgram_Active);
                    }
                    else
                    {
                        var ContactProgram_Inactive = ContactPrograms.Where(x => x.StateCodeValue == 1).FirstOrDefault();
                        if (ContactProgram_Inactive != null)
                        {
                            DetermineBtnText(ContactProgram_Inactive);
                        }
                    }
                }
                else
                {
                    DetermineBtnText(_ContactProgram);
                }
            }
            else
            {
                DetermineBtnText(_ContactProgram);
            }

            var ProgramDays = await _connection.Table<LocalDBProgramDay>().Where(x => x.ProgramId == Program.Id).OrderBy(x => x.SequenceNumber).ToListAsync();
            if (ProgramDays.Count() > 0)
            {
                SetupProgramDays(ProgramDays);
            }
            else
            {
                if (IsInternetConnected())
                {
                    await WebAPIService.GetProgramDays(_client, Program.GuidCRM);

                    var ProgramList = await _connection.Table<LocalDBProgramDay>().Where(x => x.ProgramId == Program.Id).OrderBy(x => x.SequenceNumber).ToListAsync();
                    if (ProgramList.Count() > 0)
                    {
                        SetupProgramDays(ProgramList);
                    }
                }
            }
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
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
