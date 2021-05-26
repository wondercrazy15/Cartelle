using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stance.Pages.Sub
{
    public partial class ContactPrograms : BaseContentPage
    {
        private LocalDBProgram _Program = new LocalDBProgram();
        private const string _PageName = "Contact Programs";
        private Guid _ContactProgramGuid = Guid.Empty;
        private Button _resetScheduleBtn = new Button();
        private LocalDBContactProgram _ActiveCP = new LocalDBContactProgram();

        public ContactPrograms(LocalDBProgram Program)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            MessagingCenter.Subscribe<ProgramOverview_v1>(this, "GoToWorkoutTab", (sender) => { GoToWorkoutTab(); });

            _Program = Program;
            Title = Program.Heading;
            ShowContactPrograms();
        }

        private async void GoToWorkoutTab()
        {
            try
            {
                await Navigation.PopAsync();
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                var err = ex.ToString();
            }
        }

        private async void ShowContactPrograms()
        {
            listOfContactProgram.Children.Clear();
            var ContactPrograms = await _connection.Table<LocalDBContactProgram>().Where(x => x.ProgramId == _Program.Id).ToListAsync();

            var ContactPrograms_Complete = ContactPrograms.Where(x => x.StatusCodeValue == 585860004 || x.StatusCodeValue == 585860001).ToList();
            var ContactPrograms_InactiveNotComplete = ContactPrograms.Where(x => x.StatusCodeValue != 585860004 && x.StateCodeValue == 1).ToList();
            var ContactPrograms_Active = ContactPrograms.Where(x => x.StateCodeValue == 0 && x.StatusCodeValue != 585860001).FirstOrDefault();

            foreach (var cp in ContactPrograms_Complete)
            {
                AddContactProgram(cp);
            }

            foreach (var cp in ContactPrograms_InactiveNotComplete)
            {
                AddContactProgram(cp);
            }

            if (ContactPrograms_Active != null)
            {
                AddContactProgram(ContactPrograms_Active);

                var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ContactPrograms_Active.Id && x.IsRepeat != true).ToListAsync();
                if (ContactProgramDays.Count() == ContactProgramDays.Where(x => x.StatusCodeValue == 585860004).ToList().Count() && ContactProgramDays.Count() != 0)
                {
                    ShowRepeatProgramBtn();
                } else if (ContactPrograms_Active.IsScheduleBuilt)
                {
                    var lastDate = ContactProgramDays.Where(x => x.ScheduledStartDate != null).OrderByDescending(x => x.ScheduledStartDate).Select(x => x.ScheduledStartDate).FirstOrDefault();

                    if(lastDate != null)
                    {
                        if(lastDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                        {
                            ShowRepeatProgramBtn();
                        }
                    }
                }
            }
            else if (ContactPrograms_InactiveNotComplete.Count() == 0)
            {
                ShowRepeatProgramBtn();
            }            

            var mainSL = new StackLayout
            {
                BackgroundColor = Color.FromHex("#c7c7c7"),
                Spacing = 0,
                HeightRequest = 1,
            };

            listOfContactProgram.Children.Add(mainSL);

        }

        private async void RepeatProgramBtn_Clicked(object sender, EventArgs e)
        {
            if (!IsInternetConnected())
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again.", "OK");
                return;
            }            //case 1 - contact program is active and complete (days done - in progress)
            //case 2 - contact program is inactive all completed
            RepeatBtn.IsEnabled = false;
            RepeatBtn.Text = "... ONE MOMENT";
            bool canRepeat = false;
            bool activeProgram = false;

            var ContactPrograms = await _connection.Table<LocalDBContactProgram>().Where(x => x.ProgramId == _Program.Id).ToListAsync();
            var ContactPrograms_InactiveNotComplete = ContactPrograms.Where(x => x.StatusCodeValue != 585860004 && x.StateCodeValue == 1).ToList(); //paused or not started
            var ContactPrograms_Active = ContactPrograms.Where(x => x.StateCodeValue == 0 && x.StatusCodeValue != 585860001).FirstOrDefault();

            if (ContactPrograms_InactiveNotComplete.Count() > 0)
            {
                await DisplayAlert("Cannot Repeat", "Activate your program instead", "OK");
                RepeatBtn.Text = "CANNOT REPEAT";
                return;
            }

            if (ContactPrograms_Active != null)
            {
                var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == ContactPrograms_Active.Id && x.IsRepeat != true).ToListAsync();
                if (ContactProgramDays.Count() == ContactProgramDays.Where(x => x.StatusCodeValue == 585860004).ToList().Count() && ContactProgramDays.Count() != 0)
                {
                    _ContactProgramGuid = ContactPrograms_Active.GuidCRM;
                    canRepeat = true;
                    activeProgram = true;
                }
            }
            else if (ContactPrograms_InactiveNotComplete.Count() == 0)
            {
                var cp = ContactPrograms.FirstOrDefault();
                if (cp != null)
                {
                    _ContactProgramGuid = cp.GuidCRM;
                    canRepeat = true;
                }
            }

            //Call repeat program api
            //this will create a new contact program and just add it in here as not started and reload the list
            if (canRepeat)
            {
                try
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Repeat Program" } });
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
                    }
                    else
                    {
                        await DisplayAlert("Error", "Something went wrong. Try again later.", "OK");
                        RepeatBtn.IsEnabled = true;
                        RepeatBtn.Text = "REPEAT PROGRAM";
                    }

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "RepeatProgramBtn_Clicked" } });
                    var error = ex.ToString();
                    await DisplayAlert("Error", "Something went wrong.", "OK");
                    RepeatBtn.IsEnabled = true;
                    RepeatBtn.Text = "REPEAT PROGRAM";
                }

            }
            else
            {
                await DisplayAlert("Cannot Repeat", "Cannot Repeat your program.", "OK");
                RepeatBtn.Text = "CANNOT REPEAT";
            }

        }

        private void ShowRepeatProgramBtn()
        {
            RepeatProgramSL.IsVisible = true;
        }

        private void AddContactProgram(LocalDBContactProgram contactProgram)
        {
            var mainSL = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Spacing = 0,
                HeightRequest = 70,
                Padding = new Thickness(10, 5, 5, 5)
            };

            var SecondarySL = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                //BackgroundColor = Color.Blue
            };

            mainSL.Children.Add(SecondarySL);

            var arrowSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
                //BackgroundColor = Color.Aqua
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

            string Status = "";

            var ContactProgramDays = _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == contactProgram.Id && x.IsRepeat != true).ToListAsync().Result;
            var lastDate = ContactProgramDays.Where(x => x.ScheduledStartDate != null).OrderByDescending(x => x.ScheduledStartDate).Select(x => x.ScheduledStartDate).FirstOrDefault();

            int numberCompleteCPDs = ContactProgramDays.Where(x => x.StatusCodeValue == 585860004).ToList().Count();

            if (contactProgram.StatusCodeValue == 2)
            {
                Status = "NOT STARTED";
                mainSL.BackgroundColor = Color.White;
            }
            else if (contactProgram.StatusCodeValue == 585860002)
            {
                Status = "PAUSED";
                mainSL.BackgroundColor = Color.FromHex("#e9e9e9");
            }
            else if (contactProgram.StatusCodeValue == 585860001 || contactProgram.StatusCodeValue == 585860004 || (ContactProgramDays.Count() != 0 && ContactProgramDays.Count() == ContactProgramDays.Where(x => x.StatusCodeValue == 585860004).ToList().Count()))
            {
                Status = "COMPLETED";
                mainSL.BackgroundColor = Color.FromHex("#e5f2e5");
            } else if(contactProgram.StatusCodeValue == 585860003)
            {
                Status = "ENDED";
                mainSL.BackgroundColor = Color.FromHex("#e9e9e9");
            }
            else if (contactProgram.IsScheduleBuilt)
            {
                if (lastDate != null)
                {
                    if (lastDate?.ToLocalTime().Date < DateTime.UtcNow.ToLocalTime().Date)
                    {
                        Status = "COMPLETED";
                        mainSL.BackgroundColor = Color.FromHex("#e5f2e5");
                    }
                }
            }
            else if (contactProgram.StatusCodeValue == 585860000)
            {
                Status = "IN PROGRESS";
                mainSL.BackgroundColor = Color.FromHex("#e5f8f9");
            }
            else if (contactProgram.StatusCodeValue == 1)
            {
                Status = "ACTIVE";
                mainSL.BackgroundColor = Color.FromHex("#e5f8f9");
            }

            if (Status == "" && contactProgram.StateCodeValue == 0)
            {
                Status = "IN PROGRESS";
                mainSL.BackgroundColor = Color.FromHex("#e5f8f9");
            }

            var heading = new Label
            {
                Text = Status,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
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
                //BackgroundColor = Color.Lime
            };

            headingSL.Children.Add(heading);
            SecondarySL.Children.Add(headingSL);

            var bottomSL = new StackLayout
            {
                Spacing = 0,
                Padding = new Thickness(0, 5, 0, 5),
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Color.Orange,
            };

            SecondarySL.Children.Add(bottomSL);

            if (contactProgram.StartDate.HasValue)
            {
                if (contactProgram.StartDate != (DateTime?)null)
                {
                    var startSL = new StackLayout
                    {
                        Spacing = 0,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        WidthRequest = 120,
                        // BackgroundColor = Color.Pink
                    };

                    var startDateLabel = new Label
                    {
                        Text = contactProgram.StartDate?.ToLocalTime().Date.ToString("MMM d, yyyy"),
                        FontSize = 13,
                        FontFamily = "HelveticalNeue-Bold",
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold,
                    };

                    var startLabel = new Label
                    {
                        Text = "START",
                        FontSize = 11,
                        FontFamily = "HelveticalNeue-Bold",
                        HorizontalTextAlignment = TextAlignment.Center,
                    };

                    startSL.Children.Add(startDateLabel);
                    startSL.Children.Add(startLabel);
                    bottomSL.Children.Add(startSL);
                }
            }

            if (contactProgram.EndDate.HasValue && (Status == "COMPLETED" || Status == "ENDED")) //complete
            {
                if (contactProgram.EndDate != (DateTime?)null)
                {
                    var endSL = new StackLayout
                    {
                        Spacing = 0,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        WidthRequest = 120,
                        //BackgroundColor = Color.Brown

                    };

                    var endDateLabel = new Label
                    {
                        Text = contactProgram.EndDate?.ToLocalTime().Date.ToString("MMM d, yyyy"),
                        FontSize = 13,
                        FontFamily = "HelveticalNeue-Bold",
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold,
                    };

                    var endLabel = new Label
                    {
                        Text = "END",
                        FontSize = 11,
                        FontFamily = "HelveticalNeue-Bold",
                        HorizontalTextAlignment = TextAlignment.Center,
                    };

                    endSL.Children.Add(endDateLabel);
                    endSL.Children.Add(endLabel);
                    bottomSL.Children.Add(endSL);

                }
            } else if (Status == "IN PROGRESS" && numberCompleteCPDs >= 1 && (lastDate?.ToLocalTime().Date >= DateTime.UtcNow.ToLocalTime().Date || !contactProgram.IsScheduleBuilt))
            {
                var endSL = new StackLayout
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    WidthRequest = 150,
                    //BackgroundColor = Color.Brown
                };

                var restartProgramBtn = new Button
                {
                    Text = "RESTART PROGRAM",
                    Margin = new Thickness(0,0,30,0),
                    HeightRequest = 30,
                    WidthRequest = 170,
                    FontSize = 11,
                    BorderColor = Color.FromHex("#007077"),
                    CornerRadius = 4,
                    BorderWidth = 1,
                    TextColor = Color.FromHex("#007077"),
                    FontFamily = "PingFangTC-Regular",                    
                };               
                restartProgramBtn.Clicked += RestartProgramBtn_Clicked;
                _resetScheduleBtn = restartProgramBtn;
                _ActiveCP = contactProgram;

                endSL.Children.Add(restartProgramBtn);
                bottomSL.Children.Add(endSL);
            } 
            else if (Status == "NOT STARTED" || Status == "PAUSED")
            {
                var endSL = new StackLayout
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    WidthRequest = 150,
                    //BackgroundColor = Color.Brown
                };                

                var activateBtn = new Button
                {
                    Text = "ACTIVATE",                    
                    HeightRequest = 30,
                    WidthRequest = 150,
                    FontSize = 11,
                    BorderColor = Color.FromHex("#007077"),
                    CornerRadius = 4,
                    BorderWidth = 1,
                    TextColor = Color.FromHex("#007077"),
                    FontFamily = "PingFangTC-Regular",
                };

                if (contactProgram.StartDate.HasValue)
                {
                    endSL.HorizontalOptions = LayoutOptions.EndAndExpand;
                    activateBtn.Margin = new Thickness(0, 0, 30, 0);
                } 

                activateBtn.Clicked += ActivateProgramBtn_Clicked;
                _resetScheduleBtn = activateBtn;
                _ActiveCP = contactProgram;

                endSL.Children.Add(activateBtn);
                bottomSL.Children.Add(endSL);

            }

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                // handle the tap
                ProgramDayCell_Clicked(contactProgram);
            };
            mainSL.GestureRecognizers.Add(tapGestureRecognizer);

            listOfContactProgram.Children.Add(mainSL);
        }

        private async void ActivateProgramBtn_Clicked(object sender, EventArgs e)
        {
            var activateBtn = (Button)sender;
            activateBtn.IsEnabled = false;
            activateBtn.Text = "... ONE MOMENT";
            //ResetScheduleBtn.TextColor = Color.FromHex("#007077");
            activateBtn.BackgroundColor = Color.FromHex("#007077");
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Click Activate Button" } });

            var result = await DisplayAlert("ACTIVATE PROGRAM", "Are you sure that you want to activate this program?", "ACTIVATE", "NO");
            if (!result)
            {
                activateBtn.IsEnabled = true;
                activateBtn.Text = "ACTIVATE";
                activateBtn.BackgroundColor = Color.Transparent;
                return;
            }

            if (!IsInternetConnected())
            {
                await DisplayAlert("NO INTERNET", "Connect to the internet and try again.", "OK");
                activateBtn.IsEnabled = true;
                activateBtn.Text = "ACTIVATE";
                activateBtn.BackgroundColor = Color.Transparent;
                return;
            }

            //call reset schedule here - CP stays in progress and is schedule built changes to no and cpd have the scheduled dates removed, mark a flag for if the schedule was reset and the number of times it occurs
            //update the contact program locally here
            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Activating Program" } });

                HttpResponseMessage response = await WebAPIService.ActivateProgram(_client, _ActiveCP.GuidCRM);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Reset all of the app
                    App.Current.MainPage = new MainStartingPage();
                    return;
                } else
                {
                    await DisplayAlert("ERROR", "We had a small error. Please try again later.", "OK");
                    activateBtn.IsEnabled = true;
                    activateBtn.Text = "ACTIVATE";
                    activateBtn.BackgroundColor = Color.Transparent;
                    return;
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Activate Program Failed" } });
                await DisplayAlert("Error", "Opps we had an error. Try again later.", "OK");
                activateBtn.IsEnabled = true;
                activateBtn.Text = "ACTIVATE";
                activateBtn.BackgroundColor = Color.Transparent;
            }
        }

        private async void RestartProgramBtn_Clicked(object sender, EventArgs e)
        {
            var ResetScheduleBtn = (Button)sender;
            ResetScheduleBtn.IsEnabled = false;
            ResetScheduleBtn.Text = "... ONE MOMENT";
            //ResetScheduleBtn.TextColor = Color.FromHex("#007077");
            ResetScheduleBtn.BackgroundColor = Color.FromHex("#007077");


            var result = await DisplayAlert("RESTART PROGRAM", "Restarting your program will END this program and start a new program.", "RESTART", "NO");
            if (!result)
            {
                ResetScheduleBtn.IsEnabled = true;
                ResetScheduleBtn.Text = "RESTART PROGRAM";
                ResetScheduleBtn.BackgroundColor = Color.Transparent;
                return;
            }

            var result2 = await DisplayAlert("WARNING", "This program will be marked as ENDED and a new program will be started.", "CONFIRM RESTART", "NO");
            if (!result2)
            {
                ResetScheduleBtn.IsEnabled = true;
                ResetScheduleBtn.Text = "RESTART PROGRAM";
                ResetScheduleBtn.BackgroundColor = Color.Transparent;
                return;
            }

            if (!IsInternetConnected())
            {
                await DisplayAlert("NO INTERNET", "Connect to the internet and try again.", "OK");
                ResetScheduleBtn.IsEnabled = true;
                ResetScheduleBtn.Text = "RESTART PROGRAM";
                ResetScheduleBtn.BackgroundColor = Color.Transparent;
                return;
            }

            //call reset schedule here - CP stays in progress and is schedule built changes to no and cpd have the scheduled dates removed, mark a flag for if the schedule was reset and the number of times it occurs
            //update the contact program locally here
            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Restarting Program" } });

                HttpResponseMessage response = await WebAPIService.RestartProgram(_client, _ActiveCP.GuidCRM);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    App.Current.MainPage = new MainStartingPage();
                }
                else
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _Program.Heading }, { "Action", "Restarting Program Failed" } });
                    await DisplayAlert("Error", "Opps we had an error. Try again later.", "OK");
                    ResetScheduleBtn.IsEnabled = true;
                    ResetScheduleBtn.Text = "RESTART PROGRAM";
                    ResetScheduleBtn.BackgroundColor = Color.Transparent;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "RestartProgramBtn_Clicked" } });
                await DisplayAlert("Error", "Opps we had an error. Try reloading your app from the 'More' Tab.", "OK");
                ResetScheduleBtn.IsEnabled = true;
                ResetScheduleBtn.Text = "RESTART PROGRAM";
                ResetScheduleBtn.BackgroundColor = Color.Transparent;
            }
        }

        private async void ProgramDayCell_Clicked(LocalDBContactProgram contactProgram)
        {
            var program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == contactProgram.ProgramId).FirstOrDefaultAsync();
            if (program != null)
            {
                await Navigation.PushAsync(new ProgramOverview_v1(program, contactProgram, "true"));
            }
        }

    }
}