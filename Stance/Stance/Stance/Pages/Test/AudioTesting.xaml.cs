using PCLStorage;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Test
{
    public partial class AudioTesting : ContentPage
    {
        private string _audioUrl;
        private bool _currentlyPlayingAudio = false;
        private static Guid _ContactProgramDayGuidDB = Guid.Empty;
        private string _rootFilePath;
        private string _audioFilePath;

        public AudioTesting()
        {
            InitializeComponent();

            // _audioUrl = "https://res.cloudinary.com/stance/video/upload/v1490063951/Stance/Audio_Contents/BalanceSquats_Round1_desc.mp3";
            _audioUrl = "http://res.cloudinary.com/stance/video/upload/v1490156965/Stance/Action_Contents/BalanceSquats_Round1.mp3";
            // var _audioFile = "testaudio.mp3";
            _audioFilePath = "";
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            _rootFilePath = rootFolder.Path;


            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
                        new Button {
                            Text = "Play mp3 file",
                            Command = new Command(AudioBtn)
                        }
                    }

            };

        }

        private async void AudioBtn()
        {

            _audioFilePath = await DownloadAudio("demo.mp3", _audioUrl);


            if (_currentlyPlayingAudio)
            {
                DependencyService.Get<IAudio>().Stop();
                _currentlyPlayingAudio = false;
            }
            else
            {
                if(_audioFilePath != "")
                {
                    DependencyService.Get<IAudio>().PlayAudioFileFromFile(_rootFilePath + _audioFilePath);
                    //DependencyService.Get<IAudio>().PlayAudioFileFromResource("BalanceSquats_Round1.mp3");
                    _currentlyPlayingAudio = true;
                }

            }


        }

        private static async Task<string> DownloadAudio(string audioFileName, string audioUrl)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
            IFolder subFolder = await folder.CreateFolderAsync(_ContactProgramDayGuidDB.ToString(), CreationCollisionOption.OpenIfExists);
            IFolder subSubFolder = await subFolder.CreateFolderAsync("Audios", CreationCollisionOption.OpenIfExists);
            IFile file = await subSubFolder.CreateFileAsync(audioFileName, CreationCollisionOption.ReplaceExisting);
            //_videoFilePath = file.Path.Replace(rootFolder.Path, "");
            var audioFilePath = file.Path.Replace(rootFolder.Path, "");

            using (HttpClient client = new HttpClient())
            {
                var uri = new Uri(audioUrl);

                using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    //_totalBytes += int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        using (Stream streamToWriteTo = await file.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }
                }
            }

            return audioFilePath;

        }



    }
}
