using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using StanceWeb.Models.App.Optimized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class SignIn : BaseContentPage
    {
        private const string _PageName = "Sign In";
        public string _programId = String.Empty;
        public Guid _programGuid = Guid.Empty;
        private bool _IsSigninIn = false;

        public SignIn()
        {
            InitializeComponent();
            Spinner.IsVisible = false;
            //Task.Run(() => Database.CreateAsync()).Wait();
            Task.Factory.StartNew(Database.ClearAsync).Wait();
            Task.Factory.StartNew(Database.CreateAsync).Wait();
            //Database.ClearAsync();
            //Database.CreateAsync();
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            if (Device.Idiom == TargetIdiom.Phone)
            {
                SignInBox.Padding = new Thickness(40, 60, 40, 60);
                Welcome.FontSize = 21;
                cartelleLogo.WidthRequest = 180;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                SignInBox.Padding = new Thickness(200, 160, 200, 60);
                Welcome.FontSize = 27;
                cartelleLogo.WidthRequest = 240;
            }
            else
            {
                SignInBox.Padding = new Thickness(40, 60, 40, 60);
                Welcome.FontSize = 21;
                cartelleLogo.WidthRequest = 180;
            }
        }

        private async void SignInSubmitted()
        {
            if (!_IsSigninIn)
            {
                _IsSigninIn = true;
                Spinner.IsVisible = true;
                FormValidationSpecialMessage.Text = "";
                Email_SignIn.IsEnabled = false;
                Password_SignIn.IsEnabled = false;

                if (Email_SignIn.Text == null || Email_SignIn.Text == "" || Password_SignIn.Text == null || Password_SignIn.Text == "")
                {
                    Spinner.IsVisible = false;
                    FormValidationSpecialMessage.Text = "Enter your Email and Password";
                    Email_SignIn.IsEnabled = true;
                    Password_SignIn.IsEnabled = true;
                }
                else
                {
                    if (!IsInternetConnected())
                    {
                        SignInBtn.Text = "SIGN IN";
                        Spinner.IsVisible = false;
                        FormValidationSpecialMessage.Text = "";
                        Email_SignIn.IsEnabled = true;
                        Password_SignIn.IsEnabled = true;
                        await DisplayAlert("NO INTERNET", "Please connect to the internet and try again.", "OK");
                        return;
                    }

                    try
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sign In Attempt" } });

                        SignInBtn.Text = "SIGNING IN...";
                        var response = await WebAPIService.InitialLoad(_client, Email_SignIn.Text, Password_SignIn.Text);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            HttpContent content = response.Content;
                            var json = await content.ReadAsStringAsync();
                            var signInResponse = JsonConvert.DeserializeObject<SignInResponseV2>(json);

                            if (signInResponse.ContactPrograms.Count() == 0)
                            {
                                Spinner.IsVisible = false;
                                SignInBtn.Text = "SIGN IN";
                                FormValidationSpecialMessage.Text = "You do not have a Program";
                                Email_SignIn.IsEnabled = true;
                                Password_SignIn.IsEnabled = true;
                            }
                            else
                            {
                                // If good
                                //SignInTitle.IsVisible = false;
                                Spinner.IsVisible = false;
                                FormValidationSpecialMessage.Text = "LOADING...YOUR PROFILE";
                                FormValidationSpecialMessage.TextColor = Color.White;
                                FormValidationSpecialMessage.FontSize = 14;
                                //FormValidationSpecialMessage.FontAttributes = FontAttributes.Bold;
                                //Store user credentials in app 

                                if (signInResponse.Profile != null)
                                {
                                    var profile = new LocalDBContactV2();
                                    profile.GuidCRM = signInResponse.Profile.GuidCRM;
                                    profile.StateCodeValue = signInResponse.Profile.StateCodeValue;
                                    profile.StatusCodeValue = signInResponse.Profile.StatusCodeValue;
                                    profile.FirstName = signInResponse.Profile.FirstName;
                                    profile.LastName = signInResponse.Profile.LastName;
                                    profile.Email = signInResponse.Profile.Email;
                                    profile.IsAdmin = signInResponse.Profile.IsAdmin;
                                    profile.InstagramHandle = signInResponse.Profile.InstagramHandle;
                                    profile.Birthday = (DateTime?)signInResponse.Profile.Birthday;
                                    profile.GenderTypeCode = signInResponse.Profile.GenderTypeCode;
                                    profile.Gender = signInResponse.Profile.Gender;
                                    profile.HeightCm = signInResponse.Profile.HeightCm;
                                    profile.WeightLbs = signInResponse.Profile.WeightLbs;
                                    profile.TrainingGoalTypeCode = signInResponse.Profile.TrainingGoalTypeCode;
                                    profile.TrainingGoal = signInResponse.Profile.TrainingGoal;
                                    profile.RegionTypeCode = signInResponse.Profile.RegionTypeCode;
                                    profile.Region = signInResponse.Profile.Region;
                                    await _connection.InsertAsync(profile);
                                }

                                if (signInResponse.ContactPrograms.Count() > 0)
                                {
                                    foreach (var ContactProgram in signInResponse.ContactPrograms)
                                    {
                                        var newProgram = new LocalDBProgram
                                        {
                                            GuidCRM = ContactProgram.Program.GuidCRM,
                                            Heading = ContactProgram.Program.Heading,
                                            SubHeading = ContactProgram.Program.SubHeading,
                                            PhotoUrl = ContactProgram.Program.PhotoUrl,
                                            SecondaryPhotoUrl = ContactProgram.Program.SecondaryPhotoUrl,
                                            VideoUrl = ContactProgram.Program.VideoUrl,
                                            SequenceNumber = ContactProgram.Program.SequenceNumber,
                                            TotalWeeks = ContactProgram.Program.TotalWeeks,
                                            GoalValue = ContactProgram.Program.GoalValue,
                                            Goal = ContactProgram.Program.Goal,
                                            LevelValue = ContactProgram.Program.LevelValue,
                                            Level = ContactProgram.Program.Level,
                                        };
                                        await _connection.InsertAsync(newProgram);
                                        var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefaultAsync();

                                        var newContactProgram = new LocalDBContactProgram
                                        {
                                            GuidCRM = ContactProgram.GuidCRM,
                                            StateCodeValue = ContactProgram.StateCodeValue,
                                            StatusCodeValue = ContactProgram.StatusCodeValue,
                                            StartDate = (DateTime?)ContactProgram.StartDate,
                                            EndDate = (DateTime?)ContactProgram.EndDate,
                                            IsDaysCreated = ContactProgram.IsDaysCreated,
                                            IsScheduleBuilt = ContactProgram.IsScheduleBuilt,
                                            ProgramId = Program.Id,
                                        };
                                        await _connection.InsertAsync(newContactProgram);
                                    }
                                }

                                if (signInResponse.ContactProgramDays.ContactProgramDays.Count() > 0)
                                {
                                    var Program1 = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == signInResponse.ContactProgramDays.ProgramGuid).FirstOrDefaultAsync();
                                    var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == signInResponse.ContactProgramDays.ContactProgramGuid).FirstOrDefaultAsync();

                                    foreach (var cpd in signInResponse.ContactProgramDays.ContactProgramDays)
                                    {
                                        var newProgramDay = new LocalDBProgramDay
                                        {
                                            GuidCRM = cpd.ProgramDay.GuidCRM,
                                            Heading = cpd.ProgramDay.Heading,
                                            SubHeading = cpd.ProgramDay.SubHeading,
                                            PhotoUrl = cpd.ProgramDay.PhotoUrl,
                                            SequenceNumber = cpd.ProgramDay.SequenceNumber,
                                            TotalExercises = cpd.ProgramDay.TotalExercises,
                                            TimeMinutes = cpd.ProgramDay.TimeMinutes,
                                            LevelValue = cpd.ProgramDay.LevelValue,
                                            Level = cpd.ProgramDay.Level,
                                            DayTypeValue = cpd.ProgramDay.DayTypeValue,
                                            DayType = cpd.ProgramDay.DayType,
                                            ProgramId = Program1.Id,
                                        };
                                        await _connection.InsertAsync(newProgramDay);
                                        var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == cpd.ProgramDay.GuidCRM).FirstOrDefaultAsync();

                                        var newContactProgramDay = new LocalDBContactProgramDayV2
                                        {
                                            GuidCRM = cpd.GuidCRM,
                                            StateCodeValue = cpd.StateCodeValue,
                                            StatusCodeValue = cpd.StatusCodeValue,
                                            Synced = cpd.Synced,
                                            SequenceNumber = cpd.SequenceNumber,
                                            DayTypeValue = cpd.DayTypeValue,
                                            ScheduledStartDate = (DateTime?)cpd.ScheduledStartDate,
                                            ActualStartDate = (DateTime?)cpd.ActualStartDate,
                                            ContactProgramId = ContactProgram.Id,
                                            ProgramDayId = ProgramDay.Id,
                                            ReceivedOn = DateTime.UtcNow,
                                        };
                                        await _connection.InsertAsync(newContactProgramDay);
                                    }
                                }

                                // If good
                                //SignInTitle.IsVisible = false;
                                FormValidationSpecialMessage.Text = "SUCCESS";
                                //FormValidationSpecialMessage.FontAttributes = FontAttributes.Bold;
                                //Store user credentials in app 
                                Auth.SaveCredentials(Email_SignIn.Text, Password_SignIn.Text);
                                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sign In Success" } });
                                App.Current.MainPage = new MainStartingPage();
                                return;
                            }
                        }
                        else
                        {
                            //If Bad
                            Spinner.IsVisible = false;
                            SignInBtn.Text = "SIGN IN";
                            FormValidationSpecialMessage.Text = "Email or Password is invalid";
                            Email_SignIn.IsEnabled = true;
                            Password_SignIn.IsEnabled = true;
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Sign In Failure" } });
                        }
                    }
                    catch (Exception ex)
                    {
                        // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                        SignInBtn.Text = "SIGN IN";
                        Spinner.IsVisible = false;
                        FormValidationSpecialMessage.Text = "Something went wrong, try again later";
                        Email_SignIn.IsEnabled = true;
                        Password_SignIn.IsEnabled = true;
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SignInSubmitted()" } });
                    }
                }
                _IsSigninIn = false;
            }

        }

        private async void ResetPassword_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ResetPassword());
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }
    }
}
