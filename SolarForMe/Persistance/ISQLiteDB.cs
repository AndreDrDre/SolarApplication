using System;
using SQLite;

namespace SolarForMe.Persistance
{
    public interface ISQLiteDB { 

        SQLiteAsyncConnection GetConnection();
    }
}
