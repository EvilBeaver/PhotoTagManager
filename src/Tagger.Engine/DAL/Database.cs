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

            FileRepository.Create();
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

        public int Execute(string command, object[] parameters)
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = command;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
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
