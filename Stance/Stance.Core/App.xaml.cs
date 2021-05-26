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
using Stance.Models.LocalDB;
using Stance.Utils;
using System.Linq;
using Xamarin.Essentials;
using BranchXamarinSDK;
using Stance.ViewModels;

namespace Stance
{
    public partial class App : Application, IBranchBUOSessionInterface
    {
        public static string _Mode = "prod";

        public static ContactSignUpV2 _contactSignUp = new ContactSignUpV2();
        public static string AppVersion = VersionTracking.CurrentVersion;
        public static string AppName = AppInfo.Name;
        public static Uri _absoluteUri;
        public static string _AppCenterConn;
        public static int screenWidth;
        public static int screenHeight;
        //public const string _productId_Monthly = "com.bethestance.Stance.Memberships.Monthly";
        //public const string _productId_Quarterly = "com.bethestance.Stance.Memberships.Quarterly";
        //public const string _productId_Yearly = "com.bethestance.Stance.Memberships.Yearly";
        //public const string _productId_Yearly_NoTrial = "com.bethestance.Stance.MembershipsNoTrial.Yearly";
        //public const string _productId_Monthly_NoTrial = "com.bethestance.Stance.MembershipsNoTrial.Monthly";

        public const string _productId_Monthly = "android.test.purchased";
        public const string _productId_Quarterly = "com.bethestance.stance.memberships.quarterly";
        public const string _productId_Yearly = "com.bethestance.stance.memberships.yearly";
        public const string _productId_Yearly_NoTrial = "com.bethestance.stance.membershipsnotrial.yearly";
        public const string _productId_Monthly_NoTrial = "com.bethestance.stance.membershipsnotrial.monthly";

        //Subscription
        //public const string _productId_Monthly = "com.bethestance.stance.memberships.monthly.subscription";
        //public const string _productId_Quarterly = "com.bethestance.stance.memberships.quarterly.subscription";
        //public const string _productId_Yearly = "com.bethestance.stance.memberships.yearly.subscription";
        //public const string _productId_Yearly_NoTrial = "com.bethestance.stance.membershipsnotrial.yearly";
        //public const string _productId_Monthly_NoTrial = "com.bethestance.stance.membershipsnotrial.monthly";


        public const string _resetpasswordUri = "api/v1/contacts/resetpassword/?email=";
        public const string _dashboardUri = "api/v1/dashboard/";
        public const string _programsUri = "api/v1/programs/";
        public const string _programDaysGetActiveProgramDaysUri = "api/v1/programdays/getactive/?acpid=";
        public const string _transactionsUri = "api/v1/transactions/?ProgramGuid=";
        public const string _contactProgramDaysGuidUri = "api/v1/contactprogramdays/getcpd/?ContactProgramDayGuid=";
        public const string _contactProgramDaysUri = "api/v1/contactprogramdays/";
        public const string _contactProgramDaysScheduleUri = "api/v1/contactprogramdays/schedule/?ContactProgramGuid=";
        public const string _audioProgramDayUri = "api/v1/audio/?ProgramDayGuid=";

        public const string _syncCA = "api/v1/syncca/";
        public const string _message = "api/v1/message/";

        public const string _actionsProgramDayUri = "api/v2/actions/?ProgramDayGuid=";
        public const string _programDaysProgramGuidUri = "api/v2/programdays/getprogramday/?ProgramGuid=";

        public const string _rescheduleCPD = "api/v3/schedule/modify/";

        public const string _resetSchedule = "api/v4/resetschedule/?CPGuid=";
        public const string _syncNewPlans = "api/v4/syncplans/";

        public const string _syncCPD = "api/v5/synccpd/";
        public const string _scheduleProgramDayUri = "api/v5/schedule/build/?CPGuid=";

        public const string _programsAccountGuidUri = "api/v6/programs/?AccountGuid=";
        public const string _addToMyPlans = "api/v6/addplan/?ProgramGuid=";
        public const string _repeatProgram = "api/v6/repeatprogram/?CPGuid=";
        public const string _restartProgram = "api/v6/restartprogram/?CPGuid=";
        public const string _contactProgramsActivateUri = "api/v6/contactprograms/?CPGuid=";

