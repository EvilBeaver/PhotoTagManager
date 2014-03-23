using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IPersistableImageStream : IPersistable
    {
        public string TableName { get; }
        public IImageStream ImageStream { get; }
    }
}
