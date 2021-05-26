using Microsoft.AppCenter.Analytics;
using Stance.Models.LocalDB;
using Stance.Utils;
using Stance.Utils.Auth;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class PersonalProfile : BaseContentPage
    {
        private const string _PageName = "Profile Tab";
        private const string _DefaultEmptyProfileFieldText = "Not Indicated";

        public PersonalProfile()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<EditPersonalProfile>(this, "UpdatedProfile", (sender) => { DisplaySaveSuccess(); });

            if (Auth.IsAuthenticated())
            {
                // TableSL.Padding = new Thickness(0, 30, 0, -50);
                TableViewMain.HasUnevenRows = true;
                TableViewMain.HeightRequest = 2600;
                //Email.Text = Auth.Username;
                AuthBtn.Text = "   SIGN OUT   ";
                AuthBtn.IsVisible = false;
                AuthSL.IsVisible = false;
            }
            else
            {
                ProfileInfo.IsVisible = false;
                AuthBtn.Text = "   JOIN or SIGN IN   ";
            }

            var tapGestureRecognizer = new TapGestureRecognizer
            {
                Command = new Command(EditBtn_Clicked),
            };
            EditSL.GestureRecognizers.Add(tapGestureRecognizer);


            if (!Auth.IsAuthenticated())
            {
                UserImg.HeightRequest = 250;
                UserImg.WidthRequest = 250;
                //UserImg.Source = (FileImageSource)ImageSource.FromFile("user@3x.png");
            }


            if (Device.Idiom == TargetIdiom.Phone)
            {
                FillerSL.HeightRequest = 0;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                FillerSL.HeightRequest = 100;
                ProfileInfo.Padding = new Thickness(0, 100, 0, 0);
            }
            else
            {
                FillerSL.HeightRequest = 0;
            }

            GetProfileData();

        }

        private void DisplaySaveSuccess()
        {
            GetProfileData();
        }

        private async void GetProfileData()
        {
            var Profile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();

            if (Profile != null)
            {
                Email.Detail = Profile.Email;
                FirstName.Detail = Profile.FirstName;
                LastName.Detail = Profile.LastName;
                InstagramHandle.Detail = Profile.InstagramHandle;

                if (Profile.Birthday != null)
                {
                    Birthdate.Detail = Profile.Birthday?.ToString("d MMM yyyy");
                }
                else
                {
                    Birthdate.Detail = _DefaultEmptyProfileFieldText;
                }
                if (Profile.Gender != "" && Profile.Gender != null)
                {
                    Gender.Detail = Profile.Gender;
                }
                else
                {
                    Gender.Detail = _DefaultEmptyProfileFieldText;
                }
                //if (Profile.TimeZone != "" && Profile.TimeZone != null)
                //{
                //    TimeZone.Detail = Profile.TimeZone;
                //}
                //else
                //{
                //    TimeZone.Detail = _DefaultEmptyProfileFieldText;
                //}
                if (Profile.HeightCm != 0)
                {
                    Height.Detail = Profile.HeightCm.ToString();
                }
                else
                {
                    Height.Detail = _DefaultEmptyProfileFieldText;
                }
                if (Profile.WeightLbs != 0)
                {
                    Weight.Detail = Profile.WeightLbs.ToString();
                }
                else
                {
                    Weight.Detail = _DefaultEmptyProfileFieldText;
                }
                if (Profile.TrainingGoal != "" && Profile.TrainingGoal != null)
                {
                    TrainingGoal.Detail = Profile.TrainingGoal;
                }
                else
                {
                    TrainingGoal.Detail = _DefaultEmptyProfileFieldText;
                }
                if (Profile.Region != "" && Profile.Region != null)
                {
                    Region.Detail = Profile.Region;
                }
                else
                {
                    Region.Detail = _DefaultEmptyProfileFieldText;
                }
            }
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void EditBtn_Clicked()
        {
            await Navigation.PushModalAsync(new EditPersonalProfile(), false);
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

        //private async void AuthBtn_Clicked()
        //{
        //    if (Auth.IsAuthenticated())
        //    {
        //        var result = await DisplayAlert("SIGN OUT", "Your downloaded workouts will be deleted and unavailable offline upon sign out!", "SIGN OUT", "NO");
        //        if (result)
        //        {
        //            //Sign Out
        //            Auth.DeleteCredentials();

        //            Task tw = Task.Factory.StartNew(Database.ClearAsync);
        //            tw.Wait();
        //            //await Navigation.PopModalAsync();
        //            //await Navigation.PopModalAsync();
        //            App.Current.MainPage = new SignIn();
        //            //await Navigation.PopModalAsync();
        //        }

        //    }
        //    //else
        //    //{
        //    //    //Sign In
        //    //    await Navigation.PushModalAsync(new LoginSignUpPage());
        //    //}

        //}
        

    }
}
