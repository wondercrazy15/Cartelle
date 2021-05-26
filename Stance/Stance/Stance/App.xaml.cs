using System;
using Stance.Pages.Main;
using Xamarin.Forms;
using Stance.Utils.Auth;
using System.Threading.Tasks;
using Stance.Pages.Sub;
using Stance.Utils.LocalDB;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using System.Collections.Generic;

namespace Stance
{
    public partial class App : Application
    {
        public static string _Mode = "prod";
        public static string AppName = "Cartelle";
        public static Uri _absoluteUri;
        public static string _AppCenterConn;

        public const string _contactsUri = "api/v1/contacts";
        public const string _resetpasswordUri = "api/v1/contacts/resetpassword?email=";
        public const string _dashboardUri = "api/v1/dashboard";

        public const string _programsUri = "api/v1/programs";
        public const string _programDaysGetActiveProgramDaysUri = "api/v1/programdays/getactive?acpid=";
        public const string _transactionsUri = "api/v1/transactions?ProgramGuid=";

        public const string _contactProgramDaysGuidUri = "api/v1/contactprogramdays/getcpd?ContactProgramDayGuid=";
        public const string _contactProgramDaysUri = "api/v1/contactprogramdays";
        public const string _contactProgramDaysScheduleUri = "api/v1/contactprogramdays/schedule?ContactProgramGuid=";

        public const string _audioProgramDayUri = "api/v1/audio?ProgramDayGuid=";

        public const string _syncCPD = "api/v1/synccpd";
        public const string _syncCA = "api/v1/syncca";
        public const string _message = "api/v1/message";

        public const string _actionsProgramDayUri = "api/v2/actions?ProgramDayGuid=";
        public const string _athletesUri = "api/v2/athletes";
        public const string _programsAccountGuidUri = "api/v2/programs?AccountGuid=";
        public const string _programDaysProgramGuidUri = "api/v2/programdays/getprogramday?ProgramGuid=";

        public const string _scheduleProgramDayUri = "api/v3/schedule?ContactProgramGuid=";
        public const string _rescheduleCPD = "api/v3/schedule/modify";

        public const string _download = "api/v4/download?CPDGuid=";
        public const string _initialLoad = "api/v4/initialload/";
        public const string _repeatProgram = "api/v4/repeatprogram?CPGuid=";
        public const string _resetSchedule = "api/v4/resetschedule?CPGuid=";
        public const string _syncNewPlans = "api/v4/syncplans";
        public const string _contactProgramsActivateUri = "api/v4/contactprograms?CPGuid=";
        public const string _profileUri = "api/v4/profile";
        public const string _restartProgram = "api/v4/restartprogram?CPGuid=";

        public App()
        {            
            InitializeComponent();

            if(_Mode == "prod")
            {
                _absoluteUri = new Uri("https://cartelleapi.azurewebsites.net");
                _AppCenterConn = "ios=5b7a471f-8583-4260-9c7f-cce7bfa676d5;" + "uwp={Your UWP App secret here};" + "android=c021a846-e1a5-4196-a6d1-93a5e39d7f48;";
            } else if (_Mode == "staging")
            {
                _absoluteUri = new Uri("https://cartelleapi-staging.azurewebsites.net");
                _AppCenterConn = "ios=6cddd95e-4f80-4d1d-a8d9-f6eec976dce6;" + "uwp={Your UWP App secret here};" + "android=662e2286-58ae-4892-bdb9-595ed6db6bf3;";
            }
            else //assume dev
            {
                _absoluteUri = new Uri("https://cartelleapi-dev.azurewebsites.net");
                _AppCenterConn = "ios=6cddd95e-4f80-4d1d-a8d9-f6eec976dce6;" + "uwp={Your UWP App secret here};" + "android=662e2286-58ae-4892-bdb9-595ed6db6bf3;";
            }
            //Task tw = Task.Factory.StartNew(Database.ClearAsync);
            //tw.Wait();      
            //Auth.DeleteCredentials();      
            Task.Factory.StartNew(Database.CreateAsync).Wait();
            //Task.Factory.StartNew(Database.UpdateCPDs).Wait();
            //Task.Factory.StartNew(Database.UpdateACs).Wait();
            //Task.Factory.StartNew(Database.UpdateProfile).Wait();

            Task t1 = Task.Factory.StartNew(Database.UpdateCPDs);
            Task t2 = Task.Factory.StartNew(Database.UpdateACs);
            Task t3 = Task.Factory.StartNew(Database.UpdateProfile);
            Task t4 = Task.Factory.StartNew(Database.UpdateAuCs);
            Task.WaitAll(t1, t2, t3, t4);

            if (Auth.IsAuthenticated())
            {
                MainPage = new MainStartingPage("yes");
            }
            else
            {
                MainPage = new SignIn();
            }
        }

        protected override void OnStart()
        {
            
            if (!AppCenter.Configured)
            {
                Push.PushNotificationReceived += (sender, e) =>
                {
                    // Add the notification message and title to the message
                    var summary = $"Push notification received:" +
                                        $"\n\tNotification title: {e.Title}" +
                                        $"\n\tMessage: {e.Message}";

                    // If there is custom data associated with the notification,
                    // print the entries
                    if (e.CustomData != null)
                    {
                        summary += "\n\tCustom data:\n";
                        foreach (var key in e.CustomData.Keys)
                        {
                            summary += $"\t\t{key} : {e.CustomData[key]}\n";
                        }
                    }

                    // Send the notification summary to debug output
                    System.Diagnostics.Debug.WriteLine(summary);
                };
            }

            //AppCenter.LogLevel = LogLevel.Verbose; //logs to output window
            AppCenter.Start(_AppCenterConn, typeof(Analytics), typeof(Crashes), typeof(Push));
            Analytics.TrackEvent(AppName, new Dictionary<string, string>() { { "Action", "OnStart" } });

            // Handle when your app starts            
            base.OnStart();
            //App.Current.MainPage = new TestingAppMethods("OnStart");
            //CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            //App.Current.MainPage = new MainStartingPage();
        }

        //void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        //{
        //    Type currentPage = this.MainPage.GetType();
        //    if (e.IsConnected && currentPage != typeof(NetworkViewPage))
        //        this.MainPage = new NetworkViewPage();
        //    else if (!e.IsConnected && currentPage != typeof(NoNetworkPage))
        //        this.MainPage = new NoNetworkPage();
        //}

        protected override void OnSleep()
        {
            Analytics.TrackEvent(AppName, new Dictionary<string, string>() { { "Action", "OnSleep" } });
            // Handle when your app sleeps
            //App.Current.MainPage = new TestingAppMethods("OnSleep");
            MessagingCenter.Send(this, "OnSleep");
        }

        protected override void OnResume()
        {
            Analytics.TrackEvent(AppName, new Dictionary<string, string>() { { "Action", "OnResume" } });
            // Handle when your app resumes
            //App.Current.MainPage = new TestingAppMethods("OnResume");
            //App.Current.MainPage = new MainStartingPage(); //removed so app does not pop to starting page, but now must load program day OnAppear in workout tab
            MessagingCenter.Send(this, "OnResume");
        }       


    }
}
