using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Utils;
using Stance.Utils.OptionSets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class EditPersonalProfile : BaseContentPage
    {
        private const string _PageName = "Edit Profile";
        List<OptionSetModel> _genderList;
        List<OptionSetModel> _trainingGoalList;
        List<OptionSetModel> _regionList;

        public EditPersonalProfile()
        {
            InitializeComponent();
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            List<OptionSetModel> genderList = CRMOptionSets.GenderOptionSet();
            _genderList = genderList;

            List<OptionSetModel> trainingGoalList = CRMOptionSets.TrainingGoalOptionSet();
            _trainingGoalList = trainingGoalList;

            List<OptionSetModel> regionList = CRMOptionSets.RegionOptionSet();
            _regionList = regionList;

            foreach (var item in genderList)
            {
                Gender.Items.Add(item.Name);
            }

            foreach (var item in trainingGoalList)
            {
                TrainingGoal.Items.Add(item.Name);
            }

            foreach (var item in regionList)
            {
                Region.Items.Add(item.Name);
            }

            //foreach (var tz in TimeZoneConverter.GetListOfTimeZones())
            //{
            //    TimeZone.Items.Add(tz);
            //}            

            var tapGestureRecognizer = new TapGestureRecognizer
            {
                Command = new Command(SaveProfile_Btn),
            };
            SaveSL.GestureRecognizers.Add(tapGestureRecognizer);
            Spinner.IsVisible = false;

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
                //FirstName.Text = Profile.FirstName;
                //LastName.Text = Profile.LastName;
                if(Profile.InstagramHandle == null || Profile.InstagramHandle == "")
                {
                    InstagramHandle.Text = "";
                }

                if (Profile.Birthday != null)
                {
                    Birthdate.Date = (DateTime)Profile.Birthday;
                }
                else
                {
                    //Birthdate.Date = ;
                }
                if (Profile.Gender != "")
                {
                    foreach (var item in _genderList)
                    {
                        if (Profile.GenderTypeCode == item.TypeCode)
                        {
                            Gender.SelectedIndex = item.IndexVal;
                            break;
                        }
                    }
                }
                else
                {
                    Gender.SelectedIndex = 0;
                }
                //if (Profile.TimeZone != "")
                //{
                //    var i = 0;
                //    foreach (var tz in TimeZoneConverter.GetListOfTimeZones())
                //    {
                //        if (tz == Profile.TimeZone)
                //        {
                //            TimeZone.SelectedIndex = i;
                //            break;
                //        }
                //        i++;
                //    }
                //}
                //else
                //{
                //    //TimeZone.SelectedIndex = 40;
                //}
                if (Profile.HeightCm != 0)
                {
                    //Height.Text = Profile.HeightCm.ToString();
                    heightStepper.Value = Profile.HeightCm;
                }
                else
                {
                    //Height.Text = "140";
                    heightStepper.Value = 140;
                }
                if (Profile.WeightLbs != 0)
                {
                    //Weight.Text = Profile.WeightLbs.ToString();
                    weightStepper.Value = Profile.WeightLbs;
                }
                else
                {
                    //Weight.Text = "120";
                    weightStepper.Value = 120;
                }
                if (Profile.TrainingGoal != "")
                {
                    foreach (var item in _trainingGoalList)
                    {
                        if (Profile.TrainingGoalTypeCode == item.TypeCode)
                        {
                            TrainingGoal.SelectedIndex = item.IndexVal;
                            break;
                        }
                    }
                }
                else
                {
                    TrainingGoal.SelectedIndex = 0;
                }
                if (Profile.Region != "")
                {
                    foreach (var item in _regionList)
                    {
                        if (Profile.RegionTypeCode == item.TypeCode)
                        {
                            Region.SelectedIndex = item.IndexVal;
                            break;
                        }
                    }
                }
                else
                {
                    Region.SelectedIndex = 0;
                }
            }
        }

        private async void SaveProfile_Btn()
        {
            if (!IsInternetConnected())
            {
                await DisplayAlert("NO INTERNET", "Connect to the internet and try again.", "OK");
                return;
            }

            string InstagramHandle_Text = "";

            if (InstagramHandle.Text == null || InstagramHandle.Text == "")
            {
                InstagramHandle_Text = "";
            }
            else
            {
                InstagramHandle_Text = InstagramHandle.Text;
            }

            try
            {
                if (InstagramHandle.Text != "" && InstagramHandle.Text != null)
                {
                    Regex instReg = new Regex(@"^([A-Za-z0-9_](?:(?:[A-Za-z0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z0-9_]))?)$", RegexOptions.IgnoreCase);

                    if (!instReg.Match(InstagramHandle.Text).Success)
                    {
                        InstagramHandle.BackgroundColor = Color.FromHex("#ffb2b2");
                        await DisplayAlert("INSTAGRAM HANDLE", "Your instagram handle doesn't look real.", "OK");
                        return;
                    }
                }
            } catch(Exception ex)
            {
                var err = ex.ToString();
            }


            SaveText.IsVisible = false;
            Spinner.IsVisible = true;

            try
            {
                int selectedRegionTypeCode = -1;

                foreach (var item in _regionList)
                {
                    if (Region.SelectedIndex == item.IndexVal)
                    {
                        selectedRegionTypeCode = item.TypeCode;
                        break;
                    }
                }

                var selectedRegionText = "";

                foreach (var item in _regionList)
                {
                    if (selectedRegionTypeCode == item.TypeCode)
                    {
                        selectedRegionText = item.Name;
                        break;
                    }
                }


                int selectedGenderTypeCode = -1;

                foreach (var item in _genderList)
                {
                    if (Gender.SelectedIndex == item.IndexVal)
                    {
                        selectedGenderTypeCode = item.TypeCode;
                        break;
                    }
                }

                var selectGenderText = "";

                foreach (var item in _genderList)
                {
                    if (selectedGenderTypeCode == item.TypeCode)
                    {
                        selectGenderText = item.Name;
                        break;
                    }
                }


                int selectedTrainingGoalTypeCode = -1;

                foreach (var item in _trainingGoalList)
                {
                    if (TrainingGoal.SelectedIndex == item.IndexVal)
                    {
                        selectedTrainingGoalTypeCode = item.TypeCode;
                        break;
                    }
                }

                var selectedTrainingGoalText = "";
                foreach (var item in _trainingGoalList)
                {
                    if (selectedTrainingGoalTypeCode == item.TypeCode)
                    {
                        selectedTrainingGoalText = item.Name;
                        break;
                    }
                }

                //var tzVal = TimeZone.SelectedIndex;
                //if (TimeZone.SelectedIndex < 0)
                //{
                //    await DisplayAlert("TIME ZONE REQUIRED", "Your time zone is used to schedule your training days.", "OK");
                //    return;
                //}


                APIContactV2 editedContact = new APIContactV2
                {
                    //FirstName = FirstName.Text,
                    //LastName = LastName.Text,
                    Birthday = Birthdate.Date,
                    //TimeZone = TimeZone.Items[TimeZone.SelectedIndex],
                    InstagramHandle = InstagramHandle_Text,
                    HeightCm = (float)heightStepper.Value,
                    WeightLbs = (float)weightStepper.Value,
                };

                if (selectedGenderTypeCode != -1)
                {
                    editedContact.GenderTypeCode = selectedGenderTypeCode;
                }

                if (selectedTrainingGoalTypeCode != -1)
                {
                    editedContact.TrainingGoalTypeCode = selectedTrainingGoalTypeCode;
                }

                if (selectedRegionTypeCode != -1)
                {
                    editedContact.RegionTypeCode = selectedRegionTypeCode;
                }

                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Save" } });

                var responseStatusCode = await WebAPIService.UpdateProfile(_client, editedContact);

                if (responseStatusCode == HttpStatusCode.OK)
                {

                    var Profile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();

                    if (Profile != null)
                    {
                        Profile.InstagramHandle = InstagramHandle_Text;
                        Profile.Birthday = Birthdate.Date;
                        Profile.HeightCm = (float)heightStepper.Value;
                        Profile.WeightLbs = (float)weightStepper.Value;

                        if (selectedGenderTypeCode != -1)
                        {
                            Profile.GenderTypeCode = selectedGenderTypeCode;
                            Profile.Gender = selectGenderText;
                        }

                        if (selectedTrainingGoalTypeCode != -1)
                        {
                            Profile.TrainingGoalTypeCode = selectedTrainingGoalTypeCode;
                            Profile.TrainingGoal = selectedTrainingGoalText;
                        }

                        if (selectedRegionTypeCode != -1)
                        {
                            Profile.RegionTypeCode = selectedRegionTypeCode;
                            Profile.Region = selectedRegionText;
                        }
                        await _connection.UpdateAsync(Profile);
                    }

                    MessagingCenter.Send(this, "UpdatedProfile");
                    await Navigation.PopModalAsync(false);
                }
                else
                {
                    await DisplayAlert("CANNOT UPDATE", "Try again later.", "OK");
                    await Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                SaveText.IsVisible = false;
                Spinner.IsVisible = true;
                await DisplayAlert("CANNOT UPDATE", "Opps, try again later.", "OK");
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "SaveProfile_Btn()" } });
                await Navigation.PopModalAsync();
            }
            Spinner.IsVisible = false;
            SaveText.IsVisible = true;
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Exit" } });
            await Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
