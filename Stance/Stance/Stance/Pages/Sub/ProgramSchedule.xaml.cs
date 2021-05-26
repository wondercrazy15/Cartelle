using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using System.Diagnostics;
using static Stance.Utils.MockApiHelper;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Stance.Models.API;
using Stance.Models.Transform;

namespace Stance.Pages.Sub
{
    public partial class ProgramSchedule : ContentPage
    {
        private int _weekNumber;
        private List<APIProgramDay> _ProgramDays;

        public ProgramSchedule(List<APIProgramDay> ProgramDays, int weekNumber)
        {
            InitializeComponent();
            
            _ProgramDays = ProgramDays;
            //BindList();
            NavigationPage.SetBackButtonTitle(this, "");

            _weekNumber = weekNumber;

            WeekText.Text = "WEEK " + weekNumber.ToString();

            //var apiHelper = new MockApiHelper();
            //listView.ItemsSource = apiHelper.GetProgramWeek(weekNumber);
            listView.RowHeight = 80;

            //var box1 = new MR.Gestures.BoxView();
            //box1.Swiped += (s, e) => { HandleSwipe(); };
            var programDays = _ProgramDays.Skip((weekNumber - 1)*7).Take(7);
            var PD = new List<TransformAPIProgramDay>();

            foreach (var pd in programDays)
            {
                var d =  new TransformAPIProgramDay();

                d.GuidCRM = pd.GuidCRM;
                d.Program.GuidCRM = pd.Program.GuidCRM;
                d.Heading = pd.Heading;
                d.SubHeading = pd.SubHeading;
                d.PhotoUrl = pd.PhotoUrl;
                d.SequenceNumber = pd.SequenceNumber;
                d.TotalExercises = pd.TotalExercises;
                d.TimeMinutes = pd.TimeMinutes;
                d.LevelValue = pd.LevelValue;
                d.Level = pd.Level;
                d.DayTypeValue = pd.DayTypeValue;

                if(d.DayTypeValue == 585860001)
                {
                    //Rest Day
                    d.DetailsVisible = false;
                } else
                {
                    //Workout Day
                    d.DetailsVisible = true;
                }

                PD.Add(d);
            }

            listView.ItemsSource = PD;

            if(PD.Count() <= 7)
            {
                //hide next button
                NextBtn.IsVisible = false;
                NextBtn.IsEnabled = false;
            }
        }



        private async void HandleSwipe()
        {
            await Navigation.PushAsync(new ProgramSchedule(_ProgramDays, _weekNumber + 1));
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var programDay = e.Item as TransformAPIProgramDay;
            var dayType = programDay.DayTypeValue;

            if (dayType == 585860000)
            {   
                //Workout Day, go to the ProgramDay Overview Page                
                await Navigation.PushAsync(new ProgramDayOverview(programDay));
            } else
            {
                listView.SelectedItem = null;
            }
        }


    }
}
