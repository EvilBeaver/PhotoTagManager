using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    public class Database : Tagger.Engine.DAL.IDatabase
    {
        private string _dbFileName;
        
        public Database(string dbFileName)
        {
            _dbFileName = dbFileName;
        }

        private void Create()
        {
            if (DatabaseFileExists())
            {
                throw new InvalidOperationException(String.Format("Database file {0} already exists", _dbFileName));
            }

            SQLiteConnection.CreateFile(_dbFileName);

            using (var con = OpenConnection())
            {
                using (SQLiteCommand createDataBase = con.CreateCommand())
                {
                    createDataBase.CommandText = Engine.Properties.Resources.DB_CREATION_SCRIPT;
                    createDataBase.ExecuteNonQuery();
                }
            }
            
        }

        private bool DatabaseFileExists()
        {
            return System.IO.File.Exists(_dbFileName);
        }

        public SQLiteConnection OpenConnection()
        {
            Init();

            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder();
            conString.DataSource = _dbFileName;
            SQLiteConnection con = new SQLiteConnection();
            con.ConnectionString = conString.ToString();
            try
            {
                con.Open();
            }
            catch
            {
                con.Dispose();
                throw;
            }

            return con;
        }

        public void Init()
        {
            if (!DatabaseFileExists())
            {
                Create();
            }
        }

    }
}
