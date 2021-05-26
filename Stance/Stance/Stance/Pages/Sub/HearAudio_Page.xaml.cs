using AVFoundation;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class HearAudio_Page : ContentPage
    {
        private string _audioUrl = String.Empty;

        public HearAudio_Page(string audioUrl = null)
        {
            InitializeComponent();

            if(audioUrl != null)
            {
                _audioUrl = audioUrl;
            }
            else
            {
                _audioUrl = "http://www.noiseaddicts.com/samples_1w72b820/4927.mp3";
            }

            _audioUrl = "http://www.noiseaddicts.com/samples_1w72b820/4927.mp3";

            PlayAudio();


            var sl = new StackLayout
            {

            };

            var btn = new Button
            {
                Text = "Play Audio",
                HeightRequest = 200,

            };

            btn.Clicked += (s, e) =>
            {
                PlayAudio();
            };

            var btnExit = new Button
            {
                Text = "Exit",
                HeightRequest = 50,
                BackgroundColor = Color.Aqua,
                TextColor = Color.White,

            };

            btnExit.Clicked += (s, e) =>
            {
                Exit();
            };

            sl.Children.Add(btn);
            sl.Children.Add(btnExit);


            Content = sl;


        }

        async private void Exit()
        {
            await Navigation.PopModalAsync();
        }

        private void PlayAudio()
        {
            if (_audioUrl != String.Empty)
            {
                // DependencyService.Get<IAudio>().PlayAudioFile(_audioUrl);
                var audioString = "http://www.noiseaddicts.com/samples_1w72b820/4927.mp3";

                var player = AVPlayer.FromUrl(new Foundation.NSUrl(audioString));
                player.Play();
            }
        }

        void HearAudioBtn_Clicked(object sender, EventArgs e)
        {
            PlayAudio();
        }
    }
}
