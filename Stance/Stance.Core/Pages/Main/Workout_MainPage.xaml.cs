using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Stance.Pages.Sub;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Stance.Utils;
using System.Net.Http;
using System.Net;
using Stance.Models.LocalDB;
using PCLStorage;
using System.IO;
using ModernHttpClient;
using Stance.ViewModels;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using FFImageLoading.Forms;
using BranchXamarinSDK;

namespace Stance.Pages.Main
{
    public partial class Workout_MainPage : BaseContentPage
    {
        //Animation
        //    Rotateto
        //    fadeto
        //    layoutto
        // public async Task AnimateIn() { await Task.WhenAll( first.fato.(), nextFato())  }

        private const string _PageName = "Workout Tab";
        private string _ActiveProgram = "";
        //private List<APIContactProgram> _contactPrograms = new List<APIContactProgram>();
        private Guid _ProgramDayGuid = Guid.Empty;
        private Guid _ContactProgramGuid = Guid.Empty;
        private Guid _ContactProgramDayGuid = Guid.Empty;
        private bool _DownloadingInProgress = false;
        private Dictionary<Guid, int> _numberOfDownloadAttempts = new Dictionary<Guid, int>();

        private static bool _IsWorkoutDownloaded = false;
        private static double _totalBytes;
        private DateTime _lastPageSetupTime;
        private int _minutesBetweenRefresh = 30;
        private int _hoursBetweenSync = 6;
        private int _hoursBetweenSubscriptionSync = 1;
        private bool _deactiveTopNav = false;

        private List<holdingCell> _scheduledCPD = new List<holdingCell>();
        private List<holdingCell> _weekToSave = new List<holdingCell>();
        private int _weekNumToSave = 0;
        private bool _isEditingSchedule = false;
        private holdingCell _todayScheduleCell = new holdingCell();

        public Workout_MainPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            NavigationPage.SetHasNavigationBar(this, false);
            Padding = new Thickness(0, 0, 0, 0);
            BackgroundColor = Color.Black;
            MessagingCenter.Subscribe<App>(this, "OnResume", (sender) => { OnResumeUpdatePage(); });
            MessagingCenter.Subscribe<ProgramOverview_v1>(this, "GoToWorkoutTab", (sender) => { GoToWorkoutTab(); });
            MessagingCenter.Subscribe<WorkoutSurvey>(this, "WorkoutSurveyComplete", (sender) => { WorkoutSurveyComplete(); });
            MessagingCenter.Subscribe<InitialLoading>(this, "RemovedModal", (sender) => { ResetApp(); });
            MessagingCenter.Subscribe<ProgramDayOverview>(this, "DownloadedCurrentDay", (sender) => { SetupPageV2(); });
            MessagingCenter.Subscribe<ProgramDay_v2>(this, "ExitWorkout", (sender) => { ExitWorkoutShowIAP(); });

            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            CrossConnectivity.Current.ConnectivityTypeChanged += HandleConnectivityTypeChanged;

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
            //    Priority = 1
            //};

            //toolItem1.Clicked += (s, e) =>
            //{
            //    ProfileBtn_Clicked(s, e);
            //};

            var toolItem2 = new ToolbarItem
            {
                Icon = "Message_26.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
            };

            toolItem2.Clicked += (s, e) =>
            {
                //InboxBtn_Clicked(s, e);
            };

            //var toolItem3 = new ToolbarItem
            //{
            //    Text = "My Plans",
            //    Order = ToolbarItemOrder.Secondary,
            //    Priority = 0
            //};

            //toolItem3.Clicked += (s, e) =>
            //{
            //    MyPlansBtn_Clicked(s, e);
            //};

            //var toolItem4 = new ToolbarItem
            //{
            //    Text = "Today",
            //    Order = ToolbarItemOrder.Secondary,
            //    Priority = 1
            //};

            //toolItem4.Clicked += (s, e) =>
            //{
            //    TodayBtn_Clicked(s, e);
            //};

            //var toolItem5 = new ToolbarItem
            //{
            //    Text = "Schedule",
            //    Order = ToolbarItemOrder.Secondary,
            //    Priority = 2,                                
            //};

            //toolItem5.Clicked += (s, e) =>
            //{
            //    MyWorkoutsBtn_Clicked(s, e);
            //};

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                MyPlansBtn_Clicked();
            };

            MyPlans_ToolBarItem.GestureRecognizers.Add(tapGestureRecognizer);

            var tapGestureRecognizer2 = new TapGestureRecognizer();
            tapGestureRecognizer2.Tapped += (s, e) =>
            {
                TodayBtn_Clicked();
            };

            Today_ToolBarItem.GestureRecognizers.Add(tapGestureRecognizer2);

            var tapGestureRecognizer3 = new TapGestureRecognizer();
            tapGestureRecognizer3.Tapped += (s, e) =>
            {
                ScheduleBtn_Clicked();
            };

            Schedule_ToolBarItem.GestureRecognizers.Add(tapGestureRecognizer3);

            //ToolbarItems.Add(toolItem1);
            //ToolbarItems.Add(toolItem2);
            //ToolbarItems.Add(toolItem3);
            //ToolbarItems.Add(toolItem4);
            //ToolbarItems.Add(toolItem5);
            //MainSL.BackgroundColor = Color.Aqua;

