using Microsoft.AppCenter.Analytics;
using Stance.Models.LocalDB;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Essentials;

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
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

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

            
        }

        private async void ShareBtn_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = "https://thecartelle.com",
                Title = "Cartelle fitness App"
            });
        }

        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
            MessagingCenter.Send(this, "WorkoutSurveyComplete");          
        }

        private async void ExitButton()
        {
            await Navigation.PopModalAsync();
            MessagingCenter.Send(this, "WorkoutSurveyComplete");
        }
        protected override bool OnBackButtonPressed()
        {
            ExitButton();
            return base.OnBackButtonPressed();
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
