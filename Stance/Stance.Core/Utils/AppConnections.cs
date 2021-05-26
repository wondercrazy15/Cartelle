using ModernHttpClient;
using PCLStorage;
using SQLite;
using System.Net.Http;
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
