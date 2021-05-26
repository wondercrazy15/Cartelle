using Stance.Models;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class WorkoutOverview : ContentPage
    {
        private string _id = String.Empty;

        public WorkoutOverview(string id)
        {
            InitializeComponent();
            _id = id;

            var apiHelper = new MockApiHelper();
            listView.ItemsSource = apiHelper.GetWorkoutExercises("1");
            listView.RowHeight = 100;

            var workout = apiHelper.GetProgramWorkout("1");

            workoutInfo.BindingContext = workout;
            Title = workout.Name;
            NavigationPage.SetBackButtonTitle(this, "");


        }

        async void Handle_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ExercisePage());
        }

        async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var exercise = e.SelectedItem as Exercise;
            var id = exercise.Id;
            MainContentStackLayout.IsEnabled = false;
            await Navigation.PushModalAsync(new ExercisePage(id.ToString()));
            MainContentStackLayout.IsEnabled = true;
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            //var program = e.Item as Program;
            //var id = program.Id;
            //MainContentStackLayout.IsEnabled = false;
            //await Navigation.PushAsync(new ProgramOverview(id));
            //MainContentStackLayout.IsEnabled = true;
        }


    }
}
