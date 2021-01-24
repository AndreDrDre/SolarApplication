using System;
using System.IO;
using SolarForMe.Persistance;
using SQLite;

namespace SolarForMe.Android.Persistance
{
    public class SQLiteDBAndroid : ISQLiteDB
    {
    
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "MySQLite.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}