        public const string _programsAll = "api/v7/programs/";
        public const string _subscription = "api/v7/subscription/";
        public const string _download = "api/v7/download/?CPDGuid=";
        public const string _profileUri = "api/v7/profile/";
        public const string _resendConfirmEmailUri = "api/v7/contacts/resendconfirmemail/"; 
        public const string _checkConfirmEmailUri = "api/v7/contacts/checkconfirmemail/";
        public const string _recommendedUri = "api/v7/contacts/recommendedby/?IsRecommended=";
        public const string _setTrialProgramUri = "api/v7/contacts/settrialprogram/";
        public const string _athletesUri = "api/v7/athletes/";

        public const string _signUpProcessUri = "api/v9/signupprocess/";
        public const string _initialLoad = "api/v9/initialload/?IsLogin=";
        public const string _syncNewAPIUri = "api/v9/iap/syncnewiap/";

        public const string _athleteReferralIUri = "api/v10/signupprocess/athletereferral/?accountCode=";
        public const string _contactsUri = "api/v10/signupprocess/signupform/";

        public App()
        {

            InitializeComponent();
            Xamarin.Forms.Device.SetFlags(new string[] { "MediaElement_Experimental" });
            MessagingCenter.Subscribe<Workout_MainPage, string>(this, "Height", (sender, arg) => {
                var d = double.Parse(arg);
            });
            if (_Mode == "prod")
            {
                _absoluteUri = new Uri("https://cartelleapi.azurewebsites.net");
                _AppCenterConn = "ios=5b7a471f-8583-4260-9c7f-cce7bfa676d5;" + "uwp={Your UWP App secret here};" + "android=c021a846-e1a5-4196-a6d1-93a5e39d7f48;";
            }
            else if (_Mode == "staging")
            {
                _absoluteUri = new Uri("https://cartelleapi-staging.azurewebsites.net");
                _AppCenterConn = "ios=6cddd95e-4f80-4d1d-a8d9-f6eec976dce6;" + "uwp={Your UWP App secret here};" + "android=662e2286-58ae-4892-bdb9-595ed6db6bf3;";
            }
            else //assume dev
            {
                _absoluteUri = new Uri("https://cartelleapi-dev.azurewebsites.net");
                _AppCenterConn = "ios=6cddd95e-4f80-4d1d-a8d9-f6eec976dce6;" + "uwp={Your UWP App secret here};" + "android=662e2286-58ae-4892-bdb9-595ed6db6bf3;";
            }
    
            Task.Factory.StartNew(Database.CreateAsync).Wait();

            var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            int contactPrograms = 0;

            try
            {
                contactPrograms = _connection.Table<LocalDBContactProgram>().ToListAsync().Result.Count();
            }
            catch (Exception ex)
            {
                var err = ex.ToString();

                try
                {
                    contactPrograms = _connection.Table<LocalDBContactProgram>().ToListAsync().Result.Count();
                }
                catch (Exception ex2)
                {
                    var err2 = ex2.ToString();
                }
            }

            //MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.FromHex("#17191A"), BarTextColor = Color.White };
            //return;

            if (Auth.IsAuthenticated() && contactPrograms != 0)
            {
                MainPage = new MainStartingPage(null,"yes");
                //MainPage = new RecommendedBy();
            }
            else
            {
                //MainPage = new MainStartingPage(null, "yes");

                //send to home page
                Analytics.TrackEvent(AppName, new Dictionary<string, string>() { { "Action", "Send To Home" } });
                Task tw = Task.Run(() => Database.ClearAsync());
                tw.Wait();
                Auth.DeleteCredentials();

                MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.FromHex("#17191A"), BarTextColor = Color.White };
                //MainPage = new RecommendedBy();
            }

           

        }

        #region IBranchSessionInterface implementation