            _lastPageSetupTime = DateTime.UtcNow;

            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily)
            {
                var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                if (isDeviceIphoneXorXS)
                {
                    ProgramDayButtonSL.Margin = new Thickness(0, 330, 0, 0);
                    ProgramButtonSL.Margin = new Thickness(0, 330, 0, 0);
                    NotStartedProgram.HeightRequest = 660;
                    NoActiveProgram.HeightRequest = 660;

                    ProgramCompleteScreen.HeightRequest = 650;
                    ProgramNotFoundScreen.HeightRequest = 650;
                }
                else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                {
                    ProgramDayButtonSL.Margin = new Thickness(0, 380, 0, 0);
                    ProgramButtonSL.Margin = new Thickness(0, 380, 0, 0);
                    NotStartedProgram.HeightRequest = 725;
                    NoActiveProgram.HeightRequest = 725;

                    ProgramCompleteScreen.HeightRequest = 715;
                    ProgramNotFoundScreen.HeightRequest = 715;
                }
                else
                {
                    ProgramDayButtonSL.Margin = new Thickness(0, 330, 0, 0);
                    ProgramButtonSL.Margin = new Thickness(0, 330, 0, 0);
                    NotStartedProgram.HeightRequest = 660;
                    NoActiveProgram.HeightRequest = 660;

                    ProgramCompleteScreen.HeightRequest = 650;
                    ProgramNotFoundScreen.HeightRequest = 650;
                }
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                if (isDeviceIphonePlus)
                {
                    ProgramDayButtonSL.Margin = new Thickness(0, 340, 0, 0);
                    ProgramButtonSL.Margin = new Thickness(0, 340, 0, 0);
                    NotStartedProgram.HeightRequest = 630;
                    NoActiveProgram.HeightRequest = 630;

                    ProgramCompleteScreen.HeightRequest = 625;
                    ProgramNotFoundScreen.HeightRequest = 625;
                }
                else
                {
                    ProgramDayButtonSL.Margin = new Thickness(0, 290, 0, 0);
                    ProgramButtonSL.Margin = new Thickness(0, 290, 0, 0);
                    NotStartedProgram.HeightRequest = 580;
                    NoActiveProgram.HeightRequest = 580;

                    ProgramCompleteScreen.HeightRequest = 560;
                    ProgramNotFoundScreen.HeightRequest = 560;
                }
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                if (isDeviceLargeIPad)
                {
                    ProgramDayButtonSL.Padding = new Thickness(0, 540, 0, 0);
                    ProgramButtonSL.Padding = new Thickness(0, 540, 0, 0);

                    NotStartedProgram.HeightRequest = 1270;
                    NoActiveProgram.HeightRequest = 1270;

                    ProgramCompleteScreen.HeightRequest = 1240;
                    ProgramNotFoundScreen.HeightRequest = 1240;
                }
                else
                {
                    ProgramDayButtonSL.Padding = new Thickness(0, 340, 0, 0);
                    ProgramButtonSL.Padding = new Thickness(0, 340, 0, 0);

                    NotStartedProgram.HeightRequest = 940;
                    NoActiveProgram.HeightRequest = 940;

                    ProgramCompleteScreen.HeightRequest = 940;
                    ProgramNotFoundScreen.HeightRequest = 940;
                }


                MyPlans_ToolBarItem.WidthRequest = 270;
                Today_ToolBarItem.WidthRequest = 270;
                Schedule_ToolBarItem.WidthRequest = 270;
            }
            else
            {
                ProgramDayButtonSL.Padding = new Thickness(0, 75, 0, 0);
                ProgramButtonSL.Padding = new Thickness(0, 100, 0, 0);

                NotStartedProgram.HeightRequest = 700;
                NoActiveProgram.HeightRequest = 700;
            }

            //ShowConfirmEmailIfRequired();
            ShowInitialScreenConfiguration();
            SetupPageV2();
            SetupPrograms();
            SyncData();
        }

        //private async void ShowConfirmEmailIfRequired()
        //{
        //    var profile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();
        //    if (profile != null)
        //    {
        //        var cpds = await _connection.Table<LocalDBContactProgramDayV2>().ToListAsync();
        //        if (profile.ConfirmedEmail != true && cpds.Count() == 0)
        //        {
        //            await Navigation.PushModalAsync(new ConfirmEmail(), false);
        //            return;
        //        }
        //    }
        //}

        private void ShowInitialScreenConfiguration()
        {
            //downloadingText.IsVisible = true;
            ProgressBar.IsVisible = false;
            NotLoadedYet.IsVisible = false;

            MyPlansSL.IsVisible = false;
            TodaySL.IsVisible = true;
            ScheduleSL.IsVisible = false;

            MyPlansSL.Opacity = 0;
            TodaySL.Opacity = 1;
            ScheduleSL.Opacity = 0;

            ProgramScreen.IsVisible = false;
            ProgramDayScreen.IsVisible = false;
            ProgramDayScreenRest.IsVisible = false;
            ProgramCompleteScreen.IsVisible = false;
            ProgramNotFoundScreen.IsVisible = false;
            NoActiveProgram.IsVisible = false;
            NotStartedProgram.IsVisible = false;

            ShowLoadingScreen_L1();
        }

        private void ShowLoadingScreen_L1()
        {
            NoProgram.IsVisible = false;
            HasAProgram.IsVisible = false;
            LoadingSL.IsVisible = true;
        }

        private void ShowHasProgram_L1()
        {
            LoadingSL.IsVisible = false;
            NoProgram.IsVisible = false;
            HasAProgram.IsVisible = true;
        }

        private void ShowNoProgramScreen_L1()
        {
            LoadingSL.IsVisible = false;
            HasAProgram.IsVisible = false;
            NoProgram.IsVisible = true;
        }

        private void Show_Today_ContactProgramDayScreen_L2(string dayType)
        {
            ShowHasProgram_L1();
            //MyPlansSL.IsVisible = false;
            //TodaySL.IsVisible = true;
            //MyWorkoutsSL.IsVisible = false;

            if (dayType == "workout")
            {
                ProgramScreen.IsVisible = false;
                ProgramDayScreen.IsVisible = true;
                ProgramDayScreenRest.IsVisible = false;
                ProgramCompleteScreen.IsVisible = false;
                ProgramNotFoundScreen.IsVisible = false;
            }
            else if (dayType == "rest")
            {
                ProgramScreen.IsVisible = false;
                ProgramDayScreen.IsVisible = false;
                ProgramDayScreenRest.IsVisible = true;
                ProgramCompleteScreen.IsVisible = false;
                ProgramNotFoundScreen.IsVisible = false;
            }
        }

        private void Show_Today_ContactProgramScreen_L2()
        {
            ShowHasProgram_L1();
            //MyPlansSL.IsVisible = false;
            //TodaySL.IsVisible = true;
            //MyWorkoutsSL.IsVisible = false;

            ProgramScreen.IsVisible = true;
            ProgramDayScreen.IsVisible = false;
            ProgramDayScreenRest.IsVisible = false;
            ProgramCompleteScreen.IsVisible = false;
            ProgramNotFoundScreen.IsVisible = false;

            Show_Schedule_NotStartedProgram_L2();
        }

        private void Show_Today_ProgramCompleteScreen_L2()
        {
            ShowHasProgram_L1();
            //MyPlansSL.IsVisible = false;
            //TodaySL.IsVisible = true;
            //MyWorkoutsSL.IsVisible = false;

            ProgramScreen.IsVisible = false;
            ProgramDayScreen.IsVisible = false;
            ProgramDayScreenRest.IsVisible = false;
            ProgramCompleteScreen.IsVisible = true;
            ProgramNotFoundScreen.IsVisible = false;
        }

        private void Show_Today_NoActiveProgramScreen_L2()
        {
            ShowHasProgram_L1();
            //MyPlansSL.IsVisible = false;
            //TodaySL.IsVisible = true;
            //MyWorkoutsSL.IsVisible = false;

            ProgramScreen.IsVisible = false;
            ProgramDayScreen.IsVisible = false;
            ProgramDayScreenRest.IsVisible = false;
            ProgramCompleteScreen.IsVisible = false;
            ProgramNotFoundScreen.IsVisible = true;

            Show_Schedule_NoActiveProgram_L2();
        }

        private void Show_Today_ProgramDayLoader()
        {
            NotLoadedYetProgramDay.IsVisible = true;
            NotLoadedYetProgramRestDay.IsVisible = true;
        }

        private void Hide_Today_ProgramDayLoader()
        {
            NotLoadedYetProgramDay.IsVisible = false;
            NotLoadedYetProgramRestDay.IsVisible = false;
        }

        private void Show_Schedule_NotStartedProgram_L2()
        {
            ShowHasProgram_L1();
            //MyPlansSL.IsVisible = false;
            //TodaySL.IsVisible = false;
            //MyWorkoutsSL.IsVisible = true;

            NotStartedProgram.IsVisible = true;
            NoActiveProgram.IsVisible = false;
        }

        private void Show_Schedule_NoActiveProgram_L2()
        {
            ShowHasProgram_L1();
            //MyPlansSL.IsVisible = false;
            //TodaySL.IsVisible = false;
            //MyWorkoutsSL.IsVisible = true;

            NotStartedProgram.IsVisible = false;
            NoActiveProgram.IsVisible = true;
        }

        private void Show_Schedule_HasActiveProgram_L2()
        {
            ShowHasProgram_L1();
            //MyPlansSL.IsVisible = false;
            //TodaySL.IsVisible = false;
            //MyWorkoutsSL.IsVisible = true;

            NotStartedProgram.IsVisible = false;
            NoActiveProgram.IsVisible = false;
        }

        private void Show_Workout_Loader()
        {
            LoadingSL.IsVisible = true;
        }

        private void Hide_Workout_Loader()
        {
            HasAProgram.IsVisible = true;
            LoadingSL.IsVisible = false;
        }

        private void ShowOrHideNoNetwork()
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

        private void WorkoutSurveyComplete()
        {
            ScheduleProgramDaysTopSL.IsVisible = false;
            listOfProgramDays.Children.Clear();
            DisplayProgramDayMessage();
            GoToWorkoutTab();
            //SetupSchedule(_ContactProgramGuid);
            SetupPageV2();
        }

        private async void ExitWorkoutShowIAP()
        {
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

        private void HideChallengeButtons()
        {
            ScheduleProgramDaysTopSL.IsVisible = false;
            ResetScheduleSL.IsVisible = false;
        }

        private async void DisplayProgramDayMessage()
        {
            var ContactPrograms = await _connection.Table<LocalDBContactProgram>().ToListAsync();
            if (ContactPrograms.Count() != 0)
            {
                var ActiveContactProgram = ContactPrograms.Where(x => x.StatusCodeValue == 585860000).FirstOrDefault(); //Active and Inprogress
                if (ActiveContactProgram != null)
                {
                    if (ActiveContactProgram.IsScheduleBuilt == false)
                    {
                        var currentProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ActiveContactProgram.Id && x.SequenceNumber == 1 && x.IsRepeat != true).FirstOrDefaultAsync();

                        if (currentProgramDay != null)
                        {
                            if (currentProgramDay.IsComplete == true || currentProgramDay.StateCodeValue == 1)
                            {
                                if (currentProgramDay.ScheduledStartDate == null)
                                {   //Ask to build program schedule
                                    //ProgramDayButtonSL.Padding = new Thickness(0, 10, 0, 0);
                                    GoBtnSL.IsVisible = false;
                                    ProgressBar.IsVisible = false;
                                    FirstProgramDayCompleteNowScheduleDays.IsVisible = true;
                                    programDateSL.IsVisible = false;
                                    if (ActiveContactProgram.ChallengeId != 0)
                                    {
                                        FirstProgramDayCompleteText.Text = "Congratulations on completing your workout.";
                                        DayCompleteNowScheduleDays.IsVisible = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var ProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ActiveContactProgram.Id && x.IsRepeat != true).ToListAsync();
                        foreach (var day in ProgramDays)
                        {
                            if (day.ScheduledStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date && day.IsComplete == true)
                            {
                                //ProgramDayButtonSL.Padding = new Thickness(0, 10, 0, 0);
                                //GoBtnSL.IsVisible = false;
                                ProgressBar.IsVisible = false;
                                DayCompleteNowScheduleDays.IsVisible = false;
                                FirstProgramDayCompleteNowScheduleDays.IsVisible = true;
                                programDateSL.IsVisible = false;
                                FirstProgramDayCompleteText.Text = "Tomorrow your program will auto advance";
                                FirstProgramDayCompleteText.FontSize = 12;
                                GoBtn.Text = "COMPLETED";

                                if (ActiveContactProgram.ChallengeId != 0)
                                {
                                    FirstProgramDayCompleteText.Text = "Tomorrow your challenge will auto advance";
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private async void GoToWorkoutTab()
        {
            if (!TodaySL.IsVisible)
            {
                await AnimateIn(1);
            }
        }

        private async void OnResumeUpdatePage()
        {
            var refreshTime = _lastPageSetupTime.AddMinutes(_minutesBetweenRefresh);
            if (DateTime.UtcNow > refreshTime && !_DownloadingInProgress)
            {
                bool syncRequired = await SyncRequired();
                bool subSyncRequired = await SubscriptionSyncRequired();
                if (syncRequired)
                {
                    SyncData();
                }
                else if (subSyncRequired)
                {
                    SubcriptionSyncData();
                }
                else
                {
                    SetupPageV2();
                }
            }
        }

        private async Task<bool> SyncRequired()
        {
            var synced = await _connection.Table<LocalDBSync>().FirstOrDefaultAsync();
            if (synced != null)
            {
                if (DateTime.UtcNow > synced.SyncedOn?.AddHours(_hoursBetweenSync))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private async Task<bool> SubscriptionSyncRequired()
        {
            var synced = await _connection.Table<LocalDBSync>().FirstOrDefaultAsync();
            if (synced != null)
            {
                if (DateTime.UtcNow > synced.SubscriptionSyncedOn?.AddHours(_hoursBetweenSubscriptionSync))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private async void SubcriptionSyncData()
        {
            bool syncRequired = await SubscriptionSyncRequired();
            if (!syncRequired)
                return;

            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "SubcriptionSyncData()" } });

            var response = await WebAPIService.GetSubscriptions(_client);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "SubcriptionSyncData() Complete" } });
                return;
            }
            else
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "SubcriptionSyncData() Failed" } });
            }
        }

        private async void SyncData()
        {
            if (!IsInternetConnected())
                return;

            bool syncRequired = await SyncRequired();
            if (!syncRequired)
            {
                SubcriptionSyncData();
                return;
            }

            SyncIndicator.IsVisible = true;
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Starting Auto Sync" } });
            //call API to get the latest subscription, contact program and days and update the DB, refresh app if necessary (CP state change, or new CP)
            var subscriptionsCount = (await _connection.Table<LocalDBSubscription>().ToListAsync()).Count();
            var contactPrograms = await _connection.Table<LocalDBContactProgram>().ToListAsync();
            var activeContactProgram = contactPrograms.Where(x => x.StateCodeValue == 0).FirstOrDefault();

            var response = await WebAPIService.RefreshApp(_client);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Auto Sync Complete" } });
                var subscriptionsCount2 = (await _connection.Table<LocalDBSubscription>().ToListAsync()).Count();
                var contactPrograms2 = await _connection.Table<LocalDBContactProgram>().ToListAsync();
                var activeContactProgram2 = contactPrograms2.Where(x => x.StateCodeValue == 0).FirstOrDefault();

                if (subscriptionsCount != subscriptionsCount2 || activeContactProgram.GuidCRM != activeContactProgram2.GuidCRM || contactPrograms.Count() != contactPrograms2.Count())
                {
                    //only refresh the app if their are new subscriptions or the active contact program has changed or their are new programs
                    App.Current.MainPage = new MainStartingPage();
                }
                SyncIndicator.IsVisible = false;
                return;
            }
            else
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Auto Sync Failed" } });
            }
            SyncIndicator.IsVisible = false;

        }

        private async void SetupPageV2()
        {
            _lastPageSetupTime = DateTime.UtcNow;

            //Handle refreshing
            //if (_CurrentlyOnProgramPage)
            //{
            //    //do nothing - do not show loader
            //}
            //else if (!_CurrentlyOnProgramDay)
            //{
            //    LoadingSL.IsVisible = true;
            //}
            //else
            //{
            //    Show_Today_ProgramDayLoader();
            //}
            var ContactPrograms = await _connection.Table<LocalDBContactProgram>().ToListAsync();

            if (ContactPrograms.Count() == 0)
            {
                ShowNoProgramScreen_L1();
            }
            else
            {
                var ActiveContactProgram = ContactPrograms.Where(x => x.StateCodeValue == 0).FirstOrDefault(); //Active

                if (ActiveContactProgram == null)
                {
                    Show_Today_NoActiveProgramScreen_L2();
                }
                else
                {
                    _ContactProgramGuid = ActiveContactProgram.GuidCRM;
                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ActiveContactProgram.ProgramId).FirstOrDefaultAsync();
                    if (Program != null)
                    {
                        _ActiveProgram = Program.Heading;
                        ShowGiftBox(Program);
                    }

                    if (ActiveContactProgram.ChallengeId != 0)
                    {
                        HideChallengeButtons();
                        CompleteProgramText.Text = "You have completed your challenge";
                        RepeatProgramBtn.Text = "REPEAT AS PROGRAM";
                    }

                    if (ActiveContactProgram.StatusCodeValue == 1) //Active
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Program Status", "Active" } });
                        //Get Program Data - to start program
                        DisplayContactProgram(ActiveContactProgram);
                    }
                    else if (ActiveContactProgram.StatusCodeValue == 585860000) //InProgress
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Program Status", "InProgress" } });
                        //Get ProgramDay Data - to download
                        SetupActiveProgramDay_V2(ActiveContactProgram);
                    }
                    else if (ActiveContactProgram.StatusCodeValue == 585860001) //Complete
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Program Status", "Complete" } });

                        Show_Today_ProgramCompleteScreen_L2();
                        SetupSchedule(ActiveContactProgram);
                    }
                }
            }
        }

        private async void ShowGiftBox(LocalDBProgram Program)
        {
            if (!Program.Heading.Contains("FAB IN FIVE"))
                return;

            //do they already have a valid full subscription?
            var fullSubscriptions = await _connection.Table<LocalDBSubscription>().Where(x => x.StateCodeValue == 0 && x.Type == 866660003).ToListAsync();//Active 
            foreach (var sub in fullSubscriptions)
            {
                if (sub.StatusCodeValue == 866660000 && sub.TrialEndDate >= DateTime.UtcNow)//In Trial
                {
                    GiftBox.IsVisible = false;
                    GiftBoxRest.IsVisible = false;
                    return;
                }
                else if ((sub.StatusCodeValue == 866660001 || sub.StatusCodeValue == 866660005 || sub.StatusCodeValue == 866660008) && sub.EndDate >= DateTime.UtcNow) //Active Subscription
                {
                    GiftBox.IsVisible = false;
                    GiftBoxRest.IsVisible = false;
                    return;
                }
            }
            GiftBox.IsVisible = true;
            GiftBoxRest.IsVisible = true;
        }

        private async void DisplayContactProgram(LocalDBContactProgram ActiveContactProgram)
        {
            if (ActiveContactProgram.ChallengeId != 0)
            {
                VideoBtn.IsEnabled = false;
                VideoBtn.Text = "Not Started Yet";
                ChallengeBox.IsVisible = true;
            }

            var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ActiveContactProgram.ProgramId).FirstOrDefaultAsync();

            programName.Text = Program.Heading;

            //var imgSource = new UriImageSource() { Uri = new Uri(Program.SecondaryPhotoUrl) };
            //imgSource.CachingEnabled = true;
            //imgSource.CacheValidity = TimeSpan.FromDays(7);
            //programImage.Source = imgSource;

            programImage.CacheDuration = TimeSpan.FromDays(30);
            programImage.RetryCount = 5;
            programImage.RetryDelay = 250;
            programImage.BitmapOptimizations = true;

            try
            {
                programImage.Source = new UriImageSource() { Uri = new Uri(Program.SecondaryPhotoUrl) };
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SecondaryPhotoUrl Error" } });
            }

            numberOfWeeks.Text = Program.TotalWeeks.ToString();
            programGoal.Text = Program.Goal;
            programLevel.Text = Program.Level;

            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily)
            {
                var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                if (isDeviceIphoneXorXS)
                {
                    
                    if (Xamarin.Essentials.DeviceInfo.Platform.Equals("iOS"))
                        ProgramImage.HeightRequest = 615;
                    else
                    {
                        ProgramImage.HeightRequest = 550;

                    }
                }
                else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                {
                    ProgramImage.HeightRequest = 715;
                }
                else
                {
                    ProgramImage.HeightRequest = 630;
                }
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                if (isDeviceIphonePlus)
                {
                    ProgramImage.HeightRequest = 600;
                }
                else
                {
                    ProgramImage.HeightRequest = 530;
                }
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                if (isDeviceLargeIPad)
                {
                    ProgramImage.HeightRequest = 1200;
                }
                else
                {
                    ProgramImage.HeightRequest = 890;
                }
            }
            else
            {
                ProgramImage.HeightRequest = 600;
            }
            Show_Today_ContactProgramScreen_L2();
        }

        private async void SetupSchedule(LocalDBContactProgram ActiveContactProgram)
        {
            var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ActiveContactProgram.Id && x.IsRepeat != true).OrderBy(x => x.ScheduledStartDate).ToListAsync();
            if (ContactProgramDays.Count() == 0)
                return;

            var lastDate = ContactProgramDays.Where(x => x.ScheduledStartDate != null).OrderByDescending(x => x.ScheduledStartDate).Select(x => x.ScheduledStartDate).FirstOrDefault();

            NoActiveProgram.IsVisible = false;
            listOfProgramDays.Children.Clear();

            int dayNumber = 1;
            bool AddWeekSL = true;
            int WeekNum = 1;
            int Counter = 1;
            bool IsScheduleCreated = ActiveContactProgram.IsScheduleBuilt;
            _scheduledCPD = new List<holdingCell>();

            if (ActiveContactProgram.IsScheduleBuilt == false)
            {
                ContactProgramDays = ContactProgramDays.OrderBy(x => x.SequenceNumber).ToList();
                ScheduleProgramDaysTopSL.IsVisible = true;
            }
            else if (ActiveContactProgram.StatusCodeValue == 585860000 && lastDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date) //in progress and not past last date
            {
                ResetScheduleSL.IsVisible = true;
            }

            bool isChallenge = false;

            if (ActiveContactProgram.ChallengeId != 0)
            {
                HideChallengeButtons();
                isChallenge = true;
            }

            foreach (var contactProgramDay in ContactProgramDays)
            {
                var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.Id == contactProgramDay.ProgramDayId).FirstOrDefaultAsync();
                if (ProgramDay == null)
                    break;

                if (AddWeekSL)
                {
                    var lastDayOfWeek = ContactProgramDays.Skip((WeekNum - 1) * 7).Take(7).ToList().Last();
                    bool isWeekPast = false;

                    if (lastDayOfWeek.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                    {
                        isWeekPast = true;
                    }
                    AddWeekSeprator(WeekNum, IsScheduleCreated, isWeekPast, isChallenge);
                    AddWeekSL = false;
                }
                var cell = new holdingCell();

                if (ProgramDay.DayTypeValue == 585860000)
                {
                    //WORKOUT
                    cell = AddWorkoutDay(contactProgramDay, ProgramDay, dayNumber, Counter);
                    _scheduledCPD.Add(cell);
                }
                else if (ProgramDay.DayTypeValue == 585860001)
                {
                    //REST
                    cell = AddRestDay(contactProgramDay, ProgramDay, dayNumber, Counter);
                    _scheduledCPD.Add(cell);
                }

                if (cell.cell != null && _todayScheduleCell.cell == null)
                {
                    if (cell.cell.scheduledDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                    {
                        _todayScheduleCell = cell;
                    }
                }

                dayNumber++;
                Counter++;
                if (dayNumber > 7)
                {
                    WeekNum++;
                    dayNumber = 1;
                    AddWeekSL = true;
                }
            }

            var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ActiveContactProgram.ProgramId).FirstOrDefaultAsync();
            if (Program != null)
            {
                if (Program.Heading == null)
                    return;

                if (!Program.Heading.Contains("FIT TEST") && !Program.Heading.Contains("FAB IN FIVE"))
                    return;

                var veryEndSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#17191A"),
                    HeightRequest = 200,
                };

                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    veryEndSL.HeightRequest = 450;
                }

                var quoteLabel = new Label
                {
                    Text = "Train like a Badass because good things come to those who sweat!",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#00BBCB"),
                    FontSize = 16,
                    FontFamily = "HelveticalNeue-Bold",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(20, 0, 20, 0),
                };

                if (Program.Heading.Contains("FIT TEST"))
                {
                    quoteLabel.Text = "Are you FIT? Time to find out with this FIT Test. Good luck!";
                }
                else if (Program.Heading.Contains("FAB IN FIVE"))
                {
                    quoteLabel.Text = "\"Long-term consistency trumps short-term intensity\" - Bruce Lee\n\nIf you’re reading this, you’ve taken the hardest step. You’ve gotten started! Now, make a commitment to yourself to complete All of your Fab in Five workouts this week.";
                }
                veryEndSL.Children.Add(quoteLabel);
                listOfProgramDays.Children.Add(veryEndSL);

            }
        }

        private async void BuildScheduleBtn_Clicked(object sender, EventArgs e)
        {
            var CP = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
            if (CP != null)
            {
                bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("buildschedule", CP.ProgramId)).Result;
                if (!allowDownload)
                {
                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == CP.ProgramId).FirstOrDefaultAsync();
                    if (Program == null)
                    {
                        await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                        return;
                    }
                    await Navigation.PushModalAsync(new IAP(Program.GuidCRM), false);
                    return;
                }
            }

            var result = await DisplayAlert("Start Program Today", "Program schedule will auto-advance once you start your program.", "Start Today", "I'm not ready");
            if (!result)
            {
                NotLoadedYet.IsVisible = false;
                return;
            }

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                return;
            }

            var ContactPrograms = await _connection.Table<LocalDBContactProgram>().ToListAsync();
            if (ContactPrograms.Count() != 0)
            {
                var ActiveContactProgram = ContactPrograms.Where(x => x.StatusCodeValue == 585860000).FirstOrDefault(); //Active and Inprogress
                if (ActiveContactProgram != null)
                {
                    if (ActiveContactProgram.IsScheduleBuilt == false)
                    {
                        DayCompleteNowScheduleDays.IsEnabled = false;
                        ScheduleProgramDaysTopBtn.IsEnabled = false;
                        DayCompleteNowScheduleDays.Text = "... ONE MOMENT";
                        ScheduleProgramDaysTopBtn.Text = "... ONE MOMENT";

                        try
                        {
                            bool advanceToNextDay = false;
                            var firstContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ActiveContactProgram.Id && x.SequenceNumber == 1 && x.StatusCodeValue == 585860004 && x.IsRepeat != true).FirstOrDefaultAsync(); //First Completed
                            if (firstContactProgramDay != null)
                            {
                                if (firstContactProgramDay.ActualStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                                {
                                    //First day is done, when activating the next day should be tomorrow
                                    advanceToNextDay = true;
                                }
                            }
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Building Program Schedule" } });
                            var response = await WebAPIService.BuildProgramSchedule(_client, ActiveContactProgram.GuidCRM, advanceToNextDay);

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Completed Building Program Schedule" } });

                                var ProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ActiveContactProgram.Id && x.IsRepeat != true).OrderBy(x => x.SequenceNumber).ToListAsync();
                                if (ProgramDays.Count() != 0)
                                {
                                    int advanceDayCounter = 0;
                                    if (firstContactProgramDay != null)
                                    {
                                        firstContactProgramDay.ScheduledStartDate = firstContactProgramDay.ActualStartDate;
                                        await _connection.UpdateAsync(firstContactProgramDay);
                                        ProgramDays = ProgramDays.Where(x => x.SequenceNumber != 1).ToList();

                                        if (advanceToNextDay)
                                        {
                                            //First day is done, when activating the next day should be tomorrow
                                            advanceDayCounter = 1;
                                        }
                                    }
                                    foreach (var day in ProgramDays)
                                    {
                                        day.ScheduledStartDate = DateTime.UtcNow.AddDays(advanceDayCounter);
                                        advanceDayCounter++;
                                        await _connection.UpdateAsync(day);
                                    }
                                    ActiveContactProgram.IsScheduleBuilt = true;
                                    await _connection.UpdateAsync(ActiveContactProgram);

                                    App.Current.MainPage = new MainStartingPage();
                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Error", "Try again later.", "OK");
                            }
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "BuildScheduleBtn_Clicked" } });
                            await DisplayAlert("Error", "Something went wrong.", "OK");
                        }

                    }
                }
            }

            DayCompleteNowScheduleDays.IsEnabled = true;
            ScheduleProgramDaysTopBtn.IsEnabled = true;
            DayCompleteNowScheduleDays.Text = "START PROGRAM TODAY";
            ScheduleProgramDaysTopBtn.Text = "START PROGRAM TODAY";
        }

        private async void RepeatProgramBtn_Clicked(object sender, EventArgs e)
        {
            var CP = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
            if (CP != null)
            {
                bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("repeat", CP.ProgramId)).Result;
                if (!allowDownload)
                {
                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == CP.ProgramId).FirstOrDefaultAsync();
                    if (Program == null)
                    {
                        await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                        return;
                    }
                    await Navigation.PushModalAsync(new IAP(Program.GuidCRM), false);
                    return;
                }
            }

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                return;
            }

            RepeatProgramBtn.IsEnabled = false;
            RepeatProgramBtn.Text = "... ONE MOMENT";

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Repeat Program" } });
                //deactivate current program as completed
                //create a new contact program with all the same things and set it as active active - to do
                //update the app with the contact program statuses and new contact program
                //refresh the app

                if (_ContactProgramGuid == Guid.Empty)
                {
                    await DisplayAlert("Error", "Something went wrong.", "OK");
                    return;
                }

                HttpResponseMessage response = await WebAPIService.RepeatProgram(_client, _ContactProgramGuid);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    App.Current.MainPage = new MainStartingPage();
                    //HttpContent content = response.Content;
                    //var json = await content.ReadAsStringAsync();
                    //var newContactProgramGuid = JsonConvert.DeserializeObject<Guid>(json);

                    //var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
                    //if (ContactProgram == null)
                    //    return;

                    //ContactProgram.StateCodeValue = 1;
                    //ContactProgram.StatusCodeValue = 585860004;
                    //ContactProgram.IsComplete = true;
                    //ContactProgram.EndDate = DateTime.UtcNow;

                    //var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ContactProgram.ProgramId).FirstOrDefaultAsync();
                    //if (Program == null)
                    //    return;

                    //var newContactProgram = new LocalDBContactProgram();
                    //newContactProgram.GuidCRM = newContactProgramGuid;
                    //newContactProgram.StateCodeValue = 0;//active
                    //newContactProgram.StatusCodeValue = 1;//active
                    //newContactProgram.ProgramId = Program.Id;

                    //await _connection.UpdateAsync(ContactProgram);
                    //await _connection.InsertAsync(newContactProgram);

                    //App.Current.MainPage = new MainStartingPage();
                }
                else
                {
                    await DisplayAlert("Error", "Something went wrong. Try again later.", "OK");
                    RepeatProgramBtn.IsEnabled = true;
                    RepeatProgramBtn.Text = "REPEAT PROGRAM";
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "EditBtn_Clicked_New" } });
                var error = ex.ToString();
                RepeatProgramBtn.IsEnabled = true;
            }
        }

        private async void SyncPlansBtn_Clicked(object sender, EventArgs e)
        {

            SyncPlansBtn.IsEnabled = false;
            SyncPlansBtn.Text = "... ONE MOMENT";

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "SyncPlansBtn_Clicked" } });

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
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SyncPlansBtn_Clicked" } });
                SyncPlansBtn.IsEnabled = true;
                SyncPlansBtn.Text = "SYNC MY NEW PLANS";
            }
            SyncPlansBtn.IsEnabled = true;
            SyncPlansBtn.Text = "SYNC MY NEW PLANS";
        }

        private async void GiftBtn_Clicked(object sender, EventArgs e)
        {
            GiftBox.IsEnabled = false;
            GiftBoxRest.IsEnabled = false;

            var ActiveContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
            if (ActiveContactProgram != null)
            {
                var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ActiveContactProgram.ProgramId).FirstOrDefaultAsync();
                if (Program == null)
                {
                    await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                    GiftBox.IsEnabled = true;
                    GiftBoxRest.IsEnabled = true;
                    return;
                }

                await Navigation.PushModalAsync(new IAP_FIF(Program.GuidCRM, ActiveContactProgram), false);

                GiftBox.IsEnabled = true;
                GiftBoxRest.IsEnabled = true;
                return;

            }
            await DisplayAlert("SPECIAL GIFT", "Your exclusive FAB in FIVE gift awaits after your first workout", "GOT IT");
            GiftBox.IsEnabled = true;
            GiftBoxRest.IsEnabled = true;
        }

        private void AddWeekSeprator(int num, bool IsScheduleBuilt, bool isWeekPast, bool isChallenge)
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

            if (IsScheduleBuilt && !isWeekPast)//&& !isChallenge
            {
                var editBtn = new Button
                {
                    Text = "EDIT",
                    FontSize = 10,
                    TextColor = Color.White,
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "HelveticalNeue-Bold",
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(0, 0, 10, 0),
                    BorderColor = Color.White,
                    BackgroundColor = Color.Transparent,
                    BorderWidth = 1,
                    CornerRadius = 2,
                    HeightRequest = 20,
                    WidthRequest = 40,
                };
                //editBtn.Clicked += EditBtn_Clicked1;
                //editBtn.Command = new Command<int>(EditBtn_Clicked);
                //editBtn.CommandParameter = num;
                WeekBtn weekBtn = new WeekBtn
                {
                    btn = editBtn,
                    WeekNum = num,
                    isEditing = false,
                };
                editBtn.Command = new Command<WeekBtn>(EditBtn_Clicked_New);
                editBtn.CommandParameter = weekBtn;
                weekSL.Children.Add(editBtn);
            }
            else
            {
                var editBtn = new Button
                {
                    Text = "   ",
                    FontSize = 12,
                    TextColor = Color.White,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                weekSL.Children.Add(editBtn);
            }
            listOfProgramDays.Children.Add(weekSL);
        }

        private async void EditBtn_Clicked_New(WeekBtn obj)
        {
            var CP = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
            if (CP != null)
            {
                bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("editschedule", CP.ProgramId, obj.WeekNum * 7)).Result;
                if (!allowDownload)
                {
                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == CP.ProgramId).FirstOrDefaultAsync();
                    if (Program == null)
                    {
                        await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                        return;
                    }
                    await Navigation.PushModalAsync(new IAP(Program.GuidCRM), false);
                    return;
                }
            }

            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Clicked Edit Schedule Button" } });

            if (!IsInternetConnected())
            {
                obj.btn.Text = "EDIT";
                _isEditingSchedule = false;
                await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                return;
            }

            if (!_isEditingSchedule)
            {
                bool result = await DisplayAlert("Edit Schedule", "Would you like to edit your workout schedule?", "Edit", "Cancel");
                if (result)
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Start Editing Schedule" } });
                    obj.btn.Text = "SAVE";
                    _isEditingSchedule = true;
                    //await Navigation.PushModalAsync(new EditWorkoutSchedule(weekNum, _ContactProgramGuid), false);
                    var week = _scheduledCPD
                        .Where(x => x.cell.dayNum > (obj.WeekNum - 1) * 7
                        && x.cell.dayNum <= obj.WeekNum * 7)
                        .OrderBy(x => x.cell.dayNum)
                        .Take(7)
                        .ToList();

                    _weekToSave = new List<holdingCell>();

                    foreach (var item in week)
                    {
                        var hCell = new holdingCell
                        {
                            cell = new CPDCell
                            {
                                scheduledDate = item.cell.scheduledDate,
                                ContactProgramDayGuid = item.cell.ContactProgramDayGuid,
                                ProgramDayGuid = item.cell.ProgramDayGuid,
                                dayTypeCode = item.cell.dayTypeCode,
                                dayNum = item.cell.dayNum,
                                SequenceNumber = item.cell.SequenceNumber,
                                stateCode = item.cell.stateCode,
                                statusCode = item.cell.statusCode,
                            }
                        };
                        _weekToSave.Add(hCell);
                    };

                    _weekNumToSave = obj.WeekNum;

                    int numDays = 0;
                    int numActiveDays = 0;
                    foreach (var item in week)
                    {
                        if (item.cell.dayTypeCode == 585860001 || (item.cell.dayTypeCode == 585860000 && item.cell.statusCode != 585860004))//Not complete Workout or rest
                                                                                                                                            //Active, item.cell.stateCode == 0 && item.cell.scheduledDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date
                        {
                            item.cell.bottomSL.IsVisible = false;
                            item.cell.btnSL.IsVisible = true;
                            item.cell.ArrowSL.IsVisible = false;
                            item.cell.position = numDays;
                            //item.cell.mainSL.HeightRequest = 70;
                            item.cell.Label.FontAttributes = FontAttributes.Bold;

                            if (numActiveDays == 0)
                            {
                                item.cell.upBtn.IsVisible = false;
                            }
                            else if ((numDays + 1) == week.Count())
                            {
                                item.cell.downBtn.IsVisible = false;
                            }
                            numActiveDays++;
                        }
                        numDays++;
                    }
                }

            }
            else if (obj.btn.Text == "SAVE" && _isEditingSchedule)
            {
                //if (!IsInternetConnected())
                //{
                //    btn.Text = "Edit";
                //    _isEditingSchedule = false;
                //    await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                //    return;
                //}
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Save Schedule Edits" } });

                obj.btn.Text = "SAVING";
                await Navigation.PushModalAsync(new InitialLoading(), false);

                //Save the week
                var week = _scheduledCPD
                            .Where(x => x.cell.dayNum > (obj.WeekNum - 1) * 7
                            && x.cell.dayNum <= obj.WeekNum * 7)
                            .OrderBy(x => x.cell.dayNum)
                            .Take(7)
                            .ToList();

                List<CPD3> cpds = new List<CPD3>();

                foreach (var day in week)
                {
                    var w = _weekToSave.Where(x => x.cell.dayNum == day.cell.dayNum).FirstOrDefault();
                    if (w != null)
                    {
                        if (w.cell.ContactProgramDayGuid != day.cell.ContactProgramDayGuid)
                        {
                            CPD3 cpd = new CPD3
                            {
                                guid = day.cell.ContactProgramDayGuid,
                                SequenceNumber = w.cell.SequenceNumber,
                                //StateCode = w.cell.stateCode,
                                //StatusCode = w.cell.statusCode,
                            };
                            cpds.Add(cpd);
                        }
                    }
                }

                if (cpds.Count() > 0)
                {
                    //if (!IsInternetConnected())
                    //{
                    //    await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                    //    return;
                    //}

                    try
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Schedule Changed" } });

                        var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.StateCodeValue == 0).FirstOrDefaultAsync();

                        if (ContactProgram == null)
                            return;

                        var rCPD = new RescheduledCPD3
                        {
                            ContactProgramGuid = ContactProgram.GuidCRM,
                            cpds = cpds,
                        };
                        //The week has changed, save it
                        //Save to API
                        HttpResponseMessage response = await WebAPIService.ModifySchedule(_client, rCPD);
                        //var errer = response.Content.ReadAsStringAsync().Result; //This reads the error from the API

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var cpd = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ContactProgram.Id && x.IsRepeat != true).ToListAsync();

                            foreach (var day in rCPD.cpds)
                            {
                                int sequenceNumber = day.SequenceNumber;

                                var matchingSequenceNumber = cpd.Where(x => x.SequenceNumber == day.SequenceNumber).FirstOrDefault();
                                var matchingGuid = cpd.Where(x => x.GuidCRM == day.guid).FirstOrDefault();

                                if (matchingSequenceNumber != null && matchingGuid != null)
                                {
                                    var tempSequenceNumber = matchingGuid.SequenceNumber;
                                    var tempScheduledStartDate = matchingGuid.ScheduledStartDate;

                                    matchingGuid.ScheduledStartDate = matchingSequenceNumber.ScheduledStartDate;
                                    matchingGuid.SequenceNumber = matchingSequenceNumber.SequenceNumber;
                                    matchingSequenceNumber.ScheduledStartDate = tempScheduledStartDate;
                                    matchingSequenceNumber.SequenceNumber = tempSequenceNumber;

                                    if (matchingGuid.DayTypeValue == 866660000)
                                    {
                                        //workout
                                        if (matchingGuid.ScheduledStartDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            //active active
                                            matchingGuid.StateCodeValue = 0; //active
                                            matchingGuid.StatusCodeValue = 1; //active
                                        }
                                        else if (matchingGuid.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            matchingGuid.StateCodeValue = 1; //inactive
                                            matchingGuid.StatusCodeValue = 585860006; //incomplete
                                        }
                                    }
                                    else if (matchingGuid.DayTypeValue == 866660001)
                                    {
                                        //rest
                                        if (matchingGuid.ScheduledStartDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            //active active
                                            matchingGuid.StateCodeValue = 0; //active
                                            matchingGuid.StatusCodeValue = 1; //active
                                        }
                                        else if (matchingGuid.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            //inactive complete 585860004
                                            matchingGuid.StateCodeValue = 1; //inactive
                                            matchingGuid.StatusCodeValue = 585860004; //compelted
                                        }
                                    }

                                    if (matchingSequenceNumber.DayTypeValue == 866660000)
                                    {
                                        //workout
                                        if (matchingSequenceNumber.ScheduledStartDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            //active active
                                            matchingSequenceNumber.StateCodeValue = 0; //active
                                            matchingSequenceNumber.StatusCodeValue = 1; //active
                                        }
                                        else if (matchingSequenceNumber.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            matchingSequenceNumber.StateCodeValue = 1; //inactive
                                            matchingSequenceNumber.StatusCodeValue = 585860006; //incomplete
                                        }
                                    }
                                    else if (matchingSequenceNumber.DayTypeValue == 866660001)
                                    {
                                        //rest
                                        if (matchingSequenceNumber.ScheduledStartDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            //active active
                                            matchingSequenceNumber.StateCodeValue = 0; //active
                                            matchingSequenceNumber.StatusCodeValue = 1; //active
                                        }
                                        else if (matchingSequenceNumber.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                                        {
                                            //inactive complete 585860004
                                            matchingSequenceNumber.StateCodeValue = 1; //inactive
                                            matchingSequenceNumber.StatusCodeValue = 585860004; //completed
                                        }
                                    }

                                    await _connection.UpdateAsync(matchingGuid);
                                    await _connection.UpdateAsync(matchingSequenceNumber);
                                }
                            }
                            MessagingCenter.Send(this, "DoneSavingSchedule");
                        }
                        else
                        {
                            await DisplayAlert("Error", "We had a little error. Please retry later.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "EditBtn_Clicked_New" } });
                        await DisplayAlert("Error", "We had a little error.", "OK");
                    }
                }
                else
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Schedule Not Changed" } });
                    MessagingCenter.Send(this, "DoneShowingLoading");
                }

                obj.btn.Text = "EDIT";
                _isEditingSchedule = false;
                _weekToSave = new List<holdingCell>();

                foreach (var item in week)
                {
                    //if (item.cell.stateCode == 0)//Active
                    // {
                    if (item.cell.dayTypeCode != 585860001)//NOT Rest                            
                    {
                        item.cell.ArrowSL.IsVisible = true;
                        item.cell.bottomSL.IsVisible = true;
                    }
                    item.cell.btnSL.IsVisible = false;
                    item.cell.Label.FontAttributes = FontAttributes.None;
                    //}
                }
            }
            else if (_isEditingSchedule)
            {
                //Editing another week
                await DisplayAlert("Already Editing", "You are already editing another week's schedule, save that week first.", "OK");
            }
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

        private void ResetApp()
        {
            try
            {
                Task.Delay(500).Wait(); //Try to prevent the app from crashing
                App.Current.MainPage = new MainStartingPage();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ResetApp()" } });
                App.Current.MainPage = new MainStartingPage();
            }

        }

        private holdingCell AddWorkoutDay(LocalDBContactProgramDayV2 contactProgramDay, LocalDBProgramDay programDay, int dayNumber, int Counter)
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

            var dateDay = new Label
            {
                FontSize = 13,
                TextColor = Color.FromHex("#909090"),
                FontFamily = "HelveticalNeue-Bold",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var dateLabel = new Label
            {
                FontSize = 9,
                TextColor = Color.FromHex("#909090"),
                FontFamily = "PingFangTC-Regular",
                HorizontalTextAlignment = TextAlignment.Center
            };

            if (contactProgramDay.ScheduledStartDate == null)
            {
                if (contactProgramDay.SequenceNumber == 1)
                {
                    dateLabel.FontSize = 7;
                    dateLabel.Text = "CURRENT";
                    dateLabel.FontAttributes = FontAttributes.Bold;
                    dateLabel.FontFamily = "HelveticalNeue-Bold";
                    dateLabel.TextColor = Color.FromHex("#1C516B");
                    daySL.Children.Add(dateLabel);
                }
                else
                {
                    daySL.Children.Add(dayLabel);
                    daySL.Children.Add(dayNum);
                }

                //else
                //{
                //    dateLabel.Text = "NOT SCHEDULED";
                //    dateLabel.TextColor = Color.FromHex("#ff5a5a");
                //}
            }
            else
            {
                if (contactProgramDay.ScheduledStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                {
                    dateLabel.FontSize = 10;
                    dateLabel.FontAttributes = FontAttributes.Bold;
                    dateLabel.FontFamily = "HelveticalNeue-Bold";
                    dateLabel.Text = "TODAY";
                    dateLabel.TextColor = Color.FromHex("#1C516B");
                    daySL.Children.Add(dateLabel);
                }
                else
                {
                    //dateLabel.FontSize = 10;
                    dateLabel.Text = contactProgramDay.ScheduledStartDate?.ToLocalTime().Date.ToString("MMM d");
                    //dateLabel.TextColor = Color.FromHex("#909090");
                    dateDay.Text = contactProgramDay.ScheduledStartDate?.ToLocalTime().Date.ToString("ddd");
                    daySL.Children.Add(dateDay);
                    daySL.Children.Add(dateLabel);
                }
            }

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
                //Margin = new Thickness(0, 5, 0, 0)
            };

            var headingSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
                //BackgroundColor = Color.Gray
            };

            headingSL.Children.Add(heading);
            SecondarySL.Children.Add(headingSL);

            var bottomSL = new StackLayout
            {
                Spacing = 0,
                //Padding = new Thickness(0, 5, 0, 5),
                Orientation = StackOrientation.Horizontal,
                //HorizontalOptions = LayoutOptions.CenterAndExpand, //changed
                //BackgroundColor = Color.Orange,
            };

            SecondarySL.Children.Add(bottomSL);

            if (contactProgramDay.StateCodeValue == 0) //Active
            {
                //var exerciseSL = new StackLayout
                //{
                //    Spacing = 0,
                //    VerticalOptions = LayoutOptions.CenterAndExpand,
                //    HorizontalOptions = LayoutOptions.Start,
                //    WidthRequest = 90,
                //};

                //var exerciseNumLabel = new Label
                //{
                //    Text = programDay.TotalExercises.ToString(),
                //    FontSize = 13,
                //    FontFamily = "HelveticalNeue-Bold",
                //    HorizontalTextAlignment = TextAlignment.Center,
                //    FontAttributes = FontAttributes.Bold,
                //};

                //var exerciseLabel = new Label
                //{
                //    Text = "Exercises",
                //    FontSize = 11,
                //    FontFamily = "HelveticalNeue-Bold",
                //    HorizontalTextAlignment = TextAlignment.Center,
                //};

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

                //var timeLabel = new Label
                //{
                //    Text = "Minutes",
                //    FontSize = 11,
                //    FontFamily = "HelveticalNeue-Bold",
                //    HorizontalTextAlignment = TextAlignment.Center,
                //};

                //var levelSL = new StackLayout
                //{
                //    Spacing = 0,
                //    VerticalOptions = LayoutOptions.CenterAndExpand,
                //    HorizontalOptions = LayoutOptions.End,
                //    WidthRequest = 105,
                //};

                //var levelNumLabel = new Label
                //{
                //    Text = programDay.Level,
                //    FontSize = 13,
                //    FontFamily = "HelveticalNeue-Bold",
                //    HorizontalTextAlignment = TextAlignment.Center,
                //    FontAttributes = FontAttributes.Bold,
                //};

                //var levelLabel = new Label
                //{
                //    Text = "Level",
                //    FontSize = 11,
                //    FontFamily = "HelveticalNeue-Bold",
                //    HorizontalTextAlignment = TextAlignment.Center,
                //};

                if (contactProgramDay.ScheduledStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                {
                    mainSL.BackgroundColor = Color.FromHex("#66d6df");

                    //exerciseSL.Children.Add(exerciseNumLabel);
                    //exerciseSL.Children.Add(exerciseLabel);
                    //bottomSL.Children.Add(exerciseSL);

                    timeSL.Children.Add(timeNumLabel);
                    //timeSL.Children.Add(timeLabel);
                    bottomSL.Children.Add(timeSL);

                    //levelSL.Children.Add(levelNumLabel);
                    //levelSL.Children.Add(levelLabel);
                    //bottomSL.Children.Add(levelSL);
                }
                else if (contactProgramDay.ScheduledStartDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                {
                    mainSL.BackgroundColor = Color.FromHex("#e3e4e4");
                    var DeactiveSL = new StackLayout
                    {
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Margin = new Thickness(0, 3, 0, 0)
                    };

                    var headingStatus = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromHex("#555757"),
                        Text = "MISSED",
                        FontSize = 11,
                        FontFamily = "HelveticalNeue-Bold",
                        //Margin = new Thickness(0, 5, 0, 0)
                    };

                    heading.TextColor = Color.FromHex("#909090");
                    headingStatus.TextColor = Color.FromHex("#17191A");

                    DeactiveSL.Children.Add(headingStatus);
                    bottomSL.Children.Add(DeactiveSL);
                }
                else
                {
                    //exerciseSL.Children.Add(exerciseNumLabel);
                    //exerciseSL.Children.Add(exerciseLabel);
                    //bottomSL.Children.Add(exerciseSL);

                    timeSL.Children.Add(timeNumLabel);
                    //timeSL.Children.Add(timeLabel);
                    bottomSL.Children.Add(timeSL);

                    //levelSL.Children.Add(levelNumLabel);
                    //levelSL.Children.Add(levelLabel);
                    //bottomSL.Children.Add(levelSL);
                }

            }
            else
            {
                //Deactive
                var checkMarkSL = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(0, 3, 0, 0)
                };
                bottomSL.Children.Add(checkMarkSL);

                var checkImg = new Image
                {
                    Source = "checkmark_green.png",
                    HeightRequest = 10,
                    WidthRequest = 10,
                    Margin = new Thickness(0, 0, 3, 0)
                };

                var DeactiveSL = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    //BackgroundColor = Color.Green
                    Margin = new Thickness(0, 3, 0, 0)
                };

                var headingStatus = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    Text = "COMPLETED",
                    TextColor = Color.FromHex("#909090"),
                    FontSize = 11,
                    FontFamily = "HelveticalNeue-Bold",
                    //Margin = new Thickness(0, 5, 0, 0),
                    // BackgroundColor = Color.Red
                };

                DeactiveSL.Children.Add(headingStatus);
                bottomSL.Children.Add(DeactiveSL);

                heading.TextColor = Color.FromHex("#909090");
                headingStatus.TextColor = Color.FromHex("#17191A");

                if (contactProgramDay.StatusCodeValue == 585860004)
                {
                    //Completed
                    mainSL.BackgroundColor = Color.FromHex("#76E982");//#94ea94, 48e537, #76E982
                    headingStatus.Text = "COMPLETED";
                    //checkMarkSL.Children.Add(checkImg);
                    checkMarkSL.IsVisible = false;
                }
                else if (contactProgramDay.StatusCodeValue == 585860006 || contactProgramDay.StatusCodeValue == 866660000) //incomplete and cancelled
                {
                    //Missed
                    mainSL.BackgroundColor = Color.FromHex("#e3e4e4");// #1C516B
                    headingStatus.Text = "MISSED";
                    checkMarkSL.IsVisible = false;
                }
                else
                {
                    //unknown
                    mainSL.BackgroundColor = Color.FromHex("#D6D8D8");// #e9e9e9
                    headingStatus.Text = "UNKNOWN STATUS";
                    checkMarkSL.IsVisible = false;
                }
            }

            var btnSL = new StackLayout
            {
                Spacing = 10,
                IsVisible = false,
                Padding = new Thickness(0, 0, 5, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.End,
                Orientation = StackOrientation.Horizontal,
            };

            var moveCPD_DOWNBtn = new Button
            {
                Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_down_04.png"),
                HeightRequest = 25,
                WidthRequest = 25,
                VerticalOptions = LayoutOptions.Center,
            };

            var moveCPD_UPBtn = new Button
            {
                Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_up_04.png"),
                HeightRequest = 25,
                WidthRequest = 25,
                VerticalOptions = LayoutOptions.Center,
            };

            var cpdCell = new CPDCell
            {
                Label = heading,
                LabelText = heading.Text,
                bottomSL = bottomSL,
                ArrowSL = arrowSL,
                upBtn = moveCPD_UPBtn,
                downBtn = moveCPD_DOWNBtn,
                btnSL = btnSL,
                mainSL = mainSL,

                ContactProgramDayGuid = contactProgramDay.GuidCRM,
                ProgramDayGuid = programDay.GuidCRM,
                dayNum = Counter,
                position = dayNumber,
                scheduledDate = (DateTime?)contactProgramDay.ScheduledStartDate,
                SequenceNumber = contactProgramDay.SequenceNumber,
                stateCode = contactProgramDay.StateCodeValue,
                statusCode = contactProgramDay.StatusCodeValue,
                dayTypeCode = programDay.DayTypeValue,
            };

            if (contactProgramDay.ScheduledStartDate != null)
            {
                cpdCell.scheduledDate = (DateTime)contactProgramDay.ScheduledStartDate;
            }

            moveCPD_UPBtn.Command = new Command<CPDCell>(moveCPD_UP);
            moveCPD_UPBtn.CommandParameter = cpdCell;

            moveCPD_DOWNBtn.Command = new Command<CPDCell>(moveCPD_DOWN);
            moveCPD_DOWNBtn.CommandParameter = cpdCell;

            btnSL.Children.Add(moveCPD_UPBtn);
            btnSL.Children.Add(moveCPD_DOWNBtn);

            mainSL.Children.Add(btnSL);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                // handle the tap
                ProgramDayCell_Clicked(programDay, contactProgramDay);
            };
            mainSL.GestureRecognizers.Add(tapGestureRecognizer);

            listOfProgramDays.Children.Add(mainSL);

            var hCell = new holdingCell
            {
                cell = cpdCell,
            };

            return hCell;
        }

        private async void ProgramDayCell_Clicked(LocalDBProgramDay programDay, LocalDBContactProgramDayV2 contactProgramDay)
        {
            if (programDay.DayTypeValue == 585860000)
            {
                if (_isEditingSchedule)
                {

                    var editingCell = _weekToSave.Where(x => x.cell.ProgramDayGuid == programDay.GuidCRM && (x.cell.dayTypeCode == 585860001 || x.cell.dayTypeCode == 585860000 && x.cell.statusCode != 585860004)).FirstOrDefault();

                    if (editingCell == null)
                    {
                        //Workout Day, go to the ProgramDay Overview Page                
                        await Navigation.PushModalAsync(new ProgramDayOverview(programDay, true, contactProgramDay));
                    }
                }
                else
                {
                    //Workout Day, go to the ProgramDay Overview Page                
                    await Navigation.PushModalAsync(new ProgramDayOverview(programDay, true, contactProgramDay));
                }
            }
        }

        private void moveCPD_DOWN(CPDCell obj)
        {
            try
            {
                if (obj == null)
                    return;

                var currentPosition = obj.position;
                //var dayNum = obj.dayNum;
                //var dayDate = obj.scheduledDate;

                //var cell = _scheduledCPD.Where(x => x.cell.dayNum == obj.dayNum).FirstOrDefault();

                //var pos = cell.cell.position;
                //var num = cell.cell.dayNum;
                //var date = cell.cell.scheduledDate;
                //var labelText = cell.cell.LabelText;

                if (currentPosition < 7)
                {
                    //switch cells
                    var moveDown = _scheduledCPD.Where(x => x.cell.dayNum == (obj.dayNum)).FirstOrDefault();

                    if (moveDown == null)
                        return;

                    var moveUp = new holdingCell() { cell = null };

                    for (int i = 1; i < 7; i++)
                    {
                        //item.cell.dayTypeCode == 585860001 || (item.cell.dayTypeCode == 585860000 && item.cell.statusCode != 585860004)
                        moveUp = _scheduledCPD.Where(x => x.cell.dayNum == (obj.dayNum + i) && (x.cell.dayTypeCode == 585860001 || (x.cell.dayTypeCode == 585860000 && x.cell.statusCode != 585860004))).FirstOrDefault();
                        if (moveUp != null || i >= 7)
                        {
                            break;
                        }
                    }

                    if (moveUp == null)
                        return;

                    if (moveUp.cell != null && moveDown != null)
                    {
                        SwitchCells(moveUp, moveDown);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "moveCPD_DOWN" } });
            }
        }

        private void SwitchCells(holdingCell moveUp, holdingCell moveDown)
        {
            var indexBelow = _scheduledCPD.IndexOf(_scheduledCPD.Where(x => x.cell.dayNum == moveUp.cell.dayNum).FirstOrDefault());
            var indexAbove = _scheduledCPD.IndexOf(_scheduledCPD.Where(x => x.cell.dayNum == moveDown.cell.dayNum).FirstOrDefault());

            var temp = new holdingCell
            {
                cell = new CPDCell
                {
                    //position = moveDown.cell.position,
                    //scheduledDate = moveDown.cell.scheduledDate,
                    Label = moveDown.cell.Label,
                    LabelText = moveDown.cell.LabelText,
                    dayTypeCode = moveDown.cell.dayTypeCode,
                    ContactProgramDayGuid = moveDown.cell.ContactProgramDayGuid,
                    SequenceNumber = moveDown.cell.SequenceNumber
                }
            };

            temp.cell.Label.Text = temp.cell.LabelText;

            int currentIndex = indexAbove;
            _scheduledCPD[currentIndex].cell.Label.Text = moveUp.cell.LabelText;
            _scheduledCPD[currentIndex].cell.LabelText = moveUp.cell.LabelText;

            _scheduledCPD[currentIndex].cell.dayTypeCode = moveUp.cell.dayTypeCode;
            _scheduledCPD[currentIndex].cell.ContactProgramDayGuid = moveUp.cell.ContactProgramDayGuid;

            currentIndex = indexBelow;
            _scheduledCPD[currentIndex].cell.Label.Text = temp.cell.LabelText;
            _scheduledCPD[currentIndex].cell.LabelText = temp.cell.LabelText;

            _scheduledCPD[currentIndex].cell.dayTypeCode = temp.cell.dayTypeCode;
            _scheduledCPD[currentIndex].cell.ContactProgramDayGuid = temp.cell.ContactProgramDayGuid;
        }

        private void moveCPD_UP(CPDCell obj)
        {
            try
            {
                if (obj == null)
                    return;

                var currentPosition = obj.position;
                //var dayNum = obj.dayNum;
                //var dayDate = obj.scheduledDate;

                //var cell = _scheduledCPD.Where(x => x.cell.dayNum == obj.dayNum).FirstOrDefault();

                //var pos = cell.cell.position;
                //var num = cell.cell.dayNum;
                //var date = cell.cell.scheduledDate;
                //var labelText = cell.cell.LabelText;

                if (currentPosition > 0)
                {
                    //switch cells
                    var moveUp = _scheduledCPD.Where(x => x.cell.dayNum == (obj.dayNum)).FirstOrDefault();

                    if (moveUp == null)
                        return;

                    var moveDown = new holdingCell() { cell = null };

                    for (int i = 1; i < 7; i++)
                    {
                        moveDown = _scheduledCPD.Where(x => x.cell.dayNum == (obj.dayNum - i) && (x.cell.dayTypeCode == 585860001 || (x.cell.dayTypeCode == 585860000 && x.cell.statusCode != 585860004))).FirstOrDefault();
                        if (moveDown != null || i >= 7)
                        {
                            break;
                        }
                    }

                    if (moveDown == null)
                        return;

                    if (moveDown.cell != null && moveUp != null)
                    {
                        SwitchCells(moveUp, moveDown);
                    }

                    //var moveDown = _scheduledCPD.Where(x => x.cell.dayNum == (obj.dayNum - 1)).FirstOrDefault();
                    //var moveUp = _scheduledCPD.Where(x => x.cell.dayNum == (obj.dayNum)).FirstOrDefault();
                    //SwitchCells(moveUp, moveDown);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "moveCPD_UP" } });
            }
        }

        private holdingCell AddRestDay(LocalDBContactProgramDayV2 contactProgramDay, LocalDBProgramDay programDay, int dayNumber, int Counter)
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

            var dateDay = new Label
            {
                FontSize = 13,
                TextColor = Color.FromHex("#909090"),
                FontFamily = "HelveticalNeue-Bold",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var dateLabel = new Label
            {
                FontSize = 9,
                TextColor = Color.FromHex("#909090"),
                FontFamily = "PingFangTC-Regular",
                HorizontalTextAlignment = TextAlignment.Center
            };

            if (contactProgramDay.ScheduledStartDate == null)
            {
                if (contactProgramDay.SequenceNumber == 1)
                {
                    dateLabel.FontSize = 7;
                    dateLabel.Text = "CURRENT";
                    dateLabel.FontAttributes = FontAttributes.Bold;
                    dateLabel.FontFamily = "HelveticalNeue-Bold";
                    dateLabel.TextColor = Color.FromHex("#1C516B");
                    daySL.Children.Add(dateLabel);
                }
                else
                {
                    daySL.Children.Add(dayLabel);
                    daySL.Children.Add(dayNum);
                    //dateLabel.Text = "NOT SCHEDULED";
                    //dateLabel.TextColor = Color.FromHex("#ff5a5a");
                }
            }
            else
            {
                if (contactProgramDay.ScheduledStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                {
                    dateLabel.FontSize = 10;
                    dateLabel.FontAttributes = FontAttributes.Bold;
                    dateLabel.FontFamily = "HelveticalNeue-Bold";
                    dateLabel.Text = "TODAY";
                    dateLabel.TextColor = Color.FromHex("#1C516B");
                    daySL.Children.Add(dateLabel);
                }
                else
                {
                    //dateLabel.FontSize = 10;
                    dateLabel.Text = contactProgramDay.ScheduledStartDate?.ToLocalTime().Date.ToString("MMM d");
                    //dateLabel.TextColor = Color.FromHex("#909090");
                    dateDay.Text = contactProgramDay.ScheduledStartDate?.ToLocalTime().Date.ToString("ddd");
                    daySL.Children.Add(dateDay);
                    daySL.Children.Add(dateLabel);
                }
            }

            //daySL.Children.Add(dayLabel);
            //daySL.Children.Add(dayNum);
            //daySL.Children.Add(dateLabel);

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
                //Margin = new Thickness(-35, 0, 0, 0)
                Margin = new Thickness(5, 0, 0, 0)
            };

            mainSL.Children.Add(SecondarySL);

            var heading = new Label
            {
                Text = programDay.Heading,
                FontSize = 12,
                TextColor = Color.FromHex("#17191A"),
                //FontAttributes = FontAttributes.Bold,
                FontFamily = "HelveticalNeue-Bold",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                //Margin = new Thickness(20, 0, 0, 0)
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

            if (contactProgramDay.ScheduledStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
            {
                mainSL.BackgroundColor = Color.FromHex("#66d6df");// e5f8f9
            }
            else if (contactProgramDay.ScheduledStartDate?.ToLocalTime().Date <= DateTime.UtcNow.ToLocalTime().Date)
            {
                mainSL.BackgroundColor = Color.FromHex("#76E982");// e5f2e5
                heading.TextColor = Color.FromHex("#909090");
            }


            var btnSL = new StackLayout
            {
                Spacing = 10,
                IsVisible = false,
                Padding = new Thickness(0, 0, 5, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Color.Blue,
            };

            var moveCPD_DOWNBtn = new Button
            {
                Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_down_04.png"),
                HeightRequest = 25,
                WidthRequest = 25,
                VerticalOptions = LayoutOptions.Center,
            };

            var moveCPD_UPBtn = new Button
            {
                Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_up_04.png"),
                HeightRequest = 25,
                WidthRequest = 25,
                VerticalOptions = LayoutOptions.Center,
            };

            var cpdCell = new CPDCell
            {
                Label = heading,
                LabelText = heading.Text,
                bottomSL = new StackLayout(),
                ArrowSL = new StackLayout(),
                upBtn = moveCPD_UPBtn,
                downBtn = moveCPD_DOWNBtn,
                btnSL = btnSL,
                mainSL = mainSL,

                ContactProgramDayGuid = contactProgramDay.GuidCRM,
                ProgramDayGuid = programDay.GuidCRM,
                dayNum = Counter,
                position = dayNumber,
                scheduledDate = (DateTime?)contactProgramDay.ScheduledStartDate,
                SequenceNumber = contactProgramDay.SequenceNumber,
                stateCode = contactProgramDay.StateCodeValue,
                statusCode = contactProgramDay.StatusCodeValue,
                dayTypeCode = programDay.DayTypeValue,
            };

            if (contactProgramDay.ScheduledStartDate != null)
            {
                cpdCell.scheduledDate = (DateTime)contactProgramDay.ScheduledStartDate;
            }

            moveCPD_UPBtn.Command = new Command<CPDCell>(moveCPD_UP);
            moveCPD_UPBtn.CommandParameter = cpdCell;

            moveCPD_DOWNBtn.Command = new Command<CPDCell>(moveCPD_DOWN);
            moveCPD_DOWNBtn.CommandParameter = cpdCell;

            btnSL.Children.Add(moveCPD_UPBtn);
            btnSL.Children.Add(moveCPD_DOWNBtn);

            mainSL.Children.Add(btnSL);

            listOfProgramDays.Children.Add(mainSL);

            var hCell = new holdingCell
            {
                cell = cpdCell,
            };

            return hCell;
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "OnAppearing" } });
            //MessagingCenter.Send(this, "ExitedWorkout"); //should add this back?          
            totalDownloadedBytes.IsVisible = false;
            //GoToWorkoutTab();
            //var refreshTime = _lastPageSetupTime.AddSeconds(5);
            var refreshTime = _lastPageSetupTime.AddMinutes(_minutesBetweenRefresh);
            if (DateTime.UtcNow > refreshTime && !_DownloadingInProgress)
            {
                SetupPageV2();
            }
            base.OnAppearing();
        }

        private async void ResetScheduleBtn_Clicked(object sender, EventArgs e)
        {
            var CP = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
            if (CP != null)
            {
                bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("resetschedule", CP.ProgramId)).Result;
                if (!allowDownload)
                {
                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == CP.ProgramId).FirstOrDefaultAsync();
                    if (Program == null)
                    {
                        await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                        return;
                    }
                    //await DisplayAlert("Trial Limitation", "A valid membership is necessary to continue using this functionality", "Continue Trial");
                    await Navigation.PushModalAsync(new IAP(Program.GuidCRM), false);
                    return;
                }
            }

            var ResetScheduleBtn = (Button)sender;
            ResetScheduleBtn.IsEnabled = false;
            ResetScheduleBtn.Text = "... ONE MOMENT";

            var result = await DisplayAlert("Reset Schedule", "Resetting your schedule will clear your program's schedule and your program will not auto advance each day.", "Reset", "Not now");
            if (!result)
            {
                ResetScheduleBtn.IsEnabled = true;
                ResetScheduleBtn.Text = "RESET SCHEDULE";
                return;
            }

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                ResetScheduleBtn.IsEnabled = true;
                ResetScheduleBtn.Text = "RESET SCHEDULE";
                return;
            }

            //call reset schedule here - CP stays in progress and is schedule built changes to no and cpd have the scheduled dates removed, mark a flag for if the schedule was reset and the number of times it occurs
            //update the contact program locally here
            try
            {
                var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
                if (ContactProgram == null)
                    return;

                var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ContactProgram.ProgramId).FirstOrDefaultAsync();
                if (Program == null)
                    return;

                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", Program.Heading }, { "Action", "Resetting Schedule" } });

                var apiCallResult = await WebAPIService.ResetSchedule(_client, _ContactProgramGuid);
                if (apiCallResult == HttpStatusCode.OK)
                {
                    ContactProgram.IsScheduleBuilt = false;
                    await _connection.UpdateAsync(ContactProgram);

                    var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ContactProgram.Id).ToListAsync();
                    foreach (var cpd in ContactProgramDays)
                    {
                        if (cpd.StateCodeValue == 1 && cpd.StatusCodeValue != 585860004) //inactive and not complete
                        {
                            cpd.StateCodeValue = 0; //active
                            cpd.StatusCodeValue = 1; //active
                        }
                        cpd.ScheduledStartDate = null;
                        await _connection.UpdateAsync(cpd);
                    }
                    App.Current.MainPage = new MainStartingPage();

                }
                else
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", Program.Heading }, { "Action", "Resetting Schedule Failed" } });
                    await DisplayAlert("Error", "Opps we had an error. Try reloading your app from the 'More' Tab.", "OK");
                    ResetScheduleBtn.IsEnabled = true;
                    ResetScheduleBtn.Text = "RESET SCHEDULE";
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ResetScheduleBtn_Clicked" } });
                await DisplayAlert("Error", "Opps we had an error. Try again later.", "OK");
                ResetScheduleBtn.IsEnabled = true;
                ResetScheduleBtn.Text = "RESET SCHEDULE";
            }

        }

        private async void SetupPrograms()
        {
            var ContactPrograms = await _connection.Table<LocalDBContactProgram>().ToListAsync();
            var cpl = new List<ContactProgramList>();

            foreach (var contactProgram in ContactPrograms)
            {
                var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == contactProgram.ProgramId).FirstOrDefaultAsync();
                if (Program != null)
                {
                    var cp_alreadyinlist = cpl.Where(x => x.Program.Id == Program.Id).ToList();
                    if (cp_alreadyinlist.Count() == 0)
                    {
                        ContactProgramList cpl_Item = new ContactProgramList
                        {
                            Program = Program,
                            ContactProgramStatus = contactProgram.StatusCodeValue,
                            ContactProgramState = contactProgram.StateCodeValue,
                        };

                        if (contactProgram.StateCodeValue == 0)
                        {
                            cpl_Item.IsActive = true;
                        }
                        cpl.Add(cpl_Item);
                    }
                    else
                    {
                        foreach (var cp in cp_alreadyinlist)
                        {
                            if (contactProgram.StateCodeValue == 0)
                            {
                                cp.ContactProgramState = 0;
                                cp.IsActive = true;
                            }
                        }
                    }
                }
            }

            listOfPrograms.Children.Clear();
            cpl = cpl.OrderBy(x => x.ContactProgramState).ToList();

            foreach (var program in cpl)
            {

                var mainSL = new StackLayout
                {
                    BackgroundColor = Color.White,
                    Spacing = 0,
                };

                var imgSL = new StackLayout
                {
                    Spacing = 0,
                    //Padding = new Thickness(2,2,2,0)
                };

                mainSL.Children.Add(imgSL);

                var relLayout = new RelativeLayout();

                imgSL.Children.Add(relLayout);

                //var imgSource = new UriImageSource() { Uri = new Uri(program.Program.SecondaryPhotoUrl) };
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
                    cachedImage.Source = new UriImageSource() { Uri = new Uri(program.Program.SecondaryPhotoUrl) };
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Program SecondaryPhotoUrl Error" } });
                }

                relLayout.Children.Add(cachedImage, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var heading = new Label
                {
                    Text = program.Program.Heading,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "AvenirNextCondensed-Bold",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0)
                };

                var masterHeadingSL = new StackLayout
                {
                    //HorizontalOptions = LayoutOptions.FillAndExpand,
                    //VerticalOptions = LayoutOptions.End,
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    Margin = new Thickness(0, 0, 0, 27),
                    Opacity = 0.65,
                };

                var secondaryHeadingSL = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    BackgroundColor = Color.FromHex("#555757"),
                };
                masterHeadingSL.Children.Add(secondaryHeadingSL);

                var headingSL = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.EndAndExpand,
                    Spacing = 0,
                    Padding = new Thickness(10, 6, 10, 5)
                };

                var arrowImg = new Image
                {
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Source = (FileImageSource)ImageSource.FromFile("Arrow_26.png"),
                };

                headingSL.Children.Add(heading);
                //headingSL.Children.Add(arrowImg);
                secondaryHeadingSL.Children.Add(headingSL);
                secondaryHeadingSL.Children.Add(arrowImg);

                if (program.IsActive)
                {
                    var activeLabel = new Label
                    {
                        Text = "ACTIVE",
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = "AvenirNextCondensed-Bold",
                        TextColor = Color.FromHex("#00BBCB"),
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Start,
                        //Margin = new Thickness(100, 0, 0, 0)

                    };
                    headingSL.Children.Add(activeLabel);
                }

                relLayout.Children.Add(masterHeadingSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var bottonSL = new StackLayout
                {
                    Spacing = 0,
                    Padding = new Thickness(15, 10, 15, 10),
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.White,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                var descSL = new StackLayout
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                };

                var descLabel = new Label
                {
                    Text = program.Program.SubHeading,
                    TextColor = Color.FromHex("#909090"),
                    FontSize = 13,
                    FontFamily = "HelveticalNeue-Bold"
                };

                //var iconImg = new Image
                //{
                //    Source = "arrow_26_green.png",
                //    VerticalOptions = LayoutOptions.CenterAndExpand,
                //    HorizontalOptions = LayoutOptions.End,

                //};

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
                        mainSL.HeightRequest = 540;
                        imgSL.HeightRequest = 485;
                    }
                    else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                    {
                        mainSL.HeightRequest = 595;
                        imgSL.HeightRequest = 535;
                    }
                    else
                    {
                        mainSL.HeightRequest = 540;
                        imgSL.HeightRequest = 485;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Phone)
                {
                    var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                    if (isDeviceIphonePlus)
                    {
                        mainSL.HeightRequest = 520;
                        imgSL.HeightRequest = 465;
                    }
                    else
                    {
                        mainSL.HeightRequest = 450;
                        imgSL.HeightRequest = 395;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                    if (isDeviceLargeIPad)
                    {
                        mainSL.HeightRequest = 1100;
                        imgSL.HeightRequest = 1050;
                    }
                    else
                    {
                        mainSL.HeightRequest = 900;
                        imgSL.HeightRequest = 850;
                    }
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
                    ProgCell_Clicked(program.Program);
                };

                mainSL.GestureRecognizers.Add(tapGestureRecognizer);

                listOfPrograms.Children.Add(mainSL);

            }

        }

        private async void SetupActiveProgramDay_V2(LocalDBContactProgram ActiveContactProgram)
        {
            var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ActiveContactProgram.Id && x.IsRepeat != true).OrderBy(x => x.SequenceNumber).ToListAsync();

            if (ContactProgramDays.Count() == 0)
                return;

            if (ContactProgramDays.Count() == ContactProgramDays.Where(x => x.StatusCodeValue == 585860004).ToList().Count()) //all complete
            {
                Show_Today_ProgramCompleteScreen_L2();
                SetupSchedule(ActiveContactProgram);
                return;
            }

            LocalDBContactProgramDayV2 todaysCPD = new LocalDBContactProgramDayV2();

            foreach (var item in ContactProgramDays)
            {
                if (item.ScheduledStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                {
                    todaysCPD = item;
                    DisplayContactProgramDay_v1(todaysCPD);
                    break;
                }
            }

            if (todaysCPD.GuidCRM == Guid.Empty)
            {
                //Determine if we are at the start or end
                var firstDay = ContactProgramDays.First();
                var lastDay = ContactProgramDays.Last();

                if (firstDay.ScheduledStartDate != null)
                {
                    if (firstDay.ScheduledStartDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date)
                    {
                        //show first day
                        DisplayContactProgramDay_v1(firstDay);
                        SetupSchedule(ActiveContactProgram);
                        return;
                    }
                }

                if (lastDay.ScheduledStartDate != null)
                {
                    if (lastDay.ScheduledStartDate?.ToLocalTime().Date <= DateTime.UtcNow.ToLocalTime().Date)
                    {
                        //program complete
                        Show_Today_ProgramCompleteScreen_L2();
                        SetupSchedule(ActiveContactProgram);
                        return;
                    }
                }

                //show the first day in the sequence b/c the dates have not been added yet to the contact program days
                Show_Schedule_HasActiveProgram_L2();
                DisplayContactProgramDay_v1(firstDay);
                SetupSchedule(ActiveContactProgram);
                return;
            }
            else
            {
                Show_Schedule_HasActiveProgram_L2();
                DisplayContactProgramDay_v1(todaysCPD);
                SetupSchedule(ActiveContactProgram);
                return;
            }
            //Hide_Today_ProgramDayLoader();
        }

        private async void DisplayContactProgramDay_v1(LocalDBContactProgramDayV2 contactProgramDay)
        {
            //HasAProgram.IsVisible = true;
            //LoadingSL.IsVisible = false;
            var ProgramDay = new LocalDBProgramDay();
            try
            {
                ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.Id == contactProgramDay.ProgramDayId).FirstOrDefaultAsync();
                if (ProgramDay == null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "DisplayContactProgramDay_v1" } });
                //throw new Exception("Error occured: " + ex.ToString());
            }

            _ProgramDayGuid = ProgramDay.GuidCRM;
            _ContactProgramDayGuid = contactProgramDay.GuidCRM;
            //_ContactProgramDayGuidDB = contactProgramDay.GuidCRM;
            _IsWorkoutDownloaded = contactProgramDay.IsDownloaded;

            if (ProgramDay.DayTypeValue == 585860000)
            {
                //Workout Day
                programDayName.Text = ProgramDay.Heading;

                if (contactProgramDay.ScheduledStartDate != null)
                {
                    programDayDate.Text = contactProgramDay.ScheduledStartDate?.ToLocalTime().Date.ToString("dddd, MMM d");
                }
                else
                {
                    programDateSL.IsVisible = false;
                    programDayDate.Text = "CURRENT";
                }
                NumberOfExercises.Text = ProgramDay.TotalExercises.ToString();
                TotalTime.Text = ProgramDay.TimeMinutes.ToString();
                Level.Text = ProgramDay.Level;

                //var imgSource = new UriImageSource() { Uri = new Uri(ProgramDay.PhotoUrl) };
                //imgSource.CachingEnabled = true;
                //imgSource.CacheValidity = TimeSpan.FromDays(7);
                //programDayImage.Source = imgSource;

                programDayImage.CacheDuration = TimeSpan.FromDays(30);
                programDayImage.RetryCount = 5;
                programDayImage.RetryDelay = 250;
                programDayImage.BitmapOptimizations = true;

                try
                {
                    programDayImage.Source = new UriImageSource() { Uri = new Uri(ProgramDay.PhotoUrl) };
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Program Day PhotoUrl Error" } });
                }

                if (!_IsWorkoutDownloaded)
                {
                    bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("download", ProgramDay.ProgramId, 1)).Result;
                    if (allowDownload)
                    {
                        GoBtn.Text = "DOWNLOAD MY WORKOUT";
                    }
                    else
                    {
                        GoBtn.Text = "START FREE TRIAL";
                    }
                    ProgressBar.IsVisible = false;
                    totalDownloadedBytes.IsVisible = false;
                    await ProgressBar.ProgressTo(0D, 1, Easing.Linear);
                }
                else if (_IsWorkoutDownloaded)
                {
                    GoBtn.Text = "WORKOUT";
                    ProgressBar.IsVisible = true;
                    totalDownloadedBytes.IsVisible = true;
                    await ProgressBar.ProgressTo(1D, 1, Easing.Linear);
                }

                var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
                if (isOfXFamily)
                {
                    var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                    var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                    var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                    if (isDeviceIphoneXorXS)
                    {
                        if (Xamarin.Essentials.DeviceInfo.Platform.Equals("iOS"))
                            ProgramDayImageHeight.HeightRequest = 620;
                        else
                        {

                            ProgramDayImageHeight.HeightRequest = App.screenHeight - 140;
                        }


                    }
                    else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                    {

                        if (Xamarin.Essentials.DeviceInfo.Platform.Equals("iOS"))
                            ProgramDayImageHeight.HeightRequest = 705;
                        else
                        {

                            ProgramDayImageHeight.HeightRequest = App.screenHeight - 140;
                        }
                    }
                    else
                    {
                        ProgramDayImageHeight.HeightRequest = 620;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Phone)
                {
                    var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                    if (isDeviceIphonePlus)
                    {
                        ProgramDayImageHeight.HeightRequest = 600;
                    }
                    else
                    {
                        ProgramDayImageHeight.HeightRequest = 530;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                    if (isDeviceLargeIPad)
                    {
                        ProgramDayImageHeight.HeightRequest = 1200;
                    }
                    else
                    {
                        ProgramDayImageHeight.HeightRequest = 890;
                    }
                }
                else
                {
                    ProgramDayImageHeight.HeightRequest = 600;
                }
                DisplayProgramDayMessage();
                Show_Today_ContactProgramDayScreen_L2("workout");
            }
            else if (ProgramDay.DayTypeValue == 585860001)
            {
                //Rest day
                if (contactProgramDay.ScheduledStartDate != null)
                {
                    programDayRestName.Text = contactProgramDay.ScheduledStartDate?.ToLocalTime().Date.ToString("dddd, MMMM d");
                }
                else
                {
                    programDayRestName.Text = "";
                }
                programDayRestHeading.Text = ProgramDay.Heading;
                programDayRestSubHeading.Text = ProgramDay.SubHeading;

                if (programDayRestSubHeading.Text == "")
                {
                    programDayRestSubHeadingSL.IsVisible = false;
                }

                //var imgSource = new UriImageSource() { Uri = new Uri(ProgramDay.PhotoUrl) };
                //imgSource.CachingEnabled = true;
                //imgSource.CacheValidity = TimeSpan.FromDays(7);
                //programDayRestImage.Source = imgSource;

                programDayRestImage.CacheDuration = TimeSpan.FromDays(30);
                programDayRestImage.RetryCount = 5;
                programDayRestImage.RetryDelay = 250;
                programDayRestImage.BitmapOptimizations = true;

                try
                {
                    programDayRestImage.Source = new UriImageSource() { Uri = new Uri(ProgramDay.PhotoUrl) };
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Program Rest Day PhotoUrl Error" } });
                }

                var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
                if (isOfXFamily)
                {
                    var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                    var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                    var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                    if (isDeviceIphoneXorXS)
                    {
                        RestDayIPad.Margin = new Thickness(40, 360, 40, 10);
                        ProgramDayRestImageHeight.HeightRequest = 620;
                    }
                    else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                    {
                        RestDayIPad.Margin = new Thickness(40, 400, 40, 10);
                        ProgramDayRestImageHeight.HeightRequest = 705;
                    }
                    else
                    {
                        RestDayIPad.Margin = new Thickness(40, 360, 40, 10);
                        ProgramDayRestImageHeight.HeightRequest = 620;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Phone)
                {
                    var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                    if (isDeviceIphonePlus)
                    {
                        RestDayIPad.Margin = new Thickness(40, 350, 40, 10);
                        ProgramDayRestImageHeight.HeightRequest = 600;
                    }
                    else
                    {
                        RestDayIPad.Margin = new Thickness(40, 320, 40, 10);
                        ProgramDayRestImageHeight.HeightRequest = 530;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                    if (isDeviceLargeIPad)
                    {
                        ProgramDayRestImageHeight.HeightRequest = 1200;
                        RestDayIPad.Margin = new Thickness(40, 700, 40, 10);
                    }
                    else
                    {
                        ProgramDayRestImageHeight.HeightRequest = 890;
                        RestDayIPad.Margin = new Thickness(40, 550, 40, 10);
                    }
                }
                else
                {
                    ProgramDayRestImageHeight.HeightRequest = 600;
                }

                Show_Today_ContactProgramDayScreen_L2("rest");
            }
            Hide_Today_ProgramDayLoader();
        }

        private async void StartProgram_Btn(object sender, EventArgs e)
        {
            var CP = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
            if (CP != null)
            {
                bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("startprogram", CP.ProgramId)).Result;
                if (!allowDownload)
                {
                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == CP.ProgramId).FirstOrDefaultAsync();
                    if (Program == null)
                    {
                        await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                        return;
                    }
                    await Navigation.PushModalAsync(new IAP(Program.GuidCRM), false);
                    return;
                }
            }

            var result = await DisplayAlert("Setup Program", "Are you sure that you want to setup your program?", "Setup", "Not now");
            if (!result)
            {
                NotLoadedYet.IsVisible = false;
                return;
            }

            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                NotLoadedYet.IsVisible = false;
                return;
            }

            _deactiveTopNav = true;

            try
            {
                if (_ContactProgramGuid != Guid.Empty && result)
                {
                    NotLoadedYet.IsVisible = true;
                    VideoBtn.Text = "... ONE MOMENT";
                    VideoBtn.IsEnabled = false;
                    HttpResponseMessage response = await WebAPIService.ActivateProgram(_client, _ContactProgramGuid);
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Started Program" } });

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //HttpContent content = response.Content;
                        //var json = await content.ReadAsStringAsync();
                        //var newContactProgramGuid = JsonConvert.DeserializeObject<Guid>(json);

                        var contactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
                        if (contactProgram != null)
                        {
                            //GET the first Program Day for the program
                            //2. Display first day
                            MessagingCenter.Send(this, "ProgramStarted");
                            SetupActiveProgramDay_V2(contactProgram);
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "We had a small error. Please try again later.", "OK");
                        VideoBtn.Text = "SETUP PROGRAM";
                        VideoBtn.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                VideoBtn.Text = "SETUP PROGRAM";
                VideoBtn.IsEnabled = true;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "StartProgram_Btn()" } });
            }
            NotLoadedYet.IsVisible = false;
            _deactiveTopNav = false;
        }

        private async void ProgCell_Clicked(LocalDBProgram program)
        {
            await Navigation.PushAsync(new ContactPrograms(program));
        }

        private async void GoBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var CP = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == _ContactProgramGuid).FirstOrDefaultAsync();
                if (CP != null)
                {
                    int dayNumber = 100;
                    var ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDayGuid).FirstOrDefaultAsync();
                    if (ContactProgramDay != null)
                    {
                        var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.Id == ContactProgramDay.ProgramDayId).FirstOrDefaultAsync();
                        if (ProgramDay != null)
                        {
                            dayNumber = ProgramDay.SequenceNumber;
                        }
                    }

                    bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("download", CP.ProgramId, dayNumber)).Result;
                    //bool allowDownload = true;
                    if (!allowDownload)
                    {
                        var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == CP.ProgramId).FirstOrDefaultAsync();
                        if (Program == null)
                        {
                            await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                            return;
                        }
                        await Navigation.PushModalAsync(new IAP(Program.GuidCRM), false);
                        return;
                    }
                }

                if (!_IsWorkoutDownloaded)
                {
                    bool isWifiConnected = CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.WiFi);

                    if (CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.WiFi))
                    {
                        DownloadProgramDay();
                    }
                    else
                    {
                        var result = await DisplayAlert("Faster on Wifi", "Connect to Wifi for a faster download.", "Download", "Cancel");
                        if (result)
                        {
                            DownloadProgramDay();
                        }
                    }
                }
                else
                {
                    await Navigation.PushModalAsync(new ProgramDay_v2(_ContactProgramDayGuid));
                }
            }
            catch (Exception ex)
            {

            }

        }

        private async void DownloadProgramDay()
        {
            _DownloadingInProgress = true;

            if (!IsInternetConnected())
            {
                GoBtn.Text = "NO INTERNET";
                GoBtn.IsEnabled = true;
                _DownloadingInProgress = false;
                return;
            }

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Started Downloading Program" } });

                GoBtn.IsEnabled = false;
                GoBtn.Text = "GETTING WORKOUT...";
                ProgressBar.IsVisible = true;
                totalDownloadedBytes.IsVisible = true;
                await ProgressBar.ProgressTo(.00D, 1, Easing.Linear);

                if (_numberOfDownloadAttempts.ContainsKey(_ContactProgramDayGuid))
                {
                    _numberOfDownloadAttempts[_ContactProgramDayGuid] += 1;
                }
                else
                {
                    _numberOfDownloadAttempts.Add(_ContactProgramDayGuid, 1);
                }

                var downloadResponse = await WebAPIService.GetContactProgramDayToDownload(_client, _ContactProgramDayGuid, Guid.Empty, false);

                if (downloadResponse.Message == "download limit reached")
                {
                    GoBtn.Text = "DOWNLOAD MY WORKOUT";
                    ProgressBar.IsVisible = false;
                    totalDownloadedBytes.IsVisible = false;
                    await DisplayAlert("Download Limit", "Reached maximum download attempts. Wait 15 minutes, assure that you have a good internet connection and you'll be able to try again.", "Got it");
                }
                else if (downloadResponse.Message == "bad request")
                {
                    GoBtn.Text = "DOWNLOAD MY WORKOUT";
                    ProgressBar.IsVisible = false;
                    totalDownloadedBytes.IsVisible = false;
                    await DisplayAlert("Try Again", "There was an issue. Please try again.", "Ok");
                    GoBtn.IsEnabled = true;
                }
                else if (downloadResponse.Message != "success")
                {
                    GoBtn.Text = "DOWNLOAD MY WORKOUT";
                    ProgressBar.IsVisible = false;
                    totalDownloadedBytes.IsVisible = false;
                    await DisplayAlert("Try Again", "There was an error. Please try again.", "Ok");
                    GoBtn.IsEnabled = true;
                }
                else if (downloadResponse.Message == "success")
                {
                    GoBtn.Text = "DOWNLOADING...";

                    var ContactActionsFromAPI = downloadResponse.ContactActions;
                    var AudiosFromAPI = downloadResponse.Audios;

                    if (ContactActionsFromAPI.Count() == 0)
                    {
                        _DownloadingInProgress = false;
                        return;
                    }

                    int totalFiles = ContactActionsFromAPI.Count() + AudiosFromAPI.Count();
                    double incrementPerFile = (1D / (totalFiles)) * 0.85D;
                    int totalDownloads_AC = 0;
                    int totalDownloads_AuC = 0;
                    _ContactProgramDayGuid = downloadResponse.ContactProgramDayGuid;
                    var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == downloadResponse.ProgramDayGuid).FirstOrDefaultAsync();

                    //Assume all Contact Actions are from the same ProgramDay
                    var ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDayGuid).FirstOrDefaultAsync();
                    if (ContactProgramDay == null)
                    {
                        ContactProgramDay = new LocalDBContactProgramDayV2
                        {
                            GuidCRM = _ContactProgramDayGuid,
                            ReceivedOn = DateTime.UtcNow,
                            ProgramDayId = ProgramDay.Id,
                        };
                        await _connection.InsertAsync(ContactProgramDay);
                    }

                    var ContactActionsToInsert = new List<LocalDBContactAction>();
                    List<Task<DownloadResult>> downloads = new List<Task<DownloadResult>>();
                    List<Task<DownloadResultAudio>> downloadAudio = new List<Task<DownloadResultAudio>>();

                    Task<List<LocalDBContactAction>> t1_ca = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBContactAction>().ToListAsync();
                    });
                    Task<List<LocalDBAction>> t2_a = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBAction>().ToListAsync();
                    });
                    Task<List<LocalDBActionContentV2>> t3_ac = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBActionContentV2>().ToListAsync();
                    });
                    Task<List<LocalDBAudio>> t4_au = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBAudio>().ToListAsync();
                    });
                    Task<List<LocalDBAudioContentV2>> t5_auc = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBAudioContentV2>().ToListAsync();
                    });
                    await Task.WhenAll(t1_ca, t2_a, t3_ac);

                    var ContactActions = t1_ca.Result;
                    var Actions = t2_a.Result;
                    var ActionContents = t3_ac.Result;

                    foreach (var contactAction in ContactActionsFromAPI)
                    {
                        var ContactAction = ContactActions.Where(x => x.GuidCRM == contactAction.GuidCRM).FirstOrDefault();
                        if (ContactAction == null)
                        {
                            ContactAction = new LocalDBContactAction();
                            ContactAction.Synced = contactAction.Synced;
                            ContactAction.GuidCRM = contactAction.GuidCRM;
                            ContactAction.StateCodeValue = contactAction.StateCodeValue;
                            ContactAction.StatusCodeValue = contactAction.StatusCodeValue;
                            ContactAction.ContactProgramDayId = ContactProgramDay.Id;

                            var Action = Actions.Where(x => x.GuidCRM == contactAction.Action.GuidCRM).FirstOrDefault();
                            if (Action == null)
                            {
                                Action = new LocalDBAction
                                {
                                    GuidCRM = contactAction.Action.GuidCRM,
                                    StateCodeValue = contactAction.Action.StateCodeValue,
                                    StatusCodeValue = contactAction.Action.StatusCodeValue,
                                    SequenceNumber = contactAction.Action.SequenceNumber,
                                    ContentTypeValue = contactAction.Action.ContentTypeValue,
                                    IsTrainingRound = contactAction.Action.IsTrainingRound,
                                    NumberOfReps = contactAction.Action.NumberOfReps,
                                    WeightLbs = contactAction.Action.WeightLbs,
                                    TimeSeconds = contactAction.Action.TimeSeconds,
                                    IntensityValue = contactAction.Action.IntensityValue,
                                    Heading = contactAction.Action.Heading,
                                    ProgramDayId = ProgramDay.Id
                                };
                                await _connection.InsertAsync(Action);
                                Actions.Add(Action);
                            }
                            else
                            {
                                Action.StateCodeValue = contactAction.Action.StateCodeValue;
                                Action.StatusCodeValue = contactAction.Action.StatusCodeValue;
                                Action.SequenceNumber = contactAction.Action.SequenceNumber;
                                Action.ContentTypeValue = contactAction.Action.ContentTypeValue;
                                Action.IsTrainingRound = contactAction.Action.IsTrainingRound;
                                Action.NumberOfReps = contactAction.Action.NumberOfReps;
                                Action.WeightLbs = contactAction.Action.WeightLbs;
                                Action.TimeSeconds = contactAction.Action.TimeSeconds;
                                Action.IntensityValue = contactAction.Action.IntensityValue;
                                Action.Heading = contactAction.Action.Heading;
                                Action.ProgramDayId = ProgramDay.Id;
                                await _connection.UpdateAsync(Action);
                            }
                            ContactAction.ActionId = Action.Id;
                            bool mustDownloadActionContent = false;

                            var ActionContent = ActionContents.Where(x => x.GuidCRM == contactAction.ActionContent.GuidCRM).FirstOrDefault();
                            if (ActionContent == null)
                            {
                                ActionContent = new LocalDBActionContentV2
                                {
                                    GuidCRM = contactAction.ActionContent.GuidCRM,
                                    Heading = contactAction.ActionContent.Heading,
                                    PhotoUrl = contactAction.ActionContent.PhotoUrl,
                                    VideoUrl = contactAction.ActionContent.VideoUrl,
                                    ContentTypeValue = contactAction.Action.ContentTypeValue,
                                    IsPreview = false,
                                    LastDownloadAttempt = DateTime.UtcNow,
                                    ContactProgramDayId = ContactProgramDay.Id
                                };
                                await _connection.InsertAsync(ActionContent);
                                ActionContents.Add(ActionContent);
                                mustDownloadActionContent = true;
                            }
                            else
                            {
                                if (ActionContent.PhotoUrl != contactAction.ActionContent.PhotoUrl || ActionContent.VideoUrl != contactAction.ActionContent.VideoUrl)
                                {
                                    ActionContent.PhotoFilePath = "";
                                    ActionContent.VideoFilePath = "";
                                    ActionContent.LastDownloadAttempt = DateTime.UtcNow;
                                    mustDownloadActionContent = true;
                                }
                                ActionContent.Heading = contactAction.ActionContent.Heading;
                                ActionContent.PhotoUrl = contactAction.ActionContent.PhotoUrl;
                                ActionContent.VideoUrl = contactAction.ActionContent.VideoUrl;
                                ActionContent.ContentTypeValue = contactAction.Action.ContentTypeValue;
                                ActionContent.IsPreview = false;
                                ActionContent.ContactProgramDayId = ContactProgramDay.Id;
                                await _connection.UpdateAsync(ActionContent);
                            }

                            if (mustDownloadActionContent)
                            {
                                //download action content
                                var videoUrl = contactAction.ActionContent.VideoUrl;
                                var photoUrl = contactAction.ActionContent.PhotoUrl;
                                if (videoUrl != "" && photoUrl != "" && videoUrl.Contains("https://") && Path.GetExtension(videoUrl) == ".mp4" && photoUrl.Contains("https://"))
                                {
                                    var videoFileName = contactAction.ActionContent.GuidCRM.ToString() + Path.GetExtension(videoUrl);
                                    var photoFileName = contactAction.ActionContent.GuidCRM.ToString() + Path.GetExtension(photoUrl);
                                    downloads.Add(DownloadWithUrlTrackingTaskAsync(photoFileName, videoFileName, photoUrl, videoUrl, contactAction.ActionContent.GuidCRM, _ContactProgramDayGuid));
                                    totalDownloads_AC++;
                                }
                            }
                            ContactAction.ActionContentId = ActionContent.Id;
                            ContactActionsToInsert.Add(ContactAction);
                        }

                        if (ProgressBar.Progress < 1D)
                        {
                            await ProgressBar.ProgressTo(ProgressBar.Progress + incrementPerFile, 1, Easing.Linear);
                        }
                    }
                    await _connection.InsertAllAsync(ContactActionsToInsert);

                    var ListOfActionsToUpdate = new List<LocalDBAction>();
                    foreach (var contactAction in ContactActionsToInsert)
                    {
                        var Action = Actions.Where(x => x.Id == contactAction.ActionId).FirstOrDefault();
                        Action.ActionContentId = contactAction.ActionContentId;
                        ListOfActionsToUpdate.Add(Action);
                    }
                    await _connection.UpdateAllAsync(ListOfActionsToUpdate);

                    await Task.WhenAll(t4_au, t5_auc);
                    var AudioContents = t5_auc.Result;
                    var Audios = t4_au.Result;

                    if (AudiosFromAPI.Count() != 0)
                    {
                        foreach (var item in AudiosFromAPI)
                        {
                            bool mustDownloadAudioContent = false;

                            var AudioContent = AudioContents.Where(x => x.GuidCRM == item.AudioContent.GuidCRM).FirstOrDefault();
                            if (AudioContent == null)
                            {
                                AudioContent = new LocalDBAudioContentV2
                                {
                                    GuidCRM = item.AudioContent.GuidCRM,
                                    LengthMilliseconds = item.AudioContent.LengthMilliseconds,
                                    AudioUrl = item.AudioContent.AudioUrl,
                                    LastDownloadAttempt = DateTime.UtcNow,
                                    ContactProgramDayId = ContactProgramDay.Id,
                                };
                                await _connection.InsertAsync(AudioContent);
                                AudioContents.Add(AudioContent);
                                mustDownloadAudioContent = true;
                            }
                            else
                            {
                                if (AudioContent.AudioUrl != item.AudioContent.AudioUrl)
                                {
                                    AudioContent.AudioFilePath = "";
                                    AudioContent.LastDownloadAttempt = DateTime.UtcNow;
                                    mustDownloadAudioContent = true;
                                }
                                AudioContent.LengthMilliseconds = item.AudioContent.LengthMilliseconds;
                                AudioContent.AudioUrl = item.AudioContent.AudioUrl;
                                AudioContent.ContactProgramDayId = ContactProgramDay.Id;
                                await _connection.UpdateAsync(AudioContent);
                            }

                            if (mustDownloadAudioContent)
                            {
                                var audioUrl = item.AudioContent.AudioUrl;
                                if (audioUrl != "" && audioUrl.Contains("https://") && (Path.GetExtension(audioUrl) == ".mp3" || Path.GetExtension(audioUrl) == ".wav"))
                                {
                                    var audioFileName = item.AudioContent.GuidCRM.ToString() + Path.GetExtension(audioUrl);
                                    downloadAudio.Add(DownloadAudioWithUrlTrackingTaskAsync(audioFileName, audioUrl, item.AudioContent.GuidCRM, _ContactProgramDayGuid));
                                    totalDownloads_AuC++;
                                }
                            }

                            var Audio = Audios.Where(x => x.GuidCRM == item.GuidCRM).FirstOrDefault();
                            if (Audio == null)
                            {
                                Audio = new LocalDBAudio
                                {
                                    GuidCRM = item.GuidCRM,
                                    SequenceNumber = item.SequenceNumber,
                                    PreDelay = item.PreDelay,
                                    IsRepeat = item.IsRepeat,
                                    NumberOfRepeats = item.NumberOfRepeats,
                                    RepeatCycleSeconds = item.RepeatCycleSeconds
                                };

                                if (AudioContent != null)
                                    Audio.AudioContentId = AudioContent.Id;

                                if (ProgramDay != null)
                                    Audio.ProgramDayId = ProgramDay.Id;

                                var Action = Actions.Where(x => x.GuidCRM == item.ActionGuid).FirstOrDefault();
                                if (Action != null)
                                    Audio.ActionId = Action.Id;

                                await _connection.InsertAsync(Audio);
                                Audios.Add(Audio);
                            }
                            else
                            {
                                Audio.SequenceNumber = item.SequenceNumber;
                                Audio.PreDelay = item.PreDelay;
                                Audio.IsRepeat = item.IsRepeat;
                                Audio.NumberOfRepeats = item.NumberOfRepeats;
                                Audio.RepeatCycleSeconds = item.RepeatCycleSeconds;

                                if (AudioContent != null)
                                    Audio.AudioContentId = AudioContent.Id;

                                if (ProgramDay != null)
                                    Audio.ProgramDayId = ProgramDay.Id;

                                var Action = Actions.Where(x => x.GuidCRM == item.ActionGuid).FirstOrDefault();
                                if (Action != null)
                                    Audio.ActionId = Action.Id;

                                await _connection.UpdateAsync(Audio);
                            }

                            if (ProgressBar.Progress < 1D)
                            {
                                await ProgressBar.ProgressTo(ProgressBar.Progress + incrementPerFile, 1, Easing.Linear);
                            }
                        }
                    }

                    double incrementDownload_AC = totalDownloads_AC / 20D;//* 0.1D * 0.5D
                    double incrementDownload_AuC = totalDownloads_AuC / 20D;
                    await ProgressBar.ProgressTo(0.85D, 1, Easing.Linear);
                    Task t1 = Task.Run(() => ProcessAudioDownloads(downloadAudio, incrementDownload_AuC));
                    Task t2 = Task.Run(() => ProcessVideoDownloads(downloads, incrementDownload_AC));
                    int timeout = 60000; //miliseconds
                    await Task.WhenAny(Task.WhenAll(t1, t2), Task.Delay(timeout));
                    //Check each file that was just downloaded for its size in bytes
                    //If any are zero then redownload them
                    List<Task<DownloadResult>> downloads2 = new List<Task<DownloadResult>>();
                    List<Task<DownloadResultAudio>> downloadAudio2 = new List<Task<DownloadResultAudio>>();
                    IFolder rootFolder = FileSystem.Current.LocalStorage;
                    IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
                    int RedownloadErrorCount_AC = 0;
                    int RedownloadErrorCount_AuC = 0;
                    int RedownloadCount_AC = 0;
                    int RedownloadCount_AuC = 0;

                    var ActionContent_Review = await _connection.Table<LocalDBActionContentV2>().Where(x => x.VideoUrl != "" && x.PhotoUrl != "" && x.VideoUrl != null && x.PhotoUrl != null && x.IsPreview != true && (x.ContentTypeValue == 585860000 || x.ContentTypeValue == 585860001 || x.ContentTypeValue == 585860003)).ToListAsync();//Rep, Time, Stretch
                    foreach (var ac in ActionContent_Review)
                    {
                        try
                        {
                            if (ac.PhotoFilePath == "" || ac.VideoFilePath == "" || ac.PhotoFilePath == null || ac.VideoFilePath == null)
                            {
                                //Redownload
                                var videoUrl = ac.VideoUrl;
                                var photoUrl = ac.PhotoUrl;

                                if (videoUrl != "" && photoUrl != "" && videoUrl.Contains("https://") && Path.GetExtension(videoUrl) == ".mp4" && photoUrl.Contains("https://"))
                                {
                                    var videoFileName = ac.GuidCRM.ToString() + Path.GetExtension(videoUrl);
                                    var photoFileName = ac.GuidCRM.ToString() + Path.GetExtension(photoUrl);
                                    downloads2.Add(DownloadWithUrlTrackingTaskAsync(photoFileName, videoFileName, photoUrl, videoUrl, ac.GuidCRM, _ContactProgramDayGuid));
                                }
                                RedownloadCount_AC++;
                            }
                            else
                            {
                                IFile photoFile = await folder.GetFileAsync(_rootFilePath + ac.PhotoFilePath);
                                IFile videoFile = await folder.GetFileAsync(_rootFilePath + ac.VideoFilePath);
                                int photoLength = await Task.Run(() => ReadMediaLength(photoFile));
                                int videoLength = await Task.Run(() => ReadMediaLength(videoFile));

                                if (photoLength == 0 || videoLength == 0)
                                {
                                    //Redownload
                                    var videoUrl = ac.VideoUrl;
                                    var photoUrl = ac.PhotoUrl;

                                    if (videoUrl != "" && photoUrl != "" && videoUrl.Contains("https://") && Path.GetExtension(videoUrl) == ".mp4" && photoUrl.Contains("https://"))
                                    {
                                        var videoFileName = ac.GuidCRM.ToString() + Path.GetExtension(videoUrl);
                                        var photoFileName = ac.GuidCRM.ToString() + Path.GetExtension(photoUrl);
                                        downloads2.Add(DownloadWithUrlTrackingTaskAsync(photoFileName, videoFileName, photoUrl, videoUrl, ac.GuidCRM, _ContactProgramDayGuid));
                                    }
                                    RedownloadCount_AC++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var error = ex.ToString();
                            RedownloadErrorCount_AC++;
                        }
                    }

                    if (RedownloadErrorCount_AC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadErrorCount_AC > 0" } });
                    }

                    if (RedownloadCount_AC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadCount_AC > 0" } });
                    }

                    var AudioContent_Review = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.AudioUrl != "" && x.AudioUrl != null).ToListAsync();
                    foreach (var auc in AudioContent_Review)
                    {
                        try
                        {
                            if (auc.AudioFilePath == "" || auc.AudioFilePath == null)
                            {
                                //Redownload
                                var audioUrl = auc.AudioUrl;

                                if (audioUrl != "" && audioUrl.Contains("https://") && (Path.GetExtension(audioUrl) == ".mp3" || Path.GetExtension(audioUrl) == ".wav"))
                                {
                                    var audioFileName = auc.GuidCRM.ToString() + Path.GetExtension(audioUrl);
                                    downloadAudio2.Add(DownloadAudioWithUrlTrackingTaskAsync(audioFileName, audioUrl, auc.GuidCRM, _ContactProgramDayGuid));
                                }
                                RedownloadCount_AuC++;
                            }
                            else
                            {
                                IFile audioFile = await folder.GetFileAsync(_rootFilePath + auc.AudioFilePath);
                                int audioLength = await Task.Run(() => ReadMediaLength(audioFile));

                                if (audioLength == 0)
                                {
                                    //Redownload
                                    var audioUrl = auc.AudioUrl;

                                    if (audioUrl != "" && audioUrl.Contains("https://") && (Path.GetExtension(audioUrl) == ".mp3" || Path.GetExtension(audioUrl) == ".wav"))
                                    {
                                        var audioFileName = auc.GuidCRM.ToString() + Path.GetExtension(audioUrl);
                                        downloadAudio2.Add(DownloadAudioWithUrlTrackingTaskAsync(audioFileName, audioUrl, auc.GuidCRM, _ContactProgramDayGuid));
                                    }
                                    RedownloadCount_AuC++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var error = ex.ToString();
                            RedownloadErrorCount_AuC++;
                        }
                    }

                    if (RedownloadErrorCount_AuC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadErrorCount_AuC > 0" } });
                    }

                    if (RedownloadCount_AuC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadCount_AuC > 0" } });
                    }

                    Task t3 = Task.Run(() => ProcessAudioDownloads(downloadAudio2, incrementDownload_AC));
                    Task t4 = Task.Run(() => ProcessVideoDownloads(downloads2, incrementDownload_AuC));
                    int timeout_review = 20000; //miliseconds
                    await Task.WhenAny(Task.WhenAll(t3, t4), Task.Delay(timeout_review));

                    GoBtn.Text = "REVIEWING...";

                    bool DownloadResult = await Task.Run(() => CheckIfDownloadWasSuccessful(ContactProgramDay.Id, folder));
                    if (!DownloadResult)
                    {
                        if (_numberOfDownloadAttempts[_ContactProgramDayGuid] <= 2)
                        {
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "ReDownload - Advised" } });
                            await DisplayAlert("Redownload", "We were only able to do a partial download. To download the full workout, make sure that you have a good internet connection and try again. Max 10 download attempts.", "OK");
                        }
                        else
                        {
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "ReDownload - Logout Suggested" } });
                            await DisplayAlert("Try to Log out", "Your download has failed more than twice. Try to logout and back in.", "OK");
                        }
                        GoBtn.Text = "REDOWNLOAD";
                        ProgressBar.IsVisible = false;
                        totalDownloadedBytes.IsVisible = false;
                        GoBtn.IsEnabled = true;
                    }
                    else
                    {
                        var bytesText = "";

                        if (_totalBytes >= 1000000)
                        {
                            bytesText = Math.Round((_totalBytes / 1024D) / 1024D, 1) + "MB";
                        }
                        else if (_totalBytes >= 1000)
                        {
                            bytesText = Math.Round(_totalBytes / 1024D, 1) + "KB";
                        }
                        else
                        {
                            bytesText = Math.Round(_totalBytes, 0) + "B";
                        }

                        var ContactProgramDay2 = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDayGuid).FirstOrDefaultAsync();
                        if (ContactProgramDay2 != null)
                        {
                            ContactProgramDay2.DownloadedOn = DateTime.UtcNow;
                            //ContactProgramDay2.ReceivedOn = DateTime.UtcNow;
                            ContactProgramDay2.IsDownloaded = true;
                            await _connection.UpdateAsync(ContactProgramDay2);
                        }

                        _IsWorkoutDownloaded = true;
                        GoBtn.Text = "WORKOUT";
                        totalDownloadedBytes.Text = bytesText;
                        totalDownloadedBytes.IsVisible = true;
                        GoBtn.IsEnabled = true;
                        await ProgressBar.ProgressTo(1D, 1, Easing.Linear);

                        Branch branch = Branch.GetInstance();
                        branch.SendEvent(new BranchEvent("Downloaded_Workout"));
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Completed Downloading Program" } });
                    }

                }
                else
                {
                    GoBtn.Text = "DOWNLOAD MY WORKOUT";
                    ProgressBar.IsVisible = false;
                    totalDownloadedBytes.IsVisible = false;
                    await DisplayAlert("Try Again", "There was an issue. Please try again later.", "OK");
                    GoBtn.IsEnabled = true;
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Downloading Error" } });
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                int i = 0;
                GoBtn.Text = "DOWNLOAD MY WORKOUT";
                ProgressBar.IsVisible = false;
                totalDownloadedBytes.IsVisible = false;
                await DisplayAlert("Try Again", "There was a small issue. Please try again later.", "OK");
                GoBtn.IsEnabled = true;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "DownloadProgramDay()" } });
            }
            _DownloadingInProgress = false;

        }

        private async Task<bool> CheckIfDownloadWasSuccessful(int ContactProgramDayId, IFolder folder)
        {
            bool isSuccessFul = true;

            var ActionContent_Review = await _connection.Table<LocalDBActionContentV2>().Where(x => x.LastDownloadAttempt != (DateTime?)null && x.ContactProgramDayId == ContactProgramDayId && (x.ContentTypeValue == 585860000 || x.ContentTypeValue == 585860001 || x.ContentTypeValue == 585860003)).ToListAsync();

            //ActionContent_Review = ActionContent_Review.Where(x => x.LastDownloadAttempt >= DateTime.UtcNow.AddMinutes(-5)).ToList();
            foreach (var ac in ActionContent_Review)
            {
                try
                {
                    if (ac.PhotoFilePath == "" || ac.VideoFilePath == "" || ac.PhotoFilePath == null || ac.VideoFilePath == null)
                    {
                        isSuccessFul = false;
                        break;
                    }
                    else
                    {
                        IFile photoFile = await folder.GetFileAsync(_rootFilePath + ac.PhotoFilePath);
                        IFile videoFile = await folder.GetFileAsync(_rootFilePath + ac.VideoFilePath);
                        int photoLength = await Task.Run(() => ReadMediaLength(photoFile));
                        int videoLength = await Task.Run(() => ReadMediaLength(videoFile));

                        if (photoLength == 0 || videoLength == 0)
                        {
                            isSuccessFul = false;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "CheckIfDownloadWasSuccessful_AC" } });
                    isSuccessFul = false;
                    break;
                }
            }

            if (isSuccessFul)
            {
                var AudioContent_Review = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.LastDownloadAttempt != (DateTime?)null && x.ContactProgramDayId == ContactProgramDayId).ToListAsync();

                //AudioContent_Review = AudioContent_Review.Where(x => x.LastDownloadAttempt >= DateTime.UtcNow.AddMinutes(-5)).ToList();
                foreach (var auc in AudioContent_Review)
                {
                    try
                    {
                        if (auc.AudioFilePath == "" || auc.AudioFilePath == null)
                        {
                            //Redownload
                            isSuccessFul = false;
                            break;
                        }
                        else
                        {
                            IFile audioFile = await folder.GetFileAsync(_rootFilePath + auc.AudioFilePath);
                            int audioLength = await Task.Run(() => ReadMediaLength(audioFile));

                            if (audioLength == 0)
                            {
                                //Redownload
                                isSuccessFul = false;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = ex.ToString();
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "CheckIfDownloadWasSuccessful_AuC" } });
                        isSuccessFul = false;
                        break;
                    }
                }
            }

            if (!isSuccessFul)
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Download Failed" } });
            }

            return isSuccessFul;
        }

        private async Task<int> ReadMediaLength(IFile file)
        {
            long Length = 0;
            try
            {
                using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read))
                {
                    Length = stream.Length;
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ReadMediaLength" } });
            }
            return unchecked((int)Length);
        }

        private async Task ProcessAudioDownloads(List<Task<DownloadResultAudio>> downloadAudio, double incrementDownload)
        {
            while (downloadAudio.Count > 0)
            {
                Task<DownloadResultAudio> finishedTask = await Task.WhenAny(downloadAudio.ToArray());
                downloadAudio.Remove(finishedTask);
                var AudioContent = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.GuidCRM == finishedTask.Result.AudioContentGuid).FirstOrDefaultAsync();

                if (AudioContent != null)
                {
                    AudioContent.AudioFilePath = finishedTask.Result.audioFilePath;
                    AudioContent.LastDownloadedOn = DateTime.UtcNow;
                    await _connection.UpdateAsync(AudioContent);
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ProgressBar.ProgressTo(ProgressBar.Progress + incrementDownload, 1, Easing.Linear);
                });
            }
        }

        private async Task ProcessVideoDownloads(List<Task<DownloadResult>> downloads, double incrementDownload)
        {
            while (downloads.Count > 0)
            {
                Task<DownloadResult> finishedTask = await Task.WhenAny(downloads.ToArray());
                downloads.Remove(finishedTask);
                var ActionContent = await _connection.Table<LocalDBActionContentV2>().Where(x => x.GuidCRM == finishedTask.Result.ActionContentGuid).FirstOrDefaultAsync();

                if (ActionContent != null) //If this is null then the photo and video will be blank, unlikely because the action shows up on the workout (i think this means that the action content must be present as they are dependent)
                {
                    ActionContent.PhotoFilePath = finishedTask.Result.photoFilePath;
                    ActionContent.VideoFilePath = finishedTask.Result.videoFilePath;
                    ActionContent.LastDownloadedOn = DateTime.UtcNow;
                    await _connection.UpdateAsync(ActionContent);
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ProgressBar.ProgressTo(ProgressBar.Progress + incrementDownload, 1, Easing.Linear);
                });
            }
        }

        private async Task<DownloadResult> DownloadWithUrlTrackingTaskAsync(string photoFileName, string videoFileName, string photoUrl, string videoUrl, Guid actionContentGuid, Guid _ContactProgramDayGuid)
        {
            DownloadResult result = new DownloadResult();
            result.ActionContentGuid = actionContentGuid;
            Task<string> t1 = Task.Run(() => DownloadPhoto(photoFileName, photoUrl, _ContactProgramDayGuid));
            Task<string> t2 = Task.Run(() => DownloadVideo(videoFileName, videoUrl, _ContactProgramDayGuid));
            await Task.WhenAll(t1, t2);
            result.photoFilePath = t1.Result;
            result.videoFilePath = t2.Result;
            return result;
        }

        private async Task<DownloadResultAudio> DownloadAudioWithUrlTrackingTaskAsync(string audioFileName, string audioUrl, Guid audioContentGuid, Guid _ContactProgramDayGuid)
        {
            DownloadResultAudio result = new DownloadResultAudio();
            result.AudioContentGuid = audioContentGuid;
            result.audioFilePath = await DownloadAudio(audioFileName, audioUrl, _ContactProgramDayGuid);
            return result;
        }

        private static async Task<string> DownloadVideo(string videoFileName, string videoUrl, Guid _ContactProgramDayGuid)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
            IFolder subFolder = await folder.CreateFolderAsync(_ContactProgramDayGuid.ToString(), CreationCollisionOption.OpenIfExists);
            IFolder subSubFolder = await subFolder.CreateFolderAsync("Videos", CreationCollisionOption.OpenIfExists);
            IFile file = await subSubFolder.CreateFileAsync(videoFileName, CreationCollisionOption.ReplaceExisting);
            //_videoFilePath = file.Path.Replace(rootFolder.Path, "");
            var videoFilePath = file.Path.Replace(rootFolder.Path, "");

            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                var uri = new Uri(videoUrl);

                using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    _totalBytes += int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (Stream streamToWriteTo = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo); //If this doesn't work will it not show the video and download bytes are 0
                        }
                    }
                }
            }

            return videoFilePath;

        }

        private static async Task<string> DownloadPhoto(string photoFileName, string photoUrl, Guid _ContactProgramDayGuid)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
            IFolder subFolder = await folder.CreateFolderAsync(_ContactProgramDayGuid.ToString(), CreationCollisionOption.OpenIfExists);
            IFolder subSubFolder = await subFolder.CreateFolderAsync("Photos", CreationCollisionOption.OpenIfExists);
            IFile file = await subSubFolder.CreateFileAsync(photoFileName, CreationCollisionOption.ReplaceExisting);
            // _photoFilePath = file.Path.Replace(rootFolder.Path, "");
            var photoFilePath = file.Path.Replace(rootFolder.Path, "");

            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                var uri = new Uri(photoUrl);

                using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    _totalBytes += int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (Stream streamToWriteTo = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }
                }
            }

            return photoFilePath;
        }

        private static async Task<string> DownloadAudio(string audioFileName, string audioUrl, Guid _ContactProgramDayGuid)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
            IFolder subFolder = await folder.CreateFolderAsync(_ContactProgramDayGuid.ToString(), CreationCollisionOption.OpenIfExists);
            IFolder subSubFolder = await subFolder.CreateFolderAsync("Audio", CreationCollisionOption.OpenIfExists);
            IFile file = await subSubFolder.CreateFileAsync(audioFileName, CreationCollisionOption.ReplaceExisting);
            // _photoFilePath = file.Path.Replace(rootFolder.Path, "");
            var audioFilePath = file.Path.Replace(rootFolder.Path, "");

            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                var uri = new Uri(audioUrl);

                using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    _totalBytes += int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (Stream streamToWriteTo = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }
                }
            }

            return audioFilePath;
        }

        private async Task AnimateIn(int num)
        {
            if (_deactiveTopNav)
                return;

            uint mypalnsAnimateTime = 0;
            uint todayAnimateTime = 0;
            uint myworkoutsAnimateTime = 0;

            Easing myplansEasing = Easing.CubicIn;
            Easing todayEasing = Easing.CubicIn;
            Easing myworkoutsEasing = Easing.CubicIn;

            double myplansOpacity = 0D;
            double todayOpcacity = 0D;
            double myworkoutsOpacity = 0D;

            if (num == 0)
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "My Plans" } });
                //MyPlans_ToolBarItem.BackgroundColor = Color.FromHex("#7e7e7e");
                MyPlans_Text_ToolBarItem.TextColor = Color.FromHex("#3E8DB2");
                MyPlans_Line_ToolBarItem.IsVisible = true;

                //Today_ToolBarItem.BackgroundColor = Color.White;
                Today_Text_ToolBarItem.TextColor = Color.FromHex("#7e7e7e");
                Today_Line_ToolBarItem.IsVisible = false;

                //Schedule_ToolBarItem.BackgroundColor = Color.White;
                Schedule_Text_ToolBarItem.TextColor = Color.FromHex("#7e7e7e");
                Schedule_Line_ToolBarItem.IsVisible = false;

                //myplansEasing = Easing.CubicOut;
                myplansOpacity = 1D;
                mypalnsAnimateTime = 500;
                MyPlansSL.IsVisible = true;
                TodaySL.IsVisible = false;
                ScheduleSL.IsVisible = false;
            }
            else if (num == 1)
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Today" } });
                //Today_ToolBarItem.BackgroundColor = Color.FromHex("#007077");
                Today_Text_ToolBarItem.TextColor = Color.FromHex("#3E8DB2");
                Today_Line_ToolBarItem.IsVisible = true;

                //MyPlans_ToolBarItem.BackgroundColor = Color.White;
                MyPlans_Text_ToolBarItem.TextColor = Color.FromHex("#7e7e7e");
                MyPlans_Line_ToolBarItem.IsVisible = false;

                //Schedule_ToolBarItem.BackgroundColor = Color.White;
                Schedule_Text_ToolBarItem.TextColor = Color.FromHex("#7e7e7e");
                Schedule_Line_ToolBarItem.IsVisible = false;

                //todayEasing = Easing.CubicOut;
                todayOpcacity = 1D;
                todayAnimateTime = 500;
                TodaySL.IsVisible = true;
                MyPlansSL.IsVisible = false;
                ScheduleSL.IsVisible = false;
            }
            else
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Schedule" } });
                //Schedule_ToolBarItem.BackgroundColor = Color.FromHex("#007077");
                Schedule_Text_ToolBarItem.TextColor = Color.FromHex("#3E8DB2");
                Schedule_Line_ToolBarItem.IsVisible = true;

                //Today_ToolBarItem.BackgroundColor = Color.White;
                Today_Text_ToolBarItem.TextColor = Color.FromHex("#7e7e7e");
                Today_Line_ToolBarItem.IsVisible = false;

                //MyPlans_ToolBarItem.BackgroundColor = Color.White;
                MyPlans_Text_ToolBarItem.TextColor = Color.FromHex("#7e7e7e");
                MyPlans_Line_ToolBarItem.IsVisible = false;

                //myworkoutsEasing = Easing.CubicOut;
                myworkoutsOpacity = 1D;
                myworkoutsAnimateTime = 500;
                ScheduleSL.IsVisible = true;
                MyPlansSL.IsVisible = false;
                TodaySL.IsVisible = false;

                if (_todayScheduleCell.cell != null)
                {

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await ScheduleScrollView.ScrollToAsync(_todayScheduleCell.cell.mainSL, ScrollToPosition.Center, false);
                        }
                        catch (Exception ex1)
                        {
                            try
                            {
                                await Task.Delay(1000);
                                await ScheduleScrollView.ScrollToAsync(_todayScheduleCell.cell.mainSL, ScrollToPosition.Center, false);
                            }
                            catch (Exception ex)
                            {
                                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "AnimateIn" } });
                            }
                        }
                    });

                }
            }

            await Task.WhenAll(
                MyPlansSL.FadeTo(myplansOpacity, mypalnsAnimateTime, myplansEasing),
                TodaySL.FadeTo(todayOpcacity, todayAnimateTime, todayEasing),
                ScheduleSL.FadeTo(myworkoutsOpacity, myworkoutsAnimateTime, myworkoutsEasing)
                );

        }

        private async void ProfileBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new PersonalProfile());
        }

        private async void MyPlansBtn_Clicked()
        {
            if (!MyPlansSL.IsVisible)
            {
                await AnimateIn(0);
            }
        }

        private async void TodayBtn_Clicked()
        {
            if (!TodaySL.IsVisible)
            {
                await AnimateIn(1);
            }
        }

        private async void ScheduleBtn_Clicked()
        {
            if (!ScheduleSL.IsVisible)
            {
                //CloseExerciseVideo(_openExerciseCellInfo);
                // await Task.Delay(200);
                //SetupProgramSchedule();
                //SetupActions();
                await AnimateIn(2);

            }

        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            ShowOrHideNoNetwork();
        }

        private void HandleConnectivityTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
        {
            ShowOrHideNoNetwork();
        }

    }
}
