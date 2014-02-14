using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    struct Identifier
    {
        internal int? Value { get; set; }
        public bool IsEmpty()
        {
            return Value == null;
        }
    }
}
