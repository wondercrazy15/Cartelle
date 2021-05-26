using Stance.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Utils.Auth
{
    public static class Auth
    {

        public static APIContactV2 User
        {
            get
            {
                if (!IsAuthenticated())
                {
                    //No user logged in
                    return null;
                }
                else
                {
                    //logged in
                    //get user from DB 
                    return new APIContactV2();
                }
            }

        }

        public static bool IsAuthenticated()
        {
            return DependencyService.Get<IAuthUser>().IsAuthenticated();
        }

        public static void SaveCredentials(string username, string password)
        {
            DependencyService.Get<IAuthUser>().SaveCredentials(username, password);
        }

        public static void DeleteCredentials()
        {
            DependencyService.Get<IAuthUser>().DeleteCredrentials();
        }

        public static string Username
        {
            get
            {
                return DependencyService.Get<IAuthUser>().Username;
            }
        }

        public static string Password
        {
            get
            {
                return DependencyService.Get<IAuthUser>().Password;
            }
        }

        public static string Token
        {
            get
            {
                string _auth = string.Format("{0}:{1}", Auth.Username, Auth.Password);
                string _enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(_auth));
                string _cred = string.Format("{0} {1}", "Basic", _enc);
                return _cred;
            }
        }


    }
}
