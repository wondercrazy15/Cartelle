using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Stance.Models.API;
using Stance.Models.Base;
using Stance.Models.LocalDB;
using Stance.Models.Optimized;
using Stance.Utils.Auth;
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
                request.RequestUri = new Uri(App._absoluteUri, App._initialLoad + true);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", _cred);
                return await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "InitialLoad" } });
                return mess;
            }
        }

        public async static Task SaveInitialLoadData(SignInResponseV5 signInResponse)
        {
            try
            {
                var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

                if (signInResponse.Profile != null)
                {
                    var profile = await _connection.Table<LocalDBContactV2>().FirstOrDefaultAsync();
                    if (profile != null)
                    {
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
                        profile.ConfirmedEmail = signInResponse.Profile.ConfirmedEmail;
                        profile.RP = signInResponse.Profile.RP;
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
                        profile.ConfirmedEmail = signInResponse.Profile.ConfirmedEmail;
                        profile.RP = signInResponse.Profile.RP;
                        await _connection.InsertAsync(profile);
                    }
                }

                var exitingContactProgramsToDelete = await _connection.Table<LocalDBContactProgram>().ToListAsync();

                if (signInResponse.ContactPrograms.Count() > 0)
                {
                    var Accounts = await _connection.Table<LocalDBAccount>().ToListAsync();
                    var Programs = await _connection.Table<LocalDBProgram>().ToListAsync();
                    var Challenges = await _connection.Table<LocalDBChallenge>().ToListAsync();

                    foreach (var ContactProgram in signInResponse.ContactPrograms)
                    {
                        var newAccount = Accounts.Where(x => x.GuidCRM == ContactProgram.Program.AccountGuid).FirstOrDefault();
                        if (newAccount == null)
                        {
                            newAccount = new LocalDBAccount
                            {
                                GuidCRM = ContactProgram.Program.AccountGuid,
                            };
                            await _connection.InsertAsync(newAccount);
                            Accounts.Add(newAccount);
                        }

                        var newProgram = Programs.Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefault();
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
                            newProgram.Type = ContactProgram.Program.Type;
                            newProgram.AccountId = newAccount.Id;
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
                            newProgram.Type = ContactProgram.Program.Type;
                            newProgram.AccountId = newAccount.Id;
                            await _connection.InsertAsync(newProgram);
                            Programs.Add(newProgram);
                        }

                        var newContactProgram = exitingContactProgramsToDelete.Where(x => x.GuidCRM == ContactProgram.GuidCRM).FirstOrDefault();
                        if (newContactProgram != null)
                        {
                            //newContactProgram.GuidCRM = ContactProgram.GuidCRM;
                            newContactProgram.StateCodeValue = ContactProgram.StateCodeValue;
                            newContactProgram.StatusCodeValue = ContactProgram.StatusCodeValue;
                            newContactProgram.StartDate = (DateTime?)ContactProgram.StartDate;
                            newContactProgram.EndDate = (DateTime?)ContactProgram.EndDate;
                            newContactProgram.IsDaysCreated = ContactProgram.IsDaysCreated;
                            newContactProgram.IsScheduleBuilt = ContactProgram.IsScheduleBuilt;
                            newContactProgram.ProgramId = newProgram.Id;
                            if (ContactProgram.ChallengeGuid != Guid.Empty)
                            {
                                var Challenge = Challenges.Where(x => x.GuidCRM == ContactProgram.ChallengeGuid).FirstOrDefault();
                                if (Challenge == null)
                                {
                                    Challenge = new LocalDBChallenge
                                    {
                                        GuidCRM = ContactProgram.ChallengeGuid,
                                    };
                                    await _connection.InsertAsync(Challenge);
                                    Challenges.Add(Challenge);
                                }
                                newContactProgram.ChallengeId = Challenge.Id;
                            }
                            else
                            {
                                newContactProgram.ChallengeId = 0;
                            }
                            await _connection.UpdateAsync(newContactProgram);
                            exitingContactProgramsToDelete.Remove(newContactProgram);
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
                            newContactProgram.ProgramId = newProgram.Id;
                            if (ContactProgram.ChallengeGuid != Guid.Empty)
                            {
                                var Challenge = Challenges.Where(x => x.GuidCRM == ContactProgram.ChallengeGuid).FirstOrDefault();
                                if (Challenge == null)
                                {
                                    Challenge = new LocalDBChallenge
                                    {
                                        GuidCRM = ContactProgram.ChallengeGuid,
                                    };
                                    await _connection.InsertAsync(Challenge);
                                    Challenges.Add(Challenge);
                                }
                                newContactProgram.ChallengeId = Challenge.Id;
                            }
                            else
                            {
                                newContactProgram.ChallengeId = 0;
                            }
                            await _connection.InsertAsync(newContactProgram);
                        }
                    }
                }

                if (exitingContactProgramsToDelete.Count() > 0)
                {
                    foreach (var cp in exitingContactProgramsToDelete)
                    {
                        //delete contact actions, contact program days, contact program
                        var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.ContactProgramId == cp.Id).ToListAsync();
                        if (ContactProgramDays.Count() > 0)
                        {
                            foreach (var cpd in ContactProgramDays)
                            {
                                var ContactActions = await _connection.Table<LocalDBContactAction>().Where(x => x.ContactProgramDayId == cpd.Id).ToListAsync();
                                if (ContactActions.Count() > 0)
                                {
                                    foreach (var ca in ContactActions)
                                    {
                                        await _connection.DeleteAsync(ca);
                                    }
                                }
                                await _connection.DeleteAsync(cpd);
                            }
                        }
                        await _connection.DeleteAsync(cp);
                    }
                }

                if (signInResponse.ContactProgramDays.ContactProgramDays.Count() > 0)
                {
                    var Program1 = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == signInResponse.ContactProgramDays.ProgramGuid).FirstOrDefaultAsync();
                    var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == signInResponse.ContactProgramDays.ContactProgramGuid).FirstOrDefaultAsync();
                    var ProgramDays = await _connection.Table<LocalDBProgramDay>().ToListAsync();
                    var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().ToListAsync();

                    foreach (var cpd in signInResponse.ContactProgramDays.ContactProgramDays)
                    {
                        var newProgramDay = ProgramDays.Where(x => x.GuidCRM == cpd.ProgramDay.GuidCRM).FirstOrDefault();
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
                            ProgramDays.Add(newProgramDay);
                        }

                        var newContactProgramDay = ContactProgramDays.Where(x => x.GuidCRM == cpd.GuidCRM).FirstOrDefault();
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
                            newContactProgramDay.ProgramDayId = newProgramDay.Id;
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
                            newContactProgramDay.ProgramDayId = newProgramDay.Id;
                            newContactProgramDay.ReceivedOn = DateTime.UtcNow;
                            await _connection.InsertAsync(newContactProgramDay);
                            ContactProgramDays.Add(newContactProgramDay);
                        }
                    }
                }

                var existingSubscriptionsToDelete = await _connection.Table<LocalDBSubscription>().Where(x => x.GuidCRM != Guid.Empty).ToListAsync();
                if (signInResponse.Subscriptions.Count() > 0) //Types 866660000 program, 866660001 challenge, 866660002 athlete, 866660003 full
                {
                    var Programs = await _connection.Table<LocalDBProgram>().ToListAsync();
                    var Accounts = await _connection.Table<LocalDBAccount>().ToListAsync();
                    var Challenges = await _connection.Table<LocalDBChallenge>().ToListAsync();

                    foreach (var subscription in signInResponse.Subscriptions)
                    {
                        var newSubscription = existingSubscriptionsToDelete.Where(x => x.GuidCRM == subscription.GuidCRM).FirstOrDefault();
                        if (newSubscription != null)
                        {
                            newSubscription.StateCodeValue = subscription.StateCodeValue;
                            newSubscription.StatusCodeValue = subscription.StatusCodeValue;
                            newSubscription.Type = subscription.Type;
                            newSubscription.AutoRenew = subscription.AutoRenew;
                            newSubscription.StartDate = subscription.StartDate;
                            newSubscription.EndDate = subscription.EndDate;
                            newSubscription.EndedOn = subscription.EndedOn;
                            newSubscription.StartedAsTrial = subscription.StartedAsTrial;
                            newSubscription.TrialEndedOn = subscription.TrialEndedOn;
                            newSubscription.TrialEndDate = subscription.TrialEndDate;
                            if (subscription.ProgramGuid != Guid.Empty)
                            {
                                var newProgram = Programs.Where(x => x.GuidCRM == subscription.ProgramGuid).FirstOrDefault();
                                if (newProgram == null)
                                {
                                    newProgram = new LocalDBProgram();
                                    newProgram.GuidCRM = subscription.ProgramGuid;
                                    await _connection.InsertAsync(newProgram);
                                    Programs.Add(newProgram);
                                }
                                newSubscription.ProgramId = newProgram.Id;
                            }
                            else
                            {
                                newSubscription.ProgramId = 0;
                            }
                            if (subscription.AccountGuid != Guid.Empty)
                            {
                                var newAccount = Accounts.Where(x => x.GuidCRM == subscription.AccountGuid).FirstOrDefault();
                                if (newAccount == null)
                                {
                                    newAccount = new LocalDBAccount();
                                    newAccount.GuidCRM = subscription.ProgramGuid;
                                    await _connection.InsertAsync(newAccount);
                                    Accounts.Add(newAccount);
                                }
                                newSubscription.AccountId = newAccount.Id;
                            }
                            else
                            {
                                newSubscription.AccountId = 0;
                            }
                            if (subscription.ChallengeGuid != Guid.Empty)
                            {
                                var Challenge = Challenges.Where(x => x.GuidCRM == subscription.ChallengeGuid).FirstOrDefault();
                                if (Challenge == null)
                                {
                                    Challenge = new LocalDBChallenge
                                    {
                                        GuidCRM = subscription.ChallengeGuid,
                                    };
                                    await _connection.InsertAsync(Challenge);
                                    Challenges.Add(Challenge);
                                }
                                newSubscription.ChallengeId = Challenge.Id;
                            }
                            else
                            {
                                newSubscription.ChallengeId = 0;
                            }
                            await _connection.UpdateAsync(newSubscription);
                            existingSubscriptionsToDelete.Remove(newSubscription);
                        }
                        else
                        {
                            newSubscription = new LocalDBSubscription();
                            newSubscription.GuidCRM = subscription.GuidCRM;
                            newSubscription.StateCodeValue = subscription.StateCodeValue;
                            newSubscription.StatusCodeValue = subscription.StatusCodeValue;
                            newSubscription.Type = subscription.Type;
                            newSubscription.AutoRenew = subscription.AutoRenew;
                            newSubscription.StartDate = subscription.StartDate;
                            newSubscription.EndDate = subscription.EndDate;
                            newSubscription.EndedOn = subscription.EndedOn;
                            newSubscription.StartedAsTrial = subscription.StartedAsTrial;
                            newSubscription.TrialEndedOn = subscription.TrialEndedOn;
                            newSubscription.TrialEndDate = subscription.TrialEndDate;
                            if (subscription.ProgramGuid != Guid.Empty)
                            {
                                var newProgram = Programs.Where(x => x.GuidCRM == subscription.ProgramGuid).FirstOrDefault();
                                if (newProgram == null)
                                {
                                    newProgram = new LocalDBProgram();
                                    newProgram.GuidCRM = subscription.ProgramGuid;
                                    await _connection.InsertAsync(newProgram);
                                    Programs.Add(newProgram);
                                }
                                newSubscription.ProgramId = newProgram.Id;
                            }
                            else
                            {
                                newSubscription.ProgramId = 0;
                            }
                            if (subscription.AccountGuid != Guid.Empty)
                            {
                                var newAccount = Accounts.Where(x => x.GuidCRM == subscription.AccountGuid).FirstOrDefault();
                                if (newAccount == null)
                                {
                                    newAccount = new LocalDBAccount();
                                    newAccount.GuidCRM = subscription.AccountGuid;
                                    await _connection.InsertAsync(newAccount);
                                    Accounts.Add(newAccount);
                                }
                                newSubscription.AccountId = newAccount.Id;
                            }
                            else
                            {
                                newSubscription.AccountId = 0;
                            }
                            if (subscription.ChallengeGuid != Guid.Empty)
                            {
                                var Challenge = Challenges.Where(x => x.GuidCRM == subscription.ChallengeGuid).FirstOrDefault();
                                if (Challenge == null)
                                {
                                    Challenge = new LocalDBChallenge
                                    {
                                        GuidCRM = subscription.ChallengeGuid,
                                    };
                                    await _connection.InsertAsync(Challenge);
                                    Challenges.Add(Challenge);
                                }
                                newSubscription.ChallengeId = Challenge.Id;
                            }
                            else
                            {
                                newSubscription.ChallengeId = 0;
                            }
                            await _connection.InsertAsync(newSubscription);
                        }
                    }
                }

                if (existingSubscriptionsToDelete.Count() > 0)
                {
                    foreach (var sub in existingSubscriptionsToDelete)
                    {
                        await _connection.DeleteAsync(sub);
                    }
                }

                var exitingChallengesToDelete = await _connection.Table<LocalDBChallenge>().ToListAsync();

                if (signInResponse.ContactChallenges.Count() > 0)
                {
                    var Accounts = await _connection.Table<LocalDBAccount>().ToListAsync();
                    var Programs = await _connection.Table<LocalDBProgram>().ToListAsync();

                    foreach (var obj in signInResponse.ContactChallenges)
                    {
                        var newChallenge = exitingChallengesToDelete.Where(x => x.GuidCRM == obj.Challenge.GuidCRM).FirstOrDefault();
                        if (newChallenge != null)
                        {
                            newChallenge.StateCodeValue = obj.Challenge.StateCodeValue;
                            newChallenge.StatusCodeValue = obj.Challenge.StatusCodeValue;
                            newChallenge.Name = obj.Challenge.Name;
                            newChallenge.StartDate = obj.Challenge.StartDate;
                            newChallenge.EndDate = obj.Challenge.EndDate;
                            newChallenge.Activated = obj.Challenge.Activated;
                            newChallenge.ScheduleSet = obj.Challenge.ScheduleSet;
                            if (obj.Challenge.AccountGuid != Guid.Empty)
                            {
                                var newAccount = Accounts.Where(x => x.GuidCRM == obj.Challenge.AccountGuid).FirstOrDefault();
                                if (newAccount == null)
                                {
                                    newAccount = new LocalDBAccount
                                    {
                                        GuidCRM = obj.Challenge.AccountGuid,
                                    };
                                    await _connection.InsertAsync(newAccount);
                                    Accounts.Add(newAccount);
                                }
                                newChallenge.AcccountId = newAccount.Id;
                            }
                            else
                            {
                                newChallenge.AcccountId = 0;
                            }

                            if (obj.Challenge.ProgramGuid != Guid.Empty)
                            {
                                var newProgram = Programs.Where(x => x.GuidCRM == obj.Challenge.ProgramGuid).FirstOrDefault();
                                if (newProgram == null)
                                {
                                    newProgram = new LocalDBProgram();
                                    newProgram.GuidCRM = obj.Challenge.ProgramGuid;
                                    await _connection.InsertAsync(newProgram);
                                    Programs.Add(newProgram);
                                }
                                newChallenge.AcccountId = newProgram.Id;
                            }
                            else
                            {
                                newChallenge.AcccountId = 0;
                            }
                            await _connection.UpdateAsync(newChallenge);
                            exitingChallengesToDelete.Remove(newChallenge);
                        }
                        else
                        {
                            newChallenge = new LocalDBChallenge();
                            newChallenge.GuidCRM = obj.Challenge.GuidCRM;
                            newChallenge.StateCodeValue = obj.Challenge.StateCodeValue;
                            newChallenge.StatusCodeValue = obj.Challenge.StatusCodeValue;
                            newChallenge.Name = obj.Challenge.Name;
                            newChallenge.StartDate = obj.Challenge.StartDate;
                            newChallenge.EndDate = obj.Challenge.EndDate;
                            newChallenge.Activated = obj.Challenge.Activated;
                            newChallenge.ScheduleSet = obj.Challenge.ScheduleSet;
                            if (obj.Challenge.AccountGuid != Guid.Empty)
                            {
                                var newAccount = Accounts.Where(x => x.GuidCRM == obj.Challenge.AccountGuid).FirstOrDefault();
                                if (newAccount == null)
                                {
                                    newAccount = new LocalDBAccount
                                    {
                                        GuidCRM = obj.Challenge.AccountGuid,
                                    };
                                    await _connection.InsertAsync(newAccount);
                                    Accounts.Add(newAccount);
                                }
                                newChallenge.AcccountId = newAccount.Id;
                            }
                            else
                            {
                                newChallenge.AcccountId = 0;
                            }

                            if (obj.Challenge.ProgramGuid != Guid.Empty)
                            {
                                var newProgram = Programs.Where(x => x.GuidCRM == obj.Challenge.ProgramGuid).FirstOrDefault();
                                if (newProgram == null)
                                {
                                    newProgram = new LocalDBProgram();
                                    newProgram.GuidCRM = obj.Challenge.ProgramGuid;
                                    await _connection.InsertAsync(newProgram);
                                    Programs.Add(newProgram);
                                }
                                newChallenge.AcccountId = newProgram.Id;
                            }
                            else
                            {
                                newChallenge.AcccountId = 0;
                            }
                            await _connection.InsertAsync(newChallenge);
                        }
                    }
                }

                if (exitingChallengesToDelete.Count() > 0)
                {
                    foreach (var cha in exitingChallengesToDelete)
                    {
                        await _connection.DeleteAsync(cha);
                    }
                }

                var synced = await _connection.Table<LocalDBSync>().FirstOrDefaultAsync();
                if (synced == null)
                {
                    var newSynced = new LocalDBSync
                    {
                        SyncedOn = DateTime.UtcNow,
                        SubscriptionSyncedOn = DateTime.UtcNow,
                    };
                    await _connection.InsertAsync(newSynced);
                }
                else
                {
                    synced.SyncedOn = DateTime.UtcNow;
                    synced.SubscriptionSyncedOn = DateTime.UtcNow;
                    await _connection.UpdateAsync(synced);
                }
            }
            catch (Exception ex)
            {
                var err = ex.ToString();
            }


        }

        public async static Task SaveSubscriptionData(SubscriptionsV2 Subscriptions)
        {
            var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            var existingSubscriptionsToDelete = await _connection.Table<LocalDBSubscription>().Where(x => x.GuidCRM != Guid.Empty).ToListAsync();
            if (Subscriptions.subscriptions.Count() > 0) //Types 866660000 program, 866660001 challenge, 866660002 athlete, 866660003 full
            {
                var Programs = await _connection.Table<LocalDBProgram>().ToListAsync();
                var Accounts = await _connection.Table<LocalDBAccount>().ToListAsync();

                foreach (var subscription in Subscriptions.subscriptions)
                {
                    var newSubscription = existingSubscriptionsToDelete.Where(x => x.GuidCRM == subscription.GuidCRM).FirstOrDefault();
                    if (newSubscription != null)
                    {
                        newSubscription.StateCodeValue = subscription.StateCodeValue;
                        newSubscription.StatusCodeValue = subscription.StatusCodeValue;
                        newSubscription.Type = subscription.Type;
                        newSubscription.AutoRenew = subscription.AutoRenew;
                        newSubscription.StartDate = subscription.StartDate;
                        newSubscription.EndDate = subscription.EndDate;
                        newSubscription.EndedOn = subscription.EndedOn;
                        newSubscription.StartedAsTrial = subscription.StartedAsTrial;
                        newSubscription.TrialEndedOn = subscription.TrialEndedOn;
                        newSubscription.TrialEndDate = subscription.TrialEndDate;
                        if (subscription.ProgramGuid != Guid.Empty)
                        {
                            var newProgram = Programs.Where(x => x.GuidCRM == subscription.ProgramGuid).FirstOrDefault();
                            if (newProgram == null)
                            {
                                newProgram = new LocalDBProgram();
                                newProgram.GuidCRM = subscription.ProgramGuid;
                                await _connection.InsertAsync(newProgram);
                                Programs.Add(newProgram);
                            }
                            newSubscription.ProgramId = newProgram.Id;
                        }
                        if (subscription.AccountGuid != Guid.Empty)
                        {
                            var newAccount = Accounts.Where(x => x.GuidCRM == subscription.AccountGuid).FirstOrDefault();
                            if (newAccount == null)
                            {
                                newAccount = new LocalDBAccount();
                                newAccount.GuidCRM = subscription.ProgramGuid;
                                await _connection.InsertAsync(newAccount);
                                Accounts.Add(newAccount);
                            }
                            newSubscription.AccountId = newAccount.Id;
                        }
                        await _connection.UpdateAsync(newSubscription);
                        existingSubscriptionsToDelete.Remove(newSubscription);
                    }
                    else
                    {
                        newSubscription = new LocalDBSubscription();
                        newSubscription.GuidCRM = subscription.GuidCRM;
                        newSubscription.StateCodeValue = subscription.StateCodeValue;
                        newSubscription.StatusCodeValue = subscription.StatusCodeValue;
                        newSubscription.Type = subscription.Type;
                        newSubscription.AutoRenew = subscription.AutoRenew;
                        newSubscription.StartDate = subscription.StartDate;
                        newSubscription.EndDate = subscription.EndDate;
                        newSubscription.EndedOn = subscription.EndedOn;
                        newSubscription.StartedAsTrial = subscription.StartedAsTrial;
                        newSubscription.TrialEndedOn = subscription.TrialEndedOn;
                        newSubscription.TrialEndDate = subscription.TrialEndDate;
                        if (subscription.ProgramGuid != Guid.Empty)
                        {
                            var newProgram = Programs.Where(x => x.GuidCRM == subscription.ProgramGuid).FirstOrDefault();
                            if (newProgram == null)
                            {
                                newProgram = new LocalDBProgram();
                                newProgram.GuidCRM = subscription.ProgramGuid;
                                await _connection.InsertAsync(newProgram);
                                Programs.Add(newProgram);
                            }
                            newSubscription.ProgramId = newProgram.Id;
                        }
                        if (subscription.AccountGuid != Guid.Empty)
                        {
                            var newAccount = Accounts.Where(x => x.GuidCRM == subscription.AccountGuid).FirstOrDefault();
                            if (newAccount == null)
                            {
                                newAccount = new LocalDBAccount();
                                newAccount.GuidCRM = subscription.AccountGuid;
                                await _connection.InsertAsync(newAccount);
                                Accounts.Add(newAccount);
                            }
                            newSubscription.AccountId = newAccount.Id;
                        }
                        await _connection.InsertAsync(newSubscription);
                    }

                }
            }

            if (existingSubscriptionsToDelete.Count() > 0)
            {
                foreach (var sub in existingSubscriptionsToDelete)
                {
                    await _connection.DeleteAsync(sub);
                }
            }
        }

        public async static Task<HttpResponseMessage> GetSubscriptions(HttpClient _client)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._subscription);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                var result = await _client.SendAsync(request);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = result.Content;
                    var json = await content.ReadAsStringAsync();
                    var subscriptions = JsonConvert.DeserializeObject<SubscriptionsV2>(json);

                    if (subscriptions.subscriptions.Count() == 0)
                    {
                        var new_result = new HttpResponseMessage();
                        new_result.StatusCode = HttpStatusCode.Gone;
                        return new_result;
                    }
                    else
                    {
                        await Task.Run(() => SaveSubscriptionData(subscriptions));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
                mess.StatusCode = HttpStatusCode.Gone;
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetSubscriptions" } });
                return mess;
            }
        }

        private async static Task SaveContactProgramData(ContactProgramResponseV2 contactProgramResponse)
        {
            var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            var Programs = await _connection.Table<LocalDBProgram>().ToListAsync();
            var ContactPrograms = await _connection.Table<LocalDBContactProgram>().ToListAsync();

            if (contactProgramResponse.ContactPrograms.Count() > 0)
            {
                foreach (var ContactProgram in contactProgramResponse.ContactPrograms)
                {
                    var newProgram = Programs.Where(x => x.GuidCRM == ContactProgram.Program.GuidCRM).FirstOrDefault();
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
                        newProgram.Type = ContactProgram.Program.Type;
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
                        newProgram.Type = ContactProgram.Program.Type;
                        await _connection.InsertAsync(newProgram);
                        Programs.Add(newProgram);
                    }

                    var newContactProgram = ContactPrograms.Where(x => x.GuidCRM == ContactProgram.GuidCRM).FirstOrDefault();
                    if (newContactProgram != null)
                    {
                        //newContactProgram.GuidCRM = ContactProgram.GuidCRM;
                        newContactProgram.StateCodeValue = ContactProgram.StateCodeValue;
                        newContactProgram.StatusCodeValue = ContactProgram.StatusCodeValue;
                        newContactProgram.StartDate = (DateTime?)ContactProgram.StartDate;
                        newContactProgram.EndDate = (DateTime?)ContactProgram.EndDate;
                        newContactProgram.IsDaysCreated = ContactProgram.IsDaysCreated;
                        newContactProgram.IsScheduleBuilt = ContactProgram.IsScheduleBuilt;
                        newContactProgram.ProgramId = newProgram.Id;
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
                        newContactProgram.ProgramId = newProgram.Id;
                        await _connection.InsertAsync(newContactProgram);
                        ContactPrograms.Add(newContactProgram);
                    }
                }
            }

            if (contactProgramResponse.ContactProgramDays.ContactProgramDays.Count() > 0)
            {
                var Program1 = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == contactProgramResponse.ContactProgramDays.ProgramGuid).FirstOrDefaultAsync();
                var ContactProgram = await _connection.Table<LocalDBContactProgram>().Where(x => x.GuidCRM == contactProgramResponse.ContactProgramDays.ContactProgramGuid).FirstOrDefaultAsync();
                var ProgramDays = await _connection.Table<LocalDBProgramDay>().ToListAsync();
                var ContactProgramDays = await _connection.Table<LocalDBContactProgramDayV2>().ToListAsync();

                foreach (var cpd in contactProgramResponse.ContactProgramDays.ContactProgramDays)
                {
                    var newProgramDay = ProgramDays.Where(x => x.GuidCRM == cpd.ProgramDay.GuidCRM).FirstOrDefault();
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
                        ProgramDays.Add(newProgramDay);
                    }

                    var newContactProgramDay = ContactProgramDays.Where(x => x.GuidCRM == cpd.GuidCRM).FirstOrDefault();
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
                        newContactProgramDay.ProgramDayId = newProgramDay.Id;
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
                        newContactProgramDay.ProgramDayId = newProgramDay.Id;
                        newContactProgramDay.ReceivedOn = DateTime.UtcNow;
                        await _connection.InsertAsync(newContactProgramDay);
                        ContactProgramDays.Add(newContactProgramDay);
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
                request.RequestUri = new Uri(App._absoluteUri, App._initialLoad + false);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                var result = await _client.SendAsync(request);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = result.Content;
                    var json = await content.ReadAsStringAsync();
                    var signInResponse = JsonConvert.DeserializeObject<SignInResponseV5>(json);

                    if (signInResponse.ContactPrograms.Count() == 0)
                    {
                        var new_result = new HttpResponseMessage();
                        new_result.StatusCode = HttpStatusCode.Gone;
                        return new_result;
                    }
                    else
                    {
                        await SaveInitialLoadData(signInResponse);
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

        public async static Task SaveAthletes(List<OptAccountV2> athletes)
        {
            if (athletes.Count() > 0)
            {
                var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                var AccountsToDeactivate = await _connection.Table<LocalDBAccount>().ToListAsync();

                foreach (var athlete in athletes)
                {
                    var Account = AccountsToDeactivate.Where(x => x.GuidCRM == athlete.GuidCRM).FirstOrDefault();
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
                            IGProfileUrl = athlete.IGProfileUrl,
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
                        Account.IGProfileUrl = athlete.IGProfileUrl;
                        await _connection.UpdateAsync(Account);
                        AccountsToDeactivate.Remove(Account);
                    }
                }

                foreach (var Account in AccountsToDeactivate) //was not removed from list so deactivate it
                {
                    Account.StatusCodeValue = 1;//inactive
                    Account.StateCodeValue = 2;//inactive
                    await _connection.UpdateAsync(Account);
                }
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
                    var athletes = JsonConvert.DeserializeObject<List<OptAccountV2>>(json);
                    await SaveAthletes(athletes);
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

        public async static Task GetAthleteRefferrerData(HttpClient _client, string accountCode)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._athleteReferralIUri + accountCode);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", GeneralAuth.Token);
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var signUpData = JsonConvert.DeserializeObject<AthleteOverviewData>(json);
                    await SaveAthleteSignUpData(signUpData);
                }
                else
                {
                    throw new Exception("Error - GetAthlete: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetAthleteRefferrerData" } });
            }
        }

        public async static Task SaveAthleteSignUpData(AthleteOverviewData athlete)
        {
            if (athlete == null)
                return;

            var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            var Programs = await _connection.Table<LocalDBProgram>().ToListAsync();

            var Account = await _connection.Table<LocalDBAccount>().Where(x => x.GuidCRM == athlete.GuidCRM).FirstOrDefaultAsync();
            if (Account == null)
            {
                Account = new LocalDBAccount
                {
                    GuidCRM = athlete.GuidCRM,
                    StateCodeValue = athlete.StateCodeValue,
                    StatusCodeValue = athlete.StatusCodeValue,
                    Heading = athlete.Heading,
                    SubHeading = athlete.SubHeading,
                    SecondaryPhotoUrl = athlete.SecondaryPhotoUrl,
                    Code = athlete.Code,
                };
                await _connection.InsertAsync(Account);
            }
            else
            {
                Account.StatusCodeValue = athlete.StatusCodeValue;
                Account.StateCodeValue = athlete.StateCodeValue;
                Account.Heading = athlete.Heading;
                Account.SubHeading = athlete.SubHeading;
                Account.SecondaryPhotoUrl = athlete.SecondaryPhotoUrl;
                Account.Code = athlete.Code;
                await _connection.UpdateAsync(Account);
            }

            foreach (var Program in athlete.Programs)
            {
                var newProgram = Programs.Where(x => x.GuidCRM == Program.GuidCRM).FirstOrDefault();
                if (newProgram != null)
                {
                    newProgram.StateCodeValue = Program.StateCodeValue;
                    newProgram.StatusCodeValue = Program.StatusCodeValue;
                    newProgram.Heading = Program.Heading;
                    newProgram.SubHeading = Program.SubHeading;
                    newProgram.PhotoUrl = Program.PhotoUrl;
                    newProgram.SequenceNumber = Program.SequenceNumber;
                    newProgram.Type = Program.Type;
                    newProgram.Code = Program.Code;
                    newProgram.AccountId = Account.Id;
                    await _connection.UpdateAsync(newProgram);
                }
                else
                {
                    newProgram = new LocalDBProgram();
                    newProgram.GuidCRM = Program.GuidCRM;
                    newProgram.StateCodeValue = Program.StateCodeValue;
                    newProgram.StatusCodeValue = Program.StatusCodeValue;
                    newProgram.Heading = Program.Heading;
                    newProgram.SubHeading = Program.SubHeading;
                    newProgram.PhotoUrl = Program.PhotoUrl;
                    newProgram.SequenceNumber = Program.SequenceNumber;
                    newProgram.Type = Program.Type;
                    newProgram.Code = Program.Code;
                    newProgram.AccountId = Account.Id;
                    await _connection.InsertAsync(newProgram);
                }
            }

        }

        public async static Task GetSignUpData(HttpClient _client)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._signUpProcessUri);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", GeneralAuth.Token);
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var signUpData = JsonConvert.DeserializeObject<SignUpData>(json);
                    await SaveSignUpData(signUpData.Athletes);
                }
                else
                {
                    throw new Exception("Error - GetAthletes: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "GetSignUpData" } });

            }
        }

        public async static Task SaveSignUpData(List<AthleteData> athletes)
        {
            if (athletes.Count() > 0)
            {
                var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                var AccountsToDeactivate = await _connection.Table<LocalDBAccount>().ToListAsync();
                var ProgramsToDeactive = await _connection.Table<LocalDBProgram>().ToListAsync();

                foreach (var athlete in athletes)
                {
                    var Account = AccountsToDeactivate.Where(x => x.GuidCRM == athlete.GuidCRM).FirstOrDefault();
                    if (Account == null)
                    {
                        Account = new LocalDBAccount
                        {
                            GuidCRM = athlete.GuidCRM,
                            StateCodeValue = athlete.StateCodeValue,
                            StatusCodeValue = athlete.StatusCodeValue,
                            Heading = athlete.Heading,
                            SequenceNumber = athlete.SequenceNumber,
                            IGProfileUrl = athlete.IGProfileUrl,
                            Code = athlete.Code,
                        };
                        await _connection.InsertAsync(Account);
                    }
                    else
                    {
                        Account.StatusCodeValue = athlete.StatusCodeValue;
                        Account.StateCodeValue = athlete.StateCodeValue;
                        Account.Heading = athlete.Heading;
                        Account.SequenceNumber = athlete.SequenceNumber;
                        Account.IGProfileUrl = athlete.IGProfileUrl;
                        Account.Code = athlete.Code;
                        await _connection.UpdateAsync(Account);
                        AccountsToDeactivate.Remove(Account);
                    }

                    foreach (var Program in athlete.Programs)
                    {
                        var newProgram = ProgramsToDeactive.Where(x => x.GuidCRM == Program.GuidCRM).FirstOrDefault();
                        if (newProgram != null)
                        {
                            newProgram.StateCodeValue = Program.StateCodeValue;
                            newProgram.StatusCodeValue = Program.StatusCodeValue;
                            newProgram.Heading = Program.Heading;
                            newProgram.SubHeading = Program.SubHeading;
                            newProgram.PhotoUrl = Program.PhotoUrl;
                            newProgram.SequenceNumber = Program.SequenceNumber;
                            newProgram.Type = Program.Type;
                            newProgram.Code = Program.Code;
                            newProgram.AccountId = Account.Id;
                            await _connection.UpdateAsync(newProgram);
                            ProgramsToDeactive.Remove(newProgram);
                        }
                        else
                        {
                            newProgram = new LocalDBProgram();
                            newProgram.GuidCRM = Program.GuidCRM;
                            newProgram.StateCodeValue = Program.StateCodeValue;
                            newProgram.StatusCodeValue = Program.StatusCodeValue;
                            newProgram.Heading = Program.Heading;
                            newProgram.SubHeading = Program.SubHeading;
                            newProgram.PhotoUrl = Program.PhotoUrl;
                            newProgram.SequenceNumber = Program.SequenceNumber;
                            newProgram.Type = Program.Type;
                            newProgram.Code = Program.Code;
                            newProgram.AccountId = Account.Id;
                            await _connection.InsertAsync(newProgram);
                        }
                    }
                }

                foreach (var Account in AccountsToDeactivate) //was not removed from list so deactivate it
                {
                    Account.StateCodeValue = 1;//inactive
                    Account.StatusCodeValue = 2;//inactive
                    await _connection.UpdateAsync(Account);
                }

                foreach (var Program in ProgramsToDeactive) //was not removed from list so deactivate it
                {
                    Program.StateCodeValue = 1;//inactive
                    Program.StatusCodeValue = 2;//inactive
                    await _connection.UpdateAsync(Program);
                }
            }
        }


        public async static Task<HttpStatusCode> UpdateProfile(HttpClient _client, APIContactV3 editedContact)
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
            }
            catch (Exception ex)
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
                }
                else
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

        public static async Task SyncContactProgramDaysToCRM(HttpClient _client, List<SyncContactProgramDayV2> syncCPD)
        {
            var dic = new Dictionary<string, string>();
            try
            {
                var cpd = new SyncCPDV2();
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
                Analytics.TrackEvent(_WebApiName, dic);

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
                }
                else
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
                    var contactProgramResponse = JsonConvert.DeserializeObject<ContactProgramResponseV2>(json);

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
                else
                {
                    var err = await result.Content.ReadAsStringAsync();
                    int i = 0;
                    err += "";
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
                    var contactProgramResponse = JsonConvert.DeserializeObject<ContactProgramResponseV2>(json);

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

            if (_ContactProgramDayGuid == Guid.Empty)
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
                    var err = await response.Content.ReadAsStringAsync();
                    int i = 0;
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
                List<SyncContactProgramDayV2> syncCPD = new List<SyncContactProgramDayV2>();
                List<SyncContactAction> syncCA = new List<SyncContactAction>();
                List<LocalDBContactAction> ContactActionsToSync = new List<LocalDBContactAction>();
                var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                var ContactProgramDaysToSync = await _connection.Table<LocalDBContactProgramDayV2>().Where(x => x.Synced == false && x.IsComplete == true && x.ActualStartDate != (DateTime?)null).ToListAsync();

                //Construct an Object to post to WEB API
                foreach (var cpd in ContactProgramDaysToSync)
                {
                    Guid ProgramGuid = Guid.Empty;

                    var ProgramDay = await _connection.Table<LocalDBProgramDay>().Where(x => x.Id == cpd.ProgramDayId).FirstOrDefaultAsync();
                    if (ProgramDay != null)
                    {
                        var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ProgramDay.ProgramId).FirstOrDefaultAsync();
                        if (Program != null)
                        {
                            ProgramGuid = Program.GuidCRM;
                        }
                    }

                    var newSyncCPD = new SyncContactProgramDayV2
                    {
                        GuidCRM = cpd.GuidCRM,
                        Synced = true,
                        IsComplete = cpd.IsComplete,
                        Rating = cpd.Rating,
                        ProgramGuid = ProgramGuid,
                    };

                    if (cpd.ActualStartDate.HasValue)
                    {
                        if (cpd.ActualStartDate >= DateTime.UtcNow.AddDays(-300))
                        {
                            newSyncCPD.ActualStartDate = (DateTime)cpd.ActualStartDate;
                        }
                        else { newSyncCPD.ActualStartDate = DateTime.UtcNow; }
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
                    var accountPrograms = JsonConvert.DeserializeObject<List<OptProgramV2>>(json);

                    if (accountPrograms.Count() > 0)
                    {
                        var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                        var ProgramsToDeactivate = await _connection.Table<LocalDBProgram>().Where(x => x.AccountId == AccountId).ToListAsync();

                        foreach (var program in accountPrograms)
                        {
                            var Program = ProgramsToDeactivate.Where(x => x.GuidCRM == program.GuidCRM).FirstOrDefault();
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
                                    Type = program.Type,
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

                                Program.Heading = program.Heading;
                                Program.SubHeading = program.SubHeading;
                                Program.TotalWeeks = program.TotalWeeks;
                                Program.Goal = program.Goal;
                                Program.GoalValue = program.GoalValue;
                                Program.Level = program.Level;
                                Program.LevelValue = program.LevelValue;
                                Program.Type = program.Type;

                                await _connection.UpdateAsync(Program);
                                ProgramsToDeactivate.Remove(Program);
                            }
                        }

                        foreach (var Program in ProgramsToDeactivate) //was not removed from list so deactivate it
                        {
                            Program.StatusCodeValue = 1;//inactive
                            Program.StateCodeValue = 2;//inactive
                            await _connection.UpdateAsync(Program);
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

        public static async Task SavePrograms(List<OptProgramV3> ProgramsAll)
        {
            if (ProgramsAll.Count() > 0)
            {
                var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                var ProgramsToDeactivate = await _connection.Table<LocalDBProgram>().ToListAsync();

                foreach (var program in ProgramsAll)
                {
                    var Program = ProgramsToDeactivate.Where(x => x.GuidCRM == program.GuidCRM).FirstOrDefault();
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
                            Type = program.Type,
                            Tags = program.Tags,
                            AthleteName = program.AthleteName,
                        };
                        await _connection.InsertAsync(newProgram);
                    }
                    else
                    {
                        Program.SequenceNumber = program.SequenceNumber;
                        Program.StatusCodeValue = program.StatusCodeValue;
                        Program.StateCodeValue = program.StateCodeValue;
                        Program.PhotoUrl = program.PhotoUrl;
                        Program.SecondaryPhotoUrl = program.SecondaryPhotoUrl;
                        Program.VideoUrl = program.VideoUrl;

                        Program.Heading = program.Heading;
                        Program.SubHeading = program.SubHeading;
                        Program.TotalWeeks = program.TotalWeeks;
                        Program.Goal = program.Goal;
                        Program.GoalValue = program.GoalValue;
                        Program.Level = program.Level;
                        Program.LevelValue = program.LevelValue;
                        Program.Type = program.Type;
                        Program.Tags = program.Tags;
                        Program.AthleteName = program.AthleteName;

                        await _connection.UpdateAsync(Program);
                        ProgramsToDeactivate.Remove(Program);
                    }
                }

                foreach (var Program in ProgramsToDeactivate) //was not removed from list so deactivate it
                {
                    Program.StatusCodeValue = 1;//inactive
                    Program.StateCodeValue = 2;//inactive
                    await _connection.UpdateAsync(Program);
                }

                //var Programs1 = await _connection.Table<LocalDBProgram>().ToListAsync();
                //foreach(var prog in Programs1)
                //{
                //    var pMatch = Programs1.Where(x => x.GuidCRM == prog.GuidCRM && prog.Id != x.Id).FirstOrDefault();
                //    if(pMatch != null)
                //    {
                //        await _connection.DeleteAsync(pMatch);
                //        Programs1.Remove(pMatch);
                //    }
                //}

            }



        }

        public static async Task GetAllPrograms(HttpClient _client)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._programsAll);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    var ProgramsAll = JsonConvert.DeserializeObject<List<OptProgramV3>>(json);
                    await SavePrograms(ProgramsAll);
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
                        var ProgramDays = await _connection.Table<LocalDBProgramDay>().ToListAsync();

                        foreach (var programDay in programDays)
                        {
                            var ProgramDay = ProgramDays.Where(x => x.GuidCRM == programDay.GuidCRM).FirstOrDefault();
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
                                ProgramDays.Add(ProgramDay);
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
            catch (Exception ex)
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
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                var mess = new HttpResponseMessage();
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
            }
            catch (Exception ex)
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
                    var contactProgramResponse = JsonConvert.DeserializeObject<ContactProgramResponseV2>(json);

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

        public static async Task<HttpStatusCode> AddToMyPlans(HttpClient _client, Guid ProgramGuid)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._addToMyPlans + ProgramGuid);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                var result = await _client.SendAsync(request);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = result.Content;
                    var json = await content.ReadAsStringAsync();
                    var contactProgram = JsonConvert.DeserializeObject<OptContactProgramV2>(json);

                    if (contactProgram == null)
                    {
                        return HttpStatusCode.Gone;
                    }
                    else
                    {
                        var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                        var Program = await _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == contactProgram.Program.GuidCRM).FirstOrDefaultAsync();
                        if (Program == null)
                        {
                            Program = new LocalDBProgram
                            {
                                GuidCRM = contactProgram.Program.GuidCRM,
                            };
                            await _connection.InsertAsync(Program);
                        }

                        var newCP = new LocalDBContactProgram
                        {
                            GuidCRM = contactProgram.GuidCRM,
                            StateCodeValue = contactProgram.StateCodeValue,
                            StatusCodeValue = contactProgram.StatusCodeValue,
                            ProgramId = Program.Id,
                        };
                        await _connection.InsertAsync(newCP);

                        return result.StatusCode;
                    }
                }
                return HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "AddToMyPlans" } });
                return HttpStatusCode.Gone;
            }

        }

        public static async Task<bool> CheckEmailConfirmation(HttpClient _client)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(App._absoluteUri, App._checkConfirmEmailUri);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", Auth.Auth.Token);
                var result = await _client.SendAsync(request);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = result.Content;
                    var json = await content.ReadAsStringAsync();
                    var signInResponse = JsonConvert.DeserializeObject<SignInResponseV5>(json);

                    await SaveInitialLoadData(signInResponse);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "CheckEmailConfirmation" } });
                return true;
            }
        }

        //public static async Task<bool> ResendEmailConfirmation(HttpClient _client)
        //{
        //    try
        //    {
        //        _client.DefaultRequestHeaders.Clear();
        //        var request = new HttpRequestMessage();
        //        request.RequestUri = new Uri(App._absoluteUri, App._resendConfirmEmailUri);
        //        request.Method = HttpMethod.Get;
        //        request.Headers.Add("Accept", "application/json");
        //        request.Headers.Add("Authorization", Auth.Auth.Token);
        //        var result = await _client.SendAsync(request);
        //        if (result.StatusCode == HttpStatusCode.OK)
        //            return true;

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "ResendEmailConfirmation" } });
        //        return false;
        //    }
        //}

        //public static async Task<int> RecommendedBy(HttpClient _client, bool isRecommended, Guid accountId)
        //{
        //    try
        //    {
        //        _client.DefaultRequestHeaders.Clear();
        //        var request = new HttpRequestMessage();
        //        request.RequestUri = new Uri(App._absoluteUri, App._recommendedUri + isRecommended + "&accountId=" + accountId);
        //        request.Method = HttpMethod.Get;
        //        request.Headers.Add("Accept", "application/json");
        //        request.Headers.Add("Authorization", Auth.Auth.Token);
        //        var response = await _client.SendAsync(request);
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            HttpContent content = response.Content;
        //            var json = await content.ReadAsStringAsync();
        //            var package = JsonConvert.DeserializeObject<TrialRecommendedBy>(json);
        //            await SavePrograms(package.Programs);
        //            return package.RP;
        //        }
        //        return -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "RecommendedBy" } });
        //        return -1;
        //    }
        //}

        //public async static Task<bool> SetTrialProgram(HttpClient _client, TrialProgram package, int programId)
        //{
        //    try
        //    {
        //        _client.DefaultRequestHeaders.Clear();
        //        string json = JsonConvert.SerializeObject(package);
        //        var contentString = new StringContent(json, Encoding.UTF8, "application/json");
        //        var newUri = new Uri(App._absoluteUri, App._setTrialProgramUri);
        //        _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
        //        HttpResponseMessage response = await _client.PostAsync(newUri, contentString);

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            HttpContent content = response.Content;
        //            var json2 = await content.ReadAsStringAsync();
        //            var cp = JsonConvert.DeserializeObject<TrialContactProgram>(json2);
        //            if (cp != null)
        //            {
        //                await SaveTrialContactProgram(cp, programId);
        //                return true;
        //            }
        //            return false;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "SetTrialProgram" } });
        //        return false;
        //    }
        //}

        //public static async Task SaveTrialContactProgram(TrialContactProgram ContactProgram, int programId)
        //{
        //    var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

        //    var newContactProgram = new LocalDBContactProgram();
        //    newContactProgram.GuidCRM = ContactProgram.GuidCRM;
        //    newContactProgram.StateCodeValue = ContactProgram.StateCodeValue;
        //    newContactProgram.StatusCodeValue = ContactProgram.StatusCodeValue;
        //    newContactProgram.StartDate = (DateTime?)ContactProgram.StartDate;
        //    newContactProgram.EndDate = (DateTime?)ContactProgram.EndDate;
        //    newContactProgram.IsDaysCreated = ContactProgram.IsDaysCreated;
        //    newContactProgram.IsScheduleBuilt = ContactProgram.IsScheduleBuilt;
        //    newContactProgram.ProgramId = programId;
        //    await _connection.InsertAsync(newContactProgram);
        //}

        public async static Task<bool> SyncNewIAP(HttpClient _client, SyncIAP iap)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                string json = JsonConvert.SerializeObject(iap);
                var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                var newUri = new Uri(App._absoluteUri, App._syncNewAPIUri);
                _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
                HttpResponseMessage response = await _client.PostAsync(newUri, contentString);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "SyncNewIAP" } });
                return false;
            }
        }

        //public async static Task<bool> SyncIAPs(HttpClient _client, SyncIAPsV2 iaps)
        //{
        //    try
        //    {
        //        _client.DefaultRequestHeaders.Clear();
        //        string json = JsonConvert.SerializeObject(iaps);
        //        var contentString = new StringContent(json, Encoding.UTF8, "application/json");
        //        var newUri = new Uri(App._absoluteUri, App._syncAPIsUri);
        //        _client.DefaultRequestHeaders.Add("Authorization", Auth.Auth.Token);
        //        HttpResponseMessage response = await _client.PostAsync(newUri, contentString);

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "SyncIAPs" } });
        //        return false;
        //    }
        //}

        public async static Task<bool> UpdateLocalIAPs()
        {
            try
            {
                var _connected = await CrossInAppBilling.Current.ConnectAsync(ItemType.Subscription);
                if (!_connected)
                    return false;

                var allPurchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);

                if (allPurchases.Count() == 0)
                    return false;

                var purchases = allPurchases.Where(x => x.ProductId == App._productId_Monthly ||
                                                        x.ProductId == App._productId_Quarterly ||
                                                        x.ProductId == App._productId_Yearly ||
                                                        x.ProductId == App._productId_Monthly_NoTrial ||
                                                        x.ProductId == App._productId_Yearly_NoTrial)
                                                        .ToList();

                if (purchases.Count() == 0)
                    return false;

                var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

                var existingSubscriptions = await _connection.Table<LocalDBSubscription>().Where(x => x.GuidCRM == Guid.Empty).ToListAsync();

                foreach (var purch in purchases)
                {
                    bool IsNew = false;

                    var newSubscription = existingSubscriptions.Where(x => x.StartDate == purch.TransactionDateUtc).FirstOrDefault();
                    if (newSubscription == null)
                    {
                        newSubscription = new LocalDBSubscription();
                        IsNew = true;
                    }

                    newSubscription.GuidCRM = Guid.Empty;
                    newSubscription.Type = 866660003;//full
                    newSubscription.AutoRenew = true;
                    newSubscription.StartDate = purch.TransactionDateUtc;
                    newSubscription.StartedAsTrial = true;

                    if (purch.State == PurchaseState.FreeTrial)
                    {
                        DateTime trialEndDate = purch.TransactionDateUtc.AddDays(7);
                        newSubscription.StateCodeValue = 0;
                        newSubscription.StatusCodeValue = 866660000;//free trial                        
                        newSubscription.EndDate = trialEndDate;
                        newSubscription.EndedOn = trialEndDate;
                        newSubscription.TrialEndedOn = trialEndDate;
                        newSubscription.TrialEndDate = trialEndDate;

                        if (IsNew)
                        {
                            await _connection.InsertAsync(newSubscription);
                        }
                        else
                        {
                            await _connection.UpdateAsync(newSubscription);
                        }
                    }
                    else if (purch.State == PurchaseState.Purchased)
                    {
                        DateTime endDate = DateTime.UtcNow;

                        if (purch.ProductId == App._productId_Monthly || purch.ProductId == App._productId_Monthly_NoTrial)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(1);
                        }
                        else if (purch.ProductId == App._productId_Quarterly)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(3);
                        }
                        else if (purch.ProductId == App._productId_Yearly || purch.ProductId == App._productId_Yearly_NoTrial)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(12);
                        }

                        DateTime trialEndDate = purch.TransactionDateUtc.AddDays(7);
                        newSubscription.StateCodeValue = 0;
                        newSubscription.StatusCodeValue = 866660001;//active (valid payments)                        
                        newSubscription.EndDate = endDate;
                        newSubscription.EndedOn = endDate;
                        newSubscription.TrialEndedOn = trialEndDate;
                        newSubscription.TrialEndDate = trialEndDate;

                        if (IsNew)
                        {
                            await _connection.InsertAsync(newSubscription);
                        }
                        else
                        {
                            await _connection.UpdateAsync(newSubscription);
                        }
                    }
                    else if (purch.State == PurchaseState.Refunded || purch.State == PurchaseState.Canceled)
                    {
                        DateTime endDate = DateTime.UtcNow;

                        if (purch.ProductId == App._productId_Monthly || purch.ProductId == App._productId_Monthly_NoTrial)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(1);
                        }
                        else if (purch.ProductId == App._productId_Quarterly)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(3);
                        }
                        else if (purch.ProductId == App._productId_Yearly || purch.ProductId == App._productId_Yearly_NoTrial)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(12);
                        }

                        DateTime trialEndDate = purch.TransactionDateUtc.AddDays(7);
                        newSubscription.StateCodeValue = 1;
                        newSubscription.StatusCodeValue = 866660007;//refunded                       
                        newSubscription.EndDate = endDate;
                        newSubscription.EndedOn = endDate;
                        newSubscription.TrialEndedOn = trialEndDate;
                        newSubscription.TrialEndDate = trialEndDate;

                        if (IsNew)
                        {
                            await _connection.InsertAsync(newSubscription);
                        }
                        else
                        {
                            await _connection.UpdateAsync(newSubscription);
                        }
                    }
                    else if (purch.State == PurchaseState.PaymentPending)
                    {
                        DateTime endDate = DateTime.UtcNow;

                        if (purch.ProductId == App._productId_Monthly || purch.ProductId == App._productId_Monthly_NoTrial)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(1);
                        }
                        else if (purch.ProductId == App._productId_Quarterly)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(3);
                        }
                        else if (purch.ProductId == App._productId_Yearly || purch.ProductId == App._productId_Yearly_NoTrial)
                        {
                            endDate = purch.TransactionDateUtc.AddMonths(12);
                        }

                        DateTime trialEndDate = purch.TransactionDateUtc.AddDays(7);
                        newSubscription.StateCodeValue = 1;
                        newSubscription.StatusCodeValue = 866660004;//Payment Issue (charge failed)                       
                        newSubscription.EndDate = endDate;
                        newSubscription.EndedOn = endDate;
                        newSubscription.TrialEndedOn = trialEndDate;
                        newSubscription.TrialEndDate = trialEndDate;

                        if (IsNew)
                        {
                            await _connection.InsertAsync(newSubscription);
                        }
                        else
                        {
                            await _connection.UpdateAsync(newSubscription);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _WebApiName }, { "Function", "UpdateLocalIAPs()" } });
                return false;
            }
        }
    }


    public class TrialContactProgram : BaseAttributes
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDaysCreated { get; set; }
        public bool IsScheduleBuilt { get; set; }
        public Guid ProgramGuid { get; set; }
    }

    public class TrialRecommendedBy
    {
        public int RP { get; set; }
        public List<OptProgramV3> Programs { get; set; }

        public TrialRecommendedBy()
        {
            Programs = new List<OptProgramV3>();
        }
    }

}

