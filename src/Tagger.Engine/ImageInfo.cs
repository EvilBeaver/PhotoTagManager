using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public class ImageInfo
    {
        private List<TagDescription> _tags = new List<TagDescription>();
        
        public FileLink File { get; internal set; }
        public IList<TagDescription> Tags 
        {
            get { return _tags; }
        }
    }
}
