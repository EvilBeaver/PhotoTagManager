using System;
using Tagger.Engine.DAL.Abstract;
namespace Tagger.Engine.DAL
{
    interface IDatabase : IQueryExecutor
    {
        void Init();
        ITransactionScope BeginTransaction();
        void RollbackTransaction();
        bool CommitTransaction();
        bool IsTransactionActive { get; }
        void Shutdown();
    }

    interface IQueryExecutor
    {
        IQueryReader ExecuteReader(Query query);
        int ExecuteCommand(Query query);
        object ExecuteScalar(Query query);
    }

    interface IQueryReader : IDisposable
    {
        bool HasRows { get; }
        bool ReadNext();
        object this[string field] { get; }
    }

    interface ITransactionScope : IDisposable
    {
        //void Commit();
        //void Rollback();
    }

}
