using CoreGraphics;
using Stance.Models;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class ProgramOverview : ContentPage
    {
        private string _id = String.Empty;
        private string _videoUrl = String.Empty;

        public ProgramOverview(string id)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            _id = id;
            var apiHelper = new BaseApiHelper();
            var program = apiHelper.GetAthleteProgram("1");
            Title = program.Name;

            _videoUrl = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/3_Exercise/jumping_jacks_mini_loop.mp4";

            var i = 0;

            while (i < 8)
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
                    Padding = new Thickness(10, 10, 10, 10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand,

                };

                var titleLabel = new Label
                {
                    Text = "Week " + (i + 1),
                    TextColor = Color.Black,
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 0, 10),

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
                    Padding = new Thickness(5, 5, 5, 5)

                };

                var btn = new Button
                {
                    Text = ">",
                    TextColor = Color.Gray,
                    BorderColor = Color.Gray,
                    BorderRadius = 15,
                    BorderWidth = 3,
                    HeightRequest = 30,
                    WidthRequest = 30

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
            await Navigation.PushAsync(new WorkoutOverview(""));
        }

        async void Handle_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProgramPurchase());
        }

        async void PurchaseBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginSignUpPage(), true);
        }

        async void SampleBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginSignUpPage(), false);
        }

        async void WatchVideoBtn_Clicked(object sender, EventArgs e)
        {
            //var audioString = "http://www.noiseaddicts.com/samples_1w72b820/304.mp3";

            //DependencyService.Get<IAudio>().PlayAudioFile("304.mp3");
            //DependencyService.Get<IAudio>().PlayAudioFile(audioString);

            await Navigation.PushModalAsync(new WatchVideoPage(_videoUrl));
        }

    }
}
