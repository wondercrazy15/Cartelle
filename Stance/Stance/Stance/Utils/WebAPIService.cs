using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.ViewModels;
using StanceWeb.Models.App.API;
using StanceWeb.Models.App.Optimized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Stance.Pages.Sub.WorkoutSurvey;

namespace Stance.Utils
{
    public static class WebAPIService
    {
        private const string _WebApiName = "WebAPIService";

        public async static Task<HttpResponseMessage> InitialLoad(HttpClient _client, string Email, string Password)
        {
            try
            {
                string _auth = string.Format("{0}:{1}", Email, Password);
                string _enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(_auth));
                string _cred = string.Format("{0} {1}", "Basic", _enc);
                //Send Post request with the username and password to authenticate them
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._initialLoad);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", _cred);
                return await _client.SendAsync(request);
            } catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "InitialLoad" } });
                return mess;
            }
        }

        private async static Task SaveInitialLoadData(SignInResponseV2 signInResponse)
        {
            var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            if (signInResponse.Profile != null)
            {
                var profile = await _connection.Table<LocalDBContactV2>().Where(x => x.GuidCRM == signInResponse.Profile.GuidCRM).FirstOrDefaultAsync();
                if (profile != null)
                {
                    //profile.GuidCRM = signInResponse.Profile.GuidCRM;
                    profile.StateCodeValue = signInResponse.Profile.StateCodeValue;
                    profile.StatusCodeValue = signInResponse.Profile.StatusCodeValue;
                    profile.FirstName = signInResponse.Profile.FirstName;
                    profile.LastName = signInResponse.Profile.LastName;
                    profile.Email = signInResponse.Profile.Email;
                    profile.IsAdmin = signInResponse.Profile.IsAdmin;
                    profile.InstagramHandle = signInResponse.Profile.InstagramHandle;
                    profile.Birthday = (DateTime?)signInResponse.Profile.Birthday;
                    profile.GenderTypeCode = signInResponse.Profile.GenderTypeCode;
                    profile.Gender = signInResponse.Profile.Gender;
                    profile.HeightCm = signInResponse.Profile.HeightCm;
                    profile.WeightLbs = signInResponse.Profile.WeightLbs;
                    profile.TrainingGoalTypeCode = signInResponse.Profile.TrainingGoalTypeCode;
                    profile.TrainingGoal = signInResponse.Profile.TrainingGoal;
                    profile.RegionTypeCode = signInResponse.Profile.RegionTypeCode;
                    profile.Region = signInResponse.Profile.Region;
                    await _connection.UpdateAsync(profile);
                }
                else
                {
                    profile = new LocalDBContactV2();
                    profile.GuidCRM = signInResponse.Profile.GuidCRM;
                    profile.StateCodeValue = signInResponse.Profile.StateCodeValue;
                    profile.StatusCodeValue = signInResponse.Profile.StatusCodeValue;
                    profile.FirstName = signInResponse.Profile.FirstName;
                    profile.LastName = signInResponse.Profile.LastName;
                    profile.Email = signInResponse.Profile.Email;
                    profile.IsAdmin = signInResponse.Profile.IsAdmin;
                    profile.InstagramHandle = signInResponse.Profile.InstagramHandle;
                    profile.Birthday = (DateTime?)signInResponse.Profile.Birthday;
                    profile.GenderTypeCode = signInResponse.Profile.GenderTypeCode;
                    profile.Gender = signInResponse.Profile.Gender;
                    profile.HeightCm = signInResponse.Profile.HeightCm;
                    profile.WeightLbs = signInResponse.Profile.WeightLbs;
                    profile.TrainingGoalTypeCode = signInResponse.Profile.TrainingGoalTypeCode;
                    profile.TrainingGoal = signInResponse.Profile.TrainingGoal;
                    profile.RegionTypeCode = signInResponse.Profile.RegionTypeCode;
                    profile.Region = signInResponse.Profile.Region;
                    await _connection.InsertAsync(profile);
                }
            }

            if (signInResponse.ContactPrograms.Count() > 0)
            {
                foreach (var ContactProgram in signInResponse.ContactPrograms)
                {
                    var newProgram = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefaultAsync();
                    if (newProgram != null)
                    {
                        //newProgram.GuidCRM = ContactProgram.Program.GuidCRM;
                        newProgram.Heading = ContactProgram.Program.Heading;
                        newProgram.SubHeading = ContactProgram.Program.SubHeading;
                        newProgram.PhotoUrl = ContactProgram.Program.PhotoUrl;
                        newProgram.SecondaryPhotoUrl = ContactProgram.Program.SecondaryPhotoUrl;
                        newProgram.VideoUrl = ContactProgram.Program.VideoUrl;
                        newProgram.SequenceNumber = ContactProgram.Program.SequenceNumber;
                        newProgram.TotalWeeks = ContactProgram.Program.TotalWeeks;
                        newProgram.GoalValue = ContactProgram.Program.GoalValue;
                        newProgram.Goal = ContactProgram.Program.Goal;
                        newProgram.LevelValue = ContactProgram.Program.LevelValue;
                        newProgram.Level = ContactProgram.Program.Level;
                        await _connection.UpdateAsync(newProgram);
                    }
                    else
                    {
                        newProgram = new LocalDBProgram();
                        newProgram.GuidCRM = ContactProgram.Program.GuidCRM;
                        newProgram.Heading = ContactProgram.Program.Heading;
                        newProgram.SubHeading = ContactProgram.Program.SubHeading;
                        newProgram.PhotoUrl = ContactProgram.Program.PhotoUrl;
                        newProgram.SecondaryPhotoUrl = ContactProgram.Program.SecondaryPhotoUrl;
                        newProgram.VideoUrl = ContactProgram.Program.VideoUrl;
                        newProgram.SequenceNumber = ContactProgram.Program.SequenceNumber;
                        newProgram.TotalWeeks = ContactProgram.Program.TotalWeeks;
                        newProgram.GoalValue = ContactProgram.Program.GoalValue;
                        newProgram.Goal = ContactProgram.Program.Goal;
                        newProgram.LevelValue = ContactProgram.Program.LevelValue;
                        newProgram.Level = ContactProgram.Program.Level;
                        await _connection.InsertAsync(newProgram);
                    }

                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefaultAsync();

                    var newContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == ContactProgram.GuidCRM).FirstOrDefaultAsync();
                    if (newContactProgram != null)
                    {
                        //newContactProgram.GuidCRM = ContactProgram.GuidCRM;
                        newContactProgram.StateCodeValue = ContactProgram.StateCodeValue;
                        newContactProgram.StatusCodeValue = ContactProgram.StatusCodeValue;
                        newContactProgram.StartDate = (DateTime?)ContactProgram.StartDate;
                        newContactProgram.EndDate = (DateTime?)ContactProgram.EndDate;
                        newContactProgram.IsDaysCreated = ContactProgram.IsDaysCreated;
                        newContactProgram.IsScheduleBuilt = ContactProgram.IsScheduleBuilt;
                        newContactProgram.ProgramId = Program.Id;
                        await _connection.UpdateAsync(newContactProgram);
                    }
                    else
                    {
                        newContactProgram = new LocalDBContactProgram();
                        newContactProgram.GuidCRM = ContactProgram.GuidCRM;
                        newContactProgram.StateCodeValue = ContactProgram.StateCodeValue;
                        newContactProgram.StatusCodeValue = ContactProgram.StatusCodeValue;
                        newContactProgram.StartDate = (DateTime?)ContactProgram.StartDate;
                        newContactProgram.EndDate = (DateTime?)ContactProgram.EndDate;
                        newContactProgram.IsDaysCreated = ContactProgram.IsDaysCreated;
                        newContactProgram.IsScheduleBuilt = ContactProgram.IsScheduleBuilt;
                        newContactProgram.ProgramId = Program.Id;
                        await _connection.InsertAsync(newContactProgram);
                    }
                }
            }

            if (signInResponse.ContactProgramDays.ContactProgramDays.Count() > 0)
            {
                var Program1 = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == signInResponse.ContactProgramDays.ProgramGuid).FirstOrDefaultAsync();
                var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == signInResponse.ContactProgramDays.ContactProgramGuid).FirstOrDefaultAsync();

                foreach (var cpd in signInResponse.ContactProgramDays.ContactProgramDays)
                {
                    var newProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == cpd.ProgramDay.GuidCRM).FirstOrDefaultAsync();
                    if (newProgramDay != null)
                    {
                        //newProgramDay.GuidCRM = cpd.ProgramDay.GuidCRM;
                        newProgramDay.Heading = cpd.ProgramDay.Heading;
                        newProgramDay.SubHeading = cpd.ProgramDay.SubHeading;
                        newProgramDay.PhotoUrl = cpd.ProgramDay.PhotoUrl;
                        newProgramDay.SequenceNumber = cpd.ProgramDay.SequenceNumber;
                        newProgramDay.TotalExercises = cpd.ProgramDay.TotalExercises;
                        newProgramDay.TimeMinutes = cpd.ProgramDay.TimeMinutes;
                        newProgramDay.LevelValue = cpd.ProgramDay.LevelValue;
                        newProgramDay.Level = cpd.ProgramDay.Level;
                        newProgramDay.DayTypeValue = cpd.ProgramDay.DayTypeValue;
                        newProgramDay.DayType = cpd.ProgramDay.DayType;
                        newProgramDay.ProgramId = Program1.Id;
                        await _connection.UpdateAsync(newProgramDay);
                    }
                    else
                    {
                        newProgramDay = new LocalDBProgramDay();
                        newProgramDay.GuidCRM = cpd.ProgramDay.GuidCRM;
                        newProgramDay.Heading = cpd.ProgramDay.Heading;
                        newProgramDay.SubHeading = cpd.ProgramDay.SubHeading;
                        newProgramDay.PhotoUrl = cpd.ProgramDay.PhotoUrl;
                        newProgramDay.SequenceNumber = cpd.ProgramDay.SequenceNumber;
                        newProgramDay.TotalExercises = cpd.ProgramDay.TotalExercises;
                        newProgramDay.TimeMinutes = cpd.ProgramDay.TimeMinutes;
                        newProgramDay.LevelValue = cpd.ProgramDay.LevelValue;
                        newProgramDay.Level = cpd.ProgramDay.Level;
                        newProgramDay.DayTypeValue = cpd.ProgramDay.DayTypeValue;
                        newProgramDay.DayType = cpd.ProgramDay.DayType;
                        newProgramDay.ProgramId = Program1.Id;
                        await _connection.InsertAsync(newProgramDay);
                    }

                    var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == cpd.ProgramDay.GuidCRM).FirstOrDefaultAsync();

                    var newContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == cpd.GuidCRM).FirstOrDefaultAsync();
                    if (newContactProgramDay != null)
                    {
                        //newContactProgramDay.GuidCRM = cpd.GuidCRM;
                        newContactProgramDay.StateCodeValue = cpd.StateCodeValue;
                        newContactProgramDay.StatusCodeValue = cpd.StatusCodeValue;
                        newContactProgramDay.Synced = cpd.Synced;
                        newContactProgramDay.SequenceNumber = cpd.SequenceNumber;
                        newContactProgramDay.DayTypeValue = cpd.DayTypeValue;
                        newContactProgramDay.ScheduledStartDate = (DateTime?)cpd.ScheduledStartDate;
                        newContactProgramDay.ActualStartDate = (DateTime?)cpd.ActualStartDate;
                        newContactProgramDay.ContactProgramId = ContactProgram.Id;
                        newContactProgramDay.ProgramDayId = ProgramDay.Id;
                        newContactProgramDay.ReceivedOn = DateTime.UtcNow;
                        await _connection.UpdateAsync(newContactProgramDay);
                    }
                    else
                    {
                        newContactProgramDay = new LocalDBContactProgramDayV2();
                        newContactProgramDay.GuidCRM = cpd.GuidCRM;
                        newContactProgramDay.StateCodeValue = cpd.StateCodeValue;
                        newContactProgramDay.StatusCodeValue = cpd.StatusCodeValue;
                        newContactProgramDay.Synced = cpd.Synced;
                        newContactProgramDay.SequenceNumber = cpd.SequenceNumber;
                        newContactProgramDay.DayTypeValue = cpd.DayTypeValue;
                        newContactProgramDay.ScheduledStartDate = (DateTime?)cpd.ScheduledStartDate;
                        newContactProgramDay.ActualStartDate = (DateTime?)cpd.ActualStartDate;
                        newContactProgramDay.ContactProgramId = ContactProgram.Id;
                        newContactProgramDay.ProgramDayId = ProgramDay.Id;
                        newContactProgramDay.ReceivedOn = DateTime.UtcNow;
                        await _connection.InsertAsync(newContactProgramDay);
                    }
                }
            }

        }

        private async static Task SaveContactProgramData(ContactProgramResponse contactProgramResponse)
        {
            var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();            

            if (contactProgramResponse.ContactPrograms.Count() > 0)
            {
                foreach (var ContactProgram in contactProgramResponse.ContactPrograms)
                {
                    var newProgram = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefaultAsync();
                    if (newProgram != null)
                    {
                        //newProgram.GuidCRM = ContactProgram.Program.GuidCRM;
                        newProgram.Heading = ContactProgram.Program.Heading;
                        newProgram.SubHeading = ContactProgram.Program.SubHeading;
                        newProgram.PhotoUrl = ContactProgram.Program.PhotoUrl;
                        newProgram.SecondaryPhotoUrl = ContactProgram.Program.SecondaryPhotoUrl;
                        newProgram.VideoUrl = ContactProgram.Program.VideoUrl;
                        newProgram.SequenceNumber = ContactProgram.Program.SequenceNumber;
                        newProgram.TotalWeeks = ContactProgram.Program.TotalWeeks;
                        newProgram.GoalValue = ContactProgram.Program.GoalValue;
                        newProgram.Goal = ContactProgram.Program.Goal;
                        newProgram.LevelValue = ContactProgram.Program.LevelValue;
                        newProgram.Level = ContactProgram.Program.Level;
                        await _connection.UpdateAsync(newProgram);
                    }
                    else
                    {
                        newProgram = new LocalDBProgram();
                        newProgram.GuidCRM = ContactProgram.Program.GuidCRM;
                        newProgram.Heading = ContactProgram.Program.Heading;
                        newProgram.SubHeading = ContactProgram.Program.SubHeading;
                        newProgram.PhotoUrl = ContactProgram.Program.PhotoUrl;
                        newProgram.SecondaryPhotoUrl = ContactProgram.Program.SecondaryPhotoUrl;
                        newProgram.VideoUrl = ContactProgram.Program.VideoUrl;
                        newProgram.SequenceNumber = ContactProgram.Program.SequenceNumber;
                        newProgram.TotalWeeks = ContactProgram.Program.TotalWeeks;
                        newProgram.GoalValue = ContactProgram.Program.GoalValue;
                        newProgram.Goal = ContactProgram.Program.Goal;
                        newProgram.LevelValue = ContactProgram.Program.LevelValue;
                        newProgram.Level = ContactProgram.Program.Level;
                        await _connection.InsertAsync(newProgram);
                    }

                    var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefaultAsync();

                    var newContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == ContactProgram.GuidCRM).FirstOrDefaultAsync();
                    if (newContactProgram != null)
                    {
                        //newContactProgram.GuidCRM = ContactProgram.GuidCRM;
                        newContactProgram.StateCodeValue = ContactProgram.StateCodeValue;
                        newContactProgram.StatusCodeValue = ContactProgram.StatusCodeValue;
                        newContactProgram.StartDate = (DateTime?)ContactProgram.StartDate;
                        newContactProgram.EndDate = (DateTime?)ContactProgram.EndDate;
                        newContactProgram.IsDaysCreated = ContactProgram.IsDaysCreated;
                        newContactProgram.IsScheduleBuilt = ContactProgram.IsScheduleBuilt;
                        newContactProgram.ProgramId = Program.Id;
                        await _connection.UpdateAsync(newContactProgram);
                    }
                    else
                    {
                        newContactProgram = new LocalDBContactProgram();
                        newContactProgram.GuidCRM = ContactProgram.GuidCRM;
                        newContactProgram.StateCodeValue = ContactProgram.StateCodeValue;
                        newContactProgram.StatusCodeValue = ContactProgram.StatusCodeValue;
                        newContactProgram.StartDate = (DateTime?)ContactProgram.StartDate;
                        newContactProgram.EndDate = (DateTime?)ContactProgram.EndDate;
                        newContactProgram.IsDaysCreated = ContactProgram.IsDaysCreated;
                        newContactProgram.IsScheduleBuilt = ContactProgram.IsScheduleBuilt;
                        newContactProgram.ProgramId = Program.Id;
                        await _connection.InsertAsync(newContactProgram);
                    }
                }
            }

            if (contactProgramResponse.ContactProgramDays.ContactProgramDays.Count() > 0)
            {
                var Program1 = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == contactProgramResponse.ContactProgramDays.ProgramGuid).FirstOrDefaultAsync();
                var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == contactProgramResponse.ContactProgramDays.ContactProgramGuid).FirstOrDefaultAsync();

                foreach (var cpd in contactProgramResponse.ContactProgramDays.ContactProgramDays)
                {
                    var newProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == cpd.ProgramDay.GuidCRM).FirstOrDefaultAsync();
                    if (newProgramDay != null)
                    {
                        //newProgramDay.GuidCRM = cpd.ProgramDay.GuidCRM;
                        newProgramDay.Heading = cpd.ProgramDay.Heading;
                        newProgramDay.SubHeading = cpd.ProgramDay.SubHeading;
                        newProgramDay.PhotoUrl = cpd.ProgramDay.PhotoUrl;
                        newProgramDay.SequenceNumber = cpd.ProgramDay.SequenceNumber;
                        newProgramDay.TotalExercises = cpd.ProgramDay.TotalExercises;
                        newProgramDay.TimeMinutes = cpd.ProgramDay.TimeMinutes;
                        newProgramDay.LevelValue = cpd.ProgramDay.LevelValue;
                        newProgramDay.Level = cpd.ProgramDay.Level;
                        newProgramDay.DayTypeValue = cpd.ProgramDay.DayTypeValue;
                        newProgramDay.DayType = cpd.ProgramDay.DayType;
                        newProgramDay.ProgramId = Program1.Id;
                        await _connection.UpdateAsync(newProgramDay);
                    }
                    else
                    {
                        newProgramDay = new LocalDBProgramDay();
                        newProgramDay.GuidCRM = cpd.ProgramDay.GuidCRM;
                        newProgramDay.Heading = cpd.ProgramDay.Heading;
                        newProgramDay.SubHeading = cpd.ProgramDay.SubHeading;
                        newProgramDay.PhotoUrl = cpd.ProgramDay.PhotoUrl;
                        newProgramDay.SequenceNumber = cpd.ProgramDay.SequenceNumber;
                        newProgramDay.TotalExercises = cpd.ProgramDay.TotalExercises;
                        newProgramDay.TimeMinutes = cpd.ProgramDay.TimeMinutes;
                        newProgramDay.LevelValue = cpd.ProgramDay.LevelValue;
                        newProgramDay.Level = cpd.ProgramDay.Level;
                        newProgramDay.DayTypeValue = cpd.ProgramDay.DayTypeValue;
                        newProgramDay.DayType = cpd.ProgramDay.DayType;
                        newProgramDay.ProgramId = Program1.Id;
                        await _connection.InsertAsync(newProgramDay);
                    }

                    var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == cpd.ProgramDay.GuidCRM).FirstOrDefaultAsync();

                    var newContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == cpd.GuidCRM).FirstOrDefaultAsync();
                    if (newContactProgramDay != null)
                    {
                        //newContactProgramDay.GuidCRM = cpd.GuidCRM;
                        newContactProgramDay.StateCodeValue = cpd.StateCodeValue;
                        newContactProgramDay.StatusCodeValue = cpd.StatusCodeValue;
                        newContactProgramDay.Synced = cpd.Synced;
                        newContactProgramDay.SequenceNumber = cpd.SequenceNumber;
                        newContactProgramDay.DayTypeValue = cpd.DayTypeValue;
                        newContactProgramDay.ScheduledStartDate = (DateTime?)cpd.ScheduledStartDate;
                        newContactProgramDay.ActualStartDate = (DateTime?)cpd.ActualStartDate;
                        newContactProgramDay.ContactProgramId = ContactProgram.Id;
                        newContactProgramDay.ProgramDayId = ProgramDay.Id;
                        newContactProgramDay.ReceivedOn = DateTime.UtcNow;
                        await _connection.UpdateAsync(newContactProgramDay);
                    }
                    else
                    {
                        newContactProgramDay = new LocalDBContactProgramDayV2();
                        newContactProgramDay.GuidCRM = cpd.GuidCRM;
                        newContactProgramDay.StateCodeValue = cpd.StateCodeValue;
                        newContactProgramDay.StatusCodeValue = cpd.StatusCodeValue;
                        newContactProgramDay.Synced = cpd.Synced;
                        newContactProgramDay.SequenceNumber = cpd.SequenceNumber;
                        newContactProgramDay.DayTypeValue = cpd.DayTypeValue;
                        newContactProgramDay.ScheduledStartDate = (DateTime?)cpd.ScheduledStartDate;
                        newContactProgramDay.ActualStartDate = (DateTime?)cpd.ActualStartDate;
                        newContactProgramDay.ContactProgramId = ContactProgram.Id;
                        newContactProgramDay.ProgramDayId = ProgramDay.Id;
                        newContactProgramDay.ReceivedOn = DateTime.UtcNow;
                        await _connection.InsertAsync(newContactProgramDay);
                    }
                }
            }

        }

        public async static Task<HttpResponseMessage> RefreshApp(HttpClient _client)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._initialLoad);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                var result =  await _client.SendAsync(request);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = result.Content;
                    var json = await content.ReadAsStringAsync();
                    var signInResponse = JsonConvert.DeserializeObject<SignInResponseV2>(json);

                    if (signInResponse.ContactPrograms.Count() == 0)
                    {
                        var new_result = new HttpResponseMessage();
                        new_result.StatusCode = HttpStatusCode.Gone;
                        return new_result;                            
                    }
                    else
                    {
                        await Task.Run(() => SaveInitialLoadData(signInResponse));                        
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "RefreshApp" } });
                return mess;
            }
        }

        public async static void GetContactPrograms(HttpClient _client)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._programsUri);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Stance.Utils.Auth.Auth.Token);
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var contactProgs = JsonConvert.DeserializeObject<List<APIContactProgram>>(json);

                    if (contactProgs.Count() > 0)
                    {
                        foreach (var ContactProgram in contactProgs)
                        {
                            //Task t = Task.Factory.StartNew(() => Database.SaveContactProgram(ContactProgram));
                            //t.Wait();
                            var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                            var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefaultAsync();
                            if (Program == null)
                            {
                                var newProgram = new LocalDBProgram
                                {
                                    GuidCRM = ContactProgram.Program.GuidCRM,
                                    //Cost = ContactProgram.Program.Cost,
                                    Heading = ContactProgram.Program.Heading,
                                    SubHeading = ContactProgram.Program.SubHeading,
                                    PhotoUrl = ContactProgram.Program.PhotoUrl,
                                    SecondaryPhotoUrl = ContactProgram.Program.SecondaryPhotoUrl,
                                    VideoUrl = ContactProgram.Program.VideoUrl,
                                    SequenceNumber = ContactProgram.Program.SequenceNumber,
                                    TotalWeeks = ContactProgram.Program.TotalWeeks,
                                    GoalValue = ContactProgram.Program.GoalValue,
                                    Goal = ContactProgram.Program.Goal,
                                    LevelValue = ContactProgram.Program.LevelValue,
                                    Level = ContactProgram.Program.Level,
                                };
                                await _connection.InsertAsync(newProgram);
                            }
                            else
                            {
                                Program.GuidCRM = ContactProgram.Program.GuidCRM;
                                //Program.Cost = ContactProgram.Program.Cost;
                                Program.Heading = ContactProgram.Program.Heading;
                                Program.SubHeading = ContactProgram.Program.SubHeading;
                                Program.PhotoUrl = ContactProgram.Program.PhotoUrl;
                                Program.SecondaryPhotoUrl = ContactProgram.Program.SecondaryPhotoUrl;
                                Program.VideoUrl = ContactProgram.Program.VideoUrl;
                                Program.SequenceNumber = ContactProgram.Program.SequenceNumber;
                                Program.TotalWeeks = ContactProgram.Program.TotalWeeks;
                                Program.GoalValue = ContactProgram.Program.GoalValue;
                                Program.Goal = ContactProgram.Program.Goal;
                                Program.LevelValue = ContactProgram.Program.LevelValue;
                                Program.Level = ContactProgram.Program.Level;
                                await _connection.UpdateAsync(Program);
                            }
                            Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefaultAsync();
                            var ContactProgram0 = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == ContactProgram.GuidCRM).FirstOrDefaultAsync();
                            if (ContactProgram0 == null)
                            {
                                var newContactProgram = new LocalDBContactProgram
                                {
                                    GuidCRM = ContactProgram.GuidCRM,
                                    StateCodeValue = ContactProgram.StateCodeValue,
                                    StatusCodeValue = ContactProgram.StatusCodeValue,
                                    StartDate = ContactProgram.StartDate,
                                    EndDate = ContactProgram.EndDate,
                                    TotalProgramDays = ContactProgram.TotalProgramDays,
                                    TotalProgramDaysComplete = ContactProgram.TotalProgramDaysComplete,
                                    PercentComplete = ContactProgram.PercentComplete,
                                    IsDaysCreated = ContactProgram.IsDaysCreated,
                                    ProgramId = Program.Id,
                                };
                                await _connection.InsertAsync(newContactProgram);
                            }
                            else
                            {
                                ContactProgram0.GuidCRM = ContactProgram.GuidCRM;
                                ContactProgram0.StateCodeValue = ContactProgram.StateCodeValue;
                                ContactProgram0.StatusCodeValue = ContactProgram.StatusCodeValue;
                                ContactProgram0.StartDate = ContactProgram.StartDate;
                                ContactProgram0.EndDate = ContactProgram.EndDate;
                                ContactProgram0.TotalProgramDays = ContactProgram.TotalProgramDays;
                                ContactProgram0.TotalProgramDaysComplete = ContactProgram.TotalProgramDaysComplete;
                                ContactProgram0.PercentComplete = ContactProgram.PercentComplete;
                                ContactProgram0.ProgramId = Program.Id;
                                await _connection.UpdateAsync(ContactProgram0);
                            }
                        }
                    }

                }
                else
                {
                    throw new Exception("Error - GetContactPrograms: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetContactPrograms" } });
            }
        }

        public async static void GetContactProgramSchedule(HttpClient _client, Guid ContactProgramGuid)
        {
            try
            {
                //Send Post request with the username and password to authenticate them
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._contactProgramDaysScheduleUri + ContactProgramGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Stance.Utils.Auth.Auth.Token);
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var contactProgramDays = JsonConvert.DeserializeObject<List<APIContactProgramDay>>(json);

                    if (contactProgramDays.Count() > 0)
                    {
                        var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

                        var ContactProgramDayDefault = contactProgramDays.First();

                        var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgramDayDefault.ProgramDay.Program.GuidCRM).FirstOrDefaultAsync();

                        if (Program == null)
                        {
                            var newProgram = new LocalDBProgram
                            {
                                GuidCRM = ContactProgramDayDefault.ProgramDay.Program.GuidCRM,
                                Heading = ContactProgramDayDefault.ProgramDay.Program.Heading,
                                SubHeading = ContactProgramDayDefault.ProgramDay.Program.SubHeading,
                                PhotoUrl = ContactProgramDayDefault.ProgramDay.Program.PhotoUrl,
                                SecondaryPhotoUrl = ContactProgramDayDefault.ProgramDay.Program.SecondaryPhotoUrl,
                                VideoUrl = ContactProgramDayDefault.ProgramDay.Program.VideoUrl,
                                SequenceNumber = ContactProgramDayDefault.ProgramDay.Program.SequenceNumber,
                                StateCodeValue = ContactProgramDayDefault.ProgramDay.Program.StateCodeValue,
                                StatusCodeValue = ContactProgramDayDefault.ProgramDay.Program.StatusCodeValue,
                                TotalWeeks = ContactProgramDayDefault.ProgramDay.Program.TotalWeeks,
                                GoalValue = ContactProgramDayDefault.ProgramDay.Program.GoalValue,
                                LevelValue = ContactProgramDayDefault.ProgramDay.Program.LevelValue,
                                //Cost = ContactProgramDayDefault.ProgramDay.Program.Cost,
                            };
                            await _connection.InsertAsync(newProgram);
                        }
                        else
                        {
                            Program.GuidCRM = ContactProgramDayDefault.ProgramDay.Program.GuidCRM;
                            Program.Heading = ContactProgramDayDefault.ProgramDay.Program.Heading;
                            Program.SubHeading = ContactProgramDayDefault.ProgramDay.Program.SubHeading;
                            Program.PhotoUrl = ContactProgramDayDefault.ProgramDay.Program.PhotoUrl;
                            Program.SecondaryPhotoUrl = ContactProgramDayDefault.ProgramDay.Program.SecondaryPhotoUrl;
                            Program.VideoUrl = ContactProgramDayDefault.ProgramDay.Program.VideoUrl;
                            Program.SequenceNumber = ContactProgramDayDefault.ProgramDay.Program.SequenceNumber;
                            Program.StateCodeValue = ContactProgramDayDefault.ProgramDay.Program.StateCodeValue;
                            Program.StatusCodeValue = ContactProgramDayDefault.ProgramDay.Program.StatusCodeValue;
                            Program.TotalWeeks = ContactProgramDayDefault.ProgramDay.Program.TotalWeeks;
                            Program.GoalValue = ContactProgramDayDefault.ProgramDay.Program.GoalValue;
                            Program.LevelValue = ContactProgramDayDefault.ProgramDay.Program.LevelValue;
                            //Program.Cost = ContactProgramDayDefault.ProgramDay.Program.Cost;
                            await _connection.UpdateAsync(Program);
                        }
                        Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == ContactProgramDayDefault.ProgramDay.Program.GuidCRM).FirstOrDefaultAsync();

                        var ContactProgram0 = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == ContactProgramDayDefault.ContactProgram.GuidCRM).FirstOrDefaultAsync();

                        if (ContactProgram0 == null)
                        {
                            var newContactProgram = new LocalDBContactProgram
                            {
                                GuidCRM = ContactProgramDayDefault.ContactProgram.GuidCRM,
                                ProgramId = Program.Id,
                            };
                            await _connection.InsertAsync(newContactProgram);
                        }
                        else
                        {
                            //ContactProgram0.GuidCRM = ContactProgramDayDefault.ContactProgram.GuidCRM;
                            ContactProgram0.ProgramId = Program.Id;
                            await _connection.UpdateAsync(ContactProgram0);
                        }

                        ContactProgram0 = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == ContactProgramDayDefault.ContactProgram.GuidCRM).FirstOrDefaultAsync();


                        foreach (var ContactProgramDay in contactProgramDays)
                        {
                            var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == ContactProgramDay.ProgramDay.GuidCRM).FirstOrDefaultAsync();

                            if (ProgramDay == null)
                            {
                                var newProgramDay = new LocalDBProgramDay
                                {
                                    GuidCRM = ContactProgramDay.ProgramDay.GuidCRM,
                                    Heading = ContactProgramDay.ProgramDay.Heading,
                                    SubHeading = ContactProgramDay.ProgramDay.SubHeading,
                                    PhotoUrl = ContactProgramDay.ProgramDay.PhotoUrl,
                                    SequenceNumber = ContactProgramDay.ProgramDay.SequenceNumber,
                                    TotalExercises = ContactProgramDay.ProgramDay.TotalExercises,
                                    TimeMinutes = ContactProgramDay.ProgramDay.TimeMinutes,
                                    LevelValue = ContactProgramDay.ProgramDay.LevelValue,
                                    Level = ContactProgramDay.ProgramDay.Level,
                                    DayTypeValue = ContactProgramDay.ProgramDay.DayTypeValue,
                                    DayType = ContactProgramDay.ProgramDay.DayType,
                                    ProgramId = Program.Id,
                                };
                                await _connection.InsertAsync(newProgramDay);
                            }
                            else
                            {
                                ProgramDay.Heading = ContactProgramDay.ProgramDay.Heading;
                                ProgramDay.SubHeading = ContactProgramDay.ProgramDay.SubHeading;
                                ProgramDay.PhotoUrl = ContactProgramDay.ProgramDay.PhotoUrl;
                                ProgramDay.SequenceNumber = ContactProgramDay.ProgramDay.SequenceNumber;
                                ProgramDay.TotalExercises = ContactProgramDay.ProgramDay.TotalExercises;
                                ProgramDay.TimeMinutes = ContactProgramDay.ProgramDay.TimeMinutes;
                                ProgramDay.LevelValue = ContactProgramDay.ProgramDay.LevelValue;
                                ProgramDay.Level = ContactProgramDay.ProgramDay.Level;
                                ProgramDay.DayTypeValue = ContactProgramDay.ProgramDay.DayTypeValue;
                                ProgramDay.DayType = ContactProgramDay.ProgramDay.DayType;
                                ProgramDay.ProgramId = Program.Id;
                                await _connection.UpdateAsync(ProgramDay);
                            }

                            ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == ContactProgramDay.ProgramDay.GuidCRM).FirstOrDefaultAsync();

                            var contactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == ContactProgramDay.GuidCRM).FirstOrDefaultAsync();

                            if (contactProgramDay == null)
                            {
                                var newContactProgramDay = new LocalDBContactProgramDayV2
                                {
                                    GuidCRM = ContactProgramDay.GuidCRM,
                                    StateCodeValue = ContactProgramDay.StateCodeValue,
                                    StatusCodeValue = ContactProgramDay.StatusCodeValue,
                                    Synced = ContactProgramDay.Synced,
                                    ScheduledStartDate = ContactProgramDay.ScheduledStartDate,
                                    ContactProgramId = ContactProgram0.Id,
                                    ProgramDayId = ProgramDay.Id,
                                    ReceivedOn = DateTime.UtcNow,
                                };
                                await _connection.InsertAsync(newContactProgramDay);
                            }
                            else
                            {
                                contactProgramDay.StateCodeValue = ContactProgramDay.StateCodeValue;
                                contactProgramDay.StatusCodeValue = ContactProgramDay.StatusCodeValue;
                                contactProgramDay.Synced = ContactProgramDay.Synced;
                                contactProgramDay.ScheduledStartDate = ContactProgramDay.ScheduledStartDate;
                                contactProgramDay.ContactProgramId = ContactProgram0.Id;
                                contactProgramDay.ProgramDayId = ProgramDay.Id;

                                if (!contactProgramDay.ReceivedOn.HasValue)
                                {
                                    contactProgramDay.ReceivedOn = DateTime.UtcNow;
                                }
                                await _connection.UpdateAsync(contactProgramDay);
                            }
                        }


                    }

                }
                else
                {
                    throw new Exception("Error - GetContactProgramSchedule: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetContactProgramSchedule" } });
            }

        }

        public async static Task GetAthletes(HttpClient _client)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._athletesUri);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var athletes = JsonConvert.DeserializeObject<List<APIAccount>>(json);

                    if (athletes.Count() > 0)
                    {
                        var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

                        foreach (var athlete in athletes)
                        {
                            var Account = await _connection.Table<LocalDBAccount>().Where(x => x.GuidCRM == athlete.GuidCRM).FirstOrDefaultAsync();
                            if (Account == null)
                            {
                                var newAccount = new LocalDBAccount
                                {
                                    GuidCRM = athlete.GuidCRM,
                                    Heading = athlete.Heading,
                                    SubHeading = athlete.SubHeading,
                                    PhotoUrl = athlete.PhotoUrl,
                                    SecondaryPhotoUrl = athlete.SecondaryPhotoUrl,
                                    VideoUrl = athlete.VideoUrl,
                                    SequenceNumber = athlete.SequenceNumber,
                                    StateCodeValue = athlete.StateCodeValue,
                                    StatusCodeValue = athlete.StatusCodeValue,
                                };
                                await _connection.InsertAsync(newAccount);
                            }
                            else
                            {
                                Account.Heading = athlete.Heading;
                                Account.SubHeading = athlete.SubHeading;
                                Account.PhotoUrl = athlete.PhotoUrl;
                                Account.SecondaryPhotoUrl = athlete.SecondaryPhotoUrl;
                                Account.VideoUrl = athlete.VideoUrl;
                                Account.SequenceNumber = athlete.SequenceNumber;
                                Account.StatusCodeValue = athlete.StatusCodeValue;
                                Account.StateCodeValue = athlete.StateCodeValue;
                                await _connection.UpdateAsync(Account);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Error - GetAthletes: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetAthletes" } });
            }
        }

        public async static Task<HttpStatusCode> UpdateProfile(HttpClient _client, APIContactV2 editedContact)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                string json = JsonConvert.SerializeObject(editedContact);
                var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                var newUri = new Uri(App._absoluteUri, App._profileUri);
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                HttpResponseMessage response = await _client.PostAsync(newUri, contentString);
                return response.StatusCode;
            } catch(Exception ex)
            {
                var error = ex.ToString();
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "UpdateProfile" } });
                return HttpStatusCode.Gone;
            }

        }

        public async static Task<HttpStatusCode> ResetSchedule(HttpClient _client, Guid ContactProgramGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._resetSchedule + ContactProgramGuid.ToString());
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);

                HttpResponseMessage response2 = await _client.SendAsync(request);
                return response2.StatusCode;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "ResetSchedule" } });
                return HttpStatusCode.Gone;
            }

        }

        public async static Task<HttpStatusCode> GetContactProgramDays(HttpClient _client, Guid ContactProgramGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._programDaysGetActiveProgramDaysUri + ContactProgramGuid.ToString());
                request.Method = HttpMethod.Get;
                //request.Headers.Add("Content-Type", "application/json"); Throws Error
                request.Headers.Add("Accept", "application/json");
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                //_client.DefaultRequestHeaders.Add("Content-Type", "application/json; charset=utf-8"); Throws ERROR

                HttpResponseMessage response2 = await _client.SendAsync(request);

                if (response2.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content2 = response2.Content;
                    var json2 = await content2.ReadAsStringAsync();
                    var contactProgramDay = JsonConvert.DeserializeObject<APIContactProgramDay>(json2);

                    if (contactProgramDay != null)
                    {
                        var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                        var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == contactProgramDay.ProgramDay.Program.GuidCRM).FirstOrDefaultAsync();
                        if (Program == null)
                        {
                            var newProgram = new LocalDBProgram
                            {
                                GuidCRM = contactProgramDay.ProgramDay.Program.GuidCRM,
                            };
                            await _connection.InsertAsync(newProgram);
                            Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == contactProgramDay.ProgramDay.Program.GuidCRM).FirstOrDefaultAsync();
                        }

                        var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == contactProgramDay.ProgramDay.GuidCRM).FirstOrDefaultAsync();
                        if (ProgramDay == null)
                        {
                            var newProgramDay = new LocalDBProgramDay
                            {
                                GuidCRM = contactProgramDay.ProgramDay.GuidCRM,
                                DayTypeValue = contactProgramDay.ProgramDay.DayTypeValue,
                                DayType = contactProgramDay.ProgramDay.DayType,
                                Heading = contactProgramDay.ProgramDay.Heading,
                                SubHeading = contactProgramDay.ProgramDay.SubHeading,
                                PhotoUrl = contactProgramDay.ProgramDay.PhotoUrl,
                                SequenceNumber = contactProgramDay.ProgramDay.SequenceNumber,
                                TotalExercises = contactProgramDay.ProgramDay.TotalExercises,
                                TimeMinutes = contactProgramDay.ProgramDay.TimeMinutes,
                                LevelValue = contactProgramDay.ProgramDay.LevelValue,
                                Level = contactProgramDay.ProgramDay.Level,
                                ProgramId = Program.Id,
                            };
                            await _connection.InsertAsync(newProgramDay);
                            ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == contactProgramDay.ProgramDay.GuidCRM).FirstOrDefaultAsync();
                        }

                        var ContactProgramDay = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.GuidCRM == contactProgramDay.GuidCRM).FirstOrDefaultAsync();
                        if (ContactProgramDay == null)
                        {
                            var newContactProgramDay = new LocalDBContactProgramDayV2
                            {
                                GuidCRM = contactProgramDay.GuidCRM,
                                StateCodeValue = contactProgramDay.StateCodeValue,
                                StatusCodeValue = contactProgramDay.StatusCodeValue,
                                ScheduledStartDate = contactProgramDay.ScheduledStartDate,
                                ActualStartDate = contactProgramDay.ActualStartDate,
                                ReceivedOn = DateTime.UtcNow,
                                ProgramDayId = ProgramDay.Id,
                            };
                            await _connection.InsertAsync(newContactProgramDay);
                        }
                    }
                }
                return response2.StatusCode;

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetContactProgramDays" } });
                return HttpStatusCode.Gone;
            }         

        }

        public async static Task SyncContactActionsToCRM(HttpClient _client, List<SyncContactAction> syncCA)
        {
            var dic = new Dictionary<string, string>();
            try
            {
                var ca_sync = new SyncCA();
                ca_sync.ca = syncCA;

                dic.Add("Action", "CA Sync");
                //dic.Add("Function", "SyncContactActionsToCRM");
                dic.Add("Status", "Sending");

                string json2 = JsonConvert.SerializeObject(ca_sync);
                var contentString2 = new StringContent(json2, Encoding.UTF8, "application/json");
                var newUri2 = new Uri(App._absoluteUri, App._syncCA);

                Analytics.TrackEvent(_WebApiName, dic);
                HttpResponseMessage response2 = await _client.PostAsync(newUri2, contentString2);

                dic["Status"] = "Sent";
                Analytics.TrackEvent(_WebApiName, dic);

                if (response2.StatusCode == HttpStatusCode.OK)
                {
                    dic["Status"] = "Success";
                    Analytics.TrackEvent(_WebApiName, dic);

                    var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                    var ContactActionsToSync = await _connection.Table<LocalDBContactAction>().Where(x => x.IsComplete == true && x.Synced == false).ToListAsync();

                    foreach (var item in ContactActionsToSync)
                    {
                        item.Synced = true;
                        await _connection.UpdateAsync(item);
                    }
                } else
                {
                    dic["Status"] = "Failure";
                    Analytics.TrackEvent(_WebApiName, dic);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, dic);
                //await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
            }
        }

        public static async Task SyncContactProgramDaysToCRM(HttpClient _client, List<SyncContactProgramDay> syncCPD)
        {
            var dic = new Dictionary<string, string>();
            try
            {
                var cpd = new SyncCPD();
                cpd.cpd = syncCPD;

                dic.Add("Action", "CPD Sync");
                //dic.Add("Function", "SyncContactProgramDaysToCRM");
                dic.Add("Status", "Sending");

                foreach (var item in syncCPD)
                {
                    //dic.Add("GuidCRM", item.GuidCRM.ToString());
                   // dic.Add("Sycned", item.Synced.ToString());
                    //dic.Add("IsComplete", item.IsComplete.ToString());
                    dic.Add("Rating", item.Rating.ToString());
                    //dic.Add("ActualStartDate", item.ActualStartDate.ToString("dd-MM-yyyy"));
                    break;
                }

                string json = JsonConvert.SerializeObject(cpd);
                var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                var newUri = new Uri(App._absoluteUri, App._syncCPD);

                Analytics.TrackEvent(_WebApiName, dic);
                HttpResponseMessage response = await _client.PostAsync(newUri, contentString);

                dic["Status"] = "Sent";
                Analytics.TrackEvent(_WebApiName, dic );

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dic["Status"] = "Success";
                    Analytics.TrackEvent(_WebApiName, dic);

                    var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                    var ContactProgramDaysToSync1 = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.IsComplete == true && x.Synced == false).ToListAsync();

                    foreach (var item in ContactProgramDaysToSync1)
                    {
                        item.StateCodeValue = 1;//Inactive
                        item.StatusCodeValue = 585860004; //Complete
                        item.Synced = true;
                        item.IsComplete = true;
                        await _connection.UpdateAsync(item);
                    }
                } else
                {
                    dic["Status"] = "Failure";
                    Analytics.TrackEvent(_WebApiName, dic);
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Crashes.TrackError(ex, dic);
                //await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
            }
        }

        public static async Task<HttpResponseMessage> ActivateProgram(HttpClient _client, Guid _ContactProgramGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._contactProgramsActivateUri + _ContactProgramGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                var result = await _client.SendAsync(request);

                //if (response.StatusCode == HttpStatusCode.OK)
                //{
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent content = result.Content;
                        var json = await content.ReadAsStringAsync();
                        var contactProgramResponse = JsonConvert.DeserializeObject<ContactProgramResponse>(json);

                        if (contactProgramResponse.ContactPrograms.Count() == 0)
                        {
                            var new_result = new HttpResponseMessage();
                            new_result.StatusCode = HttpStatusCode.Gone;
                            return new_result;
                        }
                        else
                        {
                            await Task.Run(() => SaveContactProgramData(contactProgramResponse));
                        }
                    }
                    return result;                   
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "ActivateProgram" } });
                return mess;
            }
        }

        public static async Task<HttpResponseMessage> RestartProgram(HttpClient _client, Guid _ContactProgramGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._restartProgram + _ContactProgramGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                var result = await _client.SendAsync(request);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = result.Content;
                    var json = await content.ReadAsStringAsync();
                    var contactProgramResponse = JsonConvert.DeserializeObject<ContactProgramResponse>(json);

                    if (contactProgramResponse.ContactPrograms.Count() == 0)
                    {
                        var new_result = new HttpResponseMessage();
                        new_result.StatusCode = HttpStatusCode.Gone;
                        return new_result;
                    }
                    else
                    {
                        await Task.Run(() => SaveContactProgramData(contactProgramResponse));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "ActivateProgram" } });
                return mess;
            }
        }

        public static async Task<DownloadResponseV2> GetContactProgramDayToDownload(HttpClient _client, Guid _ContactProgramDayGuid, Guid Parent_ContactProgramDayGuid, bool isScheduleDeviation)
        {
            bool isRepeat = false;
            Guid CPDGuid = _ContactProgramDayGuid;

            if(_ContactProgramDayGuid == Guid.Empty)
            {
                //need to create a new cpd
                //post cpd guid or parent guid, and isRepeat
                isRepeat = true;
                CPDGuid = Parent_ContactProgramDayGuid;
            }

            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._download + CPDGuid + "&isRepeat=" + isRepeat + "&isScheduleDeviation=" + isScheduleDeviation);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                var response = await _client.SendAsync(request);
                var downloadResponse = new DownloadResponseV2();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    downloadResponse = JsonConvert.DeserializeObject<DownloadResponseV2>(json);
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    downloadResponse.Message = "download limit reached";
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    downloadResponse.Message = "bad request";
                }
                return downloadResponse;

            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetContactProgramDayToDownload" } });
                return new DownloadResponseV2();
            }
        }

        public static async Task SyncToCRM(HttpClient _client)
        {
            try
            {
                List<SyncContactProgramDay> syncCPD = new List<SyncContactProgramDay>();
                List<SyncContactAction> syncCA = new List<SyncContactAction>();
                List<LocalDBContactAction> ContactActionsToSync = new List<LocalDBContactAction>();
                var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                var ContactProgramDaysToSync = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.Synced == false && x.IsComplete == true && x.ActualStartDate != (DateTime?)null).ToListAsync();

                //Construct an Object to post to WEB API
                foreach (var cpd in ContactProgramDaysToSync)
                {
                    var newSyncCPD = new SyncContactProgramDay
                    {
                        GuidCRM = cpd.GuidCRM,
                        Synced = true,
                        IsComplete = cpd.IsComplete,
                        Rating = cpd.Rating,
                    };

                    if (cpd.ActualStartDate.HasValue)
                    {
                        if(cpd.ActualStartDate >= DateTime.UtcNow.AddDays(-300))
                        {
                            newSyncCPD.ActualStartDate = (DateTime)cpd.ActualStartDate;                           
                        } else { newSyncCPD.ActualStartDate = DateTime.UtcNow; }                        
                    }
                    else
                    {
                        newSyncCPD.ActualStartDate = DateTime.UtcNow;
                    }
                    syncCPD.Add(newSyncCPD);
                }

                var caToSync = await _connection.Table<LocalDBContactAction>().Where(x => x.Synced == false && x.IsComplete == true).ToListAsync();
                ContactActionsToSync.AddRange(caToSync);

                foreach (var ca in ContactActionsToSync)
                {
                    var newSyncCA = new SyncContactAction
                    {
                        GuidCRM = ca.GuidCRM,
                        Synced = true,
                        IsComplete = true,
                        ActualNumberOfReps = ca.ActualNumberOfReps,
                        ActualWeightLbs = ca.ActualWeightLbs,
                        ActualTimeSeconds = ca.ActualTimeSeconds,
                        ActualRestTimeSeconds = ca.ActualRestTimeSeconds,
                    };
                    syncCA.Add(newSyncCA);
                }
                Analytics.TrackEvent(_WebApiName, new Dictionary<string, string>() { { "Function", "SyncToCRM" }, { "syncCPD Count", syncCPD.Count().ToString() }, { "syncCA Count", syncCA.Count().ToString() }, });

                //You must add the headers here so they are used in both requests otherwise an error is thrown says that the header Authorization has already been added
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);

                if (syncCPD.Count() > 0 && syncCA.Count() > 0)
                {
                    Task t1 = Task.Run(() => SyncContactProgramDaysToCRM(_client, syncCPD));
                    Task t2 = Task.Run(() => SyncContactActionsToCRM(_client, syncCA));
                    TaskList.Add(t1);
                    TaskList.Add(t2);
                    // TaskList.Add(Task.Factory.StartNew(() => SyncContactProgramDaysToCRM(syncCPD)).Unwrap());
                    //  TaskList.Add(Task.Factory.StartNew(() => SyncContactActionsToCRM(syncCA)).Unwrap());
                }
                else if (syncCA.Count() > 0)
                {
                    Task t2 = Task.Run(() => SyncContactActionsToCRM(_client, syncCA));
                    TaskList.Add(t2);
                    //TaskList.Add(Task.Factory.StartNew(() => SyncContactActionsToCRM(syncCA)).Unwrap());
                }
                else if (syncCPD.Count() > 0)
                {
                    Task t1 = Task.Run(() => SyncContactProgramDaysToCRM(_client, syncCPD));
                    TaskList.Add(t1);
                    //TaskList.Add(Task.Factory.StartNew(() => SyncContactProgramDaysToCRM(syncCPD)).Unwrap());
                }
                await Task.WhenAll(TaskList.ToArray());
                Task.WaitAll(TaskList.ToArray());

                //POST each and when return OK, make as syncd
                //Contact program days - syncd, complete, rating, actual start date, 
                //Contact actions - Actual number of reps, auctual weight, actual time, actual rest time, complete, syncd
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "SyncToCRM" } });
            }
        }

        public static async Task GetAccountPrograms(HttpClient _client, Guid _accountGuid, int AccountId)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._programsAccountGuidUri + _accountGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var accountPrograms = JsonConvert.DeserializeObject<List<APIProgram>>(json);

                    if (accountPrograms.Count() > 0)
                    {
                        var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

                        foreach (var program in accountPrograms)
                        {
                            var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == program.GuidCRM).FirstOrDefaultAsync();
                            if (Program == null)
                            {
                                var newProgram = new LocalDBProgram
                                {
                                    GuidCRM = program.GuidCRM,
                                    Heading = program.Heading,
                                    SubHeading = program.SubHeading,
                                    PhotoUrl = program.PhotoUrl,
                                    SecondaryPhotoUrl = program.SecondaryPhotoUrl,
                                    VideoUrl = program.VideoUrl,
                                    SequenceNumber = program.SequenceNumber,
                                    StateCodeValue = program.StateCodeValue,
                                    StatusCodeValue = program.StatusCodeValue,
                                    TotalWeeks = program.TotalWeeks,
                                    Goal = program.Goal,
                                    GoalValue = program.GoalValue,
                                    Level = program.Level,
                                    LevelValue = program.LevelValue,
                                    //Cost = program.Cost,
                                    AccountId = AccountId
                                };
                                await _connection.InsertAsync(newProgram);
                            }
                            else
                            {
                                Program.AccountId = AccountId;
                                Program.SequenceNumber = program.SequenceNumber;
                                Program.StatusCodeValue = program.StatusCodeValue;
                                Program.StateCodeValue = program.StateCodeValue;
                                Program.PhotoUrl = program.PhotoUrl;
                                Program.SecondaryPhotoUrl = program.SecondaryPhotoUrl;
                                Program.VideoUrl = program.VideoUrl;
                                await _connection.UpdateAsync(Program);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetAccountPrograms" } });
            }
        }

        public static async Task<HttpResponseMessage> GetProgramDayActions(HttpClient _client, Guid _ProgramDayGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._actionsProgramDayUri + _ProgramDayGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                return await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetProgramDayActions" } });
                return mess;
            }
        }

        public static async Task GetProgramDays(HttpClient _client, Guid _ProgramGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._programDaysProgramGuidUri + _ProgramGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);

                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var programDays = JsonConvert.DeserializeObject<List<APIProgramDay>>(json);

                    if (programDays.Count() > 0)
                    {
                        var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                        var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == _ProgramGuid).FirstOrDefaultAsync();

                        foreach (var programDay in programDays)
                        {
                            var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.GuidCRM == programDay.GuidCRM).FirstOrDefaultAsync();
                            if (ProgramDay == null)
                            {
                                ProgramDay = new LocalDBProgramDay();
                                ProgramDay.GuidCRM = programDay.GuidCRM;
                                ProgramDay.DayType = programDay.DayType;
                                ProgramDay.DayTypeValue = programDay.DayTypeValue;
                                ProgramDay.TotalActions = programDay.TotalActions;
                                ProgramDay.TotalExercises = programDay.TotalExercises;
                                ProgramDay.TimeMinutes = programDay.TimeMinutes;
                                ProgramDay.Heading = programDay.Heading;
                                ProgramDay.SubHeading = programDay.SubHeading;
                                ProgramDay.PhotoUrl = programDay.PhotoUrl;
                                ProgramDay.SequenceNumber = programDay.SequenceNumber;
                                ProgramDay.StateCodeValue = programDay.StateCodeValue;
                                ProgramDay.StatusCodeValue = programDay.StatusCodeValue;
                                ProgramDay.Level = programDay.Level;
                                ProgramDay.LevelValue = programDay.LevelValue;
                                ProgramDay.ProgramId = Program.Id;                                
                                await _connection.InsertAsync(ProgramDay);
                            }
                            else
                            {
                                //ProgramDay.GuidCRM = programDay.GuidCRM;
                                ProgramDay.DayType = programDay.DayType;
                                ProgramDay.DayTypeValue = programDay.DayTypeValue;
                                ProgramDay.TotalActions = programDay.TotalActions;
                                ProgramDay.TotalExercises = programDay.TotalExercises;
                                ProgramDay.TimeMinutes = programDay.TimeMinutes;
                                ProgramDay.Heading = programDay.Heading;
                                ProgramDay.SubHeading = programDay.SubHeading;
                                ProgramDay.PhotoUrl = programDay.PhotoUrl;
                                ProgramDay.SequenceNumber = programDay.SequenceNumber;
                                ProgramDay.StateCodeValue = programDay.StateCodeValue;
                                ProgramDay.StatusCodeValue = programDay.StatusCodeValue;
                                ProgramDay.Level = programDay.Level;
                                ProgramDay.LevelValue = programDay.LevelValue;
                                ProgramDay.ProgramId = Program.Id;
                                await _connection.UpdateAsync(ProgramDay);
                            }
                        }

                    }

                }
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetProgramDays" } });
            }
        }

        public static async Task<HttpResponseMessage> BuildProgramSchedule(HttpClient _client, Guid _ContactProgramGuid, bool advanceToNextDay)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._scheduleProgramDayUri + _ContactProgramGuid + "&advanceToNextDay=" + advanceToNextDay);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                return await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "BuildProgramSchedule" } });
                return mess;
            }
        }

        public static async Task<HttpResponseMessage> ModifySchedule(HttpClient _client, RescheduledCPD3 rCPD)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                string json = JsonConvert.SerializeObject(rCPD);
                var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                var newUri = new Uri(App._absoluteUri, App._rescheduleCPD);
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                return await _client.PostAsync(newUri, contentString);
            } catch(Exception ex)
            {
                var error = ex.ToString();
                var mess =  new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "ModifySchedule" } });
                return mess;                 
            } 
        }

        public static async Task<HttpResponseMessage> SendContactUsMessage(HttpClient _client, APIMessage Message)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                string json = JsonConvert.SerializeObject(Message);
                var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                var newUri = new Uri(App._absoluteUri, App._message);
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                return await _client.PostAsync(newUri, contentString);
            } catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "SendContactUsMessage" } });
                return mess;
            }

        }

        public static async Task<HttpResponseMessage> RepeatProgram(HttpClient _client, Guid CPDGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._repeatProgram + CPDGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                var result = await _client.SendAsync(request);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = result.Content;
                    var json = await content.ReadAsStringAsync();
                    var contactProgramResponse = JsonConvert.DeserializeObject<ContactProgramResponse>(json);

                    if (contactProgramResponse.ContactPrograms.Count() == 0)
                    {
                        var new_result = new HttpResponseMessage();
                        new_result.StatusCode = HttpStatusCode.Gone;
                        return new_result;
                    }
                    else
                    {
                        await Task.Run(() => SaveContactProgramData(contactProgramResponse));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "RepeatProgram" } });
                return mess;
            }
        }

    }
}
