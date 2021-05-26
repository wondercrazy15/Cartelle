using AVFoundation;
using Stance.Pages.Main;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class ExercisePage : ContentPage
    {
        private string _id = String.Empty;

        protected override void OnAppearing()
        {

            if (_id == "1")
            {
                VideoPlayID.Source = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/3_Exercise/jumping_jacks_mini_loop.mp4";
            }
            else if (_id == "2")
            {
                VideoPlayID.Source = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/2_Exercise/lunge_mini_loop.mp4";
            }
            else if (_id == "3")
            {
                VideoPlayID.Source = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/4_Exercise/stretch_mini_loop2.mp4";
            }
            else
            {
                VideoPlayID.Source = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/1_Exercise/stretch_mini_loop_1.mp4";
            }

            base.OnAppearing();
        }

        public ExercisePage(string id = null)
        {
            InitializeComponent();

            if (id == null)
            {
                _id = "1";
            }
            else
            {
                _id = id;

            }
            ExitBtn.IsVisible = false;

            VideoControl.Clicked += (sender, e) =>
            {
                VideoControl.IsEnabled = VideoControl.IsVisible = false;

                if (VideoControl.Text == "►")
                {
                    ExitBtn.IsVisible = false;
                    VideoControl.Text = "||";
                    VideoPlayID.Play();
                    VideoPlayID.Opacity = 1;
                    MainContainer.Opacity = 1;
                }
                else
                {
                    ExitBtn.IsVisible = true;
                    VideoControl.Text = "►";
                    VideoPlayID.Opacity = 0.3;
                    MainContainer.Opacity = 0.3;
                    VideoPlayID.Pause();
                }

                VideoControl.IsEnabled = VideoControl.IsVisible = true;

            };

            //DependencyService.Get<IAudio>().PlayAudioFile("304.mp3");



            //var audioString = "http://www.noiseaddicts.com/samples_1w72b820/4927.mp3";

            //var player = AVPlayer.FromUrl(new Foundation.NSUrl(audioString));
            //player.Play();

            //DependencyService.Get<IAudio>().PlayAudioFile(audioString);




        }

        async void ExerciseComplete_Clicked(object sender, EventArgs e)
        {

            var num = int.Parse(_id);

            if (num == 4)
            {
                await Navigation.PushModalAsync(new ExercisePage("1"));
            }
            else
            {
                var newNum = (num + 1).ToString();
                await Navigation.PushModalAsync(new ExercisePage(newNum));

            }
        }

        void ExitBtn_Clicked(object sender, EventArgs e)
        {
            //Or go to dashboard or workout results even if partially complete
            App.Current.MainPage = new MainStartingPage();
        }







    }
}
