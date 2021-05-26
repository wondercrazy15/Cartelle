using Octane.Xamarin.Forms.VideoPlayer;
using Octane.Xamarin.Forms.VideoPlayer.Events;
using Stance.Utils;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
using Stance.Utils.LocalDB;
using Stance.ViewModels;

namespace Stance.Pages.Sub
{
    public partial class WorkoutSetting : BaseContentPage
    {
        private const string _PageName = "Workout Setting Page";
        private ContactSignUpV2 _contactSignUp = null;

        public WorkoutSetting(ContactSignUpV2 contactSignUp)
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            _contactSignUp = contactSignUp;

        }

        private async void Gym_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Gym" } });
            _contactSignUp.WorkoutSetting = "gym";            
            await Navigation.PushModalAsync(new WorkoutGoal(_contactSignUp), false);
        }

        private async void Home_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Home" } });
            _contactSignUp.WorkoutSetting = "home";
            await Navigation.PushModalAsync(new WorkoutGoal(_contactSignUp), false);
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "On Appearing" } });
            base.OnAppearing();
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }

    }
}