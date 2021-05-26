using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Pages.Sub;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Pages.Main
{
    public partial class Athletes_MainPage : BaseContentPage
    {
        private const string _PageName = "Athletes Tab";
        private ObservableCollection<APIAccount> _athletes;
        private bool _IsRefreshing = false;
        private bool _SearchChangingState = false;
        private LocalDBAccount _SelectedAccount;
        private bool _IsFirstLoad = true;
        private bool _IsFirstLoad_List = true;


        public Athletes_MainPage()
        {
            InitializeComponent();

            //BindList();
            //NavigationPage.SetTitleIcon(this, Device.OnPlatform(iOS: "cartelle_logo.png", Android: "cartelle_logo.png", WinPhone: ""));
            //NavigationPage.SetHasNavigationBar(this, false);
            //MessagingCenter.Subscribe<ProgramOverview_v1>(this, "GoToWorkoutTab", (sender) => { EnabledListView(); });
            MessagingCenter.Subscribe<WorkoutSurvey>(this, "WorkoutSurveyComplete", (sender) => { SyncToCRM(); });
            MessagingCenter.Subscribe<More>(this, "LoadFixScrollIssue", (sender) => { LoadFixScrollIssue(); });

            NavigationPage.SetBackButtonTitle(this, "");
            SearchBar.IsVisible = false;
            NoResultsStackLayout.IsVisible = false;

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
            //    Priority = 1
            //};

            //toolItem1.Clicked += (s, e) =>
            //{
            //    ProfileBtn_Clicked(s, e);
            //};

            var toolItem2 = new ToolbarItem
            {
                Icon = "search_16.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };

            toolItem2.Clicked += (s, e) =>
            {
                SearchBtn_Clicked(s, e);
            };


            //ToolbarItems.Add(toolItem1);
            ToolbarItems.Add(toolItem2);

            //NavigationPage.SetTitleIcon(this, "Cartelle_112X19_White.png");

            //var apiHelper = new MockApiHelper();
            //listView.ItemsSource = apiHelper.GetAthletes();

            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily)
            {
                var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                if (isDeviceIphoneXorXS)
                {
                    listView.RowHeight = 300;
                }
                else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                {
                    listView.RowHeight = 325;
                } else
                {
                    listView.RowHeight = 300;
                }
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                if (isDeviceIphonePlus)
                {
                    listView.RowHeight = 320;
                }
                else
                {
                    listView.RowHeight = 280;
                }
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                listView.RowHeight = 750;
            }
            else
            {
                listView.RowHeight = 350;
            }

            //listView.HeightRequest = 1000;
            BindList();
        }

        protected async override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            //MessagingCenter.Send(this, "GoToWorkoutTab_Call2");
            base.OnAppearing();

            // *STANDARD MASSIVE KLUGE* Needed to get initial selection to show.
            //LocalDBAccount m = listView.SelectedItem as LocalDBAccount;
            //listView.SelectedItem = null;
            //listView.SelectedItem = m;

            if (_IsFirstLoad_List)
            {
                listView.BeginRefresh(); //this fixes the scroll lock issue
                _IsFirstLoad_List = false;
                //await Task.Delay(1500);
                listView.EndRefresh();
            }
        }

        private async void SyncToCRM()
        {
            if (IsInternetConnected())
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "SyncToCRM" } });
                await WebAPIService.SyncToCRM(_client);
            }
        }

        private async void GetAtheltesList()
        {
            if (!_IsRefreshing)
            {
                _IsRefreshing = true;

                if (!listView.IsRefreshing)
                {
                    listView.BeginRefresh();
                }

                if (IsInternetConnected())
                {
                    try
                    {
                        await WebAPIService.GetAthletes(_client);
                        ShowAccounts();
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "GetAtheltesList()" } });
                        // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("No Internet", "You are not connected to the internet. Connect to the internet and try again.", "OK");
                }
                if (listView.IsRefreshing)
                {
                    listView.EndRefresh();
                }
            }

            _IsRefreshing = false;
        }

        private async void BindList()
        {
            if (!listView.IsRefreshing)
            {
                listView.BeginRefresh();
            }
            //NotLoadedYet.IsVisible = true;

            var Accounts = await _connection.Table<LocalDBAccount>().ToListAsync();
            if (Accounts.Count() > 0)
            {
                ShowAccounts();
                //NotLoadedYet.IsVisible = false;
                if (listView.IsRefreshing)
                {
                    listView.EndRefresh();
                }
            }
            else
            {
                GetAtheltesList();
                //NotLoadedYet.IsVisible = false;
            }

        }

        private async void LoadFixScrollIssue()
        {
            //await Task.Delay(3000);

            //listView.MinimumHeightRequest = 1000;
            //await Task.Delay(1000);

           // listView.Focus();

        }

        private async void ShowAccounts()
        {
            List<LocalDBAccount> myList = new List<LocalDBAccount>();
             var ContactProfile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();
            if (ContactProfile != null)
            {
                if (ContactProfile.IsAdmin)
                {
                   myList = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000 || x.StatusCodeValue == 866660001).OrderBy(x => x.SequenceNumber).ToListAsync();//Publsihed and App Testing
                    listView.ItemsSource = myList;
                }
                else
                {
                    myList = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();//Publsihed
                    listView.ItemsSource = myList;
                }
            }
            else
            {
               myList = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();//Publsihed
                listView.ItemsSource = myList;
            }

            try
            {
                if(myList.Count() > 0)
                {
                    listView.IsEnabled = true;
                    listView.ScrollTo(myList[0], ScrollToPosition.Start, true);
                }

            } catch(Exception ex)
            {
                var st = ex.ToString();
            }
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

        private async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            var athlete = e.SelectedItem as LocalDBAccount;
            if (athlete != null)
            {
                _SelectedAccount = athlete;
                MainContentStackLayout.IsEnabled = false;
                await Navigation.PushAsync(new AthleteOverview_v1(athlete));
                MainContentStackLayout.IsEnabled = true;
            }
            listView.SelectedItem = null;

        }

        private void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //var athlete = e.Item as LocalDBAccount;
            //if (athlete != null)
            //{
            //    listView.SelectedItem = null;
            //    MainContentStackLayout.IsEnabled = false;
            //    await Navigation.PushAsync(new AthleteOverview_v1(athlete));
            //    MainContentStackLayout.IsEnabled = true;
            //}

        }

        private void Handle_Refreshing(object sender, System.EventArgs e)
        {
            //var apiHelper = new MockApiHelper();
            //listView.ItemsSource = apiHelper.GetAthletes();
            if (SearchBar.IsVisible == false)
            {
                if (_IsFirstLoad)
                {
                    _IsFirstLoad = false;
                }
                else
                {
                    GetAtheltesList();
                }
            }
            else if (listView.IsRefreshing)
            {
                listView.EndRefresh();
            }
            //await Task.Delay(2000);

        }

        private async void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //var apiHelper = new MockApiHelper();            
            //var searchResults = apiHelper.GetAthletes(e.NewTextValue);
            try
            {
                if (e.NewTextValue == null)
                {
                    NoResultsStackLayout.IsVisible = false;
                    listView.ItemsSource = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();
                    return;
                }
                if (SearchBar.Text != String.Empty)
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Searching" } });
                }
                var searchResults1 = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();
                if (searchResults1.Count() == 0)
                {
                    NoResultsStackLayout.IsVisible = false;
                    listView.ItemsSource = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();
                    return;
                }

                var searchResults = searchResults1.Where(x => x.Heading.ToLower().StartsWith(e.NewTextValue.ToLower())).ToList();                  
                listView.ItemsSource = searchResults;

                if (searchResults.Count() > 0)
                {
                    NoResultsStackLayout.IsVisible = false;
                }
                else if (e.NewTextValue != null)
                {
                    NoResultsStackLayout.IsVisible = true;
                }
                else
                {
                    NoResultsStackLayout.IsVisible = false;
                    listView.ItemsSource = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();
                }
            } catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Handle_TextChanged" } });
            }
        }

        async void ProfileBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new PersonalProfile());
        }

        private async void SearchBtn_Clicked(object sender, System.EventArgs e)
        {
            if (!_SearchChangingState)
            {
                _SearchChangingState = true;

                if (SearchBar.IsVisible == false)
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Open Search" } });
                    SearchBar.Opacity = 0;
                    SearchBar.IsVisible = true;
                    await SearchBar.FadeTo(1, 300, Easing.CubicOut);
                }
                else
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Close Search" } });
                    await SearchBar.FadeTo(0, 300, Easing.CubicIn);
                    SearchBar.IsVisible = false;
                    SearchBar.Text = String.Empty;
                    var searchResults = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();
                }

                _SearchChangingState = false;
            }

        }


    }
}
