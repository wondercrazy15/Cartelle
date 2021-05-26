﻿using Plugin.Connectivity;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Connectivity.Abstractions;
using Stance.Models.API;
using Stance.Models.LocalDB;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using ModernHttpClient;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using PCLStorage;
using System.IO;
using Stance.ViewModels;
using FFImageLoading.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using BranchXamarinSDK;

namespace Stance.Pages.Sub
{
    public partial class ProgramDayOverview : BaseContentPage
    {
        private const string _PageName = "Program Day Overview";
        private string _videoUrl = String.Empty;
        private LocalDBProgramDay _ProgramDay = new LocalDBProgramDay();
        private LocalDBContactProgramDayV2 _ContactProgramDay = new LocalDBContactProgramDayV2();
        private bool _DownloadingInProgress = false;
        private Guid _ProgramDayGuid = Guid.Empty;
        private string _ActiveProgram = "";
        //private int _ProgramId;
        List<APIAction> _actions = new List<APIAction>();
        private bool _programIsPurchased = false;
        private bool _isFromSchedule = false;
        private Dictionary<Guid, int> _numberOfDownloadAttempts = new Dictionary<Guid, int>();

        public ProgramDayOverview(LocalDBProgramDay ProgramDay, bool isFromSchedule = false, LocalDBContactProgramDayV2 ContactProgramDay = null)
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily && isFromSchedule)
            {
                var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //safeInsets.Bottom = 20;
                safeInsets.Top = 0;
                this.Padding = safeInsets;
                this.BackgroundColor = Color.Black;                
            }
            MessagingCenter.Subscribe<WorkoutSurvey>(this, "WorkoutSurveyComplete", (sender) => { WorkoutComplete(); });
            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");
            Title.Text = ProgramDay.Heading;

            _ContactProgramDay = ContactProgramDay;
            _ProgramDay = ProgramDay;
            _ProgramDayGuid = ProgramDay.GuidCRM;
            _isFromSchedule = isFromSchedule;

            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            if (IsInternetConnected())
            {
                NoNetwork.IsVisible = false;
            }
            else
            {
                NoNetwork.IsVisible = true;
            }

            //var apiHelper = new MockApiHelper();
            //var athlete = apiHelper.GetAthlete("1");
            //var imgSource = new UriImageSource() { Uri = new Uri(ProgramDay.PhotoUrl) };
            //imgSource.CachingEnabled = true;
            //imgSource.CacheValidity = TimeSpan.FromDays(7);
            //programImage.Source = imgSource;
            if (isFromSchedule)
            {
                WorkoutName.Text = ProgramDay.Heading;
                //ProgramDayNameSL.IsVisible = false;
                ProgressBar.IsVisible = true;
                ProgressBar.IsEnabled = true;

                if (ContactProgramDay != null)
                {
                    var cp = _connection.Table<LocalDBContactProgram>().Where(x => x.Id == ContactProgramDay.ContactProgramId).FirstOrDefaultAsync().Result;
                    if(cp != null)
                    {
                        if(cp.ChallengeId != 0)
                        {
                            DownloadBtn.IsVisible = false;
                            ProgressBar.IsVisible = false;
                            DetailsBar.IsVisible = true;
                        } else
                        {
                            SetupDowload(ContactProgramDay);
                        }
                    } else
                    {
                        SetupDowload(ContactProgramDay);
                    }                
                }
                else
                {
                    DownloadBtn.Text = "DOWNLOAD WORKOUT";
                }
            }
            else
            {
                //progDayName.Text = ProgramDay.Heading;
                DownloadSL.IsVisible = false;
                DetailsBar.IsVisible = true;
            }

