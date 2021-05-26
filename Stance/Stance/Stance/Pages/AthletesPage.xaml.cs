using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

using System.Net.Http;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using Stance.Models;
using Stance.Pages.Sub;

namespace Stance.Pages
{

    public partial class AthletesPage : ContentPage
    {
        List<Athlete> Athletes = new List<Athlete>();
        const string url = "http://stanceathletes.com/api/athletes"; 
            //http://jsonplaceholder.typicode.com/posts
        private HttpClient _client = new HttpClient();
        private ObservableCollection<Athlete> _athletes;

        public AthletesPage()
        {
            InitializeComponent();
            //BindingContext = vm = new AthletesViewModel();

            NavigationPage.SetTitleIcon(this, "heart.png");

            ButtonGetAthletes.Clicked += async (sender, e) =>
            {
                ButtonGetAthletes.IsEnabled = false;
                var content = await _client.GetStringAsync(url);
                var athletes = JsonConvert.DeserializeObject<List<Athlete>>(content);

                _athletes = new ObservableCollection<Athlete>(athletes);
                AthleteList.ItemsSource = _athletes;
                ButtonGetAthletes.IsEnabled = true;
            };

            //var imgSource = new UriImageSource { Uri = new Uri("http://lorempixel.com/1920/1080/sports/7/") };
            //imgSource.CachingEnabled = true;
            //imgSource.CacheValidity = TimeSpan.FromHours(8);

            //image.Source = imgSource;
            //image.Aspect = Aspect.AspectFill;
        }

        async void Handle_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new AthleteOverview(""));
        }

        //async void OnUpcomingAppointmentsButtonClicked(object sender, EventArgs e)
        //{      <Button Text="Upcoming Appointments" Clicked="OnUpcomingAppointmentsButtonClicked" VerticalOptions="CenterAndExpand" />

        //    await Navigation.PushAsync(new NavigationPage(new WorkoutPage()));
        //}



    }

}
