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
    public partial class WorkoutGoal : BaseContentPage
    {
        private const string _PageName = "Workout Goal Page";
        private ContactSignUpV2 _contactSignUp = null;

        public WorkoutGoal(ContactSignUpV2 contactSignUp)
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            _contactSignUp = contactSignUp;
        }
        private async void ToneUp_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Tone Up" } });
            _contactSignUp.WorkoutGoal = "tone up";
            await Navigation.PushModalAsync(new WorkoutLevel(_contactSignUp), false);
        }

        private async void FatLoss_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Fat Loss" } });
            _contactSignUp.WorkoutGoal = "fat loss";
            await Navigation.PushModalAsync(new WorkoutLevel(_contactSignUp), false);
        }

        private async void Strength_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Strength" } });
            _contactSignUp.WorkoutGoal = "strengthen";
            await Navigation.PushModalAsync(new WorkoutLevel(_contactSignUp), false);
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