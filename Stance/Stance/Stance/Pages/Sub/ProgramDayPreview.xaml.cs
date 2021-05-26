using Newtonsoft.Json;
using Octane.Xam.VideoPlayer;
using PCLStorage;
using Stance.Models.API;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using static Stance.Pages.Sub.ProgramDay_v2;

namespace Stance.Pages.Sub
{
    public partial class ProgramDayPreview : ContentPage
    {

        private HttpClient _client = new HttpClient();
        private Guid _ProgramDayGuid = Guid.Empty;
        List<APIAction> _actions = new List<APIAction>();

        public ProgramDayPreview(Guid ProgramDayGuid)
        {
            InitializeComponent();

            _ProgramDayGuid = ProgramDayGuid;

            GetActions();
            //var api = new MockApiHelper();
            //var actions = api.GetProgramDayActions(ProgramDayGuid);

        }


        private async void GetActions()
        {

            var parentSL = new StackLayout
            {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 0, 10, 0),
                HeightRequest = 50,
                BackgroundColor = Color.Black,
            };

            var startSL = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(0, 0, 0, 0),
            };

            var startLabel = new Label
            {
                Text = "WORKOUT",
                TextColor = Color.White,
                FontSize = 20,
                FontFamily = "AvenirNextCondensed-Bold",
                HorizontalTextAlignment = TextAlignment.Center,
            };

            var exitTopSL = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            var exitBtn = new Button
            {
                Image = (FileImageSource)ImageSource.FromFile("Delete-32.png"),
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = 26,
                WidthRequest = 26,
                //BackgroundColor = Color.Aqua,
            };
            exitBtn.Clicked += (s, e) => ExitBtn_Clicked();

            startSL.Children.Add(startLabel);
            exitTopSL.Children.Add(exitBtn);

            parentSL.Children.Add(startSL);
            parentSL.Children.Add(exitTopSL);

            listOfActions.Children.Add(parentSL);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(App._absoluteUri, App._actionsProgramDayUri + _ProgramDayGuid);
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");

            HttpResponseMessage response = await _client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var json = await content.ReadAsStringAsync();

                var actions = JsonConvert.DeserializeObject<List<APIAction>>(json);
                _actions = actions;

                foreach (var action in _actions)
                {
                    if (action.ContentTypeValue == 585860000)
                    {
                        //Exercise - Rep Based 585860000
                        // ConstructExerciseRepBasedCell(action);
                        ConstructExerciseBasedCell(action);
                    }
                    else if (action.ContentTypeValue == 585860001)
                    {
                        //Exercise - Time Based 585860001
                        ConstructExerciseBasedCell(action);
                    }
                    else if (action.ContentTypeValue == 585860002)
                    {
                        //Rest Time
                        ConstructRestTimeBasedCell(action);
                    }
                    else if (action.ContentTypeValue == 585860004)
                    {
                        //Seperator
                        ConstructSeperatorBasedCell(action);
                    }
                    else if (action.ContentTypeValue == 585860003)
                    {
                        //Stretch Time 585860003
                        ConstructExerciseBasedCell(action);
                        //ConstructStretchTimeBasedCell(action);
                    }
                }



