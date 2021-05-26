using System.Linq;
using Stance.Utils.Auth;
using Stance.iOS;
using Xamarin.Forms;
using Xamarin.Auth;

[assembly: Dependency(typeof(AuthUser))]
namespace Stance.iOS
{
    public class AuthUser : IAuthUser
    {
        public string Password
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Properties["Password"] : null;
            }
        }

        public string Username
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Username : null;
            }
        }

        public void DeleteCredrentials()
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
            if (account != null)
            {
                AccountStore.Create().Delete(account, App.AppName);
            }
        }

        public bool IsAuthenticated()
        {
            if (Username != null && Password != null)
            {
                return true;
            }
            else { return false; }
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
                AccountStore.Create().Save(account, App.AppName);
            }
        }
    }
}