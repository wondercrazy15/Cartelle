using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Stance.Models;
using Stance.Pages.Sub;
using Stance.Utils;
using Stance.Models.LocalDB;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using System;
using BranchXamarinSDK;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Stance.Pages.Main
{
    public partial class MainStartingPage : ExtendedTabbedPage, IBranchIdentityInterface
    {
        //private static SQLiteAsyncConnection _connection;
        private const string _PageName = "main starting page";

        public MainStartingPage(string eventName = null, string showLoading = null)
        {
            InitializeComponent();
            MessagingCenter.Subscribe<ProgramOverview_v1>(this, "GoToWorkoutTab", (sender) => { GoToWorkoutTab(); });
            MessagingCenter.Subscribe<WorkoutSurvey>(this, "WorkoutSurveyComplete", (sender) => { GoToDashboardTab(); });
            BackgroundColor = Color.Black;
            Task.Run(() => Database.CreateAsync()).Wait();
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            On<Android>().SetIsSmoothScrollEnabled(false);
            On<Android>().SetIsSwipePagingEnabled(false);

            var PageCollection = new List<BasePageTemplatePage>();
            BasePageTemplatePage AthletesPage = new BasePageTemplatePage();
            BasePageTemplatePage ProgramsPage = new BasePageTemplatePage();
            BasePageTemplatePage MorePage = new BasePageTemplatePage();

            if (Auth.IsAuthenticated())
            {
                try
                {
                    Branch branch = Branch.GetInstance();

                    branch.SetIdentity(Auth.Username.ToLower(), this);
                    

                if (eventName != null)
                {
                    if (eventName == "register")
                    {
                        branch.SendEvent(new BranchEvent(BranchEventType.COMPLETE_REGISTRATION));
                        AppCenter.Start(typeof(Push));
                    }
                    else if (eventName == "login")
                    {
                        branch.SendEvent(new BranchEvent("Login"));
                        AppCenter.Start(typeof(Push));
                    }
                }
            } catch(Exception ex)
                {
                    var err = ex.ToString();
                }

                AthletesPage.page = new Athletes_MainPage();
                AthletesPage.Icon = "Athlete_24px_2.png";
                AthletesPage.Title = "Athletes";

                ProgramsPage.page = new ProgramSearch();
                ProgramsPage.Icon = "Program_24px_2.png";
                ProgramsPage.Title = "Programs";

                MorePage.page = new More();
                MorePage.Icon = "Dots_24px_2.png";
                MorePage.Title = "More";

                //if (showLoading != null)
                //{
                //    Navigation.PushModalAsync(new InitialLoading(), false).Wait();
                //    MessagingCenter.Send(this, "InitialLoadingFinished");
                //}

                //if (Xamarin.Forms.Device.RuntimePlatform == "iOS")
                //{
                    BarTextColor = Color.FromHex("#3E8DB2"); //Color.Black; //.FromHex("#007077");
                                                             //ShowLoadingScreen(PageCollection);
                //}
                //else
                //{
                //    BarTextColor = Color.White;
                    
                //}
                var Workout = new BasePageTemplatePage
                {
                    page = new Workout_MainPage(),
                    Icon = "Workout_24px_2.png",
                    Title = "Workout"
                };

                var Dashboard = new BasePageTemplatePage
                {
                    page = new Progress_MainPage(),
                    Icon = "Dashboard_24px_2.png",
                    Title = "Dashboard"
                };

                PageCollection.Add(Workout);
                PageCollection.Add(Dashboard);
                PageCollection.Add(AthletesPage);
                PageCollection.Add(ProgramsPage);
                PageCollection.Add(MorePage);
                //PageCollection.Reverse();

                foreach (var pg in PageCollection)
                {
                    var navigationPage = new NavigationPage(pg.page)
                    {
                        //BarBackgroundColor = Color.FromHex("#404040"),
                        //BarTextColor = Color.White,
                        BarBackgroundColor = Color.FromHex("#17191A"),
                        //BarTextColor = Color.Blue // .FromHex("#00bac6"),//#80bd01 green, new color #00bac6 light blue, and darker blue #007077
                    };
                    //BarTextColor = Color.White;
                    //BarBackgroundColor = Color.Black;
                    navigationPage.BarTextColor = Color.White;//.FromHex("#00bac6");
                    //var platformType = Xamarin.Forms.Device.RuntimePlatform;
                    //if (platformType == "iOS")
                    //{
                        navigationPage.IconImageSource = pg.Icon;
                    //}
                    
                    navigationPage.Title = pg.Title;
                    Children.Add(navigationPage);
                }
               
            }
            else
            {
                Task.Factory.StartNew(Database.ClearAsync).Wait();
                Auth.DeleteCredentials();
                App.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.FromHex("#17191A"), BarTextColor = Color.White };
                return;
            }
        }

        public void IdentityRequestError(BranchError error)
        {
           
        }

        public void IdentitySet(Dictionary<string, object> data)
        {
            
        }

        public void LogoutComplete()
        {
            throw new NotImplementedException();
        }

        private void GoToDashboardTab()
        {
            CurrentPage = Children[1];
        }

        private void GoToWorkoutTab()
        {
            CurrentPage = Children[0];
            
        }


        //public static Action EmulateBackPressed;

        //private bool AcceptBack;

        //protected override bool OnBackButtonPressed()
        //{
        //    if (AcceptBack)
        //        return false;

        //    PromptForExit();
        //    return true;
        //}

        //private async void PromptForExit()
        //{
        //    if (await DisplayAlert("", "Are you sure you want to exit from Cartelle?", "Yes", "No"))
        //    {
        //        AcceptBack = true;
        //        EmulateBackPressed();
        //    }
        //}


    }
}
