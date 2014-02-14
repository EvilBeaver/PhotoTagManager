using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IPersistable<T>
    {
        Identifier Key { get; }
        T Value { get; }
    }
}
