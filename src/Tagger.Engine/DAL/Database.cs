using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Tagger.Engine.DAL.Abstract;

namespace Tagger.Engine.DAL
{
    class SQLiteDatabase : IDatabase, IQueryExecutor
    {
        private string _dbFileName;
        private SQLiteTransactionalQueryExecutor _currentTransaction;
        private int _transactionCounter = 0;
        
        public SQLiteDatabase(string dbFileName)
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

            FileRepository.Create(this);
            FavoritesRepository.Create(this);
            FolderRefRepository.Create(this);
        }

        private bool DatabaseFileExists()
        {
            return System.IO.File.Exists(_dbFileName);
        }

        private SQLiteConnection OpenConnection()
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

        public void Shutdown()
        {
            SQLiteConnection.ClearAllPools();
            GC.Collect();
        }

        public void Init()
        {
            if (!DatabaseFileExists())
            {
                Create();
            }
        }

        public ITransactionScope BeginTransaction()
        {
            if (_transactionCounter == 0)
            {
                _currentTransaction = new SQLiteTransactionalQueryExecutor(this);
            }

            _transactionCounter++;

            return _currentTransaction;
        }

        public bool CommitTransaction()
        {
            if (!IsTransactionActive)
            {
                throw new InvalidOperationException("Transaction is not active");
            }

            _transactionCounter--;
            try
            {
                if (_transactionCounter == 0)
                {
                    _currentTransaction.Commit();
                    TransactionCleanup();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                InternalRollback();
                throw;
            }
            
        }

        private void InternalRollback()
        {
            try
            {
                _currentTransaction.Rollback();
            }
            finally
            {
                TransactionCleanup();
            }
        }

        private void TransactionCleanup()
        {
            _transactionCounter = 0;
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }

        public void RollbackTransaction()
        {
            if (!IsTransactionActive)
            {
                throw new InvalidOperationException("Transaction is not active");
            }

            InternalRollback();

        }

        public bool IsTransactionActive
        {
            get
            {
                return _currentTransaction != null;
            }
        }

        private static void SetCommandParameters(Query query, SQLiteCommand command)
        {
            foreach (var param in query.Parameters)
            {
                command.Parameters.AddWithValue("@" + param.Key, param.Value);
            }
        }

        #region IQueryExecutor Members

        public IQueryReader ExecuteReader(Query query)
        {
            if (IsTransactionActive)
            {
                return _currentTransaction.ExecuteReader(query);
            }
            else
            {
                var con = OpenConnection();
                var cmd = con.CreateCommand();
                
                var reader = new SQLiteQueryReader();
                try
                {
                    reader.Execute(query, cmd, true);
                }
                catch
                {
                    cmd.Dispose();
                    con.Dispose();
                    throw;
                }
                return reader;
                
            }
        }

        public int ExecuteCommand(Query query)
        {
            if (IsTransactionActive)
            {
                return _currentTransaction.ExecuteCommand(query);
            }
            else
            {
                using (var con = OpenConnection())
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = query.Text;
                    SetCommandParameters(query, cmd);
                    var rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }

        public object ExecuteScalar(Query query)
        {
            if (IsTransactionActive)
            {
                return _currentTransaction.ExecuteScalar(query);
            }
            else
            {
                using (var con = OpenConnection())
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = query.Text;
                    SetCommandParameters(query, cmd);
                    return cmd.ExecuteScalar();
                }
            }
        }

        #endregion

        #region SQLiteQueryReader Class

        class SQLiteQueryReader : IQueryReader
        {
            private SQLiteDataReader _reader;
            
            public void Execute(Query query, SQLiteCommand command, bool autoClose)
            {
                command.CommandText = query.Text;
                
                SQLiteDatabase.SetCommandParameters(query, command);

                var behavior = autoClose ? CommandBehavior.CloseConnection : CommandBehavior.Default;
                try
                {
                    _reader = command.ExecuteReader(behavior);
                }
                finally
                {
                    command.Dispose();
                }

            }

            public bool ReadNext()
            {
                return _reader.Read();
            }

            public bool HasRows
            {
                get { return _reader.HasRows; }
            }

            public object this[string field]
            {
                get
                {
                    return _reader[field];
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                DisposeDbObject(_reader);

                _reader = null;
            }

            private static void DisposeDbObject(IDisposable disposable)
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            #endregion

        } 
        
        #endregion

        #region Transactional query

        class SQLiteTransactionalQueryExecutor : ITransactionScope, IQueryExecutor
        {
            SQLiteTransaction _transaction;
            SQLiteConnection _connection;

            public SQLiteTransactionalQueryExecutor(SQLiteDatabase db)
            {
                _connection = db.OpenConnection();
                _transaction = _connection.BeginTransaction();
            }
            
            public void Commit() 
            {
                _transaction.Commit();
            }
            
            public void Rollback() 
            {
                _transaction.Rollback();
            }
            
            #region IQueryExecutor Members

            public IQueryReader ExecuteReader(Query query)
            {
                var command = _connection.CreateCommand();
                command.Transaction = _transaction;

                var reader = new SQLiteQueryReader();
                reader.Execute(query, command, false);
                return reader;
            }

            public int ExecuteCommand(Query query)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    command.CommandText = query.Text;
                    SQLiteDatabase.SetCommandParameters(query, command);
                    return command.ExecuteNonQuery();
                }
            }

            public object ExecuteScalar(Query query)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    command.CommandText = query.Text;
                    SQLiteDatabase.SetCommandParameters(query, command);
                    return command.ExecuteScalar();
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                DisposeDbObject(_transaction);
                DisposeDbObject(_connection);

                _transaction = null;
                _connection = null;
            }

            private static void DisposeDbObject(IDisposable disposable)
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            #endregion
        } 

        #endregion
    }

}
