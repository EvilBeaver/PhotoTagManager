using System;
namespace Tagger.Engine.DAL
{
    public interface IDatabase
    {
        void Init();
        System.Data.SQLite.SQLiteConnection OpenConnection();
    }
}
