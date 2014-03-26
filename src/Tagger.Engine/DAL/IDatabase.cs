using System;
using Tagger.Engine.DAL.Abstract;
namespace Tagger.Engine.DAL
{
    public interface IDatabase : IQueryExecutor
    {
        void Init();
        ITransactionScope BeginTransaction();
        void RollbackTransaction();
        bool CommitTransaction();
        bool IsTransactionActive { get; }
        void Shutdown();
    }

    public interface IQueryExecutor
    {
        IQueryReader ExecuteReader(Query query);
        int ExecuteCommand(Query query);
        object ExecuteScalar(Query query);
    }

    public interface IQueryReader : IDisposable
    {
        bool HasRows { get; }
        bool ReadNext();
        object this[string field] { get; }
    }

    public interface ITransactionScope : IDisposable
    {
        //void Commit();
        //void Rollback();
    }

}