            NumberOfExercises.Text = ProgramDay.TotalExercises.ToString();
            TotalTime.Text = ProgramDay.TimeMinutes.ToString();
            Level.Text = ProgramDay.Level;
            GetActions();
            //ShowLoginPopUp();
        }

        private void SetupDowload(LocalDBContactProgramDayV2 ContactProgramDay)
        {
            _ContactProgramDay = _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.Id == ContactProgramDay.Id).FirstOrDefaultAsync().Result;

            LocalDBContactProgramDayV2 ContactProgramDay_toUse = null;

            if (ContactProgramDay.StatusCodeValue == 585860004) //complete
            {
                if (ContactProgramDay.ActualStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                {
                    //completed today, show workout btn


                }
                else
                {
                    //not completed today
                    //check if already downloaded
                    //check if downloaded is complete
                    var ContactProgramDay_Repeat = _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ProgramDayId == _ContactProgramDay.ProgramDayId && x.IsRepeat == true && x.IsDownloaded == true).OrderByDescending(x => x.DownloadedOn).ToListAsync().Result;

                    if (ContactProgramDay_Repeat.Count() != 0)
                    {
                        foreach (var cpd in ContactProgramDay_Repeat)
                        {
                            if (cpd.IsComplete == false)
                            {
                                //Show use this day
                                ContactProgramDay_toUse = cpd;
                                break;
                            }
                            else if (cpd.IsComplete == true && cpd.ActualStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                            {
                                //Show use this day
                                ContactProgramDay_toUse = cpd;
                                break;
                            }
                        }

                        if (ContactProgramDay_toUse == null)
                        {
                            //Must download a new workout
                            DownloadBtn.Text = "DOWNLOAD WORKOUT";
                            //ADD CPD NEW
                            AddNewContactProgramDay();
                        }
                        else
                        {
                            //show the downloaded workout
                            SetButtonText(ContactProgramDay_toUse);
                        }

                    }
                    else
                    {
                        //Must download a new workout
                        DownloadBtn.Text = "DOWNLOAD WORKOUT";
                        //ADD CPD NEW
                        AddNewContactProgramDay();
                    }
                }
            }
            else //not complete
            {
                //check if already downloaded
                SetButtonText(_ContactProgramDay);
            }


        }

        private void AddNewContactProgramDay()
        {
            _ContactProgramDay = new LocalDBContactProgramDayV2
            {
                GuidCRM = Guid.Empty,
                IsRepeat = true,
                DayTypeValue = _ContactProgramDay.DayTypeValue,
                SequenceNumber = _ContactProgramDay.SequenceNumber,
                StateCodeValue = 0, //active
                StatusCodeValue = 585860000, //in progress 

                ContactProgramId = _ContactProgramDay.ContactProgramId,
                ProgramDayId = _ProgramDay.Id,
                ContactProgramDayParentId = _ContactProgramDay.Id,
            };
        }

        private void SetButtonText(LocalDBContactProgramDayV2 ContactProgramDay)
        {
            _ContactProgramDay = ContactProgramDay;

            if (ContactProgramDay.IsDownloaded)
            {
                DownloadBtn.Text = "WORKOUT";
            }
            else
            {
                DownloadBtn.Text = "DOWNLOAD WORKOUT";
            }
        }

        private async void WorkoutComplete()
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
            }

        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            if (_DownloadingInProgress)
            {
                await DisplayAlert("Downloading", "Please stay on this page while downloading.", "OK");
            }
            else
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Exit" } });
                try
                {
                    await Navigation.PopModalAsync();
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                }
            }
        }

        protected override void OnAppearing()
        {
            if (_ContactProgramDay == null)
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program Day", _ProgramDay.Heading }, { "Action", "OnAppearing" } });
            }
            else
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program Day", _ProgramDay.Heading }, { "Action", "OnAppearing via Schedule" } });
            }
            base.OnAppearing();
        }

        private async void WatchVideoBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new WatchVideoPage(_videoUrl));
        }

        private async void Download_Clicked(object sender, EventArgs e)
        {
            if (_ContactProgramDay == null)
                return;

            bool allowDownload = Task.Run(() => Subscription.HasSubscriptionToProgram("download", _ProgramDay.ProgramId, _ProgramDay.SequenceNumber)).Result;
            //bool allowDownload = true;
            if (!allowDownload)
            {
                var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == _ProgramDay.ProgramId).FirstOrDefaultAsync();
                if(Program == null)
                {
                    await DisplayAlert("Opps", "We had an error. Try to refresh your App.", "Ok");
                    return;
                }
                await Navigation.PushModalAsync(new IAP(Program.GuidCRM), false);
                return;
            }

            //_ContactProgramDay = await _connection.Table<LocalDBContactProgramDay>().Where(x => x.Id == _ContactProgramDay.Id).FirstOrDefaultAsync();           
            if (!_ContactProgramDay.IsDownloaded)
            {
                bool isWifiConnected = CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.WiFi);

                if (CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.WiFi))
                {
                    DownloadProgramDay();
                }
                else
                {
                    var result = await DisplayAlert("Faster on Wifi", "Connect to Wifi for a faster download.", "Download", "Not now");

                    if (result)
                    {
                        DownloadProgramDay();
                    }
                }
            }
            else
            {
                await Navigation.PushModalAsync(new ProgramDay_v2(_ContactProgramDay.GuidCRM));
            }

        }

        private async void DownloadProgramDay()
        {
            _DownloadingInProgress = true;

            if (!IsInternetConnected())
            {
                DownloadBtn.Text = "NO INTERNET";
                DownloadBtn.IsEnabled = true;
                _DownloadingInProgress = false;
                return;
            }

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Started Downloading Program" } });

                DownloadBtn.IsEnabled = false;
                DownloadBtn.Text = "GETTING WORKOUT...";
                ProgressBar.IsVisible = true;
                await ProgressBar.ProgressTo(.00D, 1, Easing.Linear);

                var parentCPDGuid = Guid.Empty;
                bool isScheduleDeviation = false;

                if (_ContactProgramDay.GuidCRM == Guid.Empty)
                {
                    if (_ContactProgramDay.ContactProgramDayParentId != null && _ContactProgramDay.ContactProgramDayParentId != 0)
                    {
                        var Parent_ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.Id == _ContactProgramDay.ContactProgramDayParentId).FirstOrDefaultAsync();

                        if (Parent_ContactProgramDay != null)
                        {
                            parentCPDGuid = Parent_ContactProgramDay.GuidCRM;
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Repeat Contact Program Day" } });

                        }
                    }
                }

                if (_ContactProgramDay.ScheduledStartDate != null && _ContactProgramDay.ScheduledStartDate?.ToLocalTime().Date != DateTime.UtcNow.ToLocalTime().Date)
                {
                    isScheduleDeviation = true;
                }

                if (_numberOfDownloadAttempts.ContainsKey(_ContactProgramDay.GuidCRM))
                {
                    _numberOfDownloadAttempts[_ContactProgramDay.GuidCRM] += 1;
                }
                else
                {
                    _numberOfDownloadAttempts.Add(_ContactProgramDay.GuidCRM, 1);
                }

                var downloadResponse = await WebAPIService.GetContactProgramDayToDownload(_client, _ContactProgramDay.GuidCRM, parentCPDGuid, isScheduleDeviation);

                if (downloadResponse.Message == "download limit reached")
                {
                    DownloadBtn.Text = "DOWNLOAD WORKOUT";
                    ProgressBar.IsVisible = false;
                    await DisplayAlert("Download Limit", "Reached maximum download attempts. Wait 15 minutes, assure that you have a good internet connection and you'll be able to try again.", "Ok");
                }
                else if (downloadResponse.Message == "bad request")
                {
                    DownloadBtn.Text = "DOWNLOAD WORKOUT";
                    ProgressBar.IsVisible = false;
                    await DisplayAlert("Try Again", "There was an issue. Please try again.", "Ok");
                    DownloadBtn.IsEnabled = true;
                }
                else if (downloadResponse.Message != "success")
                {
                    DownloadBtn.Text = "DOWNLOAD WORKOUT";
                    ProgressBar.IsVisible = false;
                    await DisplayAlert("Try Again", "There was an error. Please try again later.", "Ok");
                    DownloadBtn.IsEnabled = true;
                }
                else if (downloadResponse.Message == "success")
                {
                    DownloadBtn.Text = "DOWNLOADING...";

                    var ContactActionsFromAPI = downloadResponse.ContactActions;
                    var AudiosFromAPI = downloadResponse.Audios;

                    if (ContactActionsFromAPI.Count() == 0)
                    {
                        _DownloadingInProgress = false;
                        return;
                    }

                    int totalFiles = ContactActionsFromAPI.Count() + AudiosFromAPI.Count();
                    double incrementPerFile = (1D / (totalFiles)) * 0.7D;
                    int totalDownloads_AC = 0;
                    int totalDownloads_AuC = 0;

                    if (_ContactProgramDay.GuidCRM == Guid.Empty)
                    {
                        //create a new CPD from the response
                        _ContactProgramDay.GuidCRM = downloadResponse.ContactProgramDayGuid;
                        _ContactProgramDay.ReceivedOn = DateTime.UtcNow;
                        _ContactProgramDay.ProgramDayId = _ProgramDay.Id;
                        //Is Schedule Deviation???
                        if (!_ContactProgramDay.ScheduledStartDate.HasValue)
                        {
                            _ContactProgramDay.IsScheduleDeviation = true;
                        }
                        else if (_ContactProgramDay.ScheduledStartDate?.ToLocalTime().Date != DateTime.UtcNow.ToLocalTime().Date)
                        {
                            _ContactProgramDay.IsScheduleDeviation = true;
                        }
                        await _connection.InsertAsync(_ContactProgramDay);
                    }

                    //Assume all Contact Actions are from the same ProgramDay
                    var ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDay.GuidCRM).FirstOrDefaultAsync();
                    if (ContactProgramDay == null)
                    {
                        ContactProgramDay = new LocalDBContactProgramDayV2
                        {
                            GuidCRM = downloadResponse.ContactProgramDayGuid,
                            ReceivedOn = DateTime.UtcNow,
                            ProgramDayId = _ProgramDay.Id,
                        };
                        await _connection.InsertAsync(ContactProgramDay);
                    }

                    _ContactProgramDay = ContactProgramDay;

                    var ContactActionsToInsert = new List<LocalDBContactAction>();
                    List<Task<DownloadResult>> downloads = new List<Task<DownloadResult>>();
                    List<Task<DownloadResultAudio>> downloadAudio = new List<Task<DownloadResultAudio>>();

                    Task<List<LocalDBContactAction>> t1_ca = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBContactAction>().ToListAsync();
                    });
                    Task<List<LocalDBAction>> t2_a = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBAction>().ToListAsync();
                    });
                    Task<List<LocalDBActionContentV2>> t3_ac = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBActionContentV2>().ToListAsync();
                    });
                    Task<List<LocalDBAudio>> t4_au = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBAudio>().ToListAsync();
                    });
                    Task<List<LocalDBAudioContentV2>> t5_auc = Task.Run(async () =>
                    {
                        return await _connection.Table<LocalDBAudioContentV2>().ToListAsync();
                    });
                    await Task.WhenAll(t1_ca, t2_a, t3_ac);

                    var ContactActions = t1_ca.Result;
                    var Actions = t2_a.Result;
                    var ActionContents = t3_ac.Result;

                    foreach (var contactAction in ContactActionsFromAPI)
                    {
                        var ContactAction = ContactActions.Where(x => x.GuidCRM == contactAction.GuidCRM).FirstOrDefault();
                        if (ContactAction == null)
                        {
                            ContactAction = new LocalDBContactAction();
                            ContactAction.Synced = contactAction.Synced;
                            ContactAction.GuidCRM = contactAction.GuidCRM;
                            ContactAction.StateCodeValue = contactAction.StateCodeValue;
                            ContactAction.StatusCodeValue = contactAction.StatusCodeValue;
                            ContactAction.ContactProgramDayId = ContactProgramDay.Id;

                            var Action = Actions.Where(x => x.GuidCRM == contactAction.Action.GuidCRM).FirstOrDefault();
                            if (Action == null)
                            {
                                Action = new LocalDBAction
                                {
                                    GuidCRM = contactAction.Action.GuidCRM,
                                    StateCodeValue = contactAction.Action.StateCodeValue,
                                    StatusCodeValue = contactAction.Action.StatusCodeValue,
                                    SequenceNumber = contactAction.Action.SequenceNumber,
                                    ContentTypeValue = contactAction.Action.ContentTypeValue,
                                    IsTrainingRound = contactAction.Action.IsTrainingRound,
                                    NumberOfReps = contactAction.Action.NumberOfReps,
                                    WeightLbs = contactAction.Action.WeightLbs,
                                    TimeSeconds = contactAction.Action.TimeSeconds,
                                    IntensityValue = contactAction.Action.IntensityValue,
                                    Heading = contactAction.Action.Heading,
                                    ProgramDayId = _ProgramDay.Id
                                };
                                await _connection.InsertAsync(Action);
                                Actions.Add(Action);
                            }
                            else
                            {
                                Action.StateCodeValue = contactAction.Action.StateCodeValue;
                                Action.StatusCodeValue = contactAction.Action.StatusCodeValue;
                                Action.SequenceNumber = contactAction.Action.SequenceNumber;
                                Action.ContentTypeValue = contactAction.Action.ContentTypeValue;
                                Action.IsTrainingRound = contactAction.Action.IsTrainingRound;
                                Action.NumberOfReps = contactAction.Action.NumberOfReps;
                                Action.WeightLbs = contactAction.Action.WeightLbs;
                                Action.TimeSeconds = contactAction.Action.TimeSeconds;
                                Action.IntensityValue = contactAction.Action.IntensityValue;
                                Action.Heading = contactAction.Action.Heading;
                                Action.ProgramDayId = _ProgramDay.Id;
                                await _connection.UpdateAsync(Action);
                            }
                            ContactAction.ActionId = Action.Id;
                            bool mustDownloadActionContent = false;

                            var ActionContent = ActionContents.Where(x => x.GuidCRM == contactAction.ActionContent.GuidCRM).FirstOrDefault();
                            if (ActionContent == null)
                            {
                                ActionContent = new LocalDBActionContentV2
                                {
                                    GuidCRM = contactAction.ActionContent.GuidCRM,
                                    Heading = contactAction.ActionContent.Heading,
                                    PhotoUrl = contactAction.ActionContent.PhotoUrl,
                                    VideoUrl = contactAction.ActionContent.VideoUrl,
                                    ContentTypeValue = contactAction.Action.ContentTypeValue,
                                    IsPreview = false,
                                    LastDownloadAttempt = DateTime.UtcNow,
                                    ContactProgramDayId = ContactProgramDay.Id
                                };
                                await _connection.InsertAsync(ActionContent);
                                ActionContents.Add(ActionContent);
                                mustDownloadActionContent = true;
                            }
                            else
                            {
                                if (ActionContent.PhotoUrl != contactAction.ActionContent.PhotoUrl || ActionContent.VideoUrl != contactAction.ActionContent.VideoUrl)
                                {
                                    ActionContent.PhotoFilePath = "";
                                    ActionContent.VideoFilePath = "";
                                    ActionContent.LastDownloadAttempt = DateTime.UtcNow;
                                    mustDownloadActionContent = true;
                                }
                                ActionContent.Heading = contactAction.ActionContent.Heading;
                                ActionContent.PhotoUrl = contactAction.ActionContent.PhotoUrl;
                                ActionContent.VideoUrl = contactAction.ActionContent.VideoUrl;
                                ActionContent.ContentTypeValue = contactAction.Action.ContentTypeValue;
                                ActionContent.IsPreview = false;
                                ActionContent.ContactProgramDayId = ContactProgramDay.Id;
                                await _connection.UpdateAsync(ActionContent);
                            }

                            if (mustDownloadActionContent)
                            {
                                //download action content
                                var videoUrl = contactAction.ActionContent.VideoUrl;
                                var photoUrl = contactAction.ActionContent.PhotoUrl;
                                if (videoUrl != "" && photoUrl != "" && videoUrl.Contains("https://") && Path.GetExtension(videoUrl) == ".mp4" && photoUrl.Contains("https://"))
                                {
                                    var videoFileName = contactAction.ActionContent.GuidCRM.ToString() + Path.GetExtension(videoUrl);
                                    var photoFileName = contactAction.ActionContent.GuidCRM.ToString() + Path.GetExtension(photoUrl);
                                    downloads.Add(DownloadWithUrlTrackingTaskAsync(photoFileName, videoFileName, photoUrl, videoUrl, contactAction.ActionContent.GuidCRM, _ContactProgramDay.GuidCRM));
                                    totalDownloads_AC++;
                                }
                            }
                            ContactAction.ActionContentId = ActionContent.Id;
                            ContactActionsToInsert.Add(ContactAction);
                        }

                        if (ProgressBar.Progress < 1D)
                        {
                            await ProgressBar.ProgressTo(ProgressBar.Progress + incrementPerFile, 1, Easing.Linear);
                        }
                    }
                    await _connection.InsertAllAsync(ContactActionsToInsert);

                    var ListOfActionsToUpdate = new List<LocalDBAction>();
                    foreach (var contactAction in ContactActionsToInsert)
                    {
                        var Action = Actions.Where(x => x.Id == contactAction.ActionId).FirstOrDefault();
                        Action.ActionContentId = contactAction.ActionContentId;
                        ListOfActionsToUpdate.Add(Action);
                    }
                    await _connection.UpdateAllAsync(ListOfActionsToUpdate);

                    await Task.WhenAll(t4_au, t5_auc);
                    var AudioContents = t5_auc.Result;
                    var Audios = t4_au.Result;

                    if (AudiosFromAPI.Count() != 0)
                    {
                        foreach (var item in AudiosFromAPI)
                        {
                            bool mustDownloadAudioContent = false;

                            var AudioContent = AudioContents.Where(x => x.GuidCRM == item.AudioContent.GuidCRM).FirstOrDefault();
                            if (AudioContent == null)
                            {
                                AudioContent = new LocalDBAudioContentV2
                                {
                                    GuidCRM = item.AudioContent.GuidCRM,
                                    LengthMilliseconds = item.AudioContent.LengthMilliseconds,
                                    AudioUrl = item.AudioContent.AudioUrl,
                                    LastDownloadAttempt = DateTime.UtcNow,
                                    ContactProgramDayId = ContactProgramDay.Id,
                                };
                                await _connection.InsertAsync(AudioContent);
                                AudioContents.Add(AudioContent);
                                mustDownloadAudioContent = true;
                            }
                            else
                            {
                                if (AudioContent.AudioUrl != item.AudioContent.AudioUrl)
                                {
                                    AudioContent.AudioFilePath = "";
                                    AudioContent.LastDownloadAttempt = DateTime.UtcNow;
                                    mustDownloadAudioContent = true;
                                }
                                AudioContent.LengthMilliseconds = item.AudioContent.LengthMilliseconds;
                                AudioContent.AudioUrl = item.AudioContent.AudioUrl;
                                AudioContent.ContactProgramDayId = ContactProgramDay.Id;
                                await _connection.UpdateAsync(AudioContent);
                            }

                            if (mustDownloadAudioContent)
                            {
                                var audioUrl = item.AudioContent.AudioUrl;
                                if (audioUrl != "" && audioUrl.Contains("https://") && (Path.GetExtension(audioUrl) == ".mp3" || Path.GetExtension(audioUrl) == ".wav"))
                                {
                                    var audioFileName = item.AudioContent.GuidCRM.ToString() + Path.GetExtension(audioUrl);
                                    downloadAudio.Add(DownloadAudioWithUrlTrackingTaskAsync(audioFileName, audioUrl, item.AudioContent.GuidCRM, _ContactProgramDay.GuidCRM));
                                    totalDownloads_AuC++;
                                }
                            }

                            var Audio = Audios.Where(x => x.GuidCRM == item.GuidCRM).FirstOrDefault();
                            if (Audio == null)
                            {
                                Audio = new LocalDBAudio
                                {
                                    GuidCRM = item.GuidCRM,
                                    SequenceNumber = item.SequenceNumber,
                                    PreDelay = item.PreDelay,
                                    IsRepeat = item.IsRepeat,
                                    NumberOfRepeats = item.NumberOfRepeats,
                                    RepeatCycleSeconds = item.RepeatCycleSeconds
                                };

                                if (AudioContent != null)
                                    Audio.AudioContentId = AudioContent.Id;

                                if (_ProgramDay != null)
                                    Audio.ProgramDayId = _ProgramDay.Id;

                                var Action = Actions.Where(x => x.GuidCRM == item.ActionGuid).FirstOrDefault();
                                if (Action != null)
                                    Audio.ActionId = Action.Id;

                                await _connection.InsertAsync(Audio);
                                Audios.Add(Audio);
                            }
                            else
                            {
                                Audio.SequenceNumber = item.SequenceNumber;
                                Audio.PreDelay = item.PreDelay;
                                Audio.IsRepeat = item.IsRepeat;
                                Audio.NumberOfRepeats = item.NumberOfRepeats;
                                Audio.RepeatCycleSeconds = item.RepeatCycleSeconds;

                                if (AudioContent != null)
                                    Audio.AudioContentId = AudioContent.Id;

                                if (_ProgramDay != null)
                                    Audio.ProgramDayId = _ProgramDay.Id;

                                var Action = Actions.Where(x => x.GuidCRM == item.ActionGuid).FirstOrDefault();
                                if (Action != null)
                                    Audio.ActionId = Action.Id;

                                await _connection.UpdateAsync(Audio);
                            }

                            if (ProgressBar.Progress < 1D)
                            {
                                await ProgressBar.ProgressTo(ProgressBar.Progress + incrementPerFile, 1, Easing.Linear);
                            }
                        }
                    }

                    double incrementDownload_AC = totalDownloads_AC / 20D;//* 0.1D * 0.5D
                    double incrementDownload_AuC = totalDownloads_AuC / 20D;
                    await ProgressBar.ProgressTo(0.70D, 1, Easing.Linear);
                    Task t1 = Task.Run(() => ProcessAudioDownloads(downloadAudio, incrementDownload_AuC));
                    Task t2 = Task.Run(() => ProcessVideoDownloads(downloads, incrementDownload_AC));
                    int timeout = 60000; //miliseconds
                    await Task.WhenAny(Task.WhenAll(t1, t2), Task.Delay(timeout));
                    //Check each file that was just downloaded for its size in bytes
                    //If any are zero then redownload them
                    List<Task<DownloadResult>> downloads2 = new List<Task<DownloadResult>>();
                    List<Task<DownloadResultAudio>> downloadAudio2 = new List<Task<DownloadResultAudio>>();
                    IFolder rootFolder = FileSystem.Current.LocalStorage;
                    IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
                    int RedownloadErrorCount_AC = 0;
                    int RedownloadErrorCount_AuC = 0;
                    int RedownloadCount_AC = 0;
                    int RedownloadCount_AuC = 0;

                    var ActionContent_Review = await _connection.Table<LocalDBActionContentV2>().Where(x => x.VideoUrl != "" && x.PhotoUrl != "" && x.VideoUrl != null && x.PhotoUrl != null && x.IsPreview != true && (x.ContentTypeValue == 585860000 || x.ContentTypeValue == 585860001 || x.ContentTypeValue == 585860003)).ToListAsync();//Rep, Time, Stretch
                    foreach (var ac in ActionContent_Review)
                    {
                        try
                        {
                            if (ac.PhotoFilePath == "" || ac.VideoFilePath == "" || ac.PhotoFilePath == null || ac.VideoFilePath == null)
                            {
                                //Redownload
                                var videoUrl = ac.VideoUrl;
                                var photoUrl = ac.PhotoUrl;

                                if (videoUrl != "" && photoUrl != "" && videoUrl.Contains("https://") && Path.GetExtension(videoUrl) == ".mp4" && photoUrl.Contains("https://"))
                                {
                                    var videoFileName = ac.GuidCRM.ToString() + Path.GetExtension(videoUrl);
                                    var photoFileName = ac.GuidCRM.ToString() + Path.GetExtension(photoUrl);
                                    downloads2.Add(DownloadWithUrlTrackingTaskAsync(photoFileName, videoFileName, photoUrl, videoUrl, ac.GuidCRM, _ContactProgramDay.GuidCRM));
                                }
                                RedownloadCount_AC++;
                            }
                            else
                            {
                                IFile photoFile = await folder.GetFileAsync(_rootFilePath + ac.PhotoFilePath);
                                IFile videoFile = await folder.GetFileAsync(_rootFilePath + ac.VideoFilePath);
                                int photoLength = await Task.Run(() => ReadMediaLength(photoFile));
                                int videoLength = await Task.Run(() => ReadMediaLength(videoFile));

                                if (photoLength == 0 || videoLength == 0)
                                {
                                    //Redownload
                                    var videoUrl = ac.VideoUrl;
                                    var photoUrl = ac.PhotoUrl;

                                    if (videoUrl != "" && photoUrl != "" && videoUrl.Contains("https://") && Path.GetExtension(videoUrl) == ".mp4" && photoUrl.Contains("https://"))
                                    {
                                        var videoFileName = ac.GuidCRM.ToString() + Path.GetExtension(videoUrl);
                                        var photoFileName = ac.GuidCRM.ToString() + Path.GetExtension(photoUrl);
                                        downloads2.Add(DownloadWithUrlTrackingTaskAsync(photoFileName, videoFileName, photoUrl, videoUrl, ac.GuidCRM, _ContactProgramDay.GuidCRM));
                                    }
                                    RedownloadCount_AC++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var error = ex.ToString();
                            RedownloadErrorCount_AC++;
                        }
                    }

                    if (RedownloadErrorCount_AC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadErrorCount_AC > 0" } });
                    }

                    if (RedownloadCount_AC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadCount_AC > 0" } });
                    }

                    var AudioContent_Review = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.AudioUrl != "" && x.AudioUrl != null).ToListAsync();
                    foreach (var auc in AudioContent_Review)
                    {
                        try
                        {
                            if (auc.AudioFilePath == "" || auc.AudioFilePath == null)
                            {
                                //Redownload
                                var audioUrl = auc.AudioUrl;

                                if (audioUrl != "" && audioUrl.Contains("https://") && (Path.GetExtension(audioUrl) == ".mp3" || Path.GetExtension(audioUrl) == ".wav"))
                                {
                                    var audioFileName = auc.GuidCRM.ToString() + Path.GetExtension(audioUrl);
                                    downloadAudio2.Add(DownloadAudioWithUrlTrackingTaskAsync(audioFileName, audioUrl, auc.GuidCRM, _ContactProgramDay.GuidCRM));
                                }
                                RedownloadCount_AuC++;
                            }
                            else
                            {
                                IFile audioFile = await folder.GetFileAsync(_rootFilePath + auc.AudioFilePath);
                                int audioLength = await Task.Run(() => ReadMediaLength(audioFile));

                                if (audioLength == 0)
                                {
                                    //Redownload
                                    var audioUrl = auc.AudioUrl;

                                    if (audioUrl != "" && audioUrl.Contains("https://") && (Path.GetExtension(audioUrl) == ".mp3" || Path.GetExtension(audioUrl) == ".wav"))
                                    {
                                        var audioFileName = auc.GuidCRM.ToString() + Path.GetExtension(audioUrl);
                                        downloadAudio2.Add(DownloadAudioWithUrlTrackingTaskAsync(audioFileName, audioUrl, auc.GuidCRM, _ContactProgramDay.GuidCRM));
                                    }
                                    RedownloadCount_AuC++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var error = ex.ToString();
                            RedownloadErrorCount_AuC++;
                        }
                    }

                    if (RedownloadErrorCount_AuC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadErrorCount_AuC > 0" } });
                    }

                    if (RedownloadCount_AuC != 0)
                    {
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "RedownloadCount_AuC > 0" } });
                    }

                    Task t3 = Task.Run(() => ProcessAudioDownloads(downloadAudio2, incrementDownload_AC));
                    Task t4 = Task.Run(() => ProcessVideoDownloads(downloads2, incrementDownload_AuC));
                    int timeout_review = 20000; //miliseconds
                    await Task.WhenAny(Task.WhenAll(t3, t4), Task.Delay(timeout_review));

                    DownloadBtn.Text = "REVIEWING...";

                    bool DownloadResult = await Task.Run(() => CheckIfDownloadWasSuccessful(ContactProgramDay, folder));
                    if (!DownloadResult)
                    {
                        if (_numberOfDownloadAttempts[_ContactProgramDay.GuidCRM] <= 2)
                        {
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "ReDownload - Advised" } });
                            await DisplayAlert("Redownload", "We were only able to do a partial download. To download the full workout, make sure that you have a good internet connection and try again. Max 10 download attempts.", "OK");
                        }
                        else
                        {
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "ReDownload - Logout Suggested" } });
                            await DisplayAlert("Try to Log Out", "Your download has failed more than twice. Try to logout and back in.", "OK");
                        }
                        DownloadBtn.Text = "REDOWNLOAD";
                        ProgressBar.IsVisible = false;
                        DownloadBtn.IsEnabled = true;
                    }
                    else
                    {
                        var ContactProgramDay2 = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDay.GuidCRM).FirstOrDefaultAsync();
                        if (ContactProgramDay2 != null)
                        {
                            ContactProgramDay2.DownloadedOn = DateTime.UtcNow;
                            //ContactProgramDay2.ReceivedOn = DateTime.UtcNow;
                            ContactProgramDay2.IsDownloaded = true;
                            await _connection.UpdateAsync(ContactProgramDay2);

                            _ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == _ContactProgramDay.GuidCRM).FirstOrDefaultAsync();
                        }
                        var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.Id == _ContactProgramDay.ContactProgramId).FirstOrDefaultAsync();
                        if (ContactProgram != null)
                        {
                            if (ContactProgram.IsScheduleBuilt)
                            {
                                if (_ContactProgramDay.ScheduledStartDate != null && _ContactProgramDay.ScheduledStartDate?.ToLocalTime().Date == DateTime.UtcNow.ToLocalTime().Date)
                                {
                                    MessagingCenter.Send(this, "DownloadedCurrentDay");//is Today
                                }
                            }
                            else
                            {
                                if (_ContactProgramDay.SequenceNumber == 1)
                                {
                                    MessagingCenter.Send(this, "DownloadedCurrentDay");//First day is showing as Today
                                }
                            }
                        }

                        Branch branch = Branch.GetInstance();
                        branch.SendEvent(new BranchEvent("Downloaded_Workout"));
                        Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Completed Downloading Program" } });
                        DownloadBtn.Text = "WORKOUT";
                        DownloadBtn.IsEnabled = true;
                        await ProgressBar.ProgressTo(1D, 1, Easing.Linear);

                    }
                }
                else
                {
                    DownloadBtn.Text = "DOWNLOAD WORKOUT";
                    ProgressBar.IsVisible = false;
                    await DisplayAlert("Try Again", "There was an issue. Please try again later.", "OK");
                    DownloadBtn.IsEnabled = true;
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Downloading Error" } });
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                int i = 0;
                DownloadBtn.Text = "DOWNLOAD WORKOUT";
                ProgressBar.IsVisible = false;
                await DisplayAlert("Try Again", "There was a small issue. Please try again later.", "OK");
                DownloadBtn.IsEnabled = true;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "DownloadProgramDay()" } });
            }
            _DownloadingInProgress = false;

        }

        private async Task<bool> CheckIfDownloadWasSuccessful(LocalDBContactProgramDayV2 ContactProgramDay, IFolder folder)
        {
            bool isSuccessFul = true;

            var ActionContent_Review = await _connection.Table<LocalDBActionContentV2>().Where(x => x.LastDownloadAttempt != (DateTime?)null && x.ContactProgramDayId == ContactProgramDay.Id && (x.ContentTypeValue == 585860000 || x.ContentTypeValue == 585860001 || x.ContentTypeValue == 585860003)).ToListAsync();

            //ActionContent_Review = ActionContent_Review.Where(x => x.LastDownloadAttempt >= DateTime.UtcNow.AddMinutes(-5)).ToList();
            foreach (var ac in ActionContent_Review)
            {
                try
                {
                    if (ac.PhotoFilePath == "" || ac.VideoFilePath == "" || ac.PhotoFilePath == null || ac.VideoFilePath == null)
                    {
                        isSuccessFul = false;
                        break;
                    }
                    else
                    {
                        IFile photoFile = await folder.GetFileAsync(_rootFilePath + ac.PhotoFilePath);
                        IFile videoFile = await folder.GetFileAsync(_rootFilePath + ac.VideoFilePath);
                        int photoLength = await Task.Run(() => ReadMediaLength(photoFile));
                        int videoLength = await Task.Run(() => ReadMediaLength(videoFile));

                        if (photoLength == 0 || videoLength == 0)
                        {
                            isSuccessFul = false;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "CheckIfDownloadWasSuccessful_AC" } });
                    isSuccessFul = false;
                    break;
                }
            }

            if (isSuccessFul)
            {
                var AudioContent_Review = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.LastDownloadAttempt != (DateTime?)null && x.ContactProgramDayId == ContactProgramDay.Id).ToListAsync();

                //AudioContent_Review = AudioContent_Review.Where(x => x.LastDownloadAttempt >= DateTime.UtcNow.AddMinutes(-5)).ToList();
                foreach (var auc in AudioContent_Review)
                {
                    try
                    {
                        if (auc.AudioFilePath == "" || auc.AudioFilePath == null)
                        {
                            //Redownload
                            isSuccessFul = false;
                            break;
                        }
                        else
                        {
                            IFile audioFile = await folder.GetFileAsync(_rootFilePath + auc.AudioFilePath);
                            int audioLength = await Task.Run(() => ReadMediaLength(audioFile));

                            if (audioLength == 0)
                            {
                                //Redownload
                                isSuccessFul = false;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = ex.ToString();
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "CheckIfDownloadWasSuccessful_AuC" } });
                        isSuccessFul = false;
                        break;
                    }
                }
            }

            if (!isSuccessFul)
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Program", _ActiveProgram }, { "Action", "Download Failed" } });
            }

            return isSuccessFul;
        }

        private async Task<int> ReadMediaLength(IFile file)
        {
            long Length = 0;
            try
            {
                using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read))
                {
                    Length = stream.Length;
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "ReadMediaLength" } });
            }
            return unchecked((int)Length);
        }

        private async Task ProcessAudioDownloads(List<Task<DownloadResultAudio>> downloadAudio, double incrementDownload)
        {
            while (downloadAudio.Count > 0)
            {
                Task<DownloadResultAudio> finishedTask = await Task.WhenAny(downloadAudio.ToArray());
                downloadAudio.Remove(finishedTask);
                var AudioContent = await _connection.Table<LocalDBAudioContentV2>().Where(x => x.GuidCRM == finishedTask.Result.AudioContentGuid).FirstOrDefaultAsync();

                if (AudioContent != null)
                {
                    AudioContent.AudioFilePath = finishedTask.Result.audioFilePath;
                    AudioContent.LastDownloadedOn = DateTime.UtcNow;
                    await _connection.UpdateAsync(AudioContent);
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ProgressBar.ProgressTo(ProgressBar.Progress + incrementDownload, 1, Easing.Linear);
                });
            }
        }

        private async Task ProcessVideoDownloads(List<Task<DownloadResult>> downloads, double incrementDownload)
        {
            while (downloads.Count > 0)
            {
                Task<DownloadResult> finishedTask = await Task.WhenAny(downloads.ToArray());
                downloads.Remove(finishedTask);
                var ActionContent = await _connection.Table<LocalDBActionContentV2>().Where(x => x.GuidCRM == finishedTask.Result.ActionContentGuid).FirstOrDefaultAsync();

                if (ActionContent != null) //If this is null then the photo and video will be blank, unlikely because the action shows up on the workout (i think this means that the action content must be present as they are dependent)
                {
                    ActionContent.PhotoFilePath = finishedTask.Result.photoFilePath;
                    ActionContent.VideoFilePath = finishedTask.Result.videoFilePath;
                    ActionContent.LastDownloadedOn = DateTime.UtcNow;
                    await _connection.UpdateAsync(ActionContent);
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ProgressBar.ProgressTo(ProgressBar.Progress + incrementDownload, 1, Easing.Linear);
                });
            }
        }

        private async Task<DownloadResult> DownloadWithUrlTrackingTaskAsync(string photoFileName, string videoFileName, string photoUrl, string videoUrl, Guid actionContentGuid, Guid _ContactProgramDayGuid)
        {
            DownloadResult result = new DownloadResult();
            result.ActionContentGuid = actionContentGuid;
            Task<string> t1 = Task.Run(() => DownloadPhoto(photoFileName, photoUrl, _ContactProgramDayGuid));
            Task<string> t2 = Task.Run(() => DownloadVideo(videoFileName, videoUrl, _ContactProgramDayGuid));
            await Task.WhenAll(t1, t2);
            result.photoFilePath = t1.Result;
            result.videoFilePath = t2.Result;
            return result;
        }

        private async Task<DownloadResultAudio> DownloadAudioWithUrlTrackingTaskAsync(string audioFileName, string audioUrl, Guid audioContentGuid, Guid _ContactProgramDayGuid)
        {
            DownloadResultAudio result = new DownloadResultAudio();
            result.AudioContentGuid = audioContentGuid;
            result.audioFilePath = await DownloadAudio(audioFileName, audioUrl, _ContactProgramDayGuid);
            return result;
        }

        private static async Task<string> DownloadVideo(string videoFileName, string videoUrl, Guid _ContactProgramDayGuid)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
            IFolder subFolder = await folder.CreateFolderAsync(_ContactProgramDayGuid.ToString(), CreationCollisionOption.OpenIfExists);
            IFolder subSubFolder = await subFolder.CreateFolderAsync("Videos", CreationCollisionOption.OpenIfExists);
            IFile file = await subSubFolder.CreateFileAsync(videoFileName, CreationCollisionOption.ReplaceExisting);
            //_videoFilePath = file.Path.Replace(rootFolder.Path, "");
            var videoFilePath = file.Path.Replace(rootFolder.Path, "");

            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                var uri = new Uri(videoUrl);

                using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (Stream streamToWriteTo = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo); //If this doesn't work will it not show the video and download bytes are 0
                        }
                    }
                }
            }

            return videoFilePath;

        }

        private static async Task<string> DownloadPhoto(string photoFileName, string photoUrl, Guid _ContactProgramDayGuid)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
            IFolder subFolder = await folder.CreateFolderAsync(_ContactProgramDayGuid.ToString(), CreationCollisionOption.OpenIfExists);
            IFolder subSubFolder = await subFolder.CreateFolderAsync("Photos", CreationCollisionOption.OpenIfExists);
            IFile file = await subSubFolder.CreateFileAsync(photoFileName, CreationCollisionOption.ReplaceExisting);
            // _photoFilePath = file.Path.Replace(rootFolder.Path, "");
            var photoFilePath = file.Path.Replace(rootFolder.Path, "");

            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                var uri = new Uri(photoUrl);

                using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (Stream streamToWriteTo = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }
                }
            }

            return photoFilePath;
        }

        private static async Task<string> DownloadAudio(string audioFileName, string audioUrl, Guid _ContactProgramDayGuid)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("ContactProgramDays", CreationCollisionOption.OpenIfExists);
            IFolder subFolder = await folder.CreateFolderAsync(_ContactProgramDayGuid.ToString(), CreationCollisionOption.OpenIfExists);
            IFolder subSubFolder = await subFolder.CreateFolderAsync("Audio", CreationCollisionOption.OpenIfExists);
            IFile file = await subSubFolder.CreateFileAsync(audioFileName, CreationCollisionOption.ReplaceExisting);
            // _photoFilePath = file.Path.Replace(rootFolder.Path, "");
            var audioFilePath = file.Path.Replace(rootFolder.Path, "");

            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                var uri = new Uri(audioUrl);

                using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (Stream streamToWriteTo = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }
                }
            }

            return audioFilePath;
        }

        private async void GetActions()
        {
            NotLoadedYet.IsVisible = true;
            listOfActions.IsVisible = false;
            if (_ContactProgramDay != null)
            {
                if (_ContactProgramDay.IsDownloaded)
                {
                    DownloadBtn.Text = "WORKOUT";
                    await ProgressBar.ProgressTo(1D, 1, Easing.Linear);
                }
                else
                {
                    DownloadBtn.Text = "DOWNLOAD WORKOUT";
                }
            }

            var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == _ProgramDay.ProgramId).FirstOrDefaultAsync();
            if (Program != null)
            {
                _ActiveProgram = Program.Heading;
            }

            var Actions = await _connection.Table<LocalDBAction>().Where(x => x.ProgramDayId == _ProgramDay.Id).ToListAsync();
            if (Actions.Count() != 0)
            {
                DisplayActions();
                return;
            }

            if (IsInternetConnected())
            {
                try
                {
                    //await DisplayAlert("calling", "Calling API for Actions", "OK");

                    var response = await WebAPIService.GetProgramDayActions(_client, _ProgramDayGuid);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent content = response.Content;
                        var json = await content.ReadAsStringAsync();
                        var actions = JsonConvert.DeserializeObject<List<APIAction>>(json);
                        _actions = actions;
                        //await DisplayAlert("got", _actions.Count().ToString(), "OK");

                        foreach (var action in _actions)
                        {
                            var ActionContent = await _connection.Table<LocalDBActionContentV2>().Where(x => x.GuidCRM == action.ActionContent.GuidCRM).FirstOrDefaultAsync();
                            if (ActionContent == null)
                            {
                                var newActionContent = new LocalDBActionContentV2
                                {
                                    GuidCRM = action.ActionContent.GuidCRM,
                                    Heading = action.ActionContent.Heading,
                                    SubHeading = action.ActionContent.SubHeading,
                                    PhotoUrl = action.ActionContent.PhotoUrl,
                                    IsPreview = true,
                                };
                                await _connection.InsertAsync(newActionContent);
                                ActionContent = await _connection.Table<LocalDBActionContentV2>().Where(x => x.GuidCRM == action.ActionContent.GuidCRM).FirstOrDefaultAsync();
                            }
                            else
                            {
                                ActionContent.Heading = action.ActionContent.Heading;
                                ActionContent.SubHeading = action.ActionContent.SubHeading;
                                ActionContent.PhotoUrl = action.ActionContent.PhotoUrl;
                                await _connection.UpdateAsync(ActionContent);
                            }

                            var Action = await _connection.Table<LocalDBAction>().Where(x => x.GuidCRM == action.GuidCRM).FirstOrDefaultAsync();
                            if (Action == null)
                            {
                                var newAction = new LocalDBAction
                                {
                                    GuidCRM = action.GuidCRM,
                                    StateCodeValue = action.StateCodeValue,
                                    StatusCodeValue = action.StatusCodeValue,
                                    SequenceNumber = action.SequenceNumber,
                                    ContentTypeValue = action.ContentTypeValue,
                                    ContentType = action.ContentType,
                                    Heading = action.Heading,
                                    NumberOfReps = action.NumberOfReps,
                                    WeightLbs = action.WeightLbs,
                                    TimeSeconds = action.TimeSeconds,
                                    IntensityValue = action.IntensityValue,
                                    Intensity = action.Intensity,
                                    ProgramDayId = _ProgramDay.Id,
                                    ActionContentId = ActionContent.Id,
                                };
                                await _connection.InsertAsync(newAction);
                            }
                            else
                            {
                                Action.StateCodeValue = action.StateCodeValue;
                                Action.StatusCodeValue = action.StatusCodeValue;
                                Action.SequenceNumber = action.SequenceNumber;
                                Action.ContentTypeValue = action.ContentTypeValue;
                                Action.ContentType = action.ContentType;
                                Action.Heading = action.Heading;
                                Action.NumberOfReps = action.NumberOfReps;
                                Action.WeightLbs = action.WeightLbs;
                                Action.TimeSeconds = action.TimeSeconds;
                                Action.IntensityValue = action.IntensityValue;
                                Action.Intensity = action.Intensity;
                                Action.ProgramDayId = _ProgramDay.Id;
                                Action.ActionContentId = ActionContent.Id;
                                await _connection.UpdateAsync(Action);
                            }
                        }
                        DisplayActions();
                    }
                    else
                    {
                        await DisplayAlert("Error", "There was an error. Try again later.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "There was an error loading the workout.", "OK");
                    // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "BindActions()" } });
                }
            }
            else
            {
                await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
            }

        }

        private async void DisplayActions()
        {
            var Actions = await _connection.Table<LocalDBAction>().Where(x => x.ProgramDayId == _ProgramDay.Id).OrderBy(x => x.SequenceNumber).ToListAsync();

            //await DisplayAlert("Action Count", Actions.Count().ToString(), "OK");

            //int i = 1;
            //var ActionContents = await _connection.Table<LocalDBActionContent>().ToListAsync();
            //await DisplayAlert("Action Content Count", ActionContents.Count().ToString(), "OK");

            //if(ActionContents.Count() != 0)
            //{
            //    await DisplayAlert("Action Content Id", ActionContents.First().Id.ToString(), "OK");
            //}

            foreach (var Action in Actions)
            {
                var ActionContent = await _connection.Table<LocalDBActionContentV2>().Where(x => x.Id == Action.ActionContentId).FirstOrDefaultAsync();

                if (Action.ContentTypeValue == 585860000)
                {
                    //Exercise - Rep Based 585860000
                    // ConstructExerciseRepBasedCell(action);
                    ConstructExerciseBasedCell(Action, ActionContent);
                }
                else if (Action.ContentTypeValue == 585860001)
                {
                    //Exercise - Time Based 585860001
                    ConstructExerciseBasedCell(Action, ActionContent);
                }
                else if (Action.ContentTypeValue == 585860002)
                {
                    //Rest Time
                    ConstructRestTimeBasedCell(Action);
                }
                else if (Action.ContentTypeValue == 585860004)
                {
                    //Seperator
                    ConstructSeperatorBasedCell(Action);
                }
                else if (Action.ContentTypeValue == 585860003)
                {
                    //Stretch Time 585860003
                    ConstructExerciseBasedCell(Action, ActionContent);
                    //ConstructStretchTimeBasedCell(action);
                }
            }
            listOfActions.IsVisible = true;
            NotLoadedYet.IsVisible = false;

        }

        private void ConstructExerciseBasedCell(LocalDBAction action, LocalDBActionContentV2 actionContent)
        {
            var parentSL = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.FromHex("#f1f1f1"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

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
                //BackgroundColor = Color.Red,
            };
        
            var img = new CachedImage()
            {
                CacheDuration = TimeSpan.FromDays(30),
                RetryCount = 5,
                RetryDelay = 250,
                BitmapOptimizations = true,
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.FromHex("#c0c0c0"),
            };

            if (actionContent != null)
            {
                if (actionContent.PhotoUrl != "")
                {
                    try
                    {
                        img.Source = new UriImageSource() { Uri = new Uri(actionContent.PhotoUrl) };
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "PhotoUrl Error" } });
                    }
                }
            }

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

                repsNumText.Text = action.NumberOfReps.ToString();
                weightNumText.Text = action.WeightLbs.ToString();

                repsStackLayout.Children.Add(repsNumText);
                repsStackLayout.Children.Add(repsText);

                weightStackLayout.Children.Add(weightNumText);
                weightStackLayout.Children.Add(weightText);

                repsControlsStackLayout.Children.Add(repsStackLayout);

                weightControlsStackLayout.Children.Add(weightStackLayout);

                controlsSL.Children.Add(subControlSL);

                subControlSL.Children.Add(repsControlsStackLayout);
                subControlSL.Children.Add(weightControlsStackLayout);
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

                controlsSL.WidthRequest = 90;//120
                //controlsSL.BackgroundColor = Color.Green;
                timeSL.Children.Add(timeText);
                controlsSL.Children.Add(timeSL);
                controlsSL.Padding = new Thickness(0, 0, 30, 0);
            }

            if (Device.Idiom == TargetIdiom.Phone)
            {
                imgRL.HeightRequest = 100;
                imgRL.WidthRequest = 100;
                headingSL.WidthRequest = 160;
                //headingSL.BackgroundColor = Color.Yellow;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                imgRL.HeightRequest = 150;
                imgRL.WidthRequest = 150;
                headingText.FontSize = 20;
                headingSL.WidthRequest = 500;
                headingSL.Padding = new Thickness(20, 0, 0, 0);
            }
            else
            {
                imgRL.HeightRequest = 100;
                imgRL.WidthRequest = 100;
                headingSL.WidthRequest = 90; ;
            }    

            cellSL.Children.Add(imgRL);
            cellSL.Children.Add(headingSL);
            cellSL.Children.Add(controlsSL);

            parentSL.Children.Add(cellSL);

            listOfActions.Children.Add(parentSL);
        }

        private void ConstructRestTimeBasedCell(LocalDBAction action)
        {
            var seperatorSL = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.White,
                HeightRequest = 38,//28
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
            };

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

            TimeSpan time = TimeSpan.FromSeconds(action.TimeSeconds);
            string str = time.ToString(@"m\:ss");            

            var timeText = new Label
            {
                Text = str,
                TextColor = Color.FromHex("#17191A"),
                FontSize = 18,//14,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "HelveticalNeue-Bold",
                VerticalOptions = LayoutOptions.Center,
            };  

            seperatorSL.Children.Add(heading);
            seperatorSL.Children.Add(timeText);
            listOfActions.Children.Add(seperatorSL);

        }

        private void ConstructSeperatorBasedCell(LocalDBAction action)
        {
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

            seperatorSL.Children.Add(heading);
            listOfActions.Children.Add(seperatorSL);
        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                NoNetwork.IsVisible = false;
            }
            else if (!e.IsConnected)
            {
                NoNetwork.IsVisible = true;
            }
        }

    }
}
