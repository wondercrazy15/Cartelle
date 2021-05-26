using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

using Xamarin.Forms;
using Stance.Models;
using Stance.Pages.Sub;
using Stance.Utils;
using Stance.Models.LocalDB;
using Stance.Utils.Auth;
using Plugin.Connectivity;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Stance.Models.API;
using PCLStorage;
using System.Diagnostics;
using ModernHttpClient;
using Stance.Utils.LocalDB;

namespace Stance.Pages.Main
{
    public partial class MainStartingPage : TabbedPage
    {
        private static SQLiteAsyncConnection _connection;
        private const string _PageName = "main starting page";

        public MainStartingPage(string showLoading = null)
        {
            InitializeComponent();
            // DependencyService.Get<ISQLiteDb>();
            MessagingCenter.Subscribe<ProgramOverview_v1>(this, "GoToWorkoutTab", (sender) => { GoToWorkoutTab(); });
            MessagingCenter.Subscribe<WorkoutSurvey>(this, "WorkoutSurveyComplete", (sender) => { GoToDashboardTab(); });

            //Task.Factory.StartNew(Database.ClearAsync).Wait();

            //Task.Factory.StartNew(Database.CreateAsync).Wait();
            Task.Run(() => Database.CreateAsync()).Wait();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            var PageCollection = new List<BasePageTemplatePage>();
            BasePageTemplatePage AthletesPage = new BasePageTemplatePage();
            BasePageTemplatePage ProfilePage = new BasePageTemplatePage();
            BasePageTemplatePage MorePage = new BasePageTemplatePage();

            if (Auth.IsAuthenticated())
            {
                AthletesPage.page = new Athletes_MainPage();
                AthletesPage.Icon = "Athlete_24px_@2.png";
                AthletesPage.Title = "Athletes";

                ProfilePage.page = new PersonalProfile();
                ProfilePage.Icon = "Profile_24px_@2.png";
                ProfilePage.Title = "Profile";

                MorePage.page = new More();
                MorePage.Icon = "Dots_24px_@2.png";
                MorePage.Title = "More";

                //if (showLoading != null)
                //{
                //    Navigation.PushModalAsync(new InitialLoading(), false).Wait();
                //    MessagingCenter.Send(this, "InitialLoadingFinished");
                //}
                BarTextColor = Color.Black; //.FromHex("#007077");
                //ShowLoadingScreen(PageCollection);

                int contactPrograms = _connection.Table<LocalDBContactProgram>().ToListAsync().Result.Count();
                if (contactPrograms == 0)
                {
                    UnAuth();
                }
                else
                {
                    var Workout = new BasePageTemplatePage
                    {
                        page = new Workout_MainPage(),
                        Icon = "Workout_24px_@2.png",
                        Title = "Workout"
                    };

                    var Dashboard = new BasePageTemplatePage
                    {
                        page = new Progress_MainPage(),
                        Icon = "Dashboard_24px_@2.png",
                        Title = "Dashboard"
                    };

                    PageCollection.Add(Workout);
                    PageCollection.Add(Dashboard);
                    PageCollection.Add(AthletesPage);
                    PageCollection.Add(ProfilePage);
                    PageCollection.Add(MorePage);
                    //PageCollection.Reverse();

                    foreach (var pg in PageCollection)
                    {
                        var navigationPage = new NavigationPage(pg.page)
                        {
                            //BarBackgroundColor = Color.FromHex("#404040"),
                            //BarTextColor = Color.White,
                            BarBackgroundColor = Color.Black,
                            //BarTextColor = Color.Blue // .FromHex("#00bac6"),//#80bd01 green, new color #00bac6 light blue, and darker blue #007077
                        };
                        //BarTextColor = Color.White;
                        //BarBackgroundColor = Color.Black;
                        navigationPage.BarTextColor = Color.White;//.FromHex("#00bac6");
                        var platformType = Xamarin.Forms.Device.RuntimePlatform;
                        if (platformType == "iOS")
                        {
                            navigationPage.Icon = pg.Icon;
                        }
                        navigationPage.Title = pg.Title;
                        Children.Add(navigationPage);
                    }
                }
            }
            else
            {
                UnAuth();
            }
        }

        private void UnAuth()
        {
            Task.Factory.StartNew(Database.ClearAsync).Wait();
            Auth.DeleteCredentials();
            App.Current.MainPage = new SignIn();
        }

        private void GoToDashboardTab()
        {
            CurrentPage = Children[1];
        }

        private void GoToWorkoutTab()
        {
            CurrentPage = Children[0];
        }


    }
}
