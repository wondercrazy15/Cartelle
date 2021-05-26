using Stance.Pages.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octane.Xam.VideoPlayer;

using Xamarin.Forms;
using Stance.Models;
using System.Threading;
using PCLStorage;

namespace Stance.Pages.Main
{
    public partial class Samples_MainPage : ContentPage
    {

        public class ExerciseVideoPlayerCell
        {
            public string videoUrl { get; set; }
            public int num { get; set; }
            public StackLayout videoSL { get; set; }
            public StackLayout overlaySL { get; set; }
            public RelativeLayout videoRL { get; set; }
        }

        VideoPlayer videoPlayer = new VideoPlayer();

        bool isVideoPlayerOpen = false;
        int openVideoPlayerId = -1;
        //StackLayout openSL = new StackLayout();
        RelativeLayout _openVideoRL = new RelativeLayout();

        ExerciseVideoPlayerCell _openExerciseCellInfo = new ExerciseVideoPlayerCell();

        double _videoPlayerHeight = 225;


        public Samples_MainPage()
        {
            InitializeComponent();

            //strategy, when they log in set to true, and store credentials in keychain
            //when a webapi says unathorized set to false
            //on each Auth page check if they are logged in or not and redirect to login if needed
            App.Current.Properties["IsLoggedIn"] = true; //and check property throughout app
            Application.Current.SavePropertiesAsync();

            NavigationPage.SetBackButtonTitle(this, "");

            videoPlayer.DisplayControls = false;
            videoPlayer.FillMode = Octane.Xam.VideoPlayer.Constants.FillMode.ResizeAspectFill;
            videoPlayer.Source = "";
            videoPlayer.Repeat = true;
            videoPlayer.AutoPlay = true;

            //videoPlayer.Source = VideoSource.FromResource("resize1.mp4");
            videoPlayer.Volume = 0;
            videoPlayer.VerticalOptions = LayoutOptions.CenterAndExpand;
            //videoPlayer.HorizontalOptions = LayoutOptions.CenterAndExpand;
            videoPlayer.HeightRequest = _videoPlayerHeight;

            var toolItem1 = new ToolbarItem
            {
                Icon = "user_26.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };

            toolItem1.Clicked += (s, e) =>
            {
                ProfileBtn_Clicked(s, e);
            };

            var toolItem2 = new ToolbarItem
            {
                Icon = "Message_26.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };

            toolItem2.Clicked += (s, e) =>
            {
                //InboxBtn_Clicked(s, e);
            };

            ToolbarItems.Add(toolItem1);
            ToolbarItems.Add(toolItem2);

            var layout = new StackLayout
            {
                Spacing = 0,
            };

            var i = 0;

            while (i < 8)
            {
                var parentSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#f1f1f1"),

                };

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
                    Image = (FileImageSource)ImageSource.FromFile("arrow_up.png"),
                    HeightRequest = 32,
                    WidthRequest = 32,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                };


                var lessRepsBtn = new Button
                {
                    Image = (FileImageSource)ImageSource.FromFile("arrow_down.png"),
                    HeightRequest = 32,
                    WidthRequest = 32,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                };

                var moreWeightBtn = new Button
                {
                    Image = (FileImageSource)ImageSource.FromFile("arrow_up.png"),
                    HeightRequest = 32,
                    WidthRequest = 32,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                };

                var lessWeightBtn = new Button
                {
                    Image = (FileImageSource)ImageSource.FromFile("arrow_down.png"),
                    HeightRequest = 32,
                    WidthRequest = 32,
                    VerticalOptions = LayoutOptions.Center,
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
                else
                {
                    cellStackLayout.BackgroundColor = Color.FromHex("#ffffff");

                }

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    img.HeightRequest = 90;
                    img.WidthRequest = 90;
                    cellStackLayout.Padding = new Thickness(0, 10, 15, 10);
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
                    cellStackLayout.Padding = new Thickness(10, 15, 25, 15);
                    middleStackLayout.Padding = new Thickness(30, 0, 0, 0);
                }

                var headingText = new Label { Text = "Bodyweight Only Lunge", FontSize = 13, };
                headingText.Margin = new Thickness(0, 0, 0, 10);

                var subheadingText = new Label { Text = "Intermediate - No Equipment - Strength", TextColor = Color.Gray, FontSize = 10, };


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

                parentSL.Children.Add(cellStackLayout);

                var videoRL = new RelativeLayout
                {
                    HeightRequest = 0,
                    Opacity = 1,
                    BackgroundColor = Color.White,
                };

                var overlaySL = new StackLayout
                {
                    Spacing = 0,
                    Opacity = 1,
                    BackgroundColor = Color.White,
                };

                var videoSL = new StackLayout
                {
                    Spacing = 0,
                    //VerticalOptions = LayoutOptions.CenterAndExpand,

                    //HeightRequest = 0,
                    //BackgroundColor = Color.FromHex("#f1f1f1"),
                };

                videoRL.Children.Add(videoSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));
                videoRL.Children.Add(overlaySL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));



                var exerciseCellInfo = new ExerciseVideoPlayerCell
                {
                    num = i,
                    videoUrl = "http://vjs.zencdn.net/v/oceans.mp4",
                    videoSL = videoSL,
                    overlaySL = overlaySL,
                    videoRL = videoRL
                };

