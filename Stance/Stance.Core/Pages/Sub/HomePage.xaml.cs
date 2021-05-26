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
using Stance.Models.LocalDB;
using Microsoft.AppCenter.Crashes;

using System.IO;

namespace Stance.Pages.Sub
{
    public partial class HomePage : BaseContentPage
    {
        private const string _PageName = "Home Page";
        private bool PauseVideo = false;
        private bool _LoadedAthletePage = false;

        public HomePage()
        {
            try
            {
                InitializeComponent();
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
                Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");
                On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
                MessagingCenter.Subscribe<App>(this, "BranchLoadMessage", (sender) => { BranchLoadMessage(); });
                MessagingCenter.Subscribe<App>(this, "OnResume", (sender) => { OnResume(); });
                MessagingCenter.Subscribe<CreateAccount>(this, "OnSignUp", (sender) => { TurnOffVolume(); });
                MessagingCenter.Subscribe<SignIn>(this, "OnSignIn", (sender) => { TurnOffVolume(); });

                
                Task.Factory.StartNew(Database.ClearAsync).Wait();
                Task.Factory.StartNew(Database.CreateAsync).Wait();

                //var tapGestureRecognizer = new TapGestureRecognizer
                //{
                //    Command = new Command(SoundsBtn_Clicked),
                //    NumberOfTapsRequired = 1,
                //};
                //SoundBtn.GestureRecognizers.Add(tapGestureRecognizer);
                
                //Stream data = DependencyService.Get<IVideoUrl>().videoUrl();
                //LoginBtn.Text="Stream" +data;
                //if (data != null)
                //{
                //    VideoPlayID.Source = VideoSource.FromStream(() => data,"mp4");
                //}

               // VideoPlayID.Source = VideoSource.FromResource("AppStoreVideo.mp4");
                // VideoPlayID.Source = VideoSource.FromFile("AppStoreVideo.mp4");//cartelle_video.mp4
                VideoPlayID.Completed += VideoComplete;
                VideoPlayID.Playing += VideoPlaying;

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) =>
                {
                    // handle the tap
                    Button_Clicked();
                };
                tapGestureRecognizer.NumberOfTapsRequired = 1;
                LoginSL.GestureRecognizers.Add(tapGestureRecognizer);
            }
            catch (Exception ex)
            {

            }
        }

        private void BranchLoadMessage()
        {
            if (App._contactSignUp.ReferralPool == "athlete" && _LoadedAthletePage == false)
            {
                //change Get Access btn to Loading and disable to until the athletes info is loaded from API
                GoBtn.IsEnabled = false;
                GoBtn.Text = "LOADING...";
                LoadAthletePage();
            }
        }

        private async void LoadAthletePage()
        {
            //call an api to get the athletes data - first check to make sure it is not already loaded
            //push modal to show AthleteOverview page            

            var Athlete = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000 && x.Code.Equals(App._contactSignUp.ReferrerCode)).FirstOrDefaultAsync();//published 
            if (Athlete != null)
            {
                //push to athlete page
                VideoPlayID.Pause();
                await Navigation.PushModalAsync(new AthleteOverview_v1(Athlete,"yes"), false);
                _LoadedAthletePage = true;
                GoBtn.IsEnabled = true;
                GoBtn.Text = "GET ACCESS NOW";
                return;
            }

            if (!IsInternetConnected())
            {
                GoBtn.IsEnabled = true;
                GoBtn.Text = "GET ACCESS NOW";
                return;
            }

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "GetAthleteData" } });
                await WebAPIService.GetAthleteRefferrerData(_client, App._contactSignUp.ReferrerCode);
                Athlete = await _connection.Table<LocalDBAccount>().Where(x => x.StatusCodeValue == 866660000 && x.Code.Equals(App._contactSignUp.ReferrerCode)).FirstOrDefaultAsync();//published 
                if(Athlete != null)
                {
                    VideoPlayID.Pause();
                    await Navigation.PushModalAsync(new AthleteOverview_v1(Athlete,"yes"), false);
                    _LoadedAthletePage = true;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "LoadAthletePage()" } });              
            }
            GoBtn.IsEnabled = true;
            GoBtn.Text = "GET ACCESS NOW";
            return;
        }

        private void TurnOffVolume()
        {
            VideoPlayID.Volume = 0;
        }

        private void OnResume()
        {
            if (!PauseVideo)
            {
                VideoPlayID.Play();
            }
        }

        protected override void OnDisappearing()
        {
            VideoPlayID.Pause();
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "On Appearing" } });

            OnResume();
            base.OnAppearing();
        }

        private void VideoPlaying(object sender, VideoPlayerEventArgs e)
        {
            if (PauseVideo)
            {
                VideoPlayID.Pause();
            }
        }

        private async void VideoComplete(object sender, VideoPlayerEventArgs e)
        {
            //cartelleLogo.IsVisible = false;
            VideoPlayID.Pause();
            PauseVideo = true;
            VideoPlayID.Repeat = false;
            //await Task.Delay(1000);
            //VideoPlayID.Source = VideoSource.FromFile("cartelle_video.mp4");
            //PauseVideo = false;
            //VideoPlayID.Repeat = true;
            //VideoPlayID.Play();
        }

        private async void GetStartedBtn_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Clicked Get Started" } });
            //DependencyService.Get<IFacebookEvent>().ClickedGetStarted();

            TurnOffVolume();

            if (App._contactSignUp.ReferralPool == "athlete")
            {
                LoadAthletePage();
            }
            else
            {
                await Navigation.PushModalAsync(new RecommendedBy(), false);
            }
        }

        private async void Button_Clicked()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Clicked Sign in" } });
            TurnOffVolume();
            await Navigation.PushModalAsync(new SignIn(), false);
        }
    }
}