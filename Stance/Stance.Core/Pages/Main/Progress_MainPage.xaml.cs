
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Stance.Models.LocalDB;
using Stance.Pages.Sub;
using Stance.Utils;
using Stance.Utils.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stance.Pages.Main
{
    public partial class Progress_MainPage : BaseContentPage
    {
        private const string _PageName = "Dashboard Tab";
        private string _ProgramHeading = "";
        private DateTime _lastPageSetupTime;
        private int _minutesBetweenRefresh = 60;

        public Progress_MainPage()
        {
            InitializeComponent();

            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            Padding = new Thickness(0,0, 0, 0);
            BackgroundColor = Color.Black;

            MessagingCenter.Subscribe<Workout_MainPage>(this, "ProgramStarted", (sender) => { SetupDashboard(); });
            MessagingCenter.Subscribe<WorkoutSurvey>(this, "RefreshDashboard", (sender) => { SyncToCRM(); });
            MessagingCenter.Subscribe<WorkoutSurvey>(this, "WorkoutSurveyComplete", (sender) => { AfterWorkoutShowIAP(); });
            MessagingCenter.Subscribe<App>(this, "OnResume", (sender) => { OnResumeUpdatePage(); });
            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");

            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            if (IsInternetConnected())
            {
                NoNetwork.IsVisible = false;
            }
            else
            {
                NoNetwork.IsVisible = true;
            }

            //var toolItem1 = new ToolbarItem
            //{
            //    Icon = "user_26.png",
            //    Order = ToolbarItemOrder.Primary,
            //    Priority = 0
            //};

            //toolItem1.Clicked += (s, e) =>
            //{
            //    ProfileBtn_Clicked(s, e);
            //};

            //ToolbarItems.Add(toolItem1);

            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily)
            {
                var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                if (isDeviceIphoneXorXS)
                {
                   // ProgramImage.HeightRequest = 650;

                    if (Xamarin.Essentials.DeviceInfo.Platform.Equals("iOS"))
                        ProgramImage.HeightRequest = 650;
                    else
                    {
                        ProgramImage.HeightRequest = App.screenHeight - 105;
                    }

                    ProgressBox.Margin = new Thickness(0, 300, 0, 0);
                }
                else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                {
                    if (Xamarin.Essentials.DeviceInfo.Platform.Equals("iOS"))
                        ProgramImage.HeightRequest = 735;
                    else
                    {

                        ProgramImage.HeightRequest = App.screenHeight - 95;
                    }

                    //ProgramImage.HeightRequest = 735;
                    ProgressBox.Margin = new Thickness(0, 350, 0, 0);
                }
                else
                {
                    ProgramImage.HeightRequest = 650;
                    ProgressBox.Margin = new Thickness(0, 300, 0, 0);
                }
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                if (isDeviceIphonePlus)
                {
                    ProgramImage.HeightRequest = 635;
                    ProgressBox.Margin = new Thickness(0, 300, 0, 0);
                }
                else
                {
                    ProgramImage.HeightRequest = 565;
                    ProgressBox.Margin = new Thickness(0, 230, 0, 0);
                }
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                if (isDeviceLargeIPad)
                {
                    ProgramImage.HeightRequest = 1250;
                    ProgressBox.Margin = new Thickness(0, 800, 0, 0);
                }
                else
                {
                    ProgramImage.HeightRequest = 910;
                    ProgressBox.Margin = new Thickness(0, 550, 0, 0);
                }
            }
            else
            {
                ProgramImage.HeightRequest = 600;
                ProgressBox.Margin = new Thickness(0, 200, 0, 0);
            }

            GettingSetupSL.IsVisible = true;
            NonAuthSL.IsVisible = false;
            DoesNotHaveActiveProgram.IsVisible = false;
            JustCompletedProgram.IsVisible = false;
            Dashboard.IsVisible = false;
            NotLoadedYetProgramDay.IsVisible = false;

            _lastPageSetupTime = DateTime.UtcNow;
            SyncToCRM();
        }

        private async void AfterWorkoutShowIAP()
        {
            SetupDashboard();

            var ActiveContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.StateCodeValue == 0).FirstOrDefaultAsync(); // Active
            if (ActiveContactProgram == null)
                return;

            var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ActiveContactProgram.ProgramId).FirstOrDefaultAsync();
            if (Program == null)
                return;

            if (!Program.Heading.Contains("FAB IN FIVE"))
                return;

            //do they already have a valid full subscription?
            var fullSubscriptions = await _connection.Table<LocalDBSubscription>().Where(x => x.StateCodeValue == 0 && x.Type == 866660003).ToListAsync();//Active 
            foreach (var sub in fullSubscriptions)
            {
                if (sub.StatusCodeValue == 866660000 && sub.TrialEndDate >= DateTime.UtcNow)//In Trial
                {
                    return;
                }
                else if ((sub.StatusCodeValue == 866660001 || sub.StatusCodeValue == 866660005 || sub.StatusCodeValue == 866660008) && sub.EndDate >= DateTime.UtcNow) //Active Subscription
                {
                    return;
                }
            }

            await Navigation.PushModalAsync(new IAP_FIF(Program.GuidCRM, ActiveContactProgram), false);

        }

        private async void SetupDashboard()
        {
            NotLoadedYetProgramDay.IsVisible = true;
            var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.StateCodeValue == 0).FirstOrDefaultAsync(); // Active

            if (ContactProgram == null)
            {
                var ContactProgram_justComplete = await _connection.Table<LocalDBContactProgram>().Where(x => x.StateCodeValue == 1).ToListAsync();

                foreach (var item in ContactProgram_justComplete)
                {
                    if (item.EndDate.HasValue)
                    {
                        if (item.EndDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                        {
                            //just finished
                            Dashboard.IsVisible = false;
                            GettingSetupSL.IsVisible = false;
                            DoesNotHaveActiveProgram.IsVisible = false;
                            JustCompletedProgram.IsVisible = true;
                            return;
                        }
                    }
                }



                //No active programs, so much be complete or need to start a program
                Dashboard.IsVisible = false;
                GettingSetupSL.IsVisible = false;
                JustCompletedProgram.IsVisible = false;
                DoesNotHaveActiveProgram.IsVisible = true;
                return;
            }

            if (ContactProgram.StatusCodeValue == 585860000 || ContactProgram.StatusCodeValue == 585860001) //InProgress, Complete
            {
                var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ContactProgram.Id && x.IsRepeat != true).ToListAsync();

                if (ContactProgramDays.Count() == 0)
                    return;

                decimal DaysPastCount = 0M;
                int currentDayNumber = 1;
                foreach (var day in ContactProgramDays)
                {
                    if (day.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date || day.IsComplete == true || day.StateCodeValue == 1) //Inactive and Compeleted && day.StatusCodeValue == 585860004
                    {
                        DaysPastCount++;
                    }

                    if (day.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                    {
                        if (currentDayNumber < ContactProgramDays.Count())
                        {
                            currentDayNumber++;
                        }
                    }
                }
                decimal PercentComplete = (DaysPastCount * 100) / (decimal)ContactProgramDays.Count();

                //Completed days (active and inactive), and workout day
                int TotalExercisedDays = ContactProgramDays.Where(x => x.StatusCodeValue == 585860001 || x.StatusCodeValue == 585860004 && x.DayTypeValue == 866660000).Count();

                //Incomplete days (not complete either active or inatice), and not workout
                int DaysMissedCount = 0;

                var daysPotentiallyMissed = ContactProgramDays.Where(x => x.ScheduledStartDate != null && x.DayTypeValue == 866660000 & x.StatusCodeValue != 585860001 && x.StatusCodeValue != 585860004).ToList();

                foreach (var day in daysPotentiallyMissed)
                {
                    if (day.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                    {
                        DaysMissedCount++;
                    }
                }

                DaysExercised.Text = TotalExercisedDays.ToString();
                PercentageDaysPast.Text = Math.Round(PercentComplete, 0, MidpointRounding.AwayFromZero) + "%";
                DaysMissed.Text = DaysMissedCount.ToString();
                ScheduleTracking.Text = "Day " + currentDayNumber + " of " + ContactProgramDays.Count();

                var ProgramActive = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ContactProgram.ProgramId).FirstOrDefaultAsync();

                if (ProgramActive == null)
                    return;

                //var imgSource = new UriImageSource() { Uri = new Uri(ProgramActive.SecondaryPhotoUrl) };
                //imgSource.CachingEnabled = true;
                //imgSource.CacheValidity = TimeSpan.FromDays(7);
                //programImage.Source = imgSource;

                programImage.CacheDuration = TimeSpan.FromDays(30);
                programImage.RetryCount = 5;
                programImage.RetryDelay = 250;
                programImage.BitmapOptimizations = true;

                try
                {
                    programImage.Source = new UriImageSource() { Uri = new Uri(ProgramActive.SecondaryPhotoUrl) };
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SecondaryPhotoUrl Error" } });
                }

                programName.Text = ProgramActive.Heading;
                _ProgramHeading = ProgramActive.Heading;
                numberOfWeeks.Text = ProgramActive.TotalWeeks.ToString();
                programGoal.Text = ProgramActive.Goal;
                programLevel.Text = ProgramActive.Level;
                GettingSetupSL.IsVisible = false;
                NonAuthSL.IsVisible = false;
                JustCompletedProgram.IsVisible = false;
                DoesNotHaveActiveProgram.IsVisible = false;
                Dashboard.IsVisible = true;
                NotLoadedYetProgramDay.IsVisible = false;

            }
            else
            {
                GettingSetupSL.IsVisible = false;
                NonAuthSL.IsVisible = false;
                JustCompletedProgram.IsVisible = false;
                DoesNotHaveActiveProgram.IsVisible = true;
                Dashboard.IsVisible = false;
                NotLoadedYetProgramDay.IsVisible = false;
            }

        }

        private async void SyncToCRM()
        {
            _lastPageSetupTime = DateTime.UtcNow;
            SetupDashboard();

            if (IsInternetConnected())
            {
                await WebAPIService.SyncToCRM(_client);
            }
        }

        public static List<Task> TaskList = new List<Task>();

        private void OnResumeUpdatePage()
        {
            if (Auth.IsAuthenticated())
            {
                var refreshTime = _lastPageSetupTime.AddMinutes(60);
                if (DateTime.UtcNow > refreshTime)
                {
                    SyncToCRM();
                }
            }
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ProgramHeading }, { "Action", "OnAppearing" } });

            if (Auth.IsAuthenticated())
            {
                //MessagingCenter.Send(this, "GoToWorkoutTab_Call1");

                var refreshTime = _lastPageSetupTime.AddMinutes(_minutesBetweenRefresh);
                if (DateTime.UtcNow > refreshTime)
                {
                    SyncToCRM();
                }
            }
            base.OnAppearing();
        }

        private async void ProfileBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PersonalProfile());
        }

        private async void ReloadBtn_Clicked(object sender, EventArgs e)
        {
            ReloadBtn.IsEnabled = false;
            ReloadBtn.Text = "... ONE MOMENT";

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "ReloadBtn_Clicked" } });

                var response = await WebAPIService.RefreshApp(_client);

                if (response.StatusCode == HttpStatusCode.OK)
                {
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
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ReloadBtn_Clicked" } });
                ReloadBtn.IsEnabled = true;
                ReloadBtn.Text = "   RELOAD   ";
            }
            ReloadBtn.IsEnabled = true;
            ReloadBtn.Text = "   RELOAD   ";

        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (IsInternetConnected())
            {
                NoNetwork.IsVisible = false;
            }
            else
            {
                NoNetwork.IsVisible = true;
            }
        }


    }
}
