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

            NavigationPage.SetBackButtonTitle(this, "");
            Title.Text = "MY PROFILE";
            //Padding = new Thickness(0, 20, 0, 0);
            //BackgroundColor = Color.Black;

            MessagingCenter.Subscribe<EditPersonalProfile>(this, "UpdatedProfile", (sender) => { DisplaySaveSuccess(); });
            TableViewMain.HasUnevenRows = true;
            TableViewMain.HeightRequest = 2600;

            var tapGestureRecognizer = new TapGestureRecognizer
            {
                Command = new Command(EditBtn_Clicked_2),
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

        private async void EditBtn_Clicked_2()
        {
            await Navigation.PushAsync(new EditPersonalProfile(), true);
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
                //LastName.Detail = Profile.LastName;
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
            await Navigation.PopAsync();
        }

        private async void EditBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditPersonalProfile(), true);
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }
       

    }
}
