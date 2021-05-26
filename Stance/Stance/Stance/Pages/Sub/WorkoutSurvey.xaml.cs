using Microsoft.AppCenter.Analytics;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class WorkoutSurvey : BaseContentPage
    {
        private const string _PageName = "Workout Survey";
        private Guid _ContactProgramDayGuid;
        public static List<Task> TaskList = new List<Task>();

        public WorkoutSurvey(Guid ContactProgramDayGuid)
        {
            InitializeComponent();
            _ContactProgramDayGuid = ContactProgramDayGuid;
            ThankYou.IsVisible = false;
            star1.Image = (FileImageSource)ImageSource.FromFile("star.png");
            star2.Image = (FileImageSource)ImageSource.FromFile("star.png");
            star3.Image = (FileImageSource)ImageSource.FromFile("star.png");
            star4.Image = (FileImageSource)ImageSource.FromFile("star.png");
            star5.Image = (FileImageSource)ImageSource.FromFile("star.png");
        }

        private void Star1Btn_Clicked(object sender, EventArgs e)
        {
            DisableAllBtn();
            star1.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            SaveRating(1);
        }

        private void Star2Btn_Clicked(object sender, EventArgs e)
        {
            DisableAllBtn();
            star1.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star2.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            SaveRating(2);
        }

        private void Star3Btn_Clicked(object sender, EventArgs e)
        {
            DisableAllBtn();
            star1.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star2.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star3.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            SaveRating(3);
        }

        private void Star4Btn_Clicked(object sender, EventArgs e)
        {
            DisableAllBtn();
            star1.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star2.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star3.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star4.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            SaveRating(4);
        }

        private void Star5Btn_Clicked(object sender, EventArgs e)
        {
            DisableAllBtn();
            star1.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star2.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star3.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star4.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            star5.Image = (FileImageSource)ImageSource.FromFile("fullstar.png");
            SaveRating(5);
        }

        public void DisableAllBtn()
        {
            star1.IsEnabled = false;
            star2.IsEnabled = false;
            star3.IsEnabled = false;
            star4.IsEnabled = false;
            star5.IsEnabled = false;
            ThankYou.IsVisible = true;
        }

        private async void SaveRating(int rating)
        {
            var ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDayGuid).FirstOrDefaultAsync();
            if (ContactProgramDay == null)            
                return;            

            ContactProgramDay.Rating = rating;
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Saving Rating" } });
            await _connection.UpdateAsync(ContactProgramDay);
            //MessagingCenter.Send(this, "RefreshDashboard");                

            SyncToCRM();
        }

        private async void SyncToCRM()
        {
            if (IsInternetConnected())
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "SyncToCRM" } });
                await WebAPIService.SyncToCRM(_client);
            }
            await Navigation.PopModalAsync();
            MessagingCenter.Send(this, "WorkoutSurveyComplete");          
        }

        public static async Task Sleep(int ms)
        {
            await Task.Delay(ms);
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