                //ADD to END - DONE button
                var endSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#80bd01"),
                    HeightRequest = 50,
                };

                var endLabel = new Label
                {
                    Text = "DONE",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    FontSize = 22,
                    FontFamily = "AvenirNextCondensed-Bold",
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                var workoutDone = new WorkoutDone
                {
                    //Id = Id,
                    EndSL = endSL
                };

                var tapGestureRecognizer = new TapGestureRecognizer
                {
                    Command = new Command<WorkoutDone>(WorkoutComplete),
                    CommandParameter = workoutDone,
                };
                endSL.GestureRecognizers.Add(tapGestureRecognizer);

                endSL.Children.Add(endLabel);
                listOfActions.Children.Add(endSL);
            }




        }

        private async void WorkoutComplete(WorkoutDone obj)
        {
            obj.EndSL.BackgroundColor = Color.Black;
            //Save to LocalDB Async

            await Navigation.PopModalAsync();

        }


        private void ConstructExerciseBasedCell(APIAction action)
        {
            var parentSL = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.FromHex("#f1f1f1"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            var cellStackLayout = new StackLayout
            {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            var controlsStackLayout = new StackLayout
            {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(0, 0, 10, 0),
            };

            var img = new Image
            {
                Aspect = Aspect.AspectFill,
            };

            var headingText = new Label
            {
                FontSize = 13,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start,
                FontFamily = "PingFangTC-Regular"
            };


            img.Source = action.ActionContent.PhotoUrl;
            headingText.Text = action.ActionContent.Heading;

            var headingSL = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Padding = new Thickness(7, 0, 0, 0),
            };

            headingSL.Children.Add(headingText);

            var exerciseCellInfo = new ExerciseVideoPlayerCell
            {
                num = action.SequenceNumber,
                videoUrl = action.ActionContent.VideoUrl,
                cellSL = cellStackLayout,
                IsComplete = false,
                TimeSeconds = action.TimeSeconds,
                ContentTypeValue = action.ContentTypeValue,
            };

            //Exercise - Rep Based 585860000
            //Stretch Time 585860003
            //Exercise - Time Based 585860001

            if (action.ContentTypeValue == 585860000)
            {
                var repsControlsStackLayout = new StackLayout
                {
                    Spacing = 0,
                    WidthRequest = 60,
                };

                var weightControlsStackLayout = new StackLayout
                {
                    Spacing = 0,
                    WidthRequest = 60,
                };

                var repsStackLayout = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Horizontal,
                };

                var weightStackLayout = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Horizontal,
                };


                var repsNumText = new Label { FontSize = 14, FontAttributes = FontAttributes.Bold, FontFamily = "PingFangTC-Semibold", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                var repsText = new Label { Text = "reps", FontSize = 12, FontFamily = "PingFangTC-Regular", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

               // var weightNumText = new Label { FontSize = 14, FontAttributes = FontAttributes.Bold, FontFamily = "PingFangTC-Semibold", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                //var weightText = new Label { Text = "lbs", FontSize = 12, FontFamily = "PingFangTC-Regular", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

                repsNumText.Text = action.NumberOfReps.ToString();
                //weightNumText.Text = action.WeightLbs.ToString();

                exerciseCellInfo.repsLabel = repsNumText;
                //exerciseCellInfo.weightLabel = weightNumText;


                repsStackLayout.Children.Add(repsNumText);
                repsStackLayout.Children.Add(repsText);

                //weightStackLayout.Children.Add(weightNumText);
                //weightStackLayout.Children.Add(weightText);

                repsControlsStackLayout.Children.Add(repsStackLayout);

               // weightControlsStackLayout.Children.Add(weightStackLayout);

                controlsStackLayout.Children.Add(repsControlsStackLayout);
                //controlsStackLayout.Children.Add(weightControlsStackLayout);
            }
            else if (action.ContentTypeValue == 585860003 || action.ContentTypeValue == 585860001)
            {
                //Time Base Exercise or Stretch
                var timeSL = new StackLayout
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                };

                var timeText = new Label
                {
                    FontSize = 14,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    FontFamily = "PingFangTC-Semibold",
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                };


                TimeSpan time = TimeSpan.FromSeconds(action.TimeSeconds);
                string str = time.ToString(@"m\:ss");
                timeText.Text = str;


                exerciseCellInfo.TimeSeconds = action.TimeSeconds;
                controlsStackLayout.WidthRequest = 140;
                timeSL.Children.Add(timeText);
                controlsStackLayout.Children.Add(timeSL);

            }

            if (Device.Idiom == TargetIdiom.Phone)
            {
                img.HeightRequest = 100;
                img.WidthRequest = 100;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                img.HeightRequest = 150;
                img.WidthRequest = 150;

            }
            else
            {
                img.HeightRequest = 100;
                img.WidthRequest = 100;

            }

            cellStackLayout.Children.Add(img);
            headingText.WidthRequest = 90;
            cellStackLayout.Children.Add(headingSL);
            cellStackLayout.Children.Add(controlsStackLayout);
            parentSL.Children.Add(cellStackLayout);

            parentSL.Children.Add(cellStackLayout);

            listOfActions.Children.Add(parentSL);

        }

        private void ConstructRestTimeBasedCell(APIAction action)
        {
            var seperatorSL = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.White,
                HeightRequest = 28,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(40, 0, 40, 0),
                Orientation = StackOrientation.Horizontal,
            };

            var heading = new Label
            {
                Text = action.ActionContent.Heading,
                TextColor = Color.Black,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "PingFangTC-Semibold",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand,

            };

            TimeSpan time = TimeSpan.FromSeconds(action.TimeSeconds);
            string str = time.ToString(@"m\:ss");

            var timeText = new Label
            {
                Text = str,
                TextColor = Color.Black,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "PingFangTC-Semibold",
                VerticalOptions = LayoutOptions.Center,
            };

            seperatorSL.Children.Add(heading);
            seperatorSL.Children.Add(timeText);

            listOfActions.Children.Add(seperatorSL);
        }

        private void ConstructSeperatorBasedCell(APIAction action)
        {
            var seperatorSL = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.Gray,
                HeightRequest = 28,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(40, 0, 0, 0),
            };

            var heading = new Label
            {
                Text = action.ActionContent.Heading,
                TextColor = Color.White,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "PingFangTC-Semibold",
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            seperatorSL.Children.Add(heading);
            listOfActions.Children.Add(seperatorSL);
        }


        private async void ExitBtn_Clicked()
        {
            await Navigation.PopModalAsync();
        }


    }
}
