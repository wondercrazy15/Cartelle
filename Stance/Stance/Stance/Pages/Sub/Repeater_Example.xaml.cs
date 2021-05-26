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
    public partial class Repeater_Example : ContentPage
    {
        public Repeater_Example()
        {
            InitializeComponent();

            //Workout - List of Exercises

            var layout = new StackLayout {
                Spacing = 0,
            };

            var i = 0;

            while (i < 8)
            {
                var cellStackLayout = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,                    
                };

                var imgStackLayout = new StackLayout
                {
                    Spacing = 0,
                    Opacity = 1,
                };

                var middleStackLayout = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.StartAndExpand,                    
                };

                var controlsStackLayout = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                };

                var repsControlsStackLayout = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                };

                var weightControlsStackLayout = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.End,
                };

                var repsStackLayout = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                };

                var weightStackLayout = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                };


                var moreRepsBtn = new Button
                {
                    Text = "+",
                    TextColor = Color.White,
                    BorderColor = Color.Black,
                    BorderRadius = 12,
                    BorderWidth = 1,
                    HeightRequest = 24,
                    WidthRequest = 24,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.FromHex("#696969"),
                    Margin = new Thickness(0, 20, 0, 0),
                };

                var lessRepsBtn = new Button
                {
                    Text = "-",
                    TextColor = Color.White,
                    BorderColor = Color.Black,
                    BorderRadius = 12,
                    BorderWidth = 1,
                    HeightRequest = 24,
                    WidthRequest = 24,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.FromHex("#696969"),
                    Margin = new Thickness(0, 20, 0, 0),
                };

                var moreWeightBtn = new Button
                {
                    Text = "+",
                    TextColor = Color.White,
                    BorderColor = Color.Black,
                    BorderRadius = 12,
                    BorderWidth = 1,
                    HeightRequest = 24,
                    WidthRequest = 24,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.FromHex("#696969"),
                    Margin = new Thickness(0, 20, 0, 0),
                };

                var lessWeightBtn = new Button
                {
                    Text = "-",
                    TextColor = Color.White,
                    BorderColor = Color.Black,
                    BorderRadius = 12,
                    BorderWidth = 1,
                    HeightRequest = 24,
                    WidthRequest = 24,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.FromHex("#696969"),
                    Margin = new Thickness(0, 20, 0, 0),
                };


                var repsNumText = new Label { Text = "15", FontSize = 20, FontAttributes = FontAttributes.Bold };
                repsNumText.Margin = new Thickness(5, 10, 5, 2);
                var repsText = new Label { Text = "reps", FontSize = 12, };
                repsText.Margin = new Thickness(5, 0, 0, 0);

                var weightNumText = new Label { Text = "30", FontSize = 20, FontAttributes = FontAttributes.Bold };
                weightNumText.Margin = new Thickness(5, 10, 5, 2);
                var weightText = new Label { Text = " lbs", FontSize = 12, };
                weightText.Margin = new Thickness(5, 0, 0, 0);


                var endStackLayout = new StackLayout
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.Center,
                };

                var doneBtn = new Button
                {
                    Text = "✓",
                    TextColor = Color.FromHex("#bdbdbd"),
                    BorderColor = Color.FromHex("#bdbdbd"),
                    BorderRadius = 15,
                    BorderWidth = 3,
                    HeightRequest = 30,
                    WidthRequest = 30,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.End
                };

                var img = new Image
                {
                    Source = "exercise_img.jpg",
                };

                if (i % 2 == 0)
                {
                    cellStackLayout.BackgroundColor = Color.FromHex("#f1f1f1");
                    doneBtn.TextColor = Color.FromHex("#7fbf7f");
                    doneBtn.BorderColor = Color.FromHex("#7fbf7f");
                    doneBtn.Text.ToUpper();
                }

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    img.HeightRequest = 90;
                    img.WidthRequest = 90;
                    cellStackLayout.Padding = new Thickness(20, 10, 15, 10);
                    middleStackLayout.Padding = new Thickness(20, 0, 0, 0);
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    img.HeightRequest = 150;
                    img.WidthRequest = 150;
                    cellStackLayout.Padding = new Thickness(40, 20, 35, 20);
                    middleStackLayout.Padding = new Thickness(40, 0, 0, 0);
                }
                else
                {
                    img.HeightRequest = 125;
                    img.WidthRequest = 125;
                    cellStackLayout.Padding = new Thickness(30, 15, 25, 15);
                    middleStackLayout.Padding = new Thickness(30, 0, 0, 0);
                }

                var headingText = new Label { Text = "Bodyweight Only Lunge", FontSize=13, };
                headingText.Margin = new Thickness(0,0,0,10);

                var subheadingText = new Label { Text = "Intermediate - No Equipment - Strength", TextColor=Color.Gray, FontSize = 10, };


                imgStackLayout.Children.Add(img);

                repsStackLayout.Children.Add(repsNumText);
                repsStackLayout.Children.Add(repsText);

                weightStackLayout.Children.Add(weightNumText);
                weightStackLayout.Children.Add(weightText);

                repsControlsStackLayout.Children.Add(moreRepsBtn);
                repsControlsStackLayout.Children.Add(repsStackLayout);
                repsControlsStackLayout.Children.Add(lessRepsBtn);

                weightControlsStackLayout.Children.Add(moreWeightBtn);
                weightControlsStackLayout.Children.Add(weightStackLayout);
                weightControlsStackLayout.Children.Add(lessWeightBtn);

                controlsStackLayout.Children.Add(repsControlsStackLayout);
                controlsStackLayout.Children.Add(weightControlsStackLayout);

                middleStackLayout.Children.Add(headingText);
                middleStackLayout.Children.Add(subheadingText);

                middleStackLayout.Children.Add(controlsStackLayout);

                endStackLayout.Children.Add(doneBtn);

                cellStackLayout.Children.Add(imgStackLayout);
                cellStackLayout.Children.Add(middleStackLayout);
                cellStackLayout.Children.Add(endStackLayout);

                var seperatorLine = new BoxView
                {
                    HorizontalOptions=LayoutOptions.FillAndExpand,
                    HeightRequest = 1,
                    Color = Color.Black,
                    Opacity = 0.15,
                    Margin = new Thickness(0,0,0,0),
                };

                layout.Children.Add(cellStackLayout);
                layout.Children.Add(seperatorLine);

                i++;
            }

            var scrollView = new ScrollView
            {
                Content = layout,
            };

            Content = scrollView;


        }
    }
}
