using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    abstract class RepositoryBase<T> : IDataRepository<T>
    {
        private IDatabase _db;

        public RepositoryBase()
        {
            _db = DatabaseService.GetInstance();
        }

        protected IDatabase Database
        {
            get
            {
                return _db;
            }
        }

        private bool IsNewObject(IPersistable<T> item)
        {
            return item.Key.IsEmpty();
        }

        abstract protected void InternalAdd(IPersistable<T> item);
        abstract protected void InternalRemove(IPersistable<T> item);
        abstract protected void InternalWrite(IPersistable<T> item);
        
        #region IDataRepository<T> Members

        public void Initialize()
        {
            _db = DatabaseService.GetInstance();
            if (_db == null)
            {
                throw new InvalidOperationException("Database is not defined");
            }
        }

        public void Add(IPersistable<T> item)
        {
            if (!IsNewObject(item))
            {
                throw new InvalidOperationException(String.Format("Item {0} is not new", item.ToString()));
            }

            InternalAdd(item);
        }

        public void Remove(IPersistable<T> item)
        {
            if (!IsNewObject(item))
            {
                InternalRemove(item);
            }
        }

        public void Write(IPersistable<T> item)
        {
            if (IsNewObject(item))
            {
                InternalAdd(item);
            }
            else
            {
                InternalWrite(item);
            }
        }

        public void WriteRange(IEnumerable<IPersistable<T>> range)
        {
            throw new NotImplementedException();
        }

        public virtual IPersistable<T> FindByKey(Identifier key)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<IPersistable<T>> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
