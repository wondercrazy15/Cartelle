using PCLStorage;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SQLite;
using Stance.Pages.Sub;
using Stance.Utils.WebAPI;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;

namespace Stance.Utils
{
    public class BaseContentPage : ContentPage
    {
        public readonly HttpClient _client;
        public readonly SQLiteAsyncConnection _connection;
        public readonly string _rootFilePath;

        public BaseContentPage()
        {
            _client = AppConnections.GetHttpClient;
            _connection = AppConnections.GetSQLConnection;
            _rootFilePath = AppConnections.GetRootFilePath;
        }

        public void RedirectToLoginIfNecessary()
        {
            if (!Auth.Auth.IsAuthenticated())
            {
                App.Current.MainPage = new SignIn();
            }
        }

        public bool IsInternetConnected()
        {
            if (CrossConnectivity.Current.IsConnected && (CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.WiFi) || CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.Cellular)))
            {
                return true;
            } else { return false; }            
        }

    }
}
