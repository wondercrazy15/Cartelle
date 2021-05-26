using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Utils.Auth
{
    public interface IAuthUser
    {
        string Username { get; }

        string Password { get; }

        bool IsAuthenticated();

        void SaveCredentials(string username, string password);

        void DeleteCredrentials();            

    }
}
