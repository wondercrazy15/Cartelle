using ModernHttpClient;
using Newtonsoft.Json;
using PCLStorage;
using SQLite;
using Stance.Models.API;
using Stance.Models.LocalDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Utils.WebAPI
{
    public static class AppConnections
    {
        private static HttpClient _client = new HttpClient(new NativeMessageHandler());
        private static readonly SQLiteAsyncConnection _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        private static readonly IFolder _rootFolder = FileSystem.Current.LocalStorage;

        public static HttpClient GetHttpClient
        {
            get { return _client; }
        }

        public static SQLiteAsyncConnection GetSQLConnection
        {
            get { return _connection; }
        }

        public static string GetRootFilePath
        {
            get { return _rootFolder.Path; }
        }

    }
}
