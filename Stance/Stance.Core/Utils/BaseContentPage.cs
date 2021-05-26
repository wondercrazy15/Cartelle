using PCLStorage;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SQLite;
using Stance.Pages.Sub;
using Stance.Utils.WebAPI;
using System;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
//using Rg.Plugins.Popup.Pages;

namespace Stance.Utils
{
    public partial class BaseContentPage : ContentPage
    {
        public readonly HttpClient _client;
        public readonly SQLiteAsyncConnection _connection;
        public readonly string _rootFilePath;

        public BaseContentPage()
        {
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            _client = AppConnections.GetHttpClient;
            _connection = AppConnections.GetSQLConnection;
            _rootFilePath = AppConnections.GetRootFilePath;
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
