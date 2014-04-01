using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tagger.Engine.DAL;

namespace EngineTests
{
    [TestClass]
    public class DatabaseServiceTest
    {
        [TestMethod]
        public void TestMethod1()
        {
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
            throw new NotImplementedException();
        }

        public int ExecuteCommand(Tagger.Engine.DAL.Abstract.Query query)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(Tagger.Engine.DAL.Abstract.Query query)
        {
            throw new NotImplementedException();
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
            get { throw new NotImplementedException(); }
        }

        public bool ReadNext()
        {
            throw new NotImplementedException();
        }

        public object this[string field]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
