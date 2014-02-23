using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IDataRepository<T> where T:IPersistable
    {
        void Initialize();
        void Add(T item);
        void Remove(T item);
        void Write(T item);
        void WriteRange(IEnumerable<T> range);
        T FindByKey(Identifier key);
        IEnumerable<T> GetAll();
    }
}
