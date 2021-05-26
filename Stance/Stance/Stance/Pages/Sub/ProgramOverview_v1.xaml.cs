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

            if(contactProgram != null)
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
            programName.Text = program.Heading;

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
            _videoUrl = program.VideoUrl;
            //_cost = program.Cost;
            _programId = program.GuidCRM.ToString();
            _ProgramGuid = program.GuidCRM;

            if (Device.Idiom == TargetIdiom.Phone)
            {
                MainImage.HeightRequest = 475;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                MainImage.HeightRequest = 950;
                LearnMoreBtnIPad.Padding = new Thickness(80, 350, 80, 10);
            }
            else
            {
                MainImage.HeightRequest = 475;
            }
            PurchaseBtn.IsVisible = false;
            GetProgramDays();
        }
    
        private async void SampleBtn_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("COMING SOON", "Sorry, there are no trials at this time.", "OK");

            //await Navigation.PushModalAsync(new LoginSignUpPage(), false);
        }

        private async void WatchVideoBtn_Clicked()
        {
            if (_ContactProgramState != "LEARN MORE")
            {
                if (_ContactProgramState == "WORKOUT")//Active
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Click Workout Button" } } );
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
                    var result = await DisplayAlert("ACTIVATE PROGRAM", "Are you sure that you want to activate this program?", "ACTIVATE", "NO");
                    if (result)
                    {
                        if (!IsInternetConnected())
                        {
                            await DisplayAlert("NO INTERNET", "Connect to the internet and try again.", "OK");
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
                                await DisplayAlert("ERROR", "We had a small error. Please try again later.", "OK");
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

        private async void ViewScheduleBtn_Clicked(object sender, EventArgs e)
        {
            //ViewScheduleBtn.BackgroundColor = Color.FromHex("#00bac6");
            //await GetProgramDays();
            //await Navigation.PushAsync(new ProgramSchedule(_ProgramDays, 1));
            //ViewScheduleBtn.BackgroundColor = Color.Black;
        }

        private void AddWeekSeprator(int num)
        {
            var weekSL = new StackLayout
            {
                Spacing = 0,
                HeightRequest = 35,
                BackgroundColor = Color.FromHex("#007077"),
            };

            var weekLabel = new Label
            {
                Text = "WEEK " + num.ToString(),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            weekSL.Children.Add(weekLabel);

            //var tapGestureRecognizer = new TapGestureRecognizer();
            //tapGestureRecognizer.Tapped += (s, e) =>
            //{
            //    // handle the tap
            //    WeekSelected(weekSL, num);
            //};

            //weekSL.GestureRecognizers.Add(tapGestureRecognizer);

            listOfProgramDays.Children.Add(weekSL);
        }

        private void AddWorkoutDay(LocalDBProgramDay programDay, int dayNumber)
        {
            var mainSL = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Spacing = 0,
                HeightRequest = 70,
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
                Text = "DAY",
                FontSize = 15,
                FontFamily = "PingFangTC-Regular",
                HorizontalTextAlignment = TextAlignment.Center
            };

            var dayNum = new Label
            {
                Text = dayNumber.ToString(),
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "AvenirNextCondensed-Medium",
                HorizontalTextAlignment = TextAlignment.Center
            };

            daySL.Children.Add(dayLabel);
            daySL.Children.Add(dayNum);

            var SecondarySL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                //BackgroundColor = Color.Blue
            };

            mainSL.Children.Add(SecondarySL);

            var arrowSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
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
                Text = programDay.Heading.ToUpper(),
                FontSize = 15,
                //FontAttributes = FontAttributes.Bold,
                FontFamily = "AvenirNextCondensed",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0) //20,5,0,0
            };

            var headingSL = new StackLayout
            {
                //BackgroundColor = Color.Red,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Spacing = 0,
                // BackgroundColor = Color.Lime
            };

            headingSL.Children.Add(heading);
            SecondarySL.Children.Add(headingSL);

            var bottomSL = new StackLayout
            {
                Spacing = 0,
                Padding = new Thickness(0, 5, 0, 5),
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Color.Orange
            };

            SecondarySL.Children.Add(bottomSL);

            var exerciseSL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = 90,
            };

            var exerciseNumLabel = new Label
            {
                Text = programDay.TotalExercises.ToString(),
                FontSize = 13,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var exerciseLabel = new Label
            {
                Text = "Exercises",
                FontSize = 11,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
            };

            exerciseSL.Children.Add(exerciseNumLabel);
            exerciseSL.Children.Add(exerciseLabel);
            bottomSL.Children.Add(exerciseSL);

            var timeSL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 60,
            };

            var timeNumLabel = new Label
            {
                Text = programDay.TimeMinutes.ToString(),
                FontSize = 13,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var timeLabel = new Label
            {
                Text = "Minutes",
                FontSize = 11,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
            };

            timeSL.Children.Add(timeNumLabel);
            timeSL.Children.Add(timeLabel);
            bottomSL.Children.Add(timeSL);

            var levelSL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = 105,
            };

            var levelNumLabel = new Label
            {
                Text = programDay.Level,
                FontSize = 13,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var levelLabel = new Label
            {
                Text = "Level",
                FontSize = 11,
                FontFamily = "HelveticalNeue-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
            };

            levelSL.Children.Add(levelNumLabel);
            levelSL.Children.Add(levelLabel);
            bottomSL.Children.Add(levelSL);

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
                HeightRequest = 45,
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
                Text = "DAY",
                FontSize = 15,
                FontFamily = "PingFangTC-Regular",
                HorizontalTextAlignment = TextAlignment.Center
            };

            var dayNum = new Label
            {
                Text = dayNumber.ToString(),
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "AvenirNextCondensed-Medium",
                HorizontalTextAlignment = TextAlignment.Center
            };

            daySL.Children.Add(dayLabel);
            daySL.Children.Add(dayNum);

            var SecondarySL = new StackLayout
            {
                //BackgroundColor = Color.Yellow,
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                //BackgroundColor = Color.Lime
                Margin = new Thickness(-35, 0, 0, 0)
            };

            mainSL.Children.Add(SecondarySL);

            var heading = new Label
            {
                Text = programDay.Heading.ToUpper(),
                FontSize = 15,
                //FontAttributes = FontAttributes.Bold,
                FontFamily = "AvenirNextCondensed",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            var headingSL = new StackLayout
            {

                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Spacing = 0,
                // BackgroundColor = Color.Blue
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

                    if(lastDate != null)
                    {
                        if(lastDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
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

            if(_ContactProgram == null)
            {
                var ContactPrograms = await _connection.Table<LocalDBContactProgram>().Where(x => x.ProgramId == Program.Id).ToListAsync();
                if (ContactPrograms.Count() != 0)
                {
                    var ContactProgram_Active = ContactPrograms.Where(x => x.StateCodeValue == 0).FirstOrDefault();
                    if(ContactProgram_Active != null)
                    {
                        DetermineBtnText(ContactProgram_Active);
                    }
                    else
                    {
                        var ContactProgram_Inactive = ContactPrograms.Where(x => x.StateCodeValue == 1).FirstOrDefault();
                        if(ContactProgram_Inactive != null)
                        {
                            DetermineBtnText(ContactProgram_Inactive);
                        }
                    }                   
                } else
                {
                    DetermineBtnText(_ContactProgram);
                }
            } else
            {
                DetermineBtnText(_ContactProgram);
            }                    

            var ProgramDays = await _connection.Table<LocalDBProgramDay>().Where(x => x.ProgramId == Program.Id).OrderBy(x => x.SequenceNumber).ToListAsync();
            if (ProgramDays.Count() > 0)
            {
                SetupProgramDays(ProgramDays);
            } else
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
