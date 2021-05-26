using Stance.Models.Local;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Test
{
    public partial class AudioWorkoutPlayingTest : ContentPage
    {

        public class ExerciseCellWithAudioAddOn
        {
            public Guid ActionGuid { get; set; }
            public int Type { get; set; } // 1 - seperator, 2- time based, 3 - rest, 4 - rep based
            public bool IsAudioCompletePlaying { get; set; }
            public int AudioClipToPlaySequenceNumber { get; set; } // Set to the first in the list, when switching to another set to it, and when done set to Guid Empty
            public ExerciseCellWithAudioAddOn nextAudioCell { get; set; }
            public List<AudioModel> AudioFiles { get; set; }
            public CancellationTokenSource cts { get; set; }
        }

        public class AudioModel
        {
            public Guid AudioGuid { get; set; }
            public int DelayMilliseconds { get; set; }
            public string FilePath { get; set; }
            public bool IsRepeat { get; set; }
            public int LengthMilliseconds { get; set; }
            public int NumberOfRepeats { get; set; }
            public int RepeatCycleSeconds { get; set; }
            public int SequenceNumber { get; set; }

        }

        private bool _currentlyInAudioTransition = false;
        //private CancellationTokenSource cts = new CancellationTokenSource();
        private ExerciseCellWithAudioAddOn _currentlyPlayingAudioCell;

        public AudioWorkoutPlayingTest()
        {
            InitializeComponent();

            _currentlyPlayingAudioCell = null;

            ExerciseCellWithAudioAddOn cell1 = new ExerciseCellWithAudioAddOn
            {
                ActionGuid = Guid.NewGuid(),
                IsAudioCompletePlaying = false,
                AudioClipToPlaySequenceNumber = 1,
                Type = 1,
                AudioFiles = new List<AudioModel>(),
                nextAudioCell = new ExerciseCellWithAudioAddOn(),
                cts = new CancellationTokenSource()
            };


            ExerciseCellWithAudioAddOn cell2 = new ExerciseCellWithAudioAddOn
            {
                ActionGuid = Guid.NewGuid(),
                IsAudioCompletePlaying = false,
                AudioClipToPlaySequenceNumber = 1,
                Type = 2,
                AudioFiles = new List<AudioModel>(),
                nextAudioCell = new ExerciseCellWithAudioAddOn(),
                cts = new CancellationTokenSource()
            };


            ExerciseCellWithAudioAddOn cell3 = new ExerciseCellWithAudioAddOn
            {
                ActionGuid = Guid.NewGuid(),
                IsAudioCompletePlaying = false,
                AudioClipToPlaySequenceNumber = 1,
                Type = 3,
                AudioFiles = new List<AudioModel>(),
                nextAudioCell = new ExerciseCellWithAudioAddOn(),
                cts = new CancellationTokenSource()
            };

            ExerciseCellWithAudioAddOn cell4 = new ExerciseCellWithAudioAddOn
            {
                ActionGuid = Guid.NewGuid(),
                IsAudioCompletePlaying = false,
                AudioClipToPlaySequenceNumber = 1,
                Type = 4,
                AudioFiles = new List<AudioModel>(),
                nextAudioCell = new ExerciseCellWithAudioAddOn(),
                cts = new CancellationTokenSource()
            };


            ExerciseCellWithAudioAddOn cell5 = new ExerciseCellWithAudioAddOn
            {
                ActionGuid = Guid.NewGuid(),
                IsAudioCompletePlaying = false,
                AudioClipToPlaySequenceNumber = 1,
                Type = 4,
                AudioFiles = new List<AudioModel>(),
                nextAudioCell = new ExerciseCellWithAudioAddOn(),
                cts = new CancellationTokenSource()
            };


            ExerciseCellWithAudioAddOn cell6 = new ExerciseCellWithAudioAddOn
            {
                ActionGuid = Guid.NewGuid(),
                IsAudioCompletePlaying = false,
                AudioClipToPlaySequenceNumber = 1,
                Type = 1,
                AudioFiles = new List<AudioModel>(),
                nextAudioCell = new ExerciseCellWithAudioAddOn(),
                cts = new CancellationTokenSource()
            };

            var AudioFiles = new List<AudioModel>()
            {
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 1,FilePath = "audio1.mp3",DelayMilliseconds = 0,LengthMilliseconds = 1000,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 2,FilePath = "audio2.mp3",DelayMilliseconds = 500,LengthMilliseconds = 1000,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 3,FilePath = "audio3.mp3",DelayMilliseconds = 500,LengthMilliseconds = 200,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 4,FilePath = "audio4.mp3",DelayMilliseconds = 500,LengthMilliseconds = 500,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 5,FilePath = "audio5.mp3",DelayMilliseconds = 500,LengthMilliseconds = 12000,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 6,FilePath = "audio4.mp3",DelayMilliseconds = 500,LengthMilliseconds = 1000,IsRepeat = true,NumberOfRepeats = 3,RepeatCycleSeconds = 5000,},
            };

            var AudioFiles2 = new List<AudioModel>()
            {
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 1,FilePath = "audio1.mp3",DelayMilliseconds = 0,LengthMilliseconds = 1000,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 2,FilePath = "audio2.mp3",DelayMilliseconds = 500,LengthMilliseconds = 1000,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 3,FilePath = "audio3.mp3",DelayMilliseconds = 500,LengthMilliseconds = 200,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
                new AudioModel { AudioGuid = Guid.NewGuid(),SequenceNumber = 4,FilePath = "audio4.mp3",DelayMilliseconds = 500,LengthMilliseconds = 500,IsRepeat = false,NumberOfRepeats = 0,RepeatCycleSeconds = 0,},
            };

            cell1.nextAudioCell = cell2;
            cell2.nextAudioCell = cell3;
            cell3.nextAudioCell = cell4;
            cell4.nextAudioCell = cell5;
            cell5.nextAudioCell = cell6;
            cell6.nextAudioCell = null;

            cell1.AudioFiles = AudioFiles2;
            cell2.AudioFiles = AudioFiles;
            cell3.AudioFiles = AudioFiles2;
            cell4.AudioFiles = AudioFiles;
            cell5.AudioFiles = AudioFiles;
            cell6.AudioFiles = AudioFiles2;

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
                        new Label
                        {
                            Text = "Intro Seperator 1", //auto plays on start, stopped when next btn touched
                        },
                        new Button {
                            Text = "Time Based 2", //plays when btn touched, stops when btn touched, restart at currently playing clip
                            Command = new Command<ExerciseCellWithAudioAddOn>(AudioBtn),
                            CommandParameter = cell2,
                        },
                        new Label
                        {
                            Text = "Rest 3", //autoplays after above
                        },
                        new Button {
                            Text = "Rep Based 4", //auto plays after above, stopped when btn touched, no restart
                            Command = new Command<ExerciseCellWithAudioAddOn>(AudioBtn),
                            CommandParameter = cell4,
                        },
                        new Button {
                            Text = "Rep Based 5", //auto plays when above btn is touched, stopped when btn touched, no restart
                            Command = new Command<ExerciseCellWithAudioAddOn>(AudioBtn),
                            CommandParameter = cell5,
                        },
                        new Label
                        {
                            Text = "Done Seperator 6", //auto plays when above btn is touched, stopped when done btn touched (not implimented)
                        },
                    }
            };

            AudioBtn(cell1);

        }

        private void AudioBtn(ExerciseCellWithAudioAddOn audioCell)
        {
            if (!_currentlyInAudioTransition)
            {
                //_currentlyInAudioTransition = true;

                if (_currentlyPlayingAudioCell != null)
                {
                    //stop current audio and play new if different than current
                    DependencyService.Get<IAudio>().Stop();
                    _currentlyPlayingAudioCell.cts.Cancel();
                    //_currentlyPlayingAudioCell.cts = new CancellationTokenSource();
                    //audioCell.cts = new CancellationTokenSource();

                    if (_currentlyPlayingAudioCell.ActionGuid == audioCell.ActionGuid)
                    {
                        //do nothign just stop
                    }
                    else
                    {
                        //play new 
                        Task.Factory.StartNew(() => PlayAudio(audioCell), audioCell.cts.Token);
                    }
                }
                else
                {
                    // No audio playing, just play it                    
                    Task.Factory.StartNew(() => PlayAudio(audioCell), audioCell.cts.Token);
                }
                //_currentlyInAudioTransition = false;
            }
        }


        private async void PlayAudio(ExerciseCellWithAudioAddOn audioCell)
        {
            _currentlyPlayingAudioCell = audioCell;

            if (!audioCell.IsAudioCompletePlaying)
            {
                //if not done playing, allow to continue or start
                var audioFilesToPlayTemp = audioCell.AudioFiles.OrderBy(x => x.SequenceNumber).ToList();

                var audioFilesToPlay = new List<AudioModel>();

                foreach (var item in audioFilesToPlayTemp)
                {
                    if (item.SequenceNumber >= audioCell.AudioClipToPlaySequenceNumber)
                    {
                        audioFilesToPlay.Add(item);
                    }
                }

                if (audioFilesToPlay.Count() > 0)
                {
                    var lastSequenceNumber = audioFilesToPlay.OrderByDescending(x => x.SequenceNumber).Select(x => x.SequenceNumber).First();
                    audioFilesToPlay.OrderBy(x => x.SequenceNumber).ToList();

                    foreach (var item in audioFilesToPlay)
                    {
                        _currentlyPlayingAudioCell.AudioClipToPlaySequenceNumber = item.SequenceNumber;

                        if (item.IsRepeat)
                        {
                            await Task.Delay(item.DelayMilliseconds);

                            for (int i = 0; i < item.NumberOfRepeats; i++)
                            {
                                await Task.Run(async () =>
                                {
                                    DependencyService.Get<IAudio>().PlayAudioFileFromResource(item.FilePath);
                                    await Task.Delay(item.LengthMilliseconds);
                                    await Task.Delay(item.RepeatCycleSeconds);
                                }, audioCell.cts.Token);
                            }
                        }
                        else
                        {
                            await Task.Delay(item.DelayMilliseconds);
                            DependencyService.Get<IAudio>().PlayAudioFileFromResource(item.FilePath);
                            await Task.Delay(item.LengthMilliseconds);
                        }

                        if (item.SequenceNumber == lastSequenceNumber)
                        {
                            _currentlyPlayingAudioCell.IsAudioCompletePlaying = true;
                        }

                    }
                }

                if (audioCell.nextAudioCell != null)
                {
                    //TYPE: 1 - seperator, 2- time based, 3 - rest, 4 - rep based
                    if (audioCell.Type == 1)
                    {
                        //Seperator, just play next audio
                        if (audioCell.nextAudioCell.Type == 1 || audioCell.nextAudioCell.Type == 2 || audioCell.nextAudioCell.Type == 3 || audioCell.nextAudioCell.Type == 4)
                        {
                            //play sound automatically for seperator, rest, and rep based 
                            // await Task.Factory.StartNew(() => PlayAudio(audioCell.nextAudioCell), audioCell.nextAudioCell.cts.Token);
                            await Task.Run(() =>
                            {
                                PlayAudio(audioCell.nextAudioCell);
                            }, audioCell.nextAudioCell.cts.Token);
                        }
                    }
                }

            }


            //query audio files for the ContactAction and start from the first one if audioCell.AudioClipGuidToPlay = Guid.Empty else start from the one with the corresponding Guid

            //must wait until the clip is done before moving on
            //_currentlyPlayingAudioFile = new Guid();
            //DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio1.mp3");
            ////wait clip length
            //await Task.Delay(500);

            ////Delay after clip
            //await Task.Delay(500);
            //_currentlyPlayingAudioFile = new Guid();
            //DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio2.mp3");
            //await Task.Delay(1000);

            //await Task.Delay(500);
            //_currentlyPlayingAudioFile = new Guid();
            //DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio3.mp3");
            //await Task.Delay(200);

            //await Task.Delay(500);
            //_currentlyPlayingAudioFile = new Guid();
            //DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio4.mp3");
            //await Task.Delay(500);

            //await Task.Delay(500);
            //_currentlyPlayingAudioFile = new Guid();
            //DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio5.mp3");
            //await Task.Delay(12000);

            //await Task.Delay(500);
            //for (int i = 0; i < 5; i++)
            //{
            //    await Task.Run(async () =>
            //    {
            //        _currentlyPlayingAudioFile = new Guid();
            //        DependencyService.Get<IAudio>().PlayAudioFileFromResource("audio4.mp3");
            //        await Task.Delay(2000);
            //        await Task.Delay(5000);
            //    }, cts.Token);
            //}
        }


    }
}
