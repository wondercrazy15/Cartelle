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
using Stance.Pages.Main;

namespace Stance.Pages.Sub
{
    public partial class FabInFivePage : ContentPage
    {
        private const string _PageName = "Fab in Five Video";
        String _FilePathToVideo = String.Empty;
        String _currentState = "FabIn5Video";

        public FabInFivePage(string videoUrl = null)
        {
            InitializeComponent();
            if (videoUrl != "" && videoUrl != null && videoUrl != String.Empty)
            {
                try
                {
                    ExplainationText.Text = "100% of people who stick to a fitness program see results. It all starts with your first workout so jump in and commit. To make it easier for you to succeed we have an exclusive Fab in Five gift that will keep you on track.";
                    VideoPlayID.Source = VideoSource.FromUri(videoUrl);
                    VideoPlayID.Completed += DismissVideoPlayer;
                } catch(Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "FabInFivePage" } });
                    ShowVideoErrorMessage();
                }
                
            }
        }


        //private async void GrowGiftBox()
        //{
        //    FirstSL.IsVisible = true;
        //    bool decreaseSize = false;
        //    int i = 20;
        //    int delayMS = 101;

        //    while(true)
        //    {
        //        if(!decreaseSize && i >= 100)
        //        {                    
        //            decreaseSize = true;
        //        }
        //        else if (!decreaseSize)
        //        {
        //            Device.BeginInvokeOnMainThread(() =>
        //            {
        //                GiftBox.HeightRequest = i;
        //                GiftBox.HeightRequest = i;
        //            });
        //            await Task.Delay(delayMS);
        //            i+=5;
        //        }
        //        else
        //        {
        //            if (i <= 70)
        //                break;

        //            Device.BeginInvokeOnMainThread(() =>
        //            {
        //                GiftBox.HeightRequest = i;
        //                GiftBox.HeightRequest = i;
        //            });
        //            await Task.Delay(delayMS);
        //            i--;
        //        }
        //        delayMS = Math.Abs(delayMS - i );
        //    }
        //}

        private async void DismissVideoPlayer(object sender, VideoPlayerEventArgs e)
        {
            try
            {
                if(_currentState == "FabIn5Video")
                {
                    SetSecondState();
                }
                else if (_currentState == "HowToVideo")
                {
                    await Navigation.PushModalAsync(new EditPersonalProfile("fromFabInFive"), true);

                    //FirstSL.IsVisible = true;
                    //show button
                }
                //GrowGiftBox();
                //VideoPlayID.Pause();
                //await Task.Delay(500);
                //VideoPlayID.Play();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "DismissVideoPlayer" } });
            }
        }

        private async void ShowVideoErrorMessage()
        {
            await DisplayAlert("Video Error", "There's an issue with this video.", "OK");
            SetSecondState();
        }

        private async void GiftBtn_Clicked(object sender, EventArgs e)
        {
            SetSecondState();
        }

        private void SetSecondState()
        {
            _currentState = "FabIn5Gift";
            VideoPlayID.IsVisible = false;
            VideoPlayID.Pause();
            FirstSL.IsVisible = false;
            VideoPlayID.IsVisible = false;
            GiftSL.IsVisible = true;
        }

        private void SetThirdState()
        {
            _currentState = "HowToVideo";          
            GiftSL.IsVisible = false;           
            VideoPlayID.Source = VideoSource.FromUri("https://thecartelle.azureedge.net/cartelle/Chosse%26Starting%20A%20program.mp4");
            VideoPlayID.IsVisible = true;
            VideoPlayID.Play();
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            ExitBtn.IsEnabled = false;
            ExitBtn.Text = "HERE WE GO";
            await Navigation.PushModalAsync(new EditPersonalProfile("fromFabInFive"), true);
            return;
        }

        private void HowToVideoBtn_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainStartingPage("login");

            //SetThirdState();
            return;
        }


        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
