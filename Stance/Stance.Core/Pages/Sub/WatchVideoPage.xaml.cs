using PCLStorage;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Octane.Xamarin.Forms.VideoPlayer;
using Octane.Xamarin.Forms.VideoPlayer.Events;

namespace Stance.Pages.Sub
{
    public partial class WatchVideoPage : ContentPage
    {
        private const string _PageName = "Watch Promo Video";
        String _FilePathToVideo = String.Empty;

        public WatchVideoPage(string videoUrl = null)
        {
            InitializeComponent();

            if(videoUrl != "" && videoUrl != null && videoUrl != String.Empty)
            {
                try
                {
                    VideoPlayID.Source = VideoSource.FromUri(videoUrl);
                    VideoPlayID.Completed += DismissVideoPlayer;
                } catch(Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "WatchVideoPage" } });
                    ShowVideoErrorMessage();
                }
                
            }
        }

        private async void ShowVideoErrorMessage()
        {
            await DisplayAlert("Video Error", "There's an issue with this video.", "OK");
            await Navigation.PopModalAsync();
        }

        private async void DismissVideoPlayer(object sender, VideoPlayerEventArgs e)
        {
            try
            {
                await Task.Delay(500);
                await Navigation.PopModalAsync();
            } catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "DismissVideoPlayer" } });
            }
        }

        private void GetFilePath()
        {
            //works below: method 2
            //var fileName = "oceans3.mp4";
            //var erwt = DependencyService.Get<IFileSystemCustom>();
            //var path = erwt.GetRootFolder(fileName);
            //_FilePathToVideo = path;
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
