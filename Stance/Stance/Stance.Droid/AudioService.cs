using System;

using Stance.Droid;
using Android.Media;
using Android.Content.Res;
using Stance.Utils;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace Stance.Droid
{
    public class AudioService : IAudio
    {
        private MediaPlayer _MediaPlayer = new MediaPlayer();
        MediaPlayer player;
        public AudioService()
        {

        }

        public void Pause()
        {
            if (player == null)
                return;
            player.Pause();
        }

        public void Resume()
        {
            if (player == null)
                return;
            if(!player.IsPlaying)
            player.Start();
        }

        public void PlayAudioFileFromFile(string fileName)
        {
            try
            {
                player = new MediaPlayer();
                //var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
                player.Prepared += (s, e) =>
                {
                    player.Start();
                };
                player.SetDataSource(fileName);
                //player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                //IF this does not work, try player.SetDataSource(fd.FileDescriptor);
                player.Prepare();
                _MediaPlayer = player;
            }
            catch (Exception ex)
            {

            }

        }

        public void PlayAudioFileFromResource(string fileName)
        {
            try
            {
                player = new MediaPlayer();
                var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
                player.Prepared += (s, e) =>
                {
                    player.Start();
                };
                player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length); //IF this does not work, try player.SetDataSource(fd.FileDescriptor);
                player.Prepare();
                _MediaPlayer = player;
            }
            catch (Exception ex)
            {

            }
        }

        public void Stop()
        {

            if (_MediaPlayer == null)
                return;

            _MediaPlayer.Stop();
        }

    }
}