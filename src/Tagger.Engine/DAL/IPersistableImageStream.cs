using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IPersistableImageStream : IPersistable
    {
        string TableName { get; }
        IImageStream ImageStream { get; }
    }
}
