using System;
using SQLite;
using Stance.Utils;
using System.IO;
using Xamarin.Forms;
using Stance.iOS;

[assembly: Dependency(typeof(Stance.iOS.SQLiteDb))]
namespace Stance.iOS
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {            
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentPath, "MySQLite.db3");

            return new SQLiteAsyncConnection(path,false);
        }
    }
}