using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Utils.Auth
{
    public static class GeneralAuth
    {
        public static string Token
        {
            get
            {
                string _auth = string.Format("{0}:{1}", "3R6dhCS888Y9", "ce2dthDyzJWV");
                string _enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(_auth));
                string _cred = string.Format("{0} {1}", "Basic", _enc);
                return _cred;
            }
        }

    }
}