                var tapGestureRecognizer = new TapGestureRecognizer
                {
                    Command = new Command<ExerciseVideoPlayerCell>(PlayExerciseVideo),
                    CommandParameter = exerciseCellInfo,
                };

                img.GestureRecognizers.Add(tapGestureRecognizer);

                var seperatorLine = new BoxView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 1,
                    Color = Color.Black,
                    Opacity = 0.15,
                    Margin = new Thickness(0, 0, 0, 0),
                };

                parentSL.Children.Add(cellStackLayout);
                parentSL.Children.Add(videoRL);

                layout.Children.Add(parentSL);

                layout.Children.Add(seperatorLine);

                i++;
            }

            var scrollView = new ScrollView
            {
                Content = layout,
            };


            Content = scrollView;



        }


        private async void PlayExerciseVideo(ExerciseVideoPlayerCell e)
        {
            //move video player to 
            e.videoSL.Children.Add(videoPlayer);
            var layout = e.videoRL;
            var overlay = e.overlaySL;

            var source = videoPlayer;

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var file = rootFolder.Path;
            var filePath = file + "/Programs/progID/workoutID/video1.mp4";

            //videoPlayer.Source = VideoSource.FromResource("resize1.mp4");
            if (e.num % 2 == 0)
            {
                filePath = file + "/Programs/progID/workoutID/video2.mp4";

                //videoPlayer.Source = VideoSource.FromFile(filePath);

                //videoPlayer.Source = VideoSource.FromResource("t2.mp4");
                //videoPlayer.Source = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/2_Exercise/lunge_mini_loop.mp4";
            }
            else
            {

                //videoPlayer.Source = e.videoUrl;
                //videoPlayer.Source = VideoSource.FromResource("resize1.mp4");
                //videoPlayer.Source = VideoSource.FromFile(filePath);
                //videoPlayer.Source = VideoSource.FromUri("http://vjs.zencdn.net/v/oceans.mp4");

            }
            videoPlayer.Source = VideoSource.FromUri("http://vjs.zencdn.net/v/oceans.mp4");
            //videoPlayer.Source = VideoSource.FromFile(filePath);

            double startingHeight; // the layout's height when we begin animation
            double endingHeight; // final desired height of the layout
            uint rate = 16; // pace at which aniation proceeds
            uint length = 200; // one second animation

            if (isVideoPlayerOpen)
            {
                if (openVideoPlayerId == e.num)
                {
                    //currently: OPEN - need to CLOSE
                    //CLOSE currently selected 
                    CloseExerciseVideo(e);
                    openVideoPlayerId = -1;
                }
                else
                {
                    //currently: OPEN - need to CLOSE
                    //CLOSE currently open 

                    var openExerciseCellInfo = new ExerciseVideoPlayerCell
                    {
                        num = _openExerciseCellInfo.num,
                        videoUrl = _openExerciseCellInfo.videoUrl,
                        videoSL = _openExerciseCellInfo.videoSL,
                        overlaySL = _openExerciseCellInfo.overlaySL,
                        videoRL = _openExerciseCellInfo.videoRL
                    };

                    CloseExerciseVideo(openExerciseCellInfo);

                    //OPEN currently selected
                    OpenExerciseVideo(e);
                    openVideoPlayerId = e.num;

                }

            }
            else if (!isVideoPlayerOpen)
            {
                //currently: CLOSED - need to OPEN
                //OPEN currently selected
                OpenExerciseVideo(e);
                openVideoPlayerId = e.num;

            }


        }

        private async void CloseExerciseVideo(ExerciseVideoPlayerCell e)
        {

            double startingHeight; // the layout's height when we begin animation
            double endingHeight; // final desired height of the layout
            uint rate = 16; // pace at which aniation proceeds
            uint length = 400; // one second animation
            var overlay = e.overlaySL;

            //currently: OPEN - need to CLOSE
            //openVideoRL.Opacity = 1;
            Action<double> callback = input => { _openVideoRL.HeightRequest = input; }; // update the height of the layout with this callback
            startingHeight = _openVideoRL.Height;
            endingHeight = 0;
            _openVideoRL.Animate("invis", callback, startingHeight, endingHeight, rate, length, Easing.SinOut);
            isVideoPlayerOpen = false;
            //await overlay.FadeTo(1, 200, Easing.CubicIn);
            videoPlayer.Pause();


        }

        private async void OpenExerciseVideo(ExerciseVideoPlayerCell e)
        {
            var layout = e.videoRL;
            var overlay = e.overlaySL;

            double startingHeight; // the layout's height when we begin animation
            double endingHeight; // final desired height of the layout
            uint rate = 16; // pace at which aniation proceeds
            uint length = 200; // one second animation
            //currently: CLOSED - need to OPEN
            videoPlayer.Play();
            layout.Opacity = 1;
            overlay.Opacity = 1;
            Action<double> callback = input => { layout.HeightRequest = input; }; // update the height of the layout with this callback
            startingHeight = 0;
            endingHeight = _videoPlayerHeight;
            layout.Animate("invis", callback, startingHeight, endingHeight, rate, length, Easing.CubicIn);
            isVideoPlayerOpen = true;
            await overlay.FadeTo(0.1, 500, Easing.CubicIn);
            _openVideoRL = layout;
            _openExerciseCellInfo = e;

        }

        async void ProfileBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new PersonalProfile());
        }


    }
}
