using Microsoft.AppCenter.Analytics;
using SQLite;
using Stance.Models.LocalDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Utils
{
    public static class Subscription
    {
        private static SQLiteAsyncConnection _connection;
        private static string _PageName = "Subscription";

        public static async Task<bool> HasSubscriptionToProgram(string actionType, int ProgramId = 0, int dayNumber = 0)
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            int AccountId = 0;

            if (ProgramId != 0)
            {
                var Program = await _connection.Table<LocalDBProgram>().Where(x => x.Id == ProgramId).FirstOrDefaultAsync();
                if (Program != null)
                {
                    AccountId = Program.AccountId;
                }
            }
            //check subscriptions: Full, or Athlete and allow to add a contact program
            var activeSubscriptions = await _connection.Table<LocalDBSubscription>().Where(x => x.StateCodeValue == 0).ToListAsync();//Active 
            if (activeSubscriptions.Count() != 0)
            {
                var fullSubscriptions = activeSubscriptions.Where(x => x.Type == 866660003).ToList(); //Full Type
                foreach (var sub in fullSubscriptions)
                {
                    if (sub.StatusCodeValue == 866660000 && sub.TrialEndDate >= DateTime.UtcNow)//In Trial
                    {
                        return true;
                    }
                    else if ((sub.StatusCodeValue == 866660001 || sub.StatusCodeValue == 866660005 || sub.StatusCodeValue == 866660008) && sub.EndDate >= DateTime.UtcNow) //Active Subscription
                    {
                        return true;
                    }
                }

                var athleteSubscriptions = activeSubscriptions.Where(x => x.Type == 866660002 && x.AccountId == AccountId).ToList(); //Athlete Type
                foreach (var sub in athleteSubscriptions)
                {
                    if (sub.StatusCodeValue == 866660000 && sub.TrialEndDate >= DateTime.UtcNow)//In Trial
                    {
                        return true;
                    }
                    else if ((sub.StatusCodeValue == 866660001 || sub.StatusCodeValue == 866660005 || sub.StatusCodeValue == 866660008) && sub.EndDate >= DateTime.UtcNow) //Active Subscription
                    {
                        return true;
                    }
                }

                var programSubscription = activeSubscriptions.Where(x => x.Type == 866660000 && x.ProgramId == ProgramId).ToList(); //Program Type
                foreach (var sub in programSubscription)
                {
                    if (sub.StatusCodeValue == 866660000 && sub.TrialEndDate >= DateTime.UtcNow)//In Trial
                    {
                        return true;
                    }
                    else if ((sub.StatusCodeValue == 866660001 || sub.StatusCodeValue == 866660005 || sub.StatusCodeValue == 866660008) && sub.EndDate >= DateTime.UtcNow) //Active Subscription
                    {
                        return true;
                    }
                }

                var challengeSubscription = activeSubscriptions.Where(x => x.Type == 866660001 && x.ProgramId == ProgramId).ToList(); //Program Type
                foreach (var sub in challengeSubscription)
                {
                    if ((sub.StatusCodeValue == 866660001 || sub.StatusCodeValue == 866660005) && sub.EndDate >= DateTime.UtcNow) //Active Subscription
                    {
                        return true;
                    }
                }
            }

            //In Trial (in-app)
            if(actionType == "activate" || actionType == "addtomyplans" || actionType == "workout" || actionType == "buildschedule" || actionType == "resetschedule" || actionType == "startprogram")
            {
                return true;
            }

            //if((actionType == "download" || actionType == "editschedule") && dayNumber <= 1)
            //{
            //    return true;
            //}

            return false;
        }

    }
}
