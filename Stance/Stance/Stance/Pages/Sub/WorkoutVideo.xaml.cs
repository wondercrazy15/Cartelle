using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Octane.Xam.VideoPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stance.Pages.Sub
{
    public partial class WorkoutVideo : ContentPage
    {
        private const string _PageName = "Workout Video Full Screen";
        private bool IsTransition = false;

        public WorkoutVideo(string videoFullFilePath)
        {
            InitializeComponent();

            var tapGestureRecognizer = new TapGestureRecognizer
            {
                Command = new Command(Exit_DoubleTapped), 
                NumberOfTapsRequired = 2,               
            };
            ExitBtn.GestureRecognizers.Add(tapGestureRecognizer);

            if (videoFullFilePath != "" && videoFullFilePath != null && videoFullFilePath != String.Empty)
            {
                VideoPlayID.Source = VideoSource.FromFile(videoFullFilePath);
            }
        }

        private async void Exit_DoubleTapped()
        {
            try
            {
                if (!IsTransition)
                {
                    IsTransition = true;
                    await Task.Delay(505);
                    await Navigation.PopModalAsync(false);
                    MessagingCenter.Send(this, "ReturnFromFullScreen");
                }
            } catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Exit_DoubleTapped()" } });
            }
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}