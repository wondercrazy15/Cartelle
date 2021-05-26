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
    public partial class WorkoutLevel : BaseContentPage
    {
        private const string _PageName = "Workout Level Page";
        private ContactSignUpV2 _contactSignUp = null;

        public WorkoutLevel(ContactSignUpV2 contactSignUp)
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            _contactSignUp = contactSignUp;
        }
        private async void Beginner_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Beginner" } });
            _contactSignUp.WorkoutLevel = "beginner";
            await Navigation.PushModalAsync(new CreateAccount(_contactSignUp), false);
        }

        private async void Intermediate_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Intermediate" } });
            _contactSignUp.WorkoutLevel = "intermediate";
            await Navigation.PushModalAsync(new CreateAccount(_contactSignUp), false);
        }

        private async void Advanced_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Advanced" } });
            _contactSignUp.WorkoutLevel = "advanced";
            await Navigation.PushModalAsync(new CreateAccount(_contactSignUp), false);
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