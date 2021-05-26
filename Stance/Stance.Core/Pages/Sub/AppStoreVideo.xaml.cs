using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Octane.Xamarin.Forms.VideoPlayer;
using Octane.Xamarin.Forms.VideoPlayer.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class AppStoreVideo : ContentPage
    {
        private const string _PageName = "App Store Video";
        private bool IsTransition = false;

        public AppStoreVideo()
        {
            InitializeComponent();

            try
            {
                VideoPlayID.Source = VideoSource.FromUri("https://thecartelle.azureedge.net/cdn/api/AppStoreVideo.mp4");
                VideoPlayID.Completed += DismissVideoPlayer;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "OnLoad" } });
            }
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Exit" } });

            await Navigation.PopModalAsync();
        }

        private async void DismissVideoPlayer(object sender, VideoPlayerEventArgs e)
        {
            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Completed Video" } });
                await Task.Delay(500);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "DismissVideoPlayer" } });
            }
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}