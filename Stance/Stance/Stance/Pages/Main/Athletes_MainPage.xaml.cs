using Plugin.Connectivity;
using Stance.Pages.Sub;
using Stance.Utils;
using System;
using System.Linq;
using Xamarin.Forms;
using Plugin.Connectivity.Abstractions;
using Stance.Models.API;
using System.Collections.ObjectModel;
using Stance.Models.LocalDB;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;

namespace Stance.Pages.Main
{
    public partial class Athletes_MainPage : BaseContentPage
    {
        private const string _PageName = "Athletes Tab";
        private ObservableCollection<APIAccount> _athletes;
        private bool _IsRefreshing = false;
        private bool _SearchChangingState = false;
        private LocalDBAccount _SelectedAccount;

        public Athletes_MainPage()
        {
            InitializeComponent();

            //BindList();
            //NavigationPage.SetTitleIcon(this, Device.OnPlatform(iOS: "cartelle_logo.png", Android: "cartelle_logo.png", WinPhone: ""));
            //NavigationPage.SetHasNavigationBar(this, false);
            //MessagingCenter.Subscribe<ProgramOverview_v1>(this, "GoToWorkoutTab", (sender) => { EnabledListView(); });

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


            NavigationPage.SetTitleIcon(this, Device.OnPlatform(iOS: "Cartelle_112X19_White.png", Android: "Cartelle_112X19_White.png", WinPhone: ""));


            //var apiHelper = new MockApiHelper();
            //listView.ItemsSource = apiHelper.GetAthletes();

            if (Device.Idiom == TargetIdiom.Phone)
            {
                listView.RowHeight = 250;//170
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                listView.RowHeight = 600;
            }
            else
            {
                listView.RowHeight = 350;
            }

            listView.HeightRequest = 1000;
            //Task task = new Task(BindList);
            //task.Start();
            //task.Wait();

            BindList();

        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            //MessagingCenter.Send(this, "GoToWorkoutTab_Call2");
            base.OnAppearing();
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

                        var ContactProfile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();

                        if (ContactProfile != null)
                        {
                            if (ContactProfile.IsAdmin)
                            {
                                //Insert Admin condition here for which programs show up
                                listView.ItemsSource = await _connection.Table<LocalDBAccount>().OrderBy(x => x.SequenceNumber).ToListAsync();//All
                            }
                            else
                            {
                                listView.ItemsSource = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();//Publsihed
                            }
                        }
                        else
                        {
                            listView.ItemsSource = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();//Publsihed
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "GetAtheltesList()" } });
                        // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("NO INTERNET", "You are not connected to the internet. Connect to the internet and try again.", "OK");
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

            var Accounts = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000).OrderBy(x => x.SequenceNumber).ToListAsync();

            if (Accounts.Count() > 0)
            {
                listView.ItemsSource = Accounts;
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
                GetAtheltesList();
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
