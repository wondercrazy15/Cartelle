using FFImageLoading;
using FFImageLoading.Cache;
using PCLStorage;
using SQLite;
using Stance.Models.LocalDB;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Stance.Utils.LocalDB
{
    public static class Database
    {
        private static SQLiteAsyncConnection _connection;

        public async static void ClearAsync()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            await _connection.CreateTableAsync<LocalDBContactV2>();
            await _connection.CreateTableAsync<LocalDBAccount>();
            await _connection.CreateTableAsync<LocalDBProgram>();
            await _connection.CreateTableAsync<LocalDBContactProgram>();
            await _connection.CreateTableAsync<LocalDBContactAction>();
            await _connection.CreateTableAsync<LocalDBContactProgramDayV2>();
            await _connection.CreateTableAsync<LocalDBChallenge>();
            await _connection.CreateTableAsync<LocalDBProgramDay>();
            await _connection.CreateTableAsync<LocalDBSubscription>();
            await _connection.CreateTableAsync<LocalDBSync>();
            await _connection.CreateTableAsync<LocalDBAction>();
            await _connection.CreateTableAsync<LocalDBActionContentV2>();
            await _connection.CreateTableAsync<LocalDBAudio>();
            await _connection.CreateTableAsync<LocalDBAudioContentV2>();
            await _connection.CreateTableAsync<LocalDBIAP>();

            await _connection.DeleteAllAsync<LocalDBContactAction>();
            await _connection.DeleteAllAsync<LocalDBProgram>();
            await _connection.DeleteAllAsync<LocalDBContactProgram>();
            await _connection.DeleteAllAsync<LocalDBProgramDay>();
            await _connection.DeleteAllAsync<LocalDBContactProgramDayV2>();
            await _connection.DeleteAllAsync<LocalDBChallenge>();
            await _connection.DeleteAllAsync<LocalDBAction>();
            await _connection.DeleteAllAsync<LocalDBActionContentV2>();
            await _connection.DeleteAllAsync<LocalDBAudioContentV2>();
            await _connection.DeleteAllAsync<LocalDBAudio>();
            await _connection.DeleteAllAsync<LocalDBContactV2>();
            await _connection.DeleteAllAsync<LocalDBAccount>();
            await _connection.DeleteAllAsync<LocalDBSubscription>();
            await _connection.DeleteAllAsync<LocalDBSync>();
            await _connection.DeleteAllAsync<LocalDBIAP>();

            //Below should not cause the App Center SDK to crash, because it does not delete its associated folders
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var folders = await rootFolder.GetFoldersAsync();
            var foldersToReview = folders.ToList();
            foreach (var folder in foldersToReview)
            {
                try
                {
                    if (folder.Name.Equals("Application Support"))
                    {
                        continue; //this fodler only contains /com.microsoft.appcenter. if deleted the App Center SDK crashes
                    }
                    else if (folder.Name.Equals("Caches"))
                    {
                        var subfolders = await folder.GetFoldersAsync();
                        // com.bethestance.Stance
                        // com.microsoft.appcenter
                        // com.plausiblelabs.crashreporter.data
                        foreach (var subFolder in subfolders)
                        {
                            if (!subFolder.Name.Equals("com.microsoft.appcenter") && !subFolder.Name.Equals("com.plausiblelabs.crashreporter.data"))
                            {
                                await subFolder.DeleteAsync(); //only delete things that are not related to App Center
                            }
                        }
                    }
                    else if (folder.Name.Equals("Cookies"))
                    {
                        await folder.DeleteAsync(); // this folder contains /com.bethestance.Stance.binarycookies file which is what we want to clear
                    }
                    else if (folder.Name.Equals("Preferences"))
                    {
                        continue; //this folder contains the info.plist file for the app
                    }
                    else
                    {
                        await folder.DeleteAsync(); //delete any other folder that has been creates in the app such as ContactProgramDays
                    }
                }
                catch (Exception ex)
                {
                    //usually error is thrown if the folder is already deleted
                    var err = ex.ToString();
                }
            }

            try
            {
                await ImageService.Instance.InvalidateCacheAsync(CacheType.All);
            }
            catch (Exception ex)
            {
                var err = ex.ToString();
            }

            //below throws an error
            //try
            //{
            //    await rootFolder.DeleteAsync();
            //} catch (Exception ex)
            //{
            //    await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
            //}
        }

        public async static void CreateAsync()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            await _connection.CreateTableAsync<LocalDBContactV2>();
            await _connection.CreateTableAsync<LocalDBAccount>();
            await _connection.CreateTableAsync<LocalDBProgram>();
            await _connection.CreateTableAsync<LocalDBContactProgram>();
            await _connection.CreateTableAsync<LocalDBContactAction>();
            await _connection.CreateTableAsync<LocalDBContactProgramDayV2>();
            await _connection.CreateTableAsync<LocalDBChallenge>();
            await _connection.CreateTableAsync<LocalDBProgramDay>();
            await _connection.CreateTableAsync<LocalDBSubscription>();
            await _connection.CreateTableAsync<LocalDBSync>();
            await _connection.CreateTableAsync<LocalDBAction>();
            await _connection.CreateTableAsync<LocalDBActionContentV2>();
            await _connection.CreateTableAsync<LocalDBAudio>();
            await _connection.CreateTableAsync<LocalDBAudioContentV2>();
            await _connection.CreateTableAsync<LocalDBIAP>();
        }

        //public async static void UpdateCPDs()
        //{
        //    _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        //    await _connection.CreateTableAsync<LocalDBContactProgramDayV2>();

        //    try
        //    {
        //        var ContactProgramDays = await _connection.Table<LocalDBContactProgramDay>().ToListAsync();
        //        if (ContactProgramDays.Count() != 0)
        //        {
        //            foreach (var item in ContactProgramDays)
        //            {
        //                var ContactProgramDay = new LocalDBContactProgramDayV2
        //                {
        //                    Id = item.Id,
        //                    ProgramDayId = item.ProgramDayId,
        //                    ContactProgramId = item.ContactProgramId,
        //                    IsComplete = item.IsComplete,
        //                    IsDownloaded = item.IsDownloaded,
        //                    DownloadedOn = item.DownloadedOn,
        //                    ReceivedOn = item.ReceivedOn,
        //                    ScheduledStartDate = item.ScheduledStartDate,
        //                    ActualStartDate = item.ActualStartDate,
        //                    PercentComplete = item.PercentComplete,
        //                    NumberOfDownloads = item.NumberOfDownloads,
        //                    Rating = item.Rating,
        //                    Synced = item.Synced,
        //                    SequenceNumber = item.SequenceNumber,
        //                    DayTypeValue = item.DayTypeValue,
        //                    GuidCRM = item.GuidCRM,
        //                    StateCodeValue = item.StateCodeValue,
        //                    StatusCodeValue = item.StatusCodeValue
        //                };
        //                await _connection.InsertAsync(ContactProgramDay);
        //                await _connection.DeleteAsync(item);
        //            }
        //        }
        //        await _connection.DropTableAsync<LocalDBContactProgramDay>();

        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.ToString();
        //    }
        //}

        //public async static void UpdateACs()
        //{
        //    _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        //    await _connection.CreateTableAsync<LocalDBActionContentV2>();

        //    try
        //    {
        //        var ActionContents = await _connection.Table<LocalDBActionContent>().ToListAsync();
        //        if (ActionContents.Count() != 0)
        //        {
        //            foreach (var item in ActionContents)
        //            {
        //                var ActionContent = new LocalDBActionContentV2
        //                {
        //                    Id = item.Id,
        //                    ProgramId = item.ProgramId,
        //                    PhotoFilePath = item.PhotoFilePath,
        //                    VideoFilePath = item.VideoFilePath,
        //                    ContentType = item.ContentType,
        //                    ContentTypeValue = item.ContentTypeValue,
        //                    Heading = item.Heading,
        //                    SubHeading = item.SubHeading,
        //                    NumberOfReps = item.NumberOfReps,
        //                    WeightLbs = item.WeightLbs,
        //                    TimeSeconds = item.TimeSeconds,
        //                    Intensity = item.Intensity,
        //                    IntensityValue = item.IntensityValue,
        //                    PhotoUrl = item.PhotoUrl,
        //                    VideoUrl = item.VideoUrl,
        //                    IsPreview = false, //set this because you know that the previous databse version
        //                    GuidCRM = item.GuidCRM,
        //                    StateCodeValue = item.StateCodeValue,
        //                    StatusCodeValue = item.StatusCodeValue
        //                };
        //                await _connection.InsertAsync(ActionContent);
        //                await _connection.DeleteAsync(item);
        //            }
        //        }
        //        await _connection.DropTableAsync<LocalDBActionContent>();

        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.ToString();
        //    }

        //}

        //public async static void UpdateAuCs()
        //{
        //    _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        //    await _connection.CreateTableAsync<LocalDBAudioContentV2>();

        //    try
        //    {
        //        var AudioContents = await _connection.Table<LocalDBAudioContent>().ToListAsync();
        //        if (AudioContents.Count() != 0)
        //        {
        //            foreach (var item in AudioContents)
        //            {
        //                var AudioContent = new LocalDBAudioContentV2
        //                {
        //                    Id = item.Id,
        //                    AudioFilePath = item.AudioFilePath,
        //                    LengthMilliseconds = item.LengthMilliseconds,
        //                    AudioUrl = item.AudioUrl,
        //                    GuidCRM = item.GuidCRM,
        //                    StateCodeValue = item.StateCodeValue,
        //                    StatusCodeValue = item.StatusCodeValue
        //                };
        //                await _connection.InsertAsync(AudioContent);
        //                await _connection.DeleteAsync(item);
        //            }
        //        }
        //        await _connection.DropTableAsync<LocalDBAudioContent>();

        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.ToString();
        //    }

        //}

        //public async static void UpdateProfile()
        //{
        //    _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        //    await _connection.CreateTableAsync<LocalDBContactV2>();

        //    try
        //    {
        //        var Profile = await _connection.Table<LocalDBContact>().FirstOrDefaultAsync();
        //        if (Profile != null)
        //        {
        //            var new_Profile = new LocalDBContactV2
        //            {
        //                Id = Profile.Id,
        //                ProfilePhotoFilePath = Profile.ProfilePhotoFilePath,
        //                BeforePhotoFilePath = Profile.BeforePhotoFilePath,
        //                AfterPhotoFilePath = Profile.AfterPhotoFilePath,
        //                ProfileVideoFilePath = Profile.ProfileVideoFilePath,
        //                FirstName = Profile.FirstName,
        //                LastName = Profile.LastName,
        //                Email = Profile.Email,
        //                Password = Profile.Password,
        //                IsAdmin = Profile.IsAdmin,
        //                Birthday = Profile.Birthday,
        //                GenderTypeCode = Profile.GenderTypeCode,
        //                Gender = Profile.Gender,
        //                HeightCm = Profile.HeightCm,
        //                WeightLbs = Profile.WeightLbs,
        //                TrainingGoalTypeCode = Profile.TrainingGoalTypeCode,
        //                TrainingGoal = Profile.TrainingGoal,
        //                RegionTypeCode = Profile.RegionTypeCode,
        //                Region = Profile.Region,
        //                InstagramHandle = "",
        //                GuidCRM = Profile.GuidCRM,
        //                StateCodeValue = Profile.StateCodeValue,
        //                StatusCodeValue = Profile.StatusCodeValue
        //            };
        //            await _connection.InsertAsync(new_Profile);
        //            await _connection.DeleteAsync(Profile);
        //        }
        //        await _connection.DropTableAsync<LocalDBContact>();
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.ToString();
        //    }
        //}






    }
}
