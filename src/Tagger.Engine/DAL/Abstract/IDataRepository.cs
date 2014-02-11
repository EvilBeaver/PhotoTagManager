using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IDataRepository<T>
    {
        void Initialize();
        void Add(IPersistable<T> item);
        void Remove(IPersistable<T> item);
        void Write(IPersistable<T> item);
        IPersistable<T> FindByKey(Identifier key);
        IEnumerable<IPersistable<T>> GetAll();
    }
}
