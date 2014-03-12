using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public class ImageInfo
    {
        private TagsCollection _tags = new TagsCollection();
        
        public FileLink File { get; internal set; }
        public TagsCollection Tags 
        {
            get { return _tags; }
        }

    }
}
