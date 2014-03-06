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

    public interface IImageStreamHost
    {
        IList<IHostableImageStream> ChildItems { get; }
    }

    public interface IHostableImageStream : IImageStream
    {
        IImageStreamHost Parent { get; }
    }

}
