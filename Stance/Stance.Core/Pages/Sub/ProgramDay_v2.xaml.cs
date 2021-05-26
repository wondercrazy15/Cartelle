using Acr.UserDialogs;
using BranchXamarinSDK;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Octane.Xamarin.Forms.VideoPlayer.Events;
using Stance.Models;
using Stance.Models.Local;
using Stance.Models.LocalDB;
using Stance.Models.Transform;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stance.Pages.Sub
{
    public partial class ProgramDay_v2 : BaseContentPage
    {
        
        private const string _PageName = "Workout";
        private LocalDBProgramDay _ProgramDay = new LocalDBProgramDay();
        private Guid _ContactProgramDayGuid;
        private MediaElement _videoPlayer = new MediaElement();
        private RelativeLayout _openVideoRL = new RelativeLayout();
        //StackLayout _openVideoRL = new StackLayout();
        private ExerciseVideoPlayerCell _openExerciseCellInfo = new ExerciseVideoPlayerCell();
        private bool _isVideoPlayerOpen = false;
        private bool _isVideoPlayerTranslationInProgress = false;
        private double _videoPlayerHeight = 400;//225;
        private double _videoPlayerHeight1 = 400;
        private double _videoPlayerHeight2 = 225;
        private int _TimerSeconds = 0;
        private bool _TimerIsActive = false;
        private Label _TimerLabel;
        private bool _StopTimer = false;
        private ExerciseVideoPlayerCell _timingExerciseCell = new ExerciseVideoPlayerCell();
        private ExerciseVideoPlayerCell _previousExerciseCell = new ExerciseVideoPlayerCell();
        private ExerciseVideoPlayerCell _currentExerciseCell = new ExerciseVideoPlayerCell();
        private ExerciseVideoPlayerCell _nextExerciseCell = new ExerciseVideoPlayerCell();
        private ExerciseVideoPlayerCell _currentExerciseCellPlayingAudio = new ExerciseVideoPlayerCell();
        private bool _currentlyInAudioTransition = false;
        private ExerciseVideoPlayerCell _currentExerciseCellPlayingVideo = new ExerciseVideoPlayerCell();
        private bool _IsReturningFromFullScreen = false;
        private bool _isOnFullScreen = false;
        private bool _continueVideoFlow = false;
        private bool _stopAllAudio = false;
        private bool _isOnPage = false;
        double _maxCallsToProgressBar = 0D;
        double _numberOfTimerProgressBarCalls = 0D;
        double _milisecondsToCallProgressBarUI = 1000D;
        int count = 1;
        private ExerciseVideoPlayerCell _currentExerciseCellJ00 = new ExerciseVideoPlayerCell();
        private ExerciseVideoPlayerCell _currentExerciseCellJ11 = new ExerciseVideoPlayerCell();


        public ProgramDay_v2(Guid ContactProgramDayGuid)
        {
            try
            {
                InitializeComponent();
                On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
                MyScrollView.IsEnabled = true;
                //MessagingCenter.Subscribe<Workout_MainPage>(this, "ExitedWorkout", (sender) => { OnExitWorkoutPage(); });
                //MessagingCenter.Subscribe<WorkoutVideo>(this, "ReturnFromFullScreen", (sender) => { IsReturingFromFullScreen_PlayVideo(); });
                MessagingCenter.Subscribe<WorkoutSurvey>(this, "WorkoutSurveyComplete", (sender) => { LeavePage(); });
                MessagingCenter.Subscribe<App>(this, "OnResume", (sender) => { OnResume(); });

                _maxCallsToProgressBar = 1000D / _milisecondsToCallProgressBarUI; //used for progress bar
                _isOnPage = true;
                _currentExerciseCellPlayingAudio = null;
                _ContactProgramDayGuid = ContactProgramDayGuid;

               // _videoPlayer.PlayRequested += _videoPlayer_PlayRequested;// VideoPlayerPlaying;
                                                                         // _videoPlayer.Failed += VideoPlayerFailed;
                                                                         // _videoPlayer.DisplayControls = false;
                                                                         //_videoPlayer.FillMode = Octane.Xamarin.Forms.VideoPlayer.Constants.FillMode.ResizeAspectFill;
                                                                         //videoPlayer.Repeat = true;
                                                                         //videoPlayer.AutoPlay = true;
                                                                         _videoPlayer.Volume = 0;

                
                _videoPlayer.AutoPlay = true;
                _videoPlayer.Aspect = Aspect.AspectFill;
                _videoPlayer.IsLooping = true;
                _videoPlayer.VerticalOptions = LayoutOptions.CenterAndExpand;
                _videoPlayer.HeightRequest = _videoPlayerHeight;
                _videoPlayer.BackgroundColor = Color.Black;

                MessagingCenter.Subscribe<App>(this, "OnSleep", (sender) => { OnSleep(); });
               
                var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
                if (isOfXFamily)
                {
                    var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                    var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                    var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();

                    if (isDeviceIphoneXorXS)
                    {
                        _videoPlayerHeight2 = 320;
                        _videoPlayerHeight1 = 500;
                        _videoPlayerHeight = 500;
                        _videoPlayer.HeightRequest = _videoPlayerHeight;
                    }
                    else if (isDeviceIphoneXSMax || isDeviceIphoneXR)
                    {
                        _videoPlayerHeight2 = 350;
                        _videoPlayerHeight1 = 600;
                        _videoPlayerHeight = 600;
                       _videoPlayer.HeightRequest = _videoPlayerHeight;
                    }
                    else
                    {
                        _videoPlayerHeight2 = 320;
                        _videoPlayerHeight1 = 500;
                        _videoPlayerHeight = 500;
                        _videoPlayer.HeightRequest = _videoPlayerHeight;
                    }
                }
                else if (Device.Idiom == TargetIdiom.Phone)
                {
                    //_videoPlayerHeight = 400;// 225;
                    _videoPlayer.HeightRequest = _videoPlayerHeight;
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    _videoPlayerHeight2 = 600;
                    _videoPlayerHeight1 = 800;
                    _videoPlayerHeight = 800;
                    _videoPlayer.HeightRequest = _videoPlayerHeight;
                }
                else
                {
                    //_videoPlayerHeight = 400;
                    _videoPlayer.HeightRequest = _videoPlayerHeight;
                }

                try
                {
                    var ContactProgramDay = _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDayGuid).FirstOrDefaultAsync().Result;

                    if (ContactProgramDay != null)
                    {
                        var ProgramDay = _connection.Table<LocalDBProgramDay>().Where(x => x.Id == ContactProgramDay.ProgramDayId).FirstOrDefaultAsync().Result;
                        if (ProgramDay != null)
                        {
                            _ProgramDay = ProgramDay;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = ex.ToString();
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ProgramDay_v2" } });
                }

                GetActions();
                
            }
            catch (Exception ex)
            {

            }
        }

       

        private void _videoPlayer_PlayRequested(object sender, EventArgs e)
        {
            try
            {
                if (_currentExerciseCellPlayingVideo == null)
                    return;

                if (_currentExerciseCellPlayingVideo.IsFirstPlay)
                {
                    FadeVideoPlayerIn();
                    _currentExerciseCellPlayingVideo.IsFirstPlay = false;
                }
                else
                {
                    _currentExerciseCellPlayingVideo.overlaySL.Opacity = 0.1;
                }
                _currentExerciseCellPlayingVideo.videoRL.HeightRequest = _videoPlayerHeight;

                _isVideoPlayerOpen = true;
                _isVideoPlayerTranslationInProgress = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "VideoPlayerPlaying" } });
            }
        }

        private async void VideoPlayerFailed(object sender, VideoPlayerErrorEventArgs e)
        {
            try
            {
                await DisplayAlert("Video Failed", "Opps our video player had an error, please re-enter workout.", "OK");
            }
            catch (Exception ex)
            {

            }
        }

        private void VideoPlayerPlaying(object sender, VideoPlayerEventArgs e)
        {
            try
            {
                if (_currentExerciseCellPlayingVideo == null)
                    return;

                if (_currentExerciseCellPlayingVideo.IsFirstPlay)
                {
                    FadeVideoPlayerIn();
                    _currentExerciseCellPlayingVideo.IsFirstPlay = false;
                }
                else
                {
                    _currentExerciseCellPlayingVideo.overlaySL.Opacity = 0.1;
                }
                _currentExerciseCellPlayingVideo.videoRL.HeightRequest = _videoPlayerHeight;

                _isVideoPlayerOpen = true;
                _isVideoPlayerTranslationInProgress = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "VideoPlayerPlaying" } });
            }
        }

        private async void FadeVideoPlayerIn()
        {
            try
            {
                await _currentExerciseCellPlayingVideo.overlaySL.FadeTo(0.1, 500, Easing.SinOut);
            }
            catch (Exception ex)
            {

            }
        }

        private void OnExitWorkoutPage()
        {
            try
            {
                _videoPlayer.Pause();
            }
            catch (Exception ex)
            {

            }

            StopAllAudio();
            _StopTimer = true;
        }

        public async void GetActions()
        {
            //Get list of Actions for this Program Day
            //These will be downloaded at this time
            //1 - need to download and store in local DB
            // 2 - need to display them here
            // 3- need to stream in content for the preview
            //requires: input - Id (Guid) to use for the query, first search database then get online

            //Add to Start - WORKOUT

            try
            {
                NotLoadedYet.IsVisible = true;
                listOfActions.IsVisible = false;
                var parentSL = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(10, 0, 10, 0),
                    HeightRequest = 50,
                    BackgroundColor = Color.FromHex("#17191A"),
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
                    FontSize = 16,
                    FontFamily = "HelveticalNeue-Bold",
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(30, 0, 0, 0),
                };

                var exitTopSL = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                var exitBtn = new Button
                {
                    Image = (FileImageSource)ImageSource.FromFile("Delete_32.png"),
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.Start,
                    HeightRequest = 26,
                    WidthRequest = 26,
                    //BackgroundColor = Color.Aqua,
                };
                exitBtn.Clicked += (s, e) => ExitBtn_Clicked();

                var expandTopSL = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                var expandBtn = new Button
                {
                    Image = (FileImageSource)ImageSource.FromFile("Expand_32.png"),
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    HeightRequest = 26,
                    WidthRequest = 26,
                    //BackgroundColor = Color.Aqua,
                };
                // expandBtn.Clicked += (s, e) => ExpandBtn_Clicked();

                exitTopSL.Children.Add(exitBtn);
                startSL.Children.Add(startLabel);
                expandTopSL.Children.Add(expandBtn);

                parentSL.Children.Add(startSL);
                parentSL.Children.Add(exitTopSL);
                //parentSL.Children.Add(expandTopSL);
                listOfActions.Children.Add(parentSL);

                var ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDayGuid).FirstOrDefaultAsync();
                if (ContactProgramDay == null)
                {
                    var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.IsDownloaded == true && x.IsRepeat != true).OrderByDescending(x => x.DownloadedOn).ToListAsync();

                    if (ContactProgramDays.Count() == 0)
                    {
                        return;
                    }
                    else
                    {
                        ContactProgramDay = ContactProgramDays.First();
                        if (ContactProgramDay != null)
                        {
                            _ContactProgramDayGuid = ContactProgramDay.GuidCRM;
                        }
                        else { return; }
                    }
                }

                var ContactActions = await _connection.Table<LocalDBContactAction>().Where(x => x.ContactProgramDayId == ContactProgramDay.Id).ToListAsync();
                var Combined = new List<CombinedExerciseData>();

                foreach (var contactAction in ContactActions)
                {
                    var Action = await _connection.Table<LocalDBAction>().Where(x => x.Id == contactAction.ActionId).FirstOrDefaultAsync();
                    var combined = new CombinedExerciseData
                    {
                        Action = Action,
                        ContactAction = contactAction,
                    };
                    Combined.Add(combined);
                }
                Combined = Combined.OrderBy(x => x.Action.SequenceNumber).ToList();
                List<int> instanceOfActionContentInTrainingRound = new List<int>();
                int i = 0;
                int j = 0;//first instance of exercise reps, time or sketch time with video

                foreach (var combine in Combined)
                {
                    //content types 
                    //exercise reps = 585860000
                    //exercise time = 585860001
                    //rest time = 585860002
                    //seperator = 585860004
                    //stretch time = 585860003

                    int setNumber = 0;
                    if (combine.Action.ContentTypeValue == 585860004 && combine.Action.IsTrainingRound)
                    {
                        //training round seperator 
                        instanceOfActionContentInTrainingRound.Clear();
                    }
                    else
                    {
                        setNumber = instanceOfActionContentInTrainingRound.Where(x => x.Equals(combine.Action.ActionContentId)).ToList().Count();
                        //if setNumber == 0 then its the first time the exercise is shown so must go down the list until the next training round seperator to see if there are any other instances, if there are then use 1 instead of 0 (nothing)
                        if (setNumber == 0)
                        {
                            var combinedSearch = Combined.Where(x => x.Action.SequenceNumber > combine.Action.SequenceNumber).ToList();
                            foreach (var item in combinedSearch)
                            {
                                if (item.Action.ContentTypeValue == 585860004 && item.Action.IsTrainingRound)
                                {
                                    break;
                                }
                                else if (item.Action.ActionContentId == combine.Action.ActionContentId)
                                {
                                    setNumber = 1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            setNumber++;
                        }
                        instanceOfActionContentInTrainingRound.Add(combine.Action.ActionContentId);
                    }


                    var action = combine.Action;
                    var SL = new StackLayout();
                        
                    if (action.ContentTypeValue == 585860000)
                    {
                        j++;
                        //Exercise - Rep Based 585860000
                        // ConstructExerciseRepBasedCell(action);
                        SL = await ConstructExerciseBasedCell(combine, setNumber, j);

                    }
                    else if (action.ContentTypeValue == 585860001)
                    {
                        j++;
                        //Exercise - Time Based 585860001
                        SL = await ConstructExerciseBasedCell(combine, setNumber, j);

                    }
                    else if (action.ContentTypeValue == 585860002)
                    {
                        //Rest Time
                        SL = await ConstructRestTimeBasedCell(combine);
                    }
                    else if (action.ContentTypeValue == 585860004)
                    {
                        //Seperator
                        SL = await ConstructSeperatorBasedCell(combine);
                    }
                    else if (action.ContentTypeValue == 585860003)
                    {
                        j++;
                        //Stretch Time 585860003
                        SL = await ConstructExerciseBasedCell(combine, setNumber, j);
                        //ConstructStretchTimeBasedCell(action);

                    }
                    listOfActions.Children.Add(SL);

                    if (i == 0)
                    {
                        _currentExerciseCellJ00 = _currentExerciseCell;
                        
                        // PLayCellsAudio(_currentExerciseCell);
                        i++;
                    }

                    if (j == 1)
                    {
                        //PlayExerciseVideo(_currentExerciseCell);
                        _currentExerciseCellJ11 = _currentExerciseCell;
                       
                        //LogicForClosingAndOpeningVideo(_currentExerciseCell);
                        j++;
                        //await Task.Delay(350);
                        //await Task.Delay(50);
                    }
                }
                PLayCellsAudio(_currentExerciseCellJ00);
                await Task.Delay(350);
                listOfActions.IsVisible = true;
                NotLoadedYet.IsVisible = false;
                
                LogicForClosingAndOpeningVideo(_currentExerciseCellJ11);

                //ADD to END - DONE button
                var endSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#00bac6"),
                    HeightRequest = 50,
                };

                var endLabel = new Label
                {
                    Text = "MARK COMPLETE",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    FontSize = 22,
                    FontFamily = "HelveticalNeue-Bold",
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) =>
                {
                    WorkoutComplete();
                };
                endSL.GestureRecognizers.Add(tapGestureRecognizer);

                endSL.Children.Add(endLabel);
                listOfActions.Children.Add(endSL);

                //ADD to very END so the last video can scroll to the center
                var veryEndSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#17191A"),
                    HeightRequest = 400,
                };

                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    veryEndSL.HeightRequest = 900;
                }

                var quoteLabel = new Label
                {
                    Text = "Train like a Badass because good things come to those who sweat!",//™\n- Cartelle
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#00BBCB"),
                    FontSize = 20,
                    FontFamily = "HelveticalNeue-Bold",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(20, 0, 20, 0),
                };
             
                veryEndSL.Children.Add(quoteLabel);
                listOfActions.Children.Add(veryEndSL);

               
            }
            catch (Exception ex)
            {
                
            }
            
       
        }

        private async Task<StackLayout> ConstructExerciseBasedCell(CombinedExerciseData combined, int setNumber, int instanceOfExerciseCell)
        {
            try
            {

                var action = combined.Action;

                var ActionContent = await _connection.Table<LocalDBActionContentV2>().Where(x => x.Id == action.ActionContentId).FirstOrDefaultAsync();

                if (ActionContent == null)
                    return new StackLayout();

                var parentSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#f1f1f1"),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                var videoRL = new RelativeLayout
                {
                    HeightRequest = 0,
                    Opacity = 1,
                    BackgroundColor = Color.FromHex("#f1f1f1"),
                };

                var overlaySL = new StackLayout
                {
                    Spacing = 0,
                    Opacity = 0.8,
                    BackgroundColor = Color.White,
                };

                var videoSL = new StackLayout
                {
                    Spacing = 0,
                    //HeightRequest  = 0,
                    BackgroundColor = Color.FromHex("#f1f1f1"),
                };

                videoRL.Children.Add(videoSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));
                videoRL.Children.Add(overlaySL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var cellSL = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.White,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                var controlsSL = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    //Padding = new Thickness(0, 0, 10, 0),
                    //BackgroundColor = Color.Red,
                    // WidthRequest = 150,
                };

                var subControlSL = new StackLayout
                {
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    //BackgroundColor = Color.Purple,
                    //Padding = new Thickness(0, 0, 10, 0),
                };

                var imgRL = new RelativeLayout()
                {
                    HorizontalOptions = LayoutOptions.Start,
                };

                var setFullSL = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 0,
                    BackgroundColor=Color.Transparent,
                    //BackgroundColor = Color.Red,
                };

                var progressSL = new StackLayout
                {
                    Spacing = 0,
                    IsVisible = false,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    
                };

                var progressBar = new CustomProgressBar {
                    ProgressColor = Color.FromHex("#00BBCB")
                };
                progressSL.Children.Add(progressBar);

                if (setNumber != 0)
                {
                    var setSL = new StackLayout
                    {
                        //BackgroundColor = Color.Black,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.EndAndExpand,
                        Spacing = 0,
                    };

                    var setLabel = new Label
                    {
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.End,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = "HelveticalNeue-Bold",
                        TextColor = Color.White,
                        Margin = new Thickness(0, 0, 5, 0),
                        Text = setNumber.ToString(),
                        //FontFamily = "PingFangTC-Semibold",
                        //BackgroundColor = Color.Yellow,
                    };
                    setSL.Children.Add(setLabel);
                    setFullSL.Children.Add(setSL);
                }

                var img = new Image
                {
                    Aspect = Aspect.AspectFill,
                };

                imgRL.Children.Add(img,
                    widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
                    heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                imgRL.Children.Add(setFullSL,
                    widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
                    heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var headingText = new Label
                {
                    FontSize = 13,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    FontFamily = "HelveticalNeue-Bold",
                    TextColor = Color.FromHex("#555757"),
                };

                var doneBtnSL = new StackLayout
                {
                    Spacing = 0,
                    //BackgroundColor = Color.Pink,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(5, 0, 5, 0),
                };

                var doneBtn = new Button
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 2, 0),
                    //BackgroundColor = Color.Yellow,                
                };

                img.Source = (FileImageSource)ImageSource.FromFile(_rootFilePath + ActionContent.PhotoFilePath);
                headingText.Text = action.Heading;

                var headingSL = new StackLayout
                {
                    Spacing = 0,
                    Padding = new Thickness(7, 0, 0, 0),
                    //BackgroundColor = Color.Blue,
                    WidthRequest = 80,
                    HorizontalOptions = LayoutOptions.Start,
                    //HorizontalOptions = LayoutOptions.StartAndExpand,
                };
                headingSL.Children.Add(headingText);

                _currentExerciseCell = _nextExerciseCell;
                _nextExerciseCell = new ExerciseVideoPlayerCell();

                _currentExerciseCell.num = action.SequenceNumber;
                _currentExerciseCell.videoUrl = ActionContent.VideoUrl;
                _currentExerciseCell.videoFilePath = ActionContent.VideoFilePath;
                _currentExerciseCell.parentSL = parentSL;
                _currentExerciseCell.videoSL = videoSL;
                _currentExerciseCell.overlaySL = overlaySL;
                _currentExerciseCell.videoRL = videoRL;
                _currentExerciseCell.cellSL = cellSL;
                _currentExerciseCell.RequiredTimeSeconds = action.TimeSeconds;
                _currentExerciseCell.CurrentTimeSeconds = action.TimeSeconds;
                _currentExerciseCell.ContentTypeValue = action.ContentTypeValue;
                _currentExerciseCell.DoneBtn = doneBtn;
                _currentExerciseCell.DoneBtnSL = doneBtnSL;
                _currentExerciseCell.PreviousExerciseCell = _previousExerciseCell;
                _currentExerciseCell.NextExerciseCell = _nextExerciseCell;
                _currentExerciseCell.ContactActionGuid = combined.ContactAction.GuidCRM;
                _currentExerciseCell.ActionId = action.Id;
                _currentExerciseCell.ActionContentId = action.ActionContentId;
                _currentExerciseCell.ProgressBar = progressBar;
                _currentExerciseCell.ProgressBarSL = progressSL;
                _currentExerciseCell.IsFirstSecond = true;

                //AddAudioToExerciseCell(_currentExerciseCell, 1);
                _currentExerciseCell.cts = new CancellationTokenSource();
                _currentExerciseCell.AudioClipToPlaySequenceNumber = 1;
                _currentExerciseCell.IsAudioCompletePlaying = false;
                var AudioFiles = await _connection.Table<LocalDBAudio>().Where(x => x.ActionId == _currentExerciseCell.ActionId).ToListAsync();
                _currentExerciseCell.AudioFiles = AudioFiles;

                if (instanceOfExerciseCell == 1)
                {
                    //_currentExerciseCell.videoSL.Children.Add(videoPlayer);
                    _currentExerciseCell.videoSL.HeightRequest = _videoPlayerHeight;
                }

                //Exercise - Rep Based 585860000
                //Stretch Time 585860003
                //Exercise - Time Based 585860001

                if (action.ContentTypeValue == 585860000)
                {

                    var repsControlsStackLayout = new StackLayout
                    {
                        Spacing = 0,
                        WidthRequest = 50,
                        //BackgroundColor = Color.Aqua,

                    };

                    var weightControlsStackLayout = new StackLayout
                    {
                        Spacing = 0,
                        WidthRequest = 50,
                        //BackgroundColor = Color.Green,
                    };

                    var repsStackLayout = new StackLayout
                    {
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.Center,
                        Orientation = StackOrientation.Horizontal,
                        //BackgroundColor = Color.Navy,

                    };

                    var weightStackLayout = new StackLayout
                    {
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.Center,
                        Orientation = StackOrientation.Horizontal,
                        //BackgroundColor = Color.Navy,

                    };

                    var moreRepsBtn = new Button
                    {
                        Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_up_04.png"),
                        BackgroundColor=Color.Transparent,
                        HeightRequest = 32,
                        WidthRequest = 32,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    var lessRepsBtn = new Button
                    {
                        Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_down_04.png"),
                        BackgroundColor = Color.Transparent,
                        HeightRequest = 32,
                        WidthRequest = 32,
                        VerticalOptions = LayoutOptions.Center,
                        //BackgroundColor = Color.Orange,

                    };

                    var moreWeightBtn = new Button
                    {
                        Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_up_04.png"),
                        BackgroundColor = Color.Transparent,
                        HeightRequest = 32,
                        WidthRequest = 32,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    var lessWeightBtn = new Button
                    {
                        Image = (FileImageSource)ImageSource.FromFile("Cartelle_Arrow_down_04.png"),
                        BackgroundColor = Color.Transparent,
                        HeightRequest = 32,
                        WidthRequest = 32,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    var repsNumText = new Label { FontSize = 13, FontAttributes = FontAttributes.Bold, FontFamily = "PingFangTC-Semibold", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                    var repsText = new Label { Text = "reps", FontSize = 10, FontFamily = "PingFangTC-Regular", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                    //Font 12 before
                    var weightNumText = new Label { FontSize = 13, FontAttributes = FontAttributes.Bold, FontFamily = "PingFangTC-Semibold", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                    var weightText = new Label { Text = "lbs", FontSize = 10, FontFamily = "PingFangTC-Regular", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

                    if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        repsNumText.FontSize = 18;
                        repsText.FontSize = 15;
                        repsControlsStackLayout.WidthRequest = 80;
                        //repsControlsStackLayout.BackgroundColor = Color.Red;

                        weightNumText.FontSize = 18;
                        weightText.FontSize = 15;
                        weightControlsStackLayout.WidthRequest = 80;
                        //weightControlsStackLayout.BackgroundColor = Color.Purple;
                    }

                    if (!combined.ContactAction.IsComplete)
                    {
                        //not complete
                        doneBtn.HeightRequest = 20;
                        doneBtn.WidthRequest = 20;

                        if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            doneBtn.HeightRequest = 40;
                            doneBtn.WidthRequest = 40;
                        }

                        doneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Circle.png");
                        doneBtn.BackgroundColor = Color.Transparent;
                        _currentExerciseCell.IsComplete = false;
                        repsNumText.Text = action.NumberOfReps.ToString();

                        var ContactAction = await _connection.Table<LocalDBContactAction>().Where(x => x.ActionContentId == action.ActionContentId && x.CompletedOn != null).OrderByDescending(x => x.CompletedOn).FirstOrDefaultAsync();
                        if (ContactAction != null)
                        {
                            weightNumText.Text = ContactAction.ActualWeightLbs.ToString("0");//0.0
                        }
                        else
                        {
                            weightNumText.Text = action.WeightLbs.ToString();
                        }
                    }
                    else
                    {
                        //complete
                        doneBtnSL.Padding = new Thickness(5, 0, 5, 0);
                        doneBtn.HeightRequest = 20;
                        doneBtn.WidthRequest = 20;

                        if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            doneBtn.HeightRequest = 40;
                            doneBtn.WidthRequest = 40;
                        }

                        doneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Check_05.png");
                        doneBtn.BackgroundColor = Color.Transparent;
                        cellSL.BackgroundColor = Color.FromHex("#e3e4e4");
                        _currentExerciseCell.IsComplete = true;

                        repsNumText.Text = combined.ContactAction.ActualNumberOfReps.ToString();
                        weightNumText.Text = combined.ContactAction.ActualWeightLbs.ToString();
                    }

                    _currentExerciseCell.repsLabel = repsNumText;
                    _currentExerciseCell.weightLabel = weightNumText;

                    moreRepsBtn.Command = new Command<ExerciseVideoPlayerCell>(MoreRepsOnAction);
                    moreRepsBtn.CommandParameter = _currentExerciseCell;

                    lessRepsBtn.Command = new Command<ExerciseVideoPlayerCell>(LessRepsOnAction);
                    lessRepsBtn.CommandParameter = _currentExerciseCell;

                    moreWeightBtn.Command = new Command<ExerciseVideoPlayerCell>(MoreWeightOnAction);
                    moreWeightBtn.CommandParameter = _currentExerciseCell;

                    lessWeightBtn.Command = new Command<ExerciseVideoPlayerCell>(LessWeightOnAction);
                    lessWeightBtn.CommandParameter = _currentExerciseCell;

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

                    controlsSL.Children.Add(subControlSL);


                    subControlSL.Children.Add(repsControlsStackLayout);
                    subControlSL.Children.Add(weightControlsStackLayout);
                    doneBtnSL.Children.Add(doneBtn);
                    subControlSL.Children.Add(doneBtnSL);
                }
                else if (action.ContentTypeValue == 585860003 || action.ContentTypeValue == 585860001)
                {
                    //Time Base Exercise or Stretch
                    var timeSL = new StackLayout
                    {
                        Spacing = 0,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                    };

                    var timeText = new Label
                    {
                        FontSize = 18,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                        FontFamily = "HelveticalNeue-Bold",
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.FromHex("#17191A")
                    };

                    TimeSpan time = TimeSpan.FromSeconds(action.TimeSeconds);
                    string str = time.ToString(@"m\:ss");
                    timeText.Text = str;

                    if (!combined.ContactAction.IsComplete)
                    {
                        //not complete
                        progressBar.Progress = 0.0D;

                        doneBtn.HeightRequest = 30;
                        doneBtn.WidthRequest = 30;

                        if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            doneBtn.HeightRequest = 40;
                            doneBtn.WidthRequest = 40;
                        }

                        //doneBtn.BackgroundColor = Color.FromHex("#00BBCB");
                        doneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Play_04.png");
                        doneBtn.BackgroundColor = Color.Transparent;
                        _currentExerciseCell.IsComplete = false;
                        _currentExerciseCell.CurrentTimeSeconds = action.TimeSeconds;

                    }
                    else
                    {
                        //complete
                        progressBar.Progress = 1.0D;

                        doneBtn.Margin = new Thickness(5, 0, 5, 0);
                        doneBtn.HeightRequest = 20;
                        doneBtn.WidthRequest = 20;

                        if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            doneBtn.HeightRequest = 40;
                            doneBtn.WidthRequest = 40;
                        }

                        doneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Check_05.png");
                        doneBtn.BackgroundColor = Color.Transparent;
                        cellSL.BackgroundColor = Color.FromHex("#e3e4e4");
                        doneBtn.IsEnabled = false;
                        TimeSpan time2 = TimeSpan.FromSeconds(0);
                        string str2 = time2.ToString(@"m\:ss");
                        timeText.Text = str2;
                        _currentExerciseCell.IsComplete = true;
                        _currentExerciseCell.CurrentTimeSeconds = 0;
                    }

                    _currentExerciseCell.TimerLabel = timeText;
                    controlsSL.WidthRequest = 105;//120
                                                  //controlsSL.BackgroundColor = Color.Green;
                    timeSL.Children.Add(timeText);
                    controlsSL.Children.Add(timeSL);

                    doneBtnSL.Children.Add(doneBtn);
                    controlsSL.Children.Add(doneBtnSL);
                }

                var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
                if (isOfXFamily)
                {
                    imgRL.HeightRequest = 100;
                    imgRL.WidthRequest = 100;
                    headingSL.WidthRequest = 125;
                }
                else if (Device.Idiom == TargetIdiom.Phone)
                {
                    var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                    if (isDeviceIphonePlus)
                    {
                        imgRL.HeightRequest = 100;
                        imgRL.WidthRequest = 100;
                        headingSL.WidthRequest = 150;
                    }
                    else
                    {
                        imgRL.HeightRequest = 100;
                        imgRL.WidthRequest = 100;
                        headingSL.WidthRequest = 125;
                    }
                    //headingSL.BackgroundColor = Color.Yellow;
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    imgRL.HeightRequest = 150;
                    imgRL.WidthRequest = 150;
                    headingText.FontSize = 20;
                    headingSL.WidthRequest = 350;
                    //headingSL.BackgroundColor = Color.Yellow;
                    headingSL.Padding = new Thickness(20, 0, 0, 0);
                }
                else
                {
                    imgRL.HeightRequest = 100;
                    imgRL.WidthRequest = 100;
                    headingSL.WidthRequest = 90;
                }

                _previousExerciseCell.NextExerciseCell = _currentExerciseCell;
                _previousExerciseCell = _currentExerciseCell;

                var tapGestureRecognizer = new TapGestureRecognizer
                {
                    Command = new Command<ExerciseVideoPlayerCell>(PlayExerciseVideo),
                    CommandParameter = _currentExerciseCell,
                };
                imgRL.GestureRecognizers.Add(tapGestureRecognizer);
                headingSL.GestureRecognizers.Add(tapGestureRecognizer);

                var tapGestureRecognizer3 = new TapGestureRecognizer
                {
                    Command = new Command<ExerciseVideoPlayerCell>(ShowVideoInFullScreen),
                    CommandParameter = _currentExerciseCell,
                    NumberOfTapsRequired = 2,
                };
                overlaySL.GestureRecognizers.Add(tapGestureRecognizer3);

                doneBtn.Command = new Command<ExerciseVideoPlayerCell>(MarkActionAsDone);
                doneBtn.CommandParameter = _currentExerciseCell;

                var tapGestureRecognizer2 = new TapGestureRecognizer
                {
                    Command = new Command<ExerciseVideoPlayerCell>(MarkActionAsDone),
                    CommandParameter = _currentExerciseCell,
                };
                doneBtnSL.GestureRecognizers.Add(tapGestureRecognizer2);

                cellSL.Children.Add(imgRL);
                cellSL.Children.Add(headingSL);
                cellSL.Children.Add(controlsSL);

                parentSL.Children.Add(cellSL);
                if (action.ContentTypeValue == 585860003 || action.ContentTypeValue == 585860001)
                {
                    parentSL.Children.Add(progressSL);
                }
                parentSL.Children.Add(videoRL);

                _previousExerciseCell = _currentExerciseCell;

                return parentSL;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<StackLayout> ConstructRestTimeBasedCell(CombinedExerciseData combined)
        {
            try
            {
                var action = combined.Action;

                //var ActionContent = await _connection.Table<LocalDBActionContent>().Where(x => x.Id == action.ActionContentId).FirstOrDefaultAsync();

                var restMainSL = new StackLayout
                {
                    Spacing = 0,
                    HeightRequest = 38,//28                
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.FromHex("#E8E8E8"),
                };

                var restRL = new RelativeLayout
                {
                    //BackgroundColor = Color.Yellow,
                };
                restMainSL.Children.Add(restRL);

                //var endSL = new StackLayout
                //{
                //    Spacing = 0,
                //    HeightRequest = 1,
                //    BackgroundColor = Color.FromHex("#D6D8D8")
                //};
                //restMainSL.Children.Add(endSL);

                var restProgressSL = new StackLayout
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor=Color.Transparent,
                    Margin = new Thickness(-1, 25, 0, 0),//37
                                                         //BackgroundColor = Color.Yellow,
                                                         //HeightRequest = 38,
                };

                var restProgress = new CustomProgressBarForRest
                {
                    ProgressColor=Color.FromHex("#00BBCB"),
                    VerticalOptions = LayoutOptions.Fill,
                    HeightRequest = 38,
                    BackgroundColor=Color.Transparent
                };

                restRL.Children.Add(restProgressSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var restSL = new StackLayout
                {
                    Spacing = 0,
                    //HeightRequest = 38,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(30, 0, 2, 0),//10
                    Orientation = StackOrientation.Horizontal,
                    //Opacity = 0.3,
                };

                restRL.Children.Add(restSL, widthConstraint: Constraint.RelativeToParent(parent => parent.Width), heightConstraint: Constraint.RelativeToParent(parent => parent.Height));

                var heading = new Label
                {
                    Text = action.Heading,// ActionContent.Heading,
                    TextColor = Color.FromHex("#555757"),
                    FontSize = 16,//14
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "HelveticalNeue-Bold",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                };

                _currentExerciseCell = _nextExerciseCell;
                _nextExerciseCell = new ExerciseVideoPlayerCell();

                TimeSpan time = TimeSpan.FromSeconds(action.TimeSeconds);
                string str = time.ToString(@"m\:ss");

                if (!combined.ContactAction.IsComplete)
                {
                    _currentExerciseCell.IsComplete = false;
                    restProgressSL.Children.Add(restProgress);
                }
                else
                {
                    restSL.BackgroundColor = Color.FromHex("#e3e4e4"); //#d5f8d9
                    TimeSpan time2 = TimeSpan.FromSeconds(0);
                    str = time2.ToString(@"m\:ss");
                    _currentExerciseCell.IsComplete = true;
                }

                var timeText = new Label
                {
                    Text = str,
                    TextColor = Color.FromHex("#17191A"),
                    FontSize = 18,//14,
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "HelveticalNeue-Bold",
                    VerticalOptions = LayoutOptions.Center,
                    //HorizontalOptions = LayoutOptions.End,
                    WidthRequest = 50,
                    Margin = new Thickness(0, 0, 0, 0), //20
                };

                _currentExerciseCell.ProgressBarForRestSL = restProgressSL;
                _currentExerciseCell.ProgressBarForRest = restProgress;

                _currentExerciseCell.num = action.SequenceNumber;
                _currentExerciseCell.RequiredTimeSeconds = action.TimeSeconds;
                _currentExerciseCell.CurrentTimeSeconds = action.TimeSeconds;
                _currentExerciseCell.ContentTypeValue = action.ContentTypeValue;
                _currentExerciseCell.TimerLabel = timeText;
                _currentExerciseCell.restSL = restSL;
                _currentExerciseCell.PreviousExerciseCell = _previousExerciseCell;
                _currentExerciseCell.NextExerciseCell = _nextExerciseCell;
                _currentExerciseCell.ContactActionGuid = combined.ContactAction.GuidCRM;
                _currentExerciseCell.ActionId = action.Id;
                _currentExerciseCell.ActionContentId = action.ActionContentId;

                //AddAudioToExerciseCell(_currentExerciseCell, 2);
                _currentExerciseCell.cts = new CancellationTokenSource();
                _currentExerciseCell.AudioClipToPlaySequenceNumber = 1;
                _currentExerciseCell.IsAudioCompletePlaying = false;
                var AudioFiles = await _connection.Table<LocalDBAudio>().Where(x => x.ActionId == _currentExerciseCell.ActionId).ToListAsync();
                _currentExerciseCell.AudioFiles = AudioFiles;

                _previousExerciseCell = _currentExerciseCell;

                var skipRestSL = new StackLayout
                {
                    Spacing = 0,
                    WidthRequest = 50, //40
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions=LayoutOptions.End,
                    //Margin = new Thickness(0, 0, 0, 15),
                    //BackgroundColor = Color.Yellow,
                };

                var skipRest = new Button
                {
                    Text = "Skip",// ActionContent.Heading,
                    TextColor = Color.FromHex("#1C516B"),
                    FontSize = 13,//14
                    Margin= new Thickness(-5, -5, 0, 10),
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "HelveticalNeue-Bold",
                    VerticalOptions = LayoutOptions.Start,
                    BackgroundColor=Color.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    IsVisible = false,
                };
                skipRestSL.Children.Add(skipRest);
                _currentExerciseCell.SkipRest = skipRest;

                skipRest.Command = new Command<ExerciseVideoPlayerCell>(SkipRestTimer);
                skipRest.CommandParameter = _currentExerciseCell;

                restSL.Children.Add(heading);
                restSL.Children.Add(timeText);
                restSL.Children.Add(skipRestSL);

                //listOfActions.Children.Add(seperatorSL);
                return restMainSL;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void SkipRestTimer(ExerciseVideoPlayerCell e)
        {
            try
            {
                if (!e.IsComplete)
                {
                    _StopTimer = true;
                    _TimerIsActive = false;
                    _timingExerciseCell.TimerInProgress = false;

                    _timingExerciseCell.SkipRest.IsVisible = false;
                    _timingExerciseCell.IsComplete = true;
                    _timingExerciseCell.IsSkipped = true;

                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<StackLayout> ConstructSeperatorBasedCell(CombinedExerciseData combined)
        {
            try
            {
                var action = combined.Action;

                //var ActionContent = await _connection.Table<LocalDBActionContent>().Where(x => x.Id == action.ActionContentId).FirstOrDefaultAsync();

                var seperatorSL = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.FromHex("#555757"),
                    HeightRequest = 28,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(15, 0, 0, 0),
                };

                var heading = new Label
                {
                    Text = action.Heading,//ActionContent.Heading,
                    TextColor = Color.White,
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "HelveticalNeue-Bold",
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                _currentExerciseCell = _nextExerciseCell;
                _nextExerciseCell = new ExerciseVideoPlayerCell();

                _currentExerciseCell.num = action.SequenceNumber;
                _currentExerciseCell.ContentTypeValue = action.ContentTypeValue;
                _currentExerciseCell.PreviousExerciseCell = _previousExerciseCell;
                _currentExerciseCell.NextExerciseCell = _nextExerciseCell;
                _currentExerciseCell.ContactActionGuid = combined.ContactAction.GuidCRM;
                _currentExerciseCell.ActionId = action.Id;
                _currentExerciseCell.ActionContentId = action.ActionContentId;

                //AddAudioToExerciseCell(_currentExerciseCell, 3);
                _currentExerciseCell.cts = new CancellationTokenSource();
                _currentExerciseCell.AudioClipToPlaySequenceNumber = 1;
                _currentExerciseCell.IsAudioCompletePlaying = false;
                var AudioFiles = await _connection.Table<LocalDBAudio>().Where(x => x.ActionId == _currentExerciseCell.ActionId).ToListAsync();
                _currentExerciseCell.AudioFiles = AudioFiles;

                _previousExerciseCell = _currentExerciseCell;

                seperatorSL.Children.Add(heading);
                //listOfActions.Children.Add(seperatorSL);
                return seperatorSL;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private bool IsDivitatingFromFlow(ExerciseVideoPlayerCell e)
        {
            try
            {
                var keepLooping = true;

                if (e == null || e.num == 1)
                {
                    return false;
                }

                var previous = e.PreviousExerciseCell;

                if (previous == null)
                    return false;

                //check type
                //content types 
                //exercise reps = 585860000
                //exercise time = 585860001
                //rest time = 585860002
                //seperator = 585860004
                //stretch time = 585860003

                if (previous.num == 1)//At the start
                {
                    if (previous.IsComplete == true)
                    {
                        return false;
                    }
                    else
                    {
                        if (previous.ContentTypeValue == 585860000 || previous.ContentTypeValue == 585860003 || previous.ContentTypeValue == 585860001 || previous.ContentTypeValue == 585860002)
                        {
                            return true;
                        }
                        else
                        {
                            //Seperator
                            return false;
                        }
                    }
                }

                int i = 0;

                while (keepLooping)
                {
                    i++;
                    if (previous == null)
                    {
                        keepLooping = false;
                        return false;
                    }

                    if (previous.IsComplete == true)
                    {
                        keepLooping = false;
                        return false;
                    }
                    else
                    {
                        if (previous.ContentTypeValue == 585860000 || previous.ContentTypeValue == 585860003 || previous.ContentTypeValue == 585860001 || previous.ContentTypeValue == 585860002)
                        {
                            keepLooping = false;
                            return true;
                        }
                        else
                        {
                            //Seperator
                            if (previous.num == 1)//At the start
                            {
                                keepLooping = false;
                                return false;
                            }
                            //Just continue search up the stack
                            previous = previous.PreviousExerciseCell;
                        }
                    }

                    if (i > 500)
                    {
                        //fail safe
                        keepLooping = false;
                        return false;
                    }

                }
                return false; // if made it here somethign went wrong 

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void LogicForClosingAndOpeningVideo(ExerciseVideoPlayerCell cellToPlayVideo)
        {
            try
            {
                int cellType = cellToPlayVideo.ContentTypeValue;

                if (cellToPlayVideo.ContentTypeValue == 585860000 || cellToPlayVideo.ContentTypeValue == 585860001 || cellToPlayVideo.ContentTypeValue == 585860003)
                {
                    //exercise reps = 585860000
                    //exercise time = 585860001
                    //rest time = 585860002
                    //seperator = 585860004
                    //stretch time = 585860003

                    if (cellToPlayVideo.PreviousExerciseCell.ContentTypeValue == 585860000 || cellToPlayVideo.PreviousExerciseCell.ContentTypeValue == 585860001 || cellToPlayVideo.PreviousExerciseCell.ContentTypeValue == 585860003)
                    {
                        CloseExerciseVideo(cellToPlayVideo.PreviousExerciseCell);
                    }

                    if (_currentExerciseCellPlayingVideo == cellToPlayVideo)
                    {
                        //close
                        
                        PlayExerciseVideo(cellToPlayVideo);
                    }
                    else if (_currentExerciseCellPlayingVideo != cellToPlayVideo)
                    {
                        //play
                        PlayExerciseVideo(cellToPlayVideo);
                    }
                }
                else //seperator or rest
                {
                    if (cellToPlayVideo.PreviousExerciseCell.ContentTypeValue == 585860000 || cellToPlayVideo.PreviousExerciseCell.ContentTypeValue == 585860001 || cellToPlayVideo.PreviousExerciseCell.ContentTypeValue == 585860003)
                    {
                        CloseExerciseVideo(cellToPlayVideo.PreviousExerciseCell);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void PlayNextCell(ExerciseVideoPlayerCell e)
        {
            try
            {
                if (e.ContentTypeValue == 585860000 || e.ContentTypeValue == 585860004)
                {
                    PLayCellsAudio(e);

                    if (_currentExerciseCellPlayingVideo != e)
                    {
                        LogicForClosingAndOpeningVideo(e);
                    }
                }
                else if (e.ContentTypeValue == 585860002)
                {
                    if (!_TimerIsActive)
                    {
                        e.TimerInProgress = true;
                        //Start timer, when done change to the same green background
                        _TimerLabel = e.TimerLabel;
                        _TimerSeconds = e.CurrentTimeSeconds;
                        _timingExerciseCell = e;
                        _TimerIsActive = true;
                        _StopTimer = false;
                        Device.StartTimer(TimeSpan.FromSeconds(1), TimerElapsed);
                        //START AUDIO
                        PLayCellsAudio(e);

                        if (e.PreviousExerciseCell.ContentTypeValue == 585860000 || e.PreviousExerciseCell.ContentTypeValue == 585860001 || e.PreviousExerciseCell.ContentTypeValue == 585860003)
                        {
                            CloseExerciseVideo(e.PreviousExerciseCell);
                        }
                        //else
                        //{
                        //    LogicForClosingAndOpeningVideo(e.PreviousExerciseCell);
                        //}
                    }
                }
                else if (e.ContentTypeValue == 585860003 || e.ContentTypeValue == 585860001)
                {
                    LogicForTimeBaseCell(e);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void MarkActionAsDone(ExerciseVideoPlayerCell e)
        {
            try
            {
                //Change the check mark to green and save the action results to the localDB
                var isComplete = e.IsComplete;

                //Exercise - Rep Based 585860000
                //Stretch Time 585860003
                //Exercise - Time Based 585860001
                //Rest time - 585860002

                if (!isComplete)
                {
                    var IsDiviting = IsDivitatingFromFlow(e);
                    //Check to make sure they dont divite from program flow, else display a warning

                    if (_currentExerciseCellPlayingAudio.num != e.num && IsDiviting)
                    {
                        if (_TimerIsActive)
                        {
                            await DisplayAlert("Timer Active", "Stop the current timer before skipping ahead.", "OK");
                            return;
                        }

                        var result = await DisplayAlert("Skip Ahead", "Are you sure that you want to skip ahead of the workout flow?", "Skip", "Not now");
                        if (!result)
                        {
                            return;
                        }
                        else
                        {
                            //Play this cells audio 
                            PlayNextCell(e);
                            return;
                        }
                    }
                    else if (_TimerIsActive && (e.ContentTypeValue == 585860000 || _timingExerciseCell != e))
                    {
                        await DisplayAlert("Timer Active", "Stop the current timer to continue.", "OK");
                        return;
                    }

                    if (e.ContentTypeValue == 585860000)
                    {
                        e.DoneBtnSL.Padding = new Thickness(5, 0, 5, 0);
                        e.DoneBtn.HeightRequest = 20;
                        e.DoneBtn.WidthRequest = 20;

                        if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            e.DoneBtn.HeightRequest = 40;
                            e.DoneBtn.WidthRequest = 40;
                        }

                        e.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Check_05.png");
                        e.DoneBtn.BackgroundColor = Color.Transparent;
                        e.cellSL.BackgroundColor = Color.FromHex("#e3e4e4"); //e5f2e5
                        e.IsComplete = true;

                        var ContactAction = await _connection.Table<LocalDBContactAction>().Where(x => x.GuidCRM == e.ContactActionGuid).FirstOrDefaultAsync();
                        if (ContactAction == null)
                            return;

                        var currentReps = 0;
                        var repsText = e.repsLabel.Text;
                        currentReps = int.Parse(repsText);

                        var currentWeight = 0F;
                        var weightText = e.weightLabel.Text;
                        currentWeight = float.Parse(weightText);

                        ContactAction.ActualNumberOfReps = currentReps;
                        ContactAction.ActualWeightLbs = currentWeight;
                        ContactAction.IsComplete = true;
                        ContactAction.CompletedOn = DateTime.UtcNow;
                        await _connection.UpdateAsync(ContactAction);

                        if (_currentExerciseCellPlayingVideo == e)
                        {
                            CloseExerciseVideo(e);
                        }

                        if (e.NextExerciseCell == null)
                            return;

                        if (!e.NextExerciseCell.IsComplete)
                        {
                            //content types 
                            //exercise reps = 585860000
                            //exercise time = 585860001
                            //rest time = 585860002
                            //seperator = 585860004
                            //stretch time = 585860003

                            //If the next is a seperator, rest or rep based then start audio
                            PlayNextCell(e.NextExerciseCell);
                        }

                        var next = e.NextExerciseCell;
                        var currenctAC = e.ActionContentId;

                        while (next != null)
                        {
                            if (next.ActionContentId == currenctAC && !next.IsComplete)
                            {
                                next.weightLabel.Text = weightText;
                            }
                            next = next.NextExerciseCell;
                        }

                    }
                    else if (e.ContentTypeValue == 585860003 || e.ContentTypeValue == 585860001)
                    {

                        LogicForTimeBaseCell(e);
                    }
                    else if (e.ContentTypeValue == 585860002) //rest timer, skip
                    {

                    }
                }
                else
                {
                    if (e.ContentTypeValue == 585860000)
                    {
                        e.DoneBtn.HeightRequest = 20;
                        e.DoneBtn.WidthRequest = 20;
                        e.DoneBtnSL.Padding = new Thickness(5, 0, 5, 0);

                        if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            e.DoneBtn.HeightRequest = 40;
                            e.DoneBtn.WidthRequest = 40;
                        }

                        e.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Circle.png");
                        e.DoneBtn.BackgroundColor = Color.Transparent;
                        e.cellSL.BackgroundColor = Color.White;
                        e.IsComplete = false;

                        var ContactAction = await _connection.Table<LocalDBContactAction>().Where(x => x.GuidCRM == e.ContactActionGuid).FirstOrDefaultAsync();
                        if (ContactAction == null)
                        {
                            return;
                        }

                        ContactAction.IsComplete = false;
                        ContactAction.CompletedOn = null;
                        await _connection.UpdateAsync(ContactAction);
                    }
                    else if (e.ContentTypeValue == 585860003 || e.ContentTypeValue == 585860001)
                    {

                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void LogicForTimeBaseCell(ExerciseVideoPlayerCell e)
        {
            if (!_TimerIsActive)
            {
                e.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Pause_01.png");
                e.DoneBtn.BackgroundColor = Color.Transparent;
                e.TimerInProgress = true;
                //Start timer, when done change to the same green check mark
                _TimerLabel = e.TimerLabel;
                _TimerSeconds = e.CurrentTimeSeconds;
                _timingExerciseCell = e;
                _TimerIsActive = true;
                _StopTimer = false;
                Device.StartTimer(TimeSpan.FromSeconds(1), TimerElapsed);

                //START AUDIO
                PLayCellsAudio(e);

                if (_currentExerciseCellPlayingVideo != e)
                {
                    LogicForClosingAndOpeningVideo(e);
                }
            }
            else if (_timingExerciseCell.num == e.num)
            {
                //Pause
                e.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Play_04.png");
                e.DoneBtn.BackgroundColor = Color.Transparent;
                _StopTimer = true;

                //STOP AUDIO
                PLayCellsAudio(e);
                if (_currentExerciseCellPlayingVideo != e)
                {
                    LogicForClosingAndOpeningVideo(e);
                }
            }
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program Day", _ProgramDay.Heading }, { "Sequence Number", _ProgramDay.SequenceNumber.ToString() }, { "Action", "OnAppearing" } });

            _isOnFullScreen = false;
            _StopTimer = false;

            base.OnAppearing();
        }

        private bool _PlayTimer = false;
        private void OnSleep()
        {
            try
            {
                
                if (_currentExerciseCellPlayingAudio != null)
                {
                    DependencyService.Get<IAudio>().Pause();
                }


                if (_currentExerciseCell != null)
                {
                    if (_TimerIsActive)
                    {
                        _StopTimer = true;
                        _PlayTimer = true;
                        PLayCellsAudio(_currentExerciseCell);
                    }
                    else
                    {
                        _PlayTimer = false;
                    }
                    
                }
            }
            catch (Exception ex)
            {

            }
            

            // PLayCellsAudio(_currentExerciseCellPlayingAudio);
            //DependencyService.Get<IAudio>().Pause();
            //_currentExerciseCell

        }
        private void OnResume()
        {
            try
            {
                if (_currentExerciseCellPlayingAudio != null)
                {
                    DependencyService.Get<IAudio>().Resume();
                }
                if (_PlayTimer)
                {
                    if (_currentExerciseCell != null)
                    {
                        if (!_TimerIsActive)
                        {
                            _timingExerciseCell.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Pause_01.png");
                            _timingExerciseCell.DoneBtn.BackgroundColor = Color.Transparent;
                            _currentExerciseCell.TimerInProgress = true;
                            //Start timer, when done change to the same green check mark
                            _TimerLabel = _timingExerciseCell.TimerLabel;
                            _TimerSeconds = _timingExerciseCell.CurrentTimeSeconds;
                            // _timingExerciseCell = _currentExerciseCell;
                            _TimerIsActive = true;
                            _StopTimer = false;
                            Device.StartTimer(TimeSpan.FromSeconds(1), TimerElapsed);

                            
                        }

                    }
                }
                // DependencyService.Get<IAudio>().Resume();
                //_StopTimer = false;
                //if (_videoPlayer.State == Octane.Xamarin.Forms.VideoPlayer.Constants.PlayerState.Paused && _isOnPage)
                //{
                //    _videoPlayer.Play();
                //}

                if (_videoPlayer == null)
                    return;
                
                //if (!_videoPlayer.CurrentState.Equals("Playing") && _isOnPage)
                //{
                //    _videoPlayer.Play();
                //}
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "OnResume()" } });
            }
        }

        protected override void OnDisappearing()
        {
            if (!_isOnFullScreen)
            {
                OnExitWorkoutPage();
                base.OnDisappearing();
            }
        }

        //private bool ProgressBarUI()
        //{
        //    if (_TimerSeconds > 0 && !_StopTimer && _numberOfTimerProgressBarCalls < _maxCallsToProgressBar && _timingExerciseCell.ProgressBar != null)
        //    {
        //        _numberOfTimerProgressBarCalls++;
        //        //double fractialSeconds = (_milisecondsToCallProgressBarUI * _numberOfTimerProgressBarCalls) / 1000D;
        //        double TimerTime = (double)_TimerSeconds;
        //        double OneSecond = 1D / ((double)_timingExerciseCell.RequiredTimeSeconds);
        //        //double additionalProgress = fractialSeconds * OneSecond;
        //        double CurrentProgressInSeconds = 1D - (TimerTime * OneSecond); //count downs so -100%

        //        try
        //        {
        //            double progress = CurrentProgressInSeconds; //+ additionalProgress;
        //            if (progress > 0.001D)
        //            {
        //                _timingExerciseCell.ProgressBar.Progress = progress;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var err = ex.ToString();
        //        }

        //        return true;
        //    }

        //    if (_TimerSeconds == 0)
        //    {
        //        _timingExerciseCell.ProgressBar.Progress = 1D;
        //        _timingExerciseCell.ProgressBarSL.HeightRequest = 0;
        //        _timingExerciseCell.ProgressBarSL.IsVisible = false;
        //    }
        //    _numberOfTimerProgressBarCalls = 0;
        //    return false;
        //}

        private bool TimerElapsed()
        {
            if (_TimerSeconds > 0)
            {
                if (!_StopTimer)
                {
                    //if(_TimerSeconds == _timingExerciseCell.RequiredTimeSeconds && _timingExerciseCell.IsFirstSecond)
                    //{
                    //    TimeSpan time = TimeSpan.FromSeconds(_TimerSeconds);
                    //    string str = time.ToString(@"m\:ss");
                    //    _TimerLabel.Text = str;
                    //    _timingExerciseCell.CurrentTimeSeconds = _TimerSeconds;
                    //    _timingExerciseCell.IsFirstSecond = false;
                    //    return true; //first time, so just wait the first second
                    //}

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        _TimerSeconds--;
                        //put here your code which updates the view
                        //TimerLabel.Text = _TimerSeconds.ToString();
                        TimeSpan time = TimeSpan.FromSeconds(_TimerSeconds);
                        string str = time.ToString(@"m\:ss");
                        _TimerLabel.Text = str;
                        _timingExerciseCell.CurrentTimeSeconds = _TimerSeconds;

                        try
                        {
                            if (_timingExerciseCell.ContentTypeValue == 585860003 || _timingExerciseCell.ContentTypeValue == 585860001)
                            {
                                _timingExerciseCell.ProgressBarSL.IsVisible = true;
                                double OneSecond = 1D / ((double)_timingExerciseCell.RequiredTimeSeconds);
                                double CurrentProgressInSeconds = 1D - (_TimerSeconds * OneSecond); //count downs so -100%
                                _timingExerciseCell.ProgressBar.Progress = CurrentProgressInSeconds;
                                //_numberOfTimerProgressBarCalls = 0;
                                //Device.StartTimer(TimeSpan.FromMilliseconds(_milisecondsToCallProgressBarUI), ProgressBarUI);
                            }
                            else if (_timingExerciseCell.ContentTypeValue == 585860002)//rest
                            {
                                _timingExerciseCell.SkipRest.IsVisible = true;

                                _timingExerciseCell.ProgressBarForRestSL.IsVisible = true;
                                double OneSecond = 1D / ((double)_timingExerciseCell.RequiredTimeSeconds);
                                double CurrentProgressInSeconds = 1D - (_TimerSeconds * OneSecond); //count downs so -100%
                                _timingExerciseCell.ProgressBarForRest.Progress = CurrentProgressInSeconds;
                            }
                        }
                        catch (Exception ex)
                        {
                            var err = ex.ToString();
                            int i = 0;
                        }

                    });
                    return true;
                }
                else
                {
                    if (_timingExerciseCell.ContentTypeValue != 585860002)
                    {
                        //do not stop the timer for rest
                        _timingExerciseCell.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Play_04.png");
                        _timingExerciseCell.DoneBtn.BackgroundColor = Color.Transparent;
                        _timingExerciseCell.TimerInProgress = false;
                        _TimerIsActive = false;
                        _StopTimer = false;
                        return false;
                    }
                    else if (_timingExerciseCell.ContentTypeValue == 585860002 && _timingExerciseCell.IsSkipped)
                    {
                        _timingExerciseCell.TimerInProgress = false;
                        _TimerIsActive = false;
                        _StopTimer = false;

                        if (_timingExerciseCell.NextExerciseCell == null)
                            return false;

                        if (!_timingExerciseCell.NextExerciseCell.IsComplete)
                        {
                            //content types 
                            //exercise reps = 585860000
                            //exercise time = 585860001
                            //rest time = 585860002
                            //seperator = 585860004
                            //stretch time = 585860003

                            //If the next is a seperator, rest or rep based then start audio
                            PlayNextCell(_timingExerciseCell.NextExerciseCell);
                        }
                        return false;
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _TimerSeconds--;
                            //put here your code which updates the view
                            //TimerLabel.Text = _TimerSeconds.ToString();
                            TimeSpan time = TimeSpan.FromSeconds(_TimerSeconds);
                            string str = time.ToString(@"m\:ss");
                            _TimerLabel.Text = str;
                            _timingExerciseCell.CurrentTimeSeconds = _TimerSeconds;
                        });

                        _timingExerciseCell.TimerInProgress = true;
                        _TimerIsActive = true;
                        _StopTimer = false;

                        return true;
                    }
                }
            }
            else
            {
                var timingContentType = _timingExerciseCell.ContentTypeValue;

                if (timingContentType == 585860001 || timingContentType == 585860003)
                {
                    _timingExerciseCell.ProgressBar.Progress = 1D;
                    _timingExerciseCell.ProgressBarSL.HeightRequest = 0;
                    _timingExerciseCell.ProgressBarSL.IsVisible = false;

                    _timingExerciseCell.DoneBtn.Margin = new Thickness(5, 0, 5, 0);
                    _timingExerciseCell.DoneBtn.HeightRequest = 20;
                    _timingExerciseCell.DoneBtn.WidthRequest = 20;

                    if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        _timingExerciseCell.DoneBtn.HeightRequest = 40;
                        _timingExerciseCell.DoneBtn.WidthRequest = 40;
                    }

                    _timingExerciseCell.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Check_05.png");
                    _timingExerciseCell.DoneBtn.BackgroundColor = Color.Transparent;
                    _timingExerciseCell.cellSL.BackgroundColor = Color.FromHex("#e3e4e4");

                }
                else if (timingContentType == 585860002)
                {
                    _timingExerciseCell.restSL.BackgroundColor = Color.FromHex("#e3e4e4");
                    _timingExerciseCell.ProgressBarForRestSL.IsVisible = false;
                    _timingExerciseCell.SkipRest.IsVisible = false;
                }

                _timingExerciseCell.IsComplete = true;

                Task t = Task.Factory.StartNew(() => SaveTimeBasedExercise(_timingExerciseCell));
                t.Wait();

                if (_timingExerciseCell.NextExerciseCell != null)
                {
                    //content types 
                    //exercise reps = 585860000
                    //exercise time = 585860001
                    //rest time = 585860002
                    //seperator = 585860004
                    //stretch time = 585860003
                    if (!_timingExerciseCell.NextExerciseCell.IsComplete)
                    {
                        //START AUDIO
                        PLayCellsAudio(_timingExerciseCell.NextExerciseCell);

                        if (_openExerciseCellInfo != _timingExerciseCell.NextExerciseCell)
                        {
                            LogicForClosingAndOpeningVideo(_timingExerciseCell.NextExerciseCell);
                        }

                        var nextContetType = _timingExerciseCell.NextExerciseCell.ContentTypeValue;

                        if (nextContetType == 585860002 || nextContetType == 585860001 || nextContetType == 585860003)
                        {
                            if (nextContetType == 585860001 || nextContetType == 585860003)
                            {
                                _timingExerciseCell.NextExerciseCell.DoneBtn.Image = (FileImageSource)ImageSource.FromFile("Cartelle_Pause_01.png");
                                _timingExerciseCell.NextExerciseCell.DoneBtn.BackgroundColor = Color.Transparent;
                            }
                            //LogicForClosingAndOpeningVideo(_timingExerciseCell);

                            _timingExerciseCell.NextExerciseCell.TimerInProgress = true;
                            //Start timer, when done change to the same green check mark
                            _TimerLabel = _timingExerciseCell.NextExerciseCell.TimerLabel;
                            _TimerSeconds = _timingExerciseCell.NextExerciseCell.CurrentTimeSeconds;
                            _timingExerciseCell = _timingExerciseCell.NextExerciseCell;
                            _TimerIsActive = true;
                            Device.StartTimer(TimeSpan.FromSeconds(1), TimerElapsed);
                        }
                        else
                        {
                            _TimerIsActive = false;
                        }
                    }
                    else
                    {
                        _TimerIsActive = false;
                    }
                }
                else
                {
                    _TimerIsActive = false;
                }

                return false;
            }
            //return true to keep timer reccuring
            //return false to stop timer
        }

        //private async Task<List<LocalDBAudio>> AddAudioToExerciseCell(ExerciseVideoPlayerCell audioCell, int i)
        //{
        //    audioCell.cts = new CancellationTokenSource();
        //    audioCell.AudioClipToPlaySequenceNumber = 1;
        //    audioCell.IsAudioCompletePlaying = false;
        //    var AudioFiles = await _connection.Table<LocalDBAudio>().Where(x => x.ActionId == audioCell.ActionId).ToListAsync();
        //    audioCell.AudioFiles = AudioFiles;
        //}

        private void PLayCellsAudio(ExerciseVideoPlayerCell audioCell)
        {
            if (!_currentlyInAudioTransition)
            {
                //_currentlyInAudioTransition = true;

                if (_currentExerciseCellPlayingAudio != null)
                {

                    //stop current audio and play new if different than current
                    DependencyService.Get<IAudio>().Stop();

                    _currentExerciseCellPlayingAudio.cts.Cancel();
                    //_currentlyPlayingAudioCell.cts = new CancellationTokenSource();
                    //audioCell.cts = new CancellationTokenSource();

                    if (_currentExerciseCellPlayingAudio.ContactActionGuid == audioCell.ContactActionGuid)
                    {
                        //do nothign just stop
                    }
                    else
                    {
                        //play new
                        //if (count == 1)
                        //{
                        //    Task.Delay(1000).ContinueWith(t => Task.Factory.StartNew(() => PlayAudio(audioCell), audioCell.cts.Token));
                        //    count++;
                        //}
                        //else
                            Task.Factory.StartNew(() => PlayAudio(audioCell), audioCell.cts.Token);

                    }
                    _currentExerciseCellPlayingAudio = audioCell;

                }
                else
                {
                    // No audio playing, just play it                    
                     Task.Factory.StartNew(() => PlayAudio(audioCell), audioCell.cts.Token);
                }
                // _currentlyInAudioTransition = false;
            }
        }

        private async void PlayAudio(ExerciseVideoPlayerCell audioCell)
        {
            _currentExerciseCellPlayingAudio = audioCell;

            if (_stopAllAudio)
                return;

            try
            {
                if (_continueVideoFlow == true)
                {
                    if (audioCell.ContentTypeValue == 585860000 && _currentExerciseCellPlayingVideo != audioCell)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            LogicForClosingAndOpeningVideo(audioCell);
                        });
                    }
                    _continueVideoFlow = false;
                }

                if (!audioCell.IsAudioCompletePlaying)
                {
                    //if not done playing, allow to continue or start
                    var audioFilesToPlayTemp = audioCell.AudioFiles.OrderBy(x => x.SequenceNumber).ToList();

                    var audioFilesToPlay = new List<LocalDBAudio>();

                    foreach (var item in audioFilesToPlayTemp)
                    {
                        if (item.SequenceNumber >= audioCell.AudioClipToPlaySequenceNumber)
                        {
                            audioFilesToPlay.Add(item);
                        }
                    }

                    if (audioFilesToPlay.Count() > 0)
                    {
                        var lastSequenceNumber = audioFilesToPlay.OrderByDescending(x => x.SequenceNumber).Select(x => x.SequenceNumber).First();
                        audioFilesToPlay.OrderBy(x => x.SequenceNumber).ToList();

                        try
                        {
                            audioCell.cts.Token.ThrowIfCancellationRequested();

                            foreach (var item in audioFilesToPlay)
                            {
                                _currentExerciseCellPlayingAudio.AudioClipToPlaySequenceNumber = item.SequenceNumber;

                                if (item.IsRepeat)
                                {
                                    await Task.Delay(item.PreDelay);

                                    for (int i = 0; i < item.NumberOfRepeats; i++)
                                    {
                                        audioCell.cts.Token.ThrowIfCancellationRequested();
                                            var AudioContent = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.Id == item.AudioContentId).FirstOrDefaultAsync();
                                            DependencyService.Get<IAudio>().PlayAudioFileFromFile(_rootFilePath + AudioContent.AudioFilePath);
                                            await Task.Delay(AudioContent.LengthMilliseconds);
                                            await Task.Delay(item.RepeatCycleSeconds);
                                        
                                    }
                                }
                                else
                                {
                                    await Task.Delay(item.PreDelay);
                                    audioCell.cts.Token.ThrowIfCancellationRequested();

                                    var AudioContent = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.Id == item.AudioContentId).FirstOrDefaultAsync();
                                    DependencyService.Get<IAudio>().PlayAudioFileFromFile(_rootFilePath + AudioContent.AudioFilePath);
                                    await Task.Delay(AudioContent.LengthMilliseconds);

                                }

                                if (item.SequenceNumber == lastSequenceNumber)
                                {
                                    _currentExerciseCellPlayingAudio.IsAudioCompletePlaying = true;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            //await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName + ": " + ex.ToString(), "OK");
                            Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "PlayAudio 1" } });
                        }
                    }

                    //content types 
                    //exercise reps = 585860000
                    //exercise time = 585860001
                    //rest time = 585860002
                    //seperator = 585860004
                    //stretch time = 585860003

                    try
                    {
                        if (audioCell.NextExerciseCell != null && !_stopAllAudio)
                        {
                            //TYPE: 1 - seperator, 2- time based, 3 - rest, 4 - rep based
                            if (audioCell.ContentTypeValue == 585860004 && !audioCell.NextExerciseCell.IsComplete)
                            {
                                //Seperator, just play next audio
                                if (audioCell.NextExerciseCell.ContentTypeValue == 585860000 || audioCell.NextExerciseCell.ContentTypeValue == 585860002 || audioCell.NextExerciseCell.ContentTypeValue == 585860004)
                                {
                                    //Rep based, or seperator or rest
                                    await Task.Run(() =>
                                    {
                                        audioCell.cts.Token.ThrowIfCancellationRequested();
                                        _continueVideoFlow = true;
                                        PlayAudio(audioCell.NextExerciseCell);
                                    }, audioCell.NextExerciseCell.cts.Token);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "PlayAudio 2" } });
                    }
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "PlayAudio 3" } });
            }

        }

        private async void SaveTimeBasedExercise(ExerciseVideoPlayerCell e)
        {
            var ContactAction = await _connection.Table<LocalDBContactAction>().Where(x => x.GuidCRM == e.ContactActionGuid).FirstOrDefaultAsync();
            if (ContactAction == null)
            {
                return;
            }

            if (e.ContentTypeValue == 585860001 || e.ContentTypeValue == 585860003)
            {
                //exercise or sketch time
                ContactAction.ActualTimeSeconds = e.RequiredTimeSeconds;
            }
            else if (e.ContentTypeValue == 585860002)
            {
                //rest
                ContactAction.ActualRestTimeSeconds = e.RequiredTimeSeconds;
            }

            ContactAction.IsComplete = true;
            ContactAction.CompletedOn = DateTime.UtcNow;
            await _connection.UpdateAsync(ContactAction);
        }

        private async void WorkoutComplete()
        {
            var result = await DisplayAlert("Mark Complete", "Your workout will now be saved.", "Mark Complete", "Not now");
            if (result)
            {
                SaveWorkout();
            }
        }

        private void StopAllAudio()
        {
            try
            {
                _stopAllAudio = true;
                DependencyService.Get<IAudio>().Stop();
                if (_currentExerciseCellPlayingAudio != null)
                {
                    _currentExerciseCellPlayingAudio.cts.Cancel();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "StopAllAudio()" } });
            }
        }

        private void ShowVideoInFullScreen(ExerciseVideoPlayerCell e)
        {
            try
            {
                if (_currentExerciseCellPlayingVideo == null && _videoPlayer == null)
                    return;

                if (_videoPlayerHeight == _videoPlayerHeight2)
                {
                    _videoPlayerHeight = _videoPlayerHeight1;
                }
                else
                {
                    _videoPlayerHeight = _videoPlayerHeight2;
                }

                _currentExerciseCellPlayingVideo.videoRL.HeightRequest = _videoPlayerHeight;
                _videoPlayer.HeightRequest = _videoPlayerHeight;

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ShowVideoInFullScreen" } });
            }

            //try
            //{
            //    _videoPlayer.Pause();
            //    _IsReturningFromFullScreen = true;
            //    _isOnFullScreen = true;
            //    await Navigation.PushModalAsync(new WorkoutVideo(_rootFilePath + e.videoFilePath), false);
            //} catch(Exception ex)
            //{
            //    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ShowVideoInFullScreen" } });
            //}
        }

        //private void IsReturingFromFullScreen_PlayVideo()
        //{
        //    try
        //    {

        //        if (_videoPlayer.State == Octane.Xamarin.Forms.VideoPlayer.Constants.PlayerState.Paused)
        //        {
        //            _videoPlayer.Play();
        //            //System.Diagnostics.Debug.WriteLine("RETURNED FROM FULL SCREEN. ");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "IsReturingFromFullScreen_PlayVideo()" } });
        //    }
        //}

        private void PlayExerciseVideo(ExerciseVideoPlayerCell e)
        {
            //Video Files here will be from the localDB downloaded
            // IFolder rootFolder = FileSystem.Current.LocalStorage;
            //var file = rootFolder.Path;
            //var filePath = file + "/Programs/progID/workoutID/video1.mp4";

            //videoPlayer.Source = VideoSource.FromFile(_rootFilePath + e.videoFilePath); //Position 1 - works but can switch with another one
            //videoPlayer.Source = VideoSource.FromFile(filePath);

            try
            {
                if (!_isVideoPlayerTranslationInProgress)
                {
                    _isVideoPlayerTranslationInProgress = true;
                    //videoPlayer.Source = VideoSource.FromFile(_rootFilePath + e.videoFilePath); //Position 2 - show fix issue with position 1

                    if (_isVideoPlayerOpen)
                    {
                        if (_openExerciseCellInfo.num == e.num)
                        {
                            //currently: OPEN - need to CLOSE
                            //CLOSE currently selected                         
                            CloseExerciseVideo(e);
                            //_currentExerciseCell.videoSL.IsVisible = false;
                            _isVideoPlayerTranslationInProgress = false;
                        }
                        else
                        {
                            //currently: OPEN - need to CLOSE
                            //CLOSE currently open 
                            CloseExerciseVideo(_openExerciseCellInfo);
                            //OPEN currently selected
                            OpenExerciseVideo(e);
                            //_currentExerciseCell.videoSL.IsVisible = true;
                            _isVideoPlayerOpen = true;
                            //await Sleep(200);
                        }
                    }
                    else if (!_isVideoPlayerOpen)
                    {
                        //currently: CLOSED - need to OPEN
                        //OPEN currently selected  
                        OpenExerciseVideo(e);
                        //_currentExerciseCell.videoSL.IsVisible = true;
                        _isVideoPlayerOpen = true;
                        //await Sleep(200);
                    }
                    //await Sleep(500);
                    _isVideoPlayerTranslationInProgress = false;
                }
                else
                {
                    //await Sleep(500);
                    _isVideoPlayerTranslationInProgress = false;
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "PlayExerciseVideo" } });
            }

        }

        private void CloseExerciseVideo(ExerciseVideoPlayerCell e)
        {
            //double startingHeight; // the layout's height when we begin animation
            //double endingHeight; // final desired height of the layout
            //uint rate = 16; // pace at which aniation proceeds
            //uint length = 400; // one second animation
            // var overlay = e.overlaySL;
            //currently: OPEN - need to CLOSE
            //Action<double> callback = input => { _openVideoRL.HeightRequest = input; }; // update the height of the layout with this callback
            //startingHeight = _openVideoRL.Height;
            // endingHeight = 0D;
            //await Sleep(500);
            _currentExerciseCellPlayingVideo = new ExerciseVideoPlayerCell();
            //_openExerciseCellInfo = new ExerciseVideoPlayerCell();

            try
            {
                e.videoRL.HeightRequest = 0;
                _openVideoRL.HeightRequest = 0;

                //_openVideoRL.Animate("invis", callback, startingHeight, endingHeight, rate, length, Easing.SinOut);
                //_videoPlayer.Repeat = false;
                _videoPlayer.AutoPlay = false;

                _isVideoPlayerOpen = false;
                _videoPlayer.Pause();


                //videoPlayer.Repeat = false;
                //videoPlayer.AutoPlay = false; 

                //_isVideoPlayerOpen = false;
                // await Task.Delay(50); // this prevents the next video from showing in the already playing videoSL
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "CloseExerciseVideo" } });
            }
        }

        private void OpenExerciseVideo(ExerciseVideoPlayerCell e)
        {
            try
            {
                _videoPlayer.Pause();
                e.videoSL.Children.Add(_videoPlayer);
                //_videoPlayer.Source = VideoSource.FromFile(_rootFilePath + e.videoFilePath); //added this

                _videoPlayer.Source = new FileMediaSource
                {
                    File = _rootFilePath + e.videoFilePath
                }; 
                _currentExerciseCellPlayingVideo = e;

                //move video player to 
                var layout = e.videoRL;
                var overlay = e.overlaySL;
                layout.Opacity = 1;
                overlay.Opacity = 1;

                //_videoPlayer.Repeat = true;
                _videoPlayer.AutoPlay = true;

                _openExerciseCellInfo = _currentExerciseCellPlayingVideo;
                _openVideoRL = _currentExerciseCellPlayingVideo.videoRL;
                _currentExerciseCellPlayingVideo.IsFirstPlay = true;

                _videoPlayer.Play();

                if (_currentExerciseCellPlayingVideo == null)
                    return;

                if (_currentExerciseCellPlayingVideo.IsFirstPlay)
                {
                    FadeVideoPlayerIn();
                    _currentExerciseCellPlayingVideo.IsFirstPlay = false;
                }
                else
                {
                    _currentExerciseCellPlayingVideo.overlaySL.Opacity = 0.1;
                }
                _currentExerciseCellPlayingVideo.videoRL.HeightRequest = _videoPlayerHeight;

                _isVideoPlayerOpen = true;
                _isVideoPlayerTranslationInProgress = false;

                if (e.num > 2)
                {
                    //don't scroll for first exercise
                    ScrollToTopOfCell();
                }
                //await MyScrollView.ScrollToAsync(e.parentSL, ScrollToPosition.Start, true);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "OpenExerciseVideo" } });
            }

        }

        private void ScrollToTopOfCell()
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (_currentExerciseCellPlayingVideo != null)
                    {
                        if (_currentExerciseCellPlayingVideo.parentSL != null)
                            await MyScrollView.ScrollToAsync(_currentExerciseCellPlayingVideo.parentSL, ScrollToPosition.Start, false);
                    }
                });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ScrollToTopOfCell()" } });
                //Device.BeginInvokeOnMainThread(async () =>
                //{
                //    await DisplayAlert("scroll error", ex.ToString(), "ok");
                //});
            }
        }

        private void MoreRepsOnAction(ExerciseVideoPlayerCell e)
        {
            //increase by one, need some kinda hold logic to speed it up later on
            if (!e.IsComplete)
            {
                var currentReps = 0;
                var repsText = e.repsLabel.Text;
                currentReps = int.Parse(repsText);

                if (currentReps < 500)
                {
                    currentReps = currentReps + 1;
                    e.repsLabel.Text = currentReps.ToString();
                }
            }
        }

        private void LessRepsOnAction(ExerciseVideoPlayerCell e)
        {
            if (!e.IsComplete)
            {
                var currentReps = 0;
                var repsText = e.repsLabel.Text;
                currentReps = int.Parse(repsText);

                if (currentReps > 0)
                {
                    currentReps = currentReps - 1;
                    e.repsLabel.Text = currentReps.ToString();
                }
            }

        }

        private float GetWeightIncreaseIncrement(float currentWeight)
        {
            float increment = 0F;

            if (currentWeight < 1500F)
            {
                if (currentWeight >= 10F && currentWeight % 2.5F == 0)
                {
                    increment = 2.5F;
                }
                else
                {
                    if (currentWeight == 0F)
                    {
                        increment = 3F;
                    }
                    else if (currentWeight == 3F)
                    {
                        increment = 2F;
                    }
                    else if (currentWeight == 5F)
                    {
                        increment = 3F;
                    }
                    else if (currentWeight == 8F)
                    {
                        increment = 2F;
                    }
                    else
                    {
                        increment = 1F;
                    }
                }
            }
            return increment;
        }

        private void MoreWeightOnAction(ExerciseVideoPlayerCell e)
        {
            //use the scale: 3,5,8,10,12.5,15,17.5,20,22,5,25,....see notes for details
            if (!e.IsComplete)
            {
                var currentWeight = 0F;
                var weightText = e.weightLabel.Text;
                currentWeight = float.Parse(weightText);

                float increment = GetWeightIncreaseIncrement(currentWeight);
                currentWeight += increment;
                e.weightLabel.Text = currentWeight.ToString();
            }
        }

        private float GetWeightDecreaseIncrement(float currentWeight)
        {
            float increment = 0F;

            if (currentWeight > 0F)
            {
                if (currentWeight >= 12.5F && currentWeight % 2.5F == 0)
                {
                    increment = 2.5F;
                }
                else
                {
                    if (currentWeight == 3F)
                    {
                        increment = 3F;
                    }
                    else if (currentWeight == 5F)
                    {
                        increment = 2F;
                    }
                    else if (currentWeight == 8F)
                    {
                        increment = 3F;
                    }
                    else if (currentWeight == 10F)
                    {
                        increment = 2F;
                    }
                    else
                    {
                        increment = 1F;
                    }
                }
            }
            return increment;
        }

        private void LessWeightOnAction(ExerciseVideoPlayerCell e)
        {
            try
            {
                if (!e.IsComplete)
                {
                    var currentWeight = 0F;
                    var weightText = e.weightLabel.Text;
                    currentWeight = float.Parse(weightText);

                    float increment = GetWeightDecreaseIncrement(currentWeight);
                    currentWeight -= increment;

                    if (currentWeight < 0F)
                        currentWeight = 0F;

                    e.weightLabel.Text = currentWeight.ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task Sleep(int ms)
        {
            await Task.Delay(ms);
        }

        private async void ExitBtn_Clicked()
        {
            try
            {
                var result = await DisplayAlert("Are you done?", "Would you like to mark this workout as complete?", "Mark Complete", "Exit only");
                if (result)
                {
                    SaveWorkout();
                }
                else
                {
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program Day", _ProgramDay.Heading }, { "Sequence Number", _ProgramDay.SequenceNumber.ToString() }, { "Action", "Exit" } });
                    await Navigation.PopModalAsync();
                    MessagingCenter.Send(this, "ExitWorkout");

                }
                _isOnPage = false;
            }
            catch (Exception ex)
            {

            }
        }

        private async void LeavePage()
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "LeavePage()" } });
            }
        }

        private async void SaveWorkout()
        {
            try
            {
                var ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDayGuid).FirstOrDefaultAsync();
                if (ContactProgramDay == null)
                    return;

                ContactProgramDay.IsComplete = true;
                ContactProgramDay.StateCodeValue = 1;//Inactive
                ContactProgramDay.StatusCodeValue = 585860004; //Completed
                ContactProgramDay.ActualStartDate = DateTime.UtcNow;
                await _connection.UpdateAsync(ContactProgramDay);

                Branch branch = Branch.GetInstance();
                branch.SendEvent(new BranchEvent("Completed_Workout"));

                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program Day", _ProgramDay.Heading }, { "Sequence Number", _ProgramDay.SequenceNumber.ToString() }, { "Action", "Mark Complete" } });
                OnExitWorkoutPage();
                await Navigation.PushModalAsync(new WorkoutSurvey(_ContactProgramDayGuid), false);
            }
            catch (Exception ex)
            {

            }
        }

        private async void ExpandBtn_Clicked()
        {
            //Go to large view
            try
            {
                await Navigation.PushModalAsync(new WorkoutSurvey(_ContactProgramDayGuid), false);
            }
            catch (Exception ex)
            {

            }
        }

    }

}
