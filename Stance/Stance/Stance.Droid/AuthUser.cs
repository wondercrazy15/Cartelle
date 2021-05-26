using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;
using Stance.Utils.Auth;
using Stance.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthUser))]
namespace Stance.Droid
{
    public class AuthUser : IAuthUser
    {
        public string Password
        {
            get
            {
                var account = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Properties["Password"] : null;
            }
        }

        public string Username
        {
            get
            {
                var account = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Username : null;
            }
        }

        public void DeleteCredrentials()
        {
            var account = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
            if (account != null)
            {
                AccountStore.Create(Forms.Context).Delete(account, App.AppName);
            }
        }

        public bool IsAuthenticated()
        {
            if(Username != null && Password != null)
            {
                return true;
            } else { return false; }
        }

        public void SaveCredentials(string username, string password)
        {
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Account account = new Account
                {
                    Username = username
                };
                account.Properties.Add("Password", password);
                AccountStore.Create(Forms.Context).Save(account, App.AppName);
            }
        }
    }
}