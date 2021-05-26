using Microsoft.AppCenter.Crashes;
using Stance.Pages.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stance.Pages.Sub
{
    public partial class InitialLoading : ContentPage
    {
        private const string _PageName = "InitialLoading";
        private Stopwatch _timer = new Stopwatch();
        private int _minTimeMiliseconds = 2000;

        public InitialLoading()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<MainStartingPage>(this, "InitialLoadingFinished", (sender) => { LoadingFinished(); });
            MessagingCenter.Subscribe<Workout_MainPage>(this, "DoneSavingSchedule", (sender) => { ScheduleSaved(); });
            MessagingCenter.Subscribe<Workout_MainPage>(this, "DoneShowingLoading", (sender) => { DoneShowingLoading(); });
            _timer.Start();
        }

        private async void DoneShowingLoading()
        {
            try
            {
                _timer.Stop();
                await Navigation.PopModalAsync(false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "DoneShowingLoading()" } });
            }
        }

        private async void ScheduleSaved()
        {
            try
            {
                _timer.Stop();
                await Navigation.PopModalAsync(false);
                MessagingCenter.Send(this, "RemovedModal");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ScheduleSaved()" } });
            }            
        }

        private async void LoadingFinished()
        {
            try
            {
                var currentTime = (int)_timer.ElapsedMilliseconds;
                if (currentTime < _minTimeMiliseconds)
                {
                    await Task.Delay(Math.Abs(_minTimeMiliseconds - currentTime));
                }
                _timer.Stop();
                await Navigation.PopModalAsync(false);
            } catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "LoadingFinished()" } });
            }
        }

    }
}