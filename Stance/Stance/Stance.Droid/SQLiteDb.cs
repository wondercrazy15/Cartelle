using System;

using Stance.Droid;
using Xamarin.Forms;
using System.IO;
using Stance.Utils;
using SQLite;


[assembly: Dependency(typeof(SQLiteDb))]
namespace Stance.Droid
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentPath, "MySQLite.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}