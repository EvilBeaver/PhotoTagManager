using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    public class Database
    {
        private string _dbName;
        
        public Database(string dbName)
        {
            _dbName = dbName;
        }

        public void Create()
        {
            string databaseFullName = DatabaseFileFullName();
            if (DatabaseFileExists())
            {
                throw new InvalidOperationException(String.Format("Database file {0} already exists", databaseFullName));
            }

            SQLiteConnection.CreateFile(databaseFullName);

            using (var con = OpenConnection())
            {
                using (SQLiteCommand createDataBase = con.CreateCommand())
                {
                    createDataBase.CommandText = Engine.Properties.Resources.DB_CREATION_SCRIPT;
                    createDataBase.ExecuteNonQuery();
                }
            }
            
        }

        private string DatabaseFileFullName()
        {
            var asm = System.Reflection.Assembly.GetCallingAssembly();
            var asmPath = System.IO.Path.GetDirectoryName(asm.Location);
            return System.IO.Path.Combine(asmPath, _dbName);
        }

        private bool DatabaseFileExists()
        {
            return System.IO.File.Exists(DatabaseFileFullName());
        }

        public SQLiteConnection OpenConnection()
        {
            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder();
            conString.DataSource = _dbName;
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
