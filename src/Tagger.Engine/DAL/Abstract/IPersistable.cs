using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IPersistable<T>
    {
        void Hydrate(IDataRepository<T> repo);
        void Persist(IDataRepository<T> repo);
        Identifier Key { get; }
        T Value { get; }
    }
}
