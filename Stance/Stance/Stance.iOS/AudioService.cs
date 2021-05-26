using System;
using Xamarin.Forms;
using Stance.iOS;
using System.IO;
using Foundation;
using AVFoundation;
using Stance.Utils;
using AudioToolbox;

[assembly: Dependency (typeof (AudioService))]
namespace Stance.iOS
{
    public class AudioService : NSObject, IAudio, IAVAudioPlayerDelegate
    {
        private AVAudioPlayer _Player;

        public AudioService()
        {
        }


        public void Dispose()
        {

        }

        public void Stop()
        {
            if (_Player == null)
                return;

            _Player.Stop();

        }

        public void Pause()
        {
            if (_Player == null)
                return;

            _Player.Pause();

        }

        public void PlayAudioFileFromResource(string fileName)
        {
            string FileName = fileName;
            string FilePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(FileName), Path.GetExtension(FileName));
            var url = NSUrl.FromString(FilePath);
            var _player = AVAudioPlayer.FromUrl(url);
            _player.Volume = 1.0f;
            _player.Delegate = this;
            //_player.Volume = 100f; //Causes distortion
            _player.PrepareToPlay();
            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => { _player = null; };
            _player.Play();
            _Player = _player;
        }

        public void PlayAudioFileFromFile(string fileName)
        {
            string sFilePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
            var url = NSUrl.FromFilename(fileName);
            var mp3 = AudioToolbox.AudioSource.Open(url, AudioFilePermission.Read, AudioFileType.MP3);
            if (mp3 != null)
            {
                //make sure the file path exists to the audio file
                var _player = AVAudioPlayer.FromUrl(url);
                _player.Volume = 1.0f;
                _player.Delegate = this;
                //_player.Volume = 100f; //Causes distortion
                _player.PrepareToPlay();
                _player.FinishedPlaying += (object sender, AVStatusEventArgs e) =>
                {
                    _player = null;
                };
                _player.Play();
                _Player = _player;
            }
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }
    }
}