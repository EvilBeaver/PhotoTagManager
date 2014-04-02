using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tagger.Engine.DAL;

namespace EngineTests
{
    [TestClass]
    public class DatabaseServiceTest
    {
        [TestMethod]
        public void DBServiceAccessOK()
        {
            var db = new DBDummy();
            Assert.IsNull(DatabaseService.GetInstance());

            DatabaseService.RegisterInstance(db);
            Assert.AreSame(db, DatabaseService.GetInstance());

            DatabaseService.ShutdownInstance();
            Assert.IsNull(DatabaseService.GetInstance());

            try
            {
                var fr = DatabaseService.FileRepository;
            }
            catch(InvalidOperationException e)
            {
                if (e.Message == "Database is not specified")
                {
                    return;
                }
            }

            Assert.Fail("Exception \"Database is not specified\" was not thrown");

        }

        [TestMethod]
        public void RepositoryPropertiesCheck()
        {
            var db = new DBDummy();
            Assert.IsNull(DatabaseService.GetInstance());

            DatabaseService.RegisterInstance(db);

            Assert.IsTrue(DatabaseService.FileRepository is FileRepository);
            Assert.IsTrue(DatabaseService.FavoritesRepository is FavoritesRepository);
            Assert.IsTrue(DatabaseService.FolderRefRepository is FolderRefRepository);

            DatabaseService.ShutdownInstance();

        }
    }

    class DBDummy : IDatabase
    {
        #region IDatabase Members

        public void Init()
        {
            //throw new NotImplementedException();
        }

        public ITransactionScope BeginTransaction()
        {
            return new DummyTransaction();
        }

        public void RollbackTransaction()
        {
            //throw new NotImplementedException();
        }

        public bool CommitTransaction()
        {
            return true;//throw new NotImplementedException();
        }

        public bool IsTransactionActive
        {
            get { return false; }
        }

        public void Shutdown()
        {
            //
        }

        #endregion

        #region IQueryExecutor Members

        public IQueryReader ExecuteReader(Tagger.Engine.DAL.Abstract.Query query)
        {
            return new DummyDBReader();
        }

        public int ExecuteCommand(Tagger.Engine.DAL.Abstract.Query query)
        {
            return 0;
        }

        public object ExecuteScalar(Tagger.Engine.DAL.Abstract.Query query)
        {
            return null;
        }

        #endregion
    }

    class DummyTransaction : ITransactionScope
    {
        #region IDisposable Members

        public void Dispose()
        {
            //
        }

        #endregion
    }

    class DummyDBReader : IQueryReader
    {
        #region IQueryReader Members

        public bool HasRows
        {
            get { return true; }
        }

        public bool ReadNext()
        {
            return true;
        }

        public object this[string field]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