        public void SessionRequestError(BranchError error)
        {
        }

        public void InitSessionComplete(BranchUniversalObject buo, BranchLinkProperties blp)
        {
            try
            {
                if (!Auth.IsAuthenticated())
                {
                    Branch branch = Branch.GetInstance();
                    Dictionary<string, object> sessionParams_init = branch.GetFirstReferringParams();
                    Dictionary<string, object> sessionParams = branch.GetLastReferringParams();

                    //if link is for Athlete, show Athlete screen
                    //if link is for Cartelle, show normal sign-up process

                    var referralPool = sessionParams.Where(x => x.Key == "ReferralPool").Select(x => x.Value).FirstOrDefault(); //cartelle or athlete
                    var referrerCode = sessionParams.Where(x => x.Key == "ReferrerCode").Select(x => x.Value).FirstOrDefault();
                    var leadSource = sessionParams.Where(x => x.Key == "LeadSource").Select(x => x.Value).FirstOrDefault();
                    var ad_name = sessionParams.Where(x => x.Key == "~ad_name").Select(x => x.Value).FirstOrDefault();

                    if (referralPool == null)
                    {
                        referralPool = sessionParams_init.Where(x => x.Key == "ReferralPool").Select(x => x.Value).FirstOrDefault(); //cartelle or athlete
                    }
                    if (referrerCode == null)
                    {
                        referrerCode = sessionParams_init.Where(x => x.Key == "ReferrerCode").Select(x => x.Value).FirstOrDefault(); //cartelle or athlete
                    }
                    if(leadSource == null && ad_name == null)
                    {
                        leadSource = sessionParams_init.Where(x => x.Key == "LeadSource").Select(x => x.Value).FirstOrDefault(); 
                    } 

                    if (referralPool != null)
                    {
                        _contactSignUp.ReferralPool = referralPool.ToString().ToLower();
                    }
                    if(referrerCode != null)
                    {
                        _contactSignUp.ReferrerCode = referrerCode.ToString().ToLower();
                    }

                    if(ad_name != null)
                    {
                        _contactSignUp.LeadSource = ad_name.ToString();
                    }
                    else if (leadSource != null)
                    {
                        _contactSignUp.LeadSource = leadSource.ToString().ToLower();
                    }

                    MessagingCenter.Send(this, "BranchLoadMessage");

                    //this.MainPage = new IAP(Guid.Empty);

                    //foreach (var item in sessionParams.ToList())
                    //{
                    //    System.Diagnostics.Debug.WriteLine("Print Key/Value:");
                    //    System.Diagnostics.Debug.WriteLine(item.Key.ToString());
                    //    System.Diagnostics.Debug.WriteLine(item.Value.ToString());
                    //}

                }
            } catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", "App" }, { "Function", "InitSessionComplete" } });
            }

        }
  
        #endregion

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
                    //System.Diagnostics.Debug.WriteLine(summary);
                };
            }

            //AppCenter.LogLevel = LogLevel.Verbose; //logs to output window
            AppCenter.Start(_AppCenterConn, typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent(AppName, new Dictionary<string, string>() { { "Action", "OnStart" } });

            var firstLaunch = VersionTracking.IsFirstLaunchEver;
            if (firstLaunch)
            {
                //Send this to facebook pixel - this is done automatically on install but did not work well with facebook out of the box
                //DependencyService.Get<IFacebookEvent>().AppInstall();
                Analytics.TrackEvent(AppName, new Dictionary<string, string>() { { "Action", "Install" } });
            }
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
            try
            {
                Analytics.TrackEvent(AppName, new Dictionary<string, string>() { { "Action", "OnResume" } });
                // Handle when your app resumes
                //App.Current.MainPage = new TestingAppMethods("OnResume");
                //App.Current.MainPage = new MainStartingPage(); //removed so app does not pop to starting page, but now must load program day OnAppear in workout tab
                MessagingCenter.Send(this, "OnResume");
            }
            catch (Exception ex)
            {

            }
            
        }       
         

    }
}
