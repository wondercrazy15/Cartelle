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
    public partial class AthleteOverview : ContentPage
    {
        private string _id = String.Empty;
        private string _videoUrl = String.Empty;
        private string _facebookUrl = String.Empty;
        private string _instagramUrl = String.Empty;
        private string _youtubeUrl = String.Empty;


        public AthleteOverview(string id)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            _id = id; 
            _videoUrl = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/3_Exercise/jumping_jacks_mini_loop.mp4";
            _facebookUrl = "http://facebook.com";
            _instagramUrl = "http://instagram.com";
            _youtubeUrl = "http://youtube.com";

            var apiHelper = new BaseApiHelper();
            //listView.ItemsSource = apiHelper.GetAthletePrograms();
            //listView.RowHeight = 150;

            var athlete = apiHelper.GetAthlete("1");

            //athleteInfo.BindingContext = athlete;
            //Title = athlete.Name;

            var i = 0;

            while (i < 4)
            {
                var stackLayout = new StackLayout
                {
                    HeightRequest = 100,
                    WidthRequest = 100,
                    BackgroundColor = Color.White,
                    Orientation = StackOrientation.Horizontal,                
                    
                };

                var imgStackLayout = new StackLayout
                {

                };

                var img = new Image
                {
                    Source = "exercise_img.jpg",
                    HeightRequest = 100,
                    WidthRequest = 100
                };

                imgStackLayout.Children.Add(img);

                var middleStackLayout = new StackLayout
                {
                    Spacing = 0,
                    Padding = new Thickness(10,10,10,10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand,                   

                };

                var titleLabel = new Label
                {
                    Text="Get Ripped Fast",
                    TextColor = Color.Black,
                    FontSize = 13,
                    Margin = new Thickness(0,0,0,10),

                };

                var descLabel = new Label
                {
                    Text = "Join me on this 8 week program to get ripped fast. This is your chance to get in great shape and have fun doing it!",
                    TextColor = Color.Gray,
                    FontSize = 10,
                };

                middleStackLayout.Children.Add(titleLabel);
                middleStackLayout.Children.Add(descLabel);

                var endStackLayout = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.End,
                    Padding = new Thickness(5,5,5,5)

                };

                var btn = new Button
                {
                    Text=">",
                    TextColor = Color.Gray,
                    BorderColor = Color.Gray,
                    BorderRadius = 15,
                    BorderWidth = 3,
                    HeightRequest = 30,
                    WidthRequest = 30,                   

                };

                btn.Clicked += (s, e) => {
                    // handle the tap
                    ProgCell_Clicked("");
                };

                endStackLayout.Children.Add(btn);

                stackLayout.Children.Add(imgStackLayout);
                stackLayout.Children.Add(middleStackLayout);
                stackLayout.Children.Add(endStackLayout);

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => {
                    // handle the tap
                    ProgCell_Clicked("");
                };

                stackLayout.GestureRecognizers.Add(tapGestureRecognizer);


                listOfPrograms.Children.Add(stackLayout);
                i++;
            }


        }

        async void ProgCell_Clicked(string progId = null)
        {
            await Navigation.PushAsync(new ProgramOverview(""));
        }



        void Handle_IDClicked(object sender, EventArgs e)
        {
            DisplayAlert("ID is:", _id, "OK");

        }

        //async void Handle_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new ProgramOverview());
        //}

        async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            //var program = e.SelectedItem as Program;
            //var id = program.Id;
            //MainContentStackLayout.IsEnabled = false;
            await Navigation.PushAsync(new ProgramOverview(""));
            //MainContentStackLayout.IsEnabled = true;
        }


        async void WatchVideoBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new WatchVideoPage(_videoUrl));
        }

        async void Facebook_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new GenericWebviewPage(_facebookUrl));
        }

        async void Instagram_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new GenericWebviewPage(_instagramUrl));
        }

        async void Youtube_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new GenericWebviewPage(_youtubeUrl));
        }

        //async void NavigateToNextPageAsync(string id)
        //{
        //    MainContentStackLayout.IsEnabled = false;
        //    await Navigation.PushAsync(new ProgramOverview(id));
        //    MainContentStackLayout.IsEnabled = true;
        //}

    }
}
