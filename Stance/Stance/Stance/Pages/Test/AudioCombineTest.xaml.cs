using Stance.Models.Local;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using static Stance.Pages.Test.AudioWorkoutPlayingTest;

namespace Stance.Pages.Test
{
    public partial class AudioCombineTest : ContentPage
    {
        private bool _currentlyPlayingAudio = false;
        private bool _currentlyInAudioTransition = false;
        private Guid _currentlyPlayingAudioFile = Guid.Empty;
        private List<Guid> _pausedAudioFileList = new List<Guid>();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private int _AudioBtnNumber = 0;


        public AudioCombineTest()
        {
            InitializeComponent();

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
                        new Button {
                            Text = "START AUDIO 1",
                            Command = new Command<int>(AudioBtn),
                            CommandParameter = 1,
                        },
                        new Button {
                            Text = "START AUDIO 2",
                            Command = new Command<int>(AudioBtn),
                            CommandParameter = 2,
                        },
                        new Button {
                            Text = "START AUDIO 3",
                            Command = new Command<int>(AudioBtn),
                            CommandParameter = 3,
                        },
                        
                    }
            };

        }

        private void AudioBtn(int i)
        {
            if(_currentlyPlayingAudio && !_currentlyInAudioTransition && _AudioBtnNumber == i)
            {
                //Stop currently playing audio - double tap same button
                _currentlyInAudioTransition = true;
                DependencyService.Get<IAudio>().Stop();
                cts.Cancel();
                _pausedAudioFileList.Add(_currentlyPlayingAudioFile);
                _currentlyPlayingAudioFile = Guid.Empty;
                _AudioBtnNumber = 0; //no audio playing at all
                _currentlyPlayingAudio = false;
                _currentlyInAudioTransition = false;

            } else if (_currentlyPlayingAudio && !_currentlyInAudioTransition)
            {
                //Stop current Audio
                _currentlyInAudioTransition = true;
                DependencyService.Get<IAudio>().Stop();
                cts.Cancel();
                _pausedAudioFileList.Add(_currentlyPlayingAudioFile);
                _currentlyPlayingAudioFile = Guid.Empty;
                _AudioBtnNumber = i;
                _currentlyPlayingAudio = false;
                _currentlyInAudioTransition = false;

                //Start new Audio
                Task.Factory.StartNew(() => PlayAudio(Guid.Empty), cts.Token);
                _currentlyPlayingAudio = true;

            }
            else if (!_currentlyInAudioTransition && _AudioBtnNumber == 0)
            {
                //Initial Start of first Audio
                _AudioBtnNumber = i;
                Task.Factory.StartNew(() => PlayAudio(Guid.Empty), cts.Token);
                _currentlyPlayingAudio = true;
            }
        }

        private async void PlayAudio(Guid ActionGuid)
        {
            //must wait until the clip is done before moving on
            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio1.mp3");
            //wait clip length
            await Task.Delay(500);
            //Delay after clip
            await Task.Delay(500);
            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio2.mp3");
            await Task.Delay(1000);
            await Task.Delay(500);
            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio3.mp3");
            await Task.Delay(200);
            await Task.Delay(500);

            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio4.mp3");
            await Task.Delay(500);
            await Task.Delay(500);

            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio5.mp3");
            await Task.Delay(12000);
            await Task.Delay(500);

            
            for(int i = 0; i < 5; i++)
            {
                await Task.Run(async () =>
                {
                    _currentlyPlayingAudioFile = new Guid();
                    DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio4.mp3");
                    await Task.Delay(2000);
                    await Task.Delay(10000);
                });
            }
        }


        private async void PlayAudio1(Guid ActionGuid)
        {
            var listOfAudio = new List<AudioModel>();

            foreach(var item in listOfAudio)
            {



            }


            _currentlyPlayingAudioFile = listOfAudio.ElementAt(0).AudioGuid;
            await Task.Delay(listOfAudio.ElementAt(0).DelayMilliseconds);
            DependencyService.Get<IAudio>().PlayAudioFileFromResource(listOfAudio.ElementAt(0).FilePath);

            if (listOfAudio.ElementAt(0).IsRepeat)
            {

            }

            await Task.Delay(500);
            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio2.mp3");
            await Task.Delay(500);
            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio3.mp3");
            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio4.mp3");
            await Task.Delay(1000);
            _currentlyPlayingAudioFile = new Guid();
            DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio5.mp3");
            await Task.Delay(5000);
            int i = 5;
            while (i > 0)
            {
                _currentlyPlayingAudioFile = new Guid();
                DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio6_repeat.mp3");
                await Task.Delay(5000);
                i--;
            }
        }


    }
}
