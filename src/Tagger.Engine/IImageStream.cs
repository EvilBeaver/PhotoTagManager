using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public interface IImageStream
    {
        string Name { get; set; }
        IList<ImageInfo> Images { get; }
    }
}
