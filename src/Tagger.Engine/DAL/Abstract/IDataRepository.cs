using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IDataRepository<T> where T:IPersistable
    {
        void Remove(T item);
        void Write(T item);
        T FindByKey(Identifier key);
        IEnumerable<T> GetAll();
    }
}
